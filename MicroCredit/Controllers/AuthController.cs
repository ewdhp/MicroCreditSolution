// filepath: /home/ewd/MicroCreditSolution/MicroCredit/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // Add this namespace
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroCredit.Services;
using System.ComponentModel.DataAnnotations;

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _serviceSid;
        private readonly string _twilioAuthHeader;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtTokenService _jwtTokenService;
        private readonly UserFingerprintService _userFingerprintService;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, JwtTokenService jwtTokenService, UserFingerprintService userFingerprintService)
        {
            _httpClient = new HttpClient();
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _serviceSid = configuration["Twilio:ServiceSid"];
            var authBytes = Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}");
            _twilioAuthHeader = Convert.ToBase64String(authBytes);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _twilioAuthHeader);
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _userFingerprintService = userFingerprintService;

            // Log the Twilio configuration values to verify they are being loaded correctly
            _logger.LogInformation("Twilio AccountSid: {AccountSid}", _accountSid);
            _logger.LogInformation("Twilio AuthToken: {AuthToken}", _authToken);
            _logger.LogInformation("Twilio ServiceSid: {ServiceSid}", _serviceSid);
        }

        // ✅ Send verification SMS
        [HttpPost("send")]
        public async Task<IActionResult> SendSms([FromBody] PhoneNumberRequest request)
        {
            _logger.LogInformation("Received request to send verification SMS to {PhoneNumber}", request.PhoneNumber);

            if (string.IsNullOrEmpty(request.PhoneNumber) || !request.PhoneNumber.StartsWith("+"))
            {
                _logger.LogWarning("Invalid phone number format: {PhoneNumber}", request.PhoneNumber);
                return BadRequest(new { message = "Phone number must be in E.164 format (e.g., +1234567890)" });
            }

            var url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/Verifications";
            var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("To", request.PhoneNumber),
                new KeyValuePair<string, string>("Channel", "sms")
            });

            try
            {
                _logger.LogInformation("Sending request to Twilio API: {Url} with payload: {Payload}", url, JsonSerializer.Serialize(payload));
                var response = await _httpClient.PostAsync(url, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send verification SMS. Status Code: {StatusCode}, Response: {ResponseContent}", response.StatusCode, responseContent);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to send verification SMS" });
                }

                _logger.LogInformation("Verification SMS sent to {PhoneNumber}", request.PhoneNumber);
                return Ok(new { message = "Verification SMS sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending verification SMS");
                return StatusCode(500, new { message = "An error occurred while sending verification SMS" });
            }
        }

        // ✅ Verify code
        [HttpPost("verify")]
        public async Task<IActionResult> VerifySms([FromBody] VerificationRequest request)
        {
            _logger.LogInformation("Received request to verify code for {PhoneNumber}", request.PhoneNumber);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedPhoneNumber = request.PhoneNumber.Trim();

            var url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/VerificationCheck";
            var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("To", sanitizedPhoneNumber),
                new KeyValuePair<string, string>("Code", request.Code)
            });

            try
            {
                _logger.LogInformation("Sending request to Twilio API: {Url} with payload: {Payload}", url, JsonSerializer.Serialize(payload));
                var response = await _httpClient.PostAsync(url, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Received response from Twilio API: {ResponseContent}", responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Verification failed for {PhoneNumber}. Status Code: {StatusCode}, Response: {ResponseContent}", sanitizedPhoneNumber, response.StatusCode, responseContent);
                    return StatusCode((int)response.StatusCode, new { message = "Verification failed" });
                }

                var responseData = JsonSerializer.Deserialize<TwilioVerificationResponse>(responseContent);

                if (responseData.status == "approved" && responseData.valid)
                {
                    // Generate user fingerprint
                    var fingerprint = _userFingerprintService.GenerateUserFingerprint(HttpContext);

                    // Generate JWT token upon successful verification
                    var token = _jwtTokenService.GenerateJwtToken(sanitizedPhoneNumber, fingerprint);

                    _logger.LogInformation("Verification successful for {PhoneNumber}", sanitizedPhoneNumber);
                    return Ok(new { message = "Verification successful", token });
                }
                else
                {
                    _logger.LogWarning("Invalid code for {PhoneNumber}", sanitizedPhoneNumber);
                    return BadRequest(new { message = "Invalid code" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying the code for {PhoneNumber}", sanitizedPhoneNumber);
                return StatusCode(500, new { message = "An error occurred while verifying the code" });
            }
        }
    }

    // ✅ Request models
    public class PhoneNumberRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class VerificationRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+\d{10,15}$", ErrorMessage = "Phone number must be in E.164 format (e.g., +1234567890) and have between 10 and 15 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
    }

    // ✅ Response model
    public class TwilioVerificationResponse
    {
        public string status { get; set; } // Change to lowercase to match the JSON response
        public bool valid { get; set; } // Change to lowercase to match the JSON response
    }
}