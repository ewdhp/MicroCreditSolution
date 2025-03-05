import requests
import json

# Configuration
auth_base_url = "https://localhost:5001/api/testauth"
phase_base_url = "https://localhost:5001/api/phases/next-phase"
user_base_url = "https://localhost:5001/api/users"
phone_number = "+523321890176"
verification_code = "123456"  # Replace with the actual verification code

# Disable SSL warnings (not recommended for production)
requests.packages.urllib3.disable_warnings(requests.packages.urllib3.exceptions.InsecureRequestWarning)

def send_sms(action):
    url = f"{auth_base_url}/send"
    payload = {
        "action": action,
        "Phone": phone_number
    }
    headers = {
        "Content-Type": "application/json"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        print("Verification SMS sent successfully.")
    else:
        print(f"Failed to send verification SMS: {response.status_code} - {response.text}")

def verify_sms(action):
    url = f"{auth_base_url}/verify"
    payload = {
        "action": action,
        "Phone": phone_number,
        "Code": verification_code
    }
    headers = {
        "Content-Type": "application/json"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        token = response.json().get("token")
        print("Verification successful. Token received.")
        print(f"Token: {token}")  # Log the token for debugging
        return token
    elif response.status_code == 400 and "User already exists" in response.text:
        print("User already exists. Switching to login.")
        return "USER_EXISTS"
    else:
        print(f"Failed to verify code: {response.status_code} - {response.text}")
        return None

def reset_phase(token, phase):
    url = f"{user_base_url}/reset-phase"
    payload = {
        "Phase": phase
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.put(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 204:
        print("User phase reset successfully.")
    else:
        print(f"Failed to reset user phase: {response.status_code} - {response.text}")

def call_phase(token, request_type, somefield):
    url = phase_base_url
    payload = {
        "Somefield": somefield,
        "RequestType": request_type
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        print("Phase called successfully.")
        print(response.json())
    else:
        print(f"Failed to call phase: {response.status_code} - {response.text}")

if __name__ == "__main__":
    # First, try to signup to get the token
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        # If user already exists, login to get the token
        send_sms("login")
        token = verify_sms("login")
    
    if token and token != "USER_EXISTS":
        # Reset the user's phase to 1 (Loan phase)
        reset_phase(token, 1)      
        # Call the Phase method with a LoanRequest
        call_phase(token, "ApprovalRequest", True)