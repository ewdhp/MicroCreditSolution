import requests
import json

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

def create_loan(token, amount):
    url = f"{loan_base_url}/create"
    payload = {
        "Amount": amount  # Ensure the amount is a float
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.post(url, headers=headers, json=payload, verify=False)
    if response.status_code == 201:
        loan_id = response.json().get("id")
        print("Loan created successfully.")
        return loan_id
    else:
        print(f"Failed to create loan: {response.status_code} - {response.text}")
        return None

def update_loan_status(token, status):
    url = f"{loan_base_url}/update"
    payload = {
        "Status": status
    }
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }
    response = requests.put(url, headers=headers, json=payload, verify=False)
    if response.status_code == 204:
        print("Loan status updated successfully.")
    else:
        print(f"Failed to update loan status: {response.status_code} - {response.text}")

def get_current_loan(token):
    url = f"{loan_base_url}/current-loan"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        loan = response.json()
        print("Current loan retrieved successfully.")
        print(json.dumps(loan, indent=4))
        return loan
    else:
        print(f"Failed to retrieve current loan: {response.status_code} - {response.text}")
        return None

def delete_loan(token):
    url = f"{loan_base_url}"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.delete(url, headers=headers, verify=False)
    if response.status_code == 200:
        print("Loan deleted successfully.")
    else:
        print(f"Failed to delete loan: {response.status_code} - {response.text}")

def delete_all_loans(token):
    url = f"{loan_base_url}/all"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.delete(url, headers=headers, verify=False)
    if response.status_code == 200:
        print("All loans deleted successfully.")
    else:
        print(f"Failed to delete all loans: {response.status_code} - {response.text}")

def get_all_loans(token):
    url = f"{loan_base_url}/all"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        loans = response.json()
        print("All loans retrieved successfully.")
        print(json.dumps(loans, indent=4))
        return loans
    else:
        print(f"Failed to retrieve all loans: {response.status_code} - {response.text}")
        return None
def are_all_loans_paid(token):
    url = f"{loan_base_url}/are-all-paid"  # Adjust the endpoint as per your API
    headers = {
        "Authorization": f"Bearer {token}"
    }
    response = requests.get(url, headers=headers, verify=False)
    if response.status_code == 200:
        result = response.json()
        all_paid = result.get("allPaid")
        loan = result.get("loan")
        print("Are all loans paid:", all_paid)
        if loan:
            print("Unpaid loan details:")
            print(json.dumps(loan, indent=4))
        return all_paid, loan
    else:
        print(f"Failed to check if all loans are paid: {response.status_code} - {response.text}")
        return None, None
    
if __name__ == "__main__":
    # First, try to signup to get the token
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        # If user already exists, login to get the token
        send_sms("login")
        token = verify_sms("login")
    
    if token and token != "USER_EXISTS":
        
     
            # Check if all loans are paid
            all_paid, unpaid_loan = are_all_loans_paid(token)
            if all_paid:
                print("All loans are paid.")
            elif unpaid_loan:
                print("There are unpaid loans.")
                print(json.dumps(unpaid_loan, indent=4))


""" 
   # Retrieve all loans
        all_loans = get_all_loans(token)

        amount = 150.0  # Example amount
        loan_id = create_loan(token, amount)
        print(f"Loan id: {loan_id}")
        
        if loan_id: 
            # Update the status of the created loan
            update_loan_status(token, 1)  # Use integer value for status
            
            # Retrieve the current loan
            current_loan = get_current_loan(token)

            # Introduce a delay of 1 second
            time.sleep(1)
"""
    

