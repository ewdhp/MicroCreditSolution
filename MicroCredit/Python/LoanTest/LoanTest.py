import requests
import json
from datetime import datetime, timedelta

# Configuration
auth_base_url = "https://localhost:5001/api/testauth"
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
    url = loan_base_url
    payload = {
        "UserId": 1,  # Replace with the actual user ID
        "StartDate": datetime.now().isoformat(),
        "EndDate": (datetime.now() + timedelta(days=30)).isoformat(),
        "Amount": 1000,
        "InterestRate": 5.0,
        "Currency": "USD",
        "Status": "Active",  # Ensure this matches the expected enum value
        "LoanDescription": "Test Loan"
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

def get_loan(token, loan_id):
    url = f"{loan_base_url}/{loan_id}"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        loan = response.json()
        print("Loan retrieved successfully.")
        print(loan)
        return loan
    else:
        print(f"Failed to retrieve loan: {response.status_code} - {response.text}")
        return None

def update_loan(token, loan_id):
    url = f"{loan_base_url}/{loan_id}"
    payload = {
        "UserId": 1,  # Replace with the actual user ID
        "StartDate": datetime.now().isoformat(),
        "EndDate": (datetime.now() + timedelta(days=30)).isoformat(),
        "Amount": 1500,
        "InterestRate": 4.5,
        "Currency": "USD",
        "Status": "Active",  # Ensure this matches the expected enum value
        "LoanDescription": "Updated Test Loan"
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.put(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 204:
        print("Loan updated successfully.")
    else:
        print(f"Failed to update loan: {response.status_code} - {response.text}")

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

if __name__ == "__main__":
    # First, try to signup to get the token
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        # If user already exists, login to get the token
        send_sms("login")
        token = verify_sms("login")
    
    if token and token != "USER_EXISTS":
        # Create a loan
        loan_id = create_loan(token)
        
        if loan_id:
            # Get all loans
            get_loans(token)
            
            # Get the created loan
            get_loan(token, loan_id)
            
            # Update the created loan
            update_loan(token, loan_id)
            
            # Delete the created loan
            delete_loan(token, loan_id)