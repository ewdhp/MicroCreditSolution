import requests
from requests.auth import HTTPBasicAuth

import requests
from requests.auth import HTTPBasicAuth

account_sid = 'AC23f88289374bd1212027f88ec0bf0c27'
auth_token = '2269e69c15c86407bbdec1a486657b0a'
service_sid = 'VAc6245af6c94f63ff1903cb8024c918ad'
phone_number = '+1234567890'

url = f'https://verify.twilio.com/v2/Services/{service_sid}/Verifications'
payload = {
    'To': phone_number,
    'Channel': 'sms'
}

response = requests.post(url, data=payload, auth=HTTPBasicAuth(account_sid, auth_token))

print(response.status_code)
print(response.json())
