import requests
import json
from datetime import datetime, timedelta

# Configuration
auth_base_url = "https://localhost:5001/api/testauth"
phase_base_url = "https://localhost:5001/api/phases"
loan_base_url = "https://localhost:5001/api/loans"
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

def create_loan(token):
    url = f"{loan_base_url}/create"
    payload = {
        "Amount": 100,
        "EndDate": (datetime.utcnow() + timedelta(days=30)).isoformat() + "Z",  # Ensure UTC format
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 201:
        loan_id = response.json().get("id")
        print("Loan created successfully.")
        print(f"Loan ID: {loan_id}")
        return loan_id
    else:
        print(f"Failed to create loan: {response.status_code} - {response.text}")
        return None

def reset_phase(token):
    url = f"{phase_base_url}/reset"
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    print(f"Resetting phase with token {token}...")
    response = requests.post(url, headers=headers, verify=False)
    print(f"reset_phase status code: {response.status_code}")
    print(f"reset_phase response: {response.text}")

def call_next_phase(token):
    url = f"{phase_base_url}/next-phase"
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    payload = {
        "Status": 0,
        "Amount": 100,
        "EndDate": (datetime.utcnow() + timedelta(days=30)).isoformat() + "Z"
    }
    print(f"Calling next phase using token {token}...")
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    print(f"call_next_phase status code: {response.status_code}")
    try:
        print(f"call_next_phase response: {response.json()}")
    except json.JSONDecodeError:
        print("Response is not in JSON format or is empty")

def delete_all_loans(token):
    loans = get_loans(token)
    if loans:
        for loan in loans:
            delete_loan(token, loan['id'])
    print("All loans deleted successfully.")

def get_loans(token):
    url = loan_base_url
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        loans = response.json()
        print("Loans retrieved successfully.")
        print(loans)
        return loans
    else:
        print(f"Failed to retrieve loans: {response.status_code} - {response.text}")
        return None

def delete_loan(token, loan_id):
    url = f"{loan_base_url}/{loan_id}"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.delete(url, headers=headers, verify=False)
    if response.status_code == 204:
        print("Loan deleted successfully.")
    else:
        print(f"Failed to delete loan: {response.status_code} - {response.text}")
    print(f"Loan deleted")

if __name__ == "__main__":
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        send_sms("login")
        token = verify_sms("login")
    
    if token and token != "USER_EXISTS":
        delete_all_loans(token)
        call_next_phase(token)
    else:
        print("Failed to retrieve token")