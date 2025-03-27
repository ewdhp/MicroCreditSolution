import requests
import json

# Configuration
base_url = "https://localhost:5001/api/auth"
phone_number = "+523321890176"

# Disable SSL warnings (not recommended for production)
requests.packages.urllib3.disable_warnings(requests.packages.urllib3.exceptions.InsecureRequestWarning)

def send_sms(action):
    url = f"{base_url}/send"
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

def verify_sms(action, verification_code):
    url = f"{base_url}/verify"
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
    else:
        print(f"Failed to verify code: {response.status_code} - {response.text}")
        return None

def delete_user(token):
    url = f"{base_url}/delete"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.delete(url, headers=headers, verify=False)
    if response.status_code == 204:
        print("User deleted successfully.")
    else:
        print(f"Failed to delete user: {response.status_code} - {response.text}")

def check_user_exists(token):
    url = f"{base_url}/user"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        print("User exists.")
        return True
    elif response.status_code == 404:
        print("User does not exist.")
        return False
    else:
        print(f"Failed to check user existence: {response.status_code} - {response.text}")
        return False

if __name__ == "__main__":
    # First, try to login to get the token
    send_sms("login")
    verification_code = input("Enter the verification code you received: ")
    token = verify_sms("login", verification_code)
    
    if token:
        if check_user_exists(token):
            delete_user(token)
    
    # Proceed with signup
    send_sms("signup")
    verification_code = input("Enter the verification code you received: ")
    token = verify_sms("signup", verification_code)
    if token:
        print("Signup process completed successfully.")