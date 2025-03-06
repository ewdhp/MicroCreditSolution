import requests
import json

# Configuration
auth_base_url = "https://localhost:5001/api/testauth"
phase_base_url = "https://localhost:5001/api/phases/phase"
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
    print(f"Resetting phase to {phase} with token {token}...")
    response = requests.put(url, headers=headers, data=json.dumps(payload), verify=False)
    print(f"reset_phase status code: {response.status_code}")
    print(f"reset_phase response: {response.text}")

def call_phase(token, request_type, phase_action):
    url = phase_base_url
    payload = {
        "Type": request_type,
        "Action": phase_action
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    print(f"Calling phase with type {request_type} and action {phase_action} using token {token}...")
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    print(f"call_phase status code: {response.status_code}")
    try:
        print(f"call_phase response: {response.json()}")
    except json.JSONDecodeError:
        print("Response is not in JSON format or is empty")

if __name__ == "__main__":
     # First, try to signup to get the token
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        # If user already exists, login to get the token
        send_sms("login")
        token = verify_sms("login")
    
    if token and token != "USER_EXISTS":
        reset_phase(token, 1)
        call_phase(token, "Loan", "validate")
        call_phase(token, "Loan", "view")
    else:
        print("Failed to retrieve token")