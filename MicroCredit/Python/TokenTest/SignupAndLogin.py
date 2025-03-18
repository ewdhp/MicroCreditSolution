import requests
import json
from datetime import datetime, timedelta

# Configuration
auth_base_url = "https://localhost:5001/api/testauth"
phase_base_url = "https://localhost:5001/api/phases"
loan_base_url = "https://localhost:5001/api/loans"
user_base_url = "https://localhost:5001/api/users"

newUser = "TestUser"  # Ensure this value is set correctly
phone_number = "+523321890176"  # Ensure this value is set correctly
verification_code = "123456"  # Replace with the actual verification code

# Disable SSL warnings (not recommended for production)
requests.packages.urllib3.disable_warnings(requests.packages.urllib3.exceptions.InsecureRequestWarning)

def send_sms(action):
    url = f"{auth_base_url}/send"
    payload = {
        "action": action,
        "Phone": phone_number,
        "Name": newUser  # Ensure Name is included in the payload
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
        "Code": verification_code,
        "Name": newUser  # Ensure Name is included in the payload
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

def delete_user(token):
    url = f"{user_base_url}"
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }
    response = requests.delete(url, headers=headers, verify=False)
    if response.status_code == 204:
        print("User deleted successfully.")
    else:
        print(f"Failed to delete user: {response.status_code} - {response.text}")

def delete_all_users():
    url = f"{user_base_url}/all"
    response = requests.delete(url, verify=False)
    if response.status_code == 204:
        print("All users deleted successfully.")
    else:
        print(f"Failed to delete all users: {response.status_code} - {response.text}")

if __name__ == "__main__":
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        send_sms("login")
        token = verify_sms("login")
    
    if token:
        print("Token retrieved successfully.")

    else:
        print("Failed to retrieve token")

    delete_all_users()