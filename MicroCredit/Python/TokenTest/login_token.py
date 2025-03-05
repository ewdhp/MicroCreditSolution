import requests
import json

# Configuration
base_url = "https://localhost:5001/api/testauth"
phone_number = "+523321890176"

# Disable SSL warnings (not recommended for production)
requests.packages.urllib3.disable_warnings(requests.packages.urllib3.exceptions.InsecureRequestWarning)

def send_sms():
    url = f"{base_url}/send"
    payload = {
        "action": "login",
        "Phone": phone_number
    }
    headers = {
        "Content-Type": "application/json"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        print("Verification SMS simulated successfully.")
    else:
        print(f"Failed to simulate sending verification SMS: {response.status_code} - {response.text}")

def verify_sms(verification_code):
    url = f"{base_url}/verify"
    payload = {
        "action": "login",
        "Phone": phone_number,
        "Code": verification_code
    }
    headers = {
        "Content-Type": "application/json"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        token = response.json().get("token")
        print("Verification simulated successfully. Token received.")
        print(f"Token: {token}")  # Log the token for debugging
        return token
    else:
        print(f"Failed to simulate verification: {response.status_code} - {response.text}")
        return None

def get_user_info(token):
    url = "https://localhost:5001/api/users"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        user_info = response.json()
        print("User info retrieved successfully.")
        print(json.dumps(user_info, indent=2))
    else:
        print(f"Failed to retrieve user info: {response.status_code} - {response.text}")

if __name__ == "__main__":
    send_sms()
    verification_code = input("Enter the verification code you received (use 123456 for simulation): ")
    token = verify_sms(verification_code)
    if token:
        get_user_info(token)