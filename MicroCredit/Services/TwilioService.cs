using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class TwilioService
{
    private readonly string _accountSid = "AC23f88289374bd1212027f88ec0bf0c27";
    private readonly string _authToken = "fedc8978719541bd4f46c9a7bc3875ae";
    private readonly string _serviceSid = "VAc6245af6c94f63ff1903cb8024c918ad";
    private readonly HttpClient _httpClient;

    public TwilioService()
    {
        _httpClient = new HttpClient();
        var authTokenBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authTokenBase64);
    }

    // Send a verification code
    public async Task<bool> SendCode(string phoneNumber)
    {
        // Ensure phone number is in E.164 format
        if (!phoneNumber.StartsWith("+"))
        {
            throw new ArgumentException("Phone number must be in E.164 format, e.g., +1234567890");
        }

        string url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/Verifications";
        var payload = new { To = phoneNumber, Channel = "sms" };

        var response = await _httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VerifyCode(string phoneNumber, string code)
    {
        if (!phoneNumber.StartsWith("+"))
        {
            throw new ArgumentException("Phone number must be in E.164 format, e.g., +1234567890");
        }

        string url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/VerificationCheck";
        var payload = new { To = phoneNumber, Code = code };

        var response = await _httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            var responseData = JsonSerializer.Deserialize<TwilioVerificationResponse>(await response.Content.ReadAsStringAsync());
            return responseData != null && responseData.Status == "approved";
        }

        return false;
    }

}

// Response model for verification check
public class TwilioVerificationResponse
{
    public required string Status { get; set; }
}
