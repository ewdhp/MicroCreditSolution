import requests
import json
from concurrent.futures import ThreadPoolExecutor

# Configuration
auth_base_url = "https://localhost:5001/api/testauth"
loan_base_url = "https://localhost:5001/api/loan/next"
phone_number = "+523321890176"
verification_code = "123456"

# Disable SSL warnings (not recommended for production)
requests.packages.urllib3.disable_warnings(requests.packages.urllib3.exceptions.InsecureRequestWarning)

def send_sms(action):
    """Send an SMS for signup or login."""
    url = f"{auth_base_url}/send"
    payload = {
        "action": action,
        "Phone": phone_number
    }
    headers = {"Content-Type": "application/json"}
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        print("Verification SMS sent successfully.")
    else:
        print(f"Failed to send verification SMS: {response.status_code} - {response.text}")

def verify_sms(action):
    """Verify the SMS code and retrieve a token."""
    url = f"{auth_base_url}/verify"
    payload = {
        "action": action,
        "Phone": phone_number,
        "Code": verification_code
    }
    headers = {"Content-Type": "application/json"}
    response = requests.post(url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        token = response.json().get("token")
        print("Verification successful. Token received.")
        return token
    elif response.status_code == 400 and "User already exists" in response.text:
        print("User already exists. Switching to login.")
        return "USER_EXISTS"
    else:
        print(f"Failed to verify code: {response.status_code} - {response.text}")
        return None

def test_phase_service(token, payload):
    """Send a request to the PhaseService."""
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }
    response = requests.post(loan_base_url, headers=headers, data=json.dumps(payload), verify=False)
    if response.status_code == 200:
        print("Phase processed successfully. Response:")
        print(json.dumps(response.json(), indent=4))
    else:
        print(f"Phase processing failed: {response.status_code} - {response.text}")

def test_max_concurrent_requests(token, payload, max_concurrent_calls):
    """Test maximum concurrent requests."""
    def send_request():
        test_phase_service(token, payload)

    print(f"\nTesting with {max_concurrent_calls} concurrent calls...")
    with ThreadPoolExecutor(max_workers=max_concurrent_calls) as executor:
        futures = [executor.submit(send_request) for _ in range(max_concurrent_calls)]
        for future in futures:
            future.result()

if __name__ == "__main__":
    # Step 1: Send SMS for signup
    send_sms("signup")
    token = verify_sms("signup")
    
    if token == "USER_EXISTS":
        # If the user already exists, switch to login
        send_sms("login")
        token = verify_sms("login")
    
    if not token:
        print("Failed to retrieve token. Exiting.")
        exit(1)

    # Test maximum concurrent requests
    payload = {"type": "Pre", "LoanData": None, "Amount": 100, "Data": None, "payMethod": None}
    max_concurrent_calls = 5  # Adjust this value to test different levels of concurrency
    test_max_concurrent_requests(token, payload, max_concurrent_calls)