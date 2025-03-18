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
phone = "3321890176"
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

def get_current_user(token):
    url = f"{user_base_url}/current"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        user = response.json()
        print(f"Current user retrieved successfully. {user}")
        return user
    else:
        print(f"Failed to retrieve current user: {response.status_code} - {response.text}")
        return None

def create_user(token):
    url = user_base_url
    payload = {
        "Phone": phone,
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 201:
        user = response.json()
        print(f"User created successfully. {user}")
        return user
    else:
        print(f"Failed to create user: {response.status_code} - {response.text}")

def update_user(token):
    url = user_base_url
    payload = {
        "Phone": phone,
        "Name": newUser 
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.put(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 204:
        print("User updated successfully.")
        # Retrieve and print the updated user
        updated_user = get_current_user(token)
        print(f"Updated User: {updated_user}")
    else:
        print(f"Failed to update user: {response.status_code} - {response.text}")

def delete_user(token):
    url = user_base_url
    headers = {
        "Authorization": f"Bearer {token}"
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

         # Create user
        create_user(token)

        # Get current user
        get_current_user(token)

        # Update user
        update_user(token)

        # Delete user
        delete_user(token)

    else:
        print("Failed to retrieve token")

