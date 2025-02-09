using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroCredit.Services;
using System.ComponentModel.DataAnnotations; // Add this namespace

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string AccountSid = "AC23f88289374bd1212027f88ec0bf0c27";
        private const string AuthToken = "fedc8978719541bd4f46c9a7bc3875ae";
        private const string ServiceSid = "VAc6245af6c94f63ff1903cb8024c918ad";
        private readonly string _twilioAuthHeader;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtTokenService _jwtTokenService;
        private readonly UserFingerprintService _userFingerprintService;

        public AuthController(ILogger<AuthController> logger, JwtTokenService jwtTokenService, UserFingerprintService userFingerprintService)
        {
            _httpClient = new HttpClient();
            var authBytes = Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}");
            _twilioAuthHeader = Convert.ToBase64String(authBytes);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _twilioAuthHeader);
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _userFingerprintService = userFingerprintService;
        }

        // ✅ Send verification SMS
        [HttpPost("send")]
        public async Task<IActionResult> SendVerificationSms([FromBody] PhoneNumberRequest request)
        {
            _logger.LogInformation("Received request to send verification SMS to {PhoneNumber}", request.PhoneNumber);

            if (string.IsNullOrEmpty(request.PhoneNumber) || !request.PhoneNumber.StartsWith("+"))
            {
                _logger.LogWarning("Invalid phone number format: {PhoneNumber}", request.PhoneNumber);
                return BadRequest(new { message = "Phone number must be in E.164 format (e.g., +1234567890)" });
            }

            var url = $"https://verify.twilio.com/v2/Services/{ServiceSid}/Verifications";
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
        public async Task<IActionResult> VerifyCode([FromBody] VerificationRequest request)
        {
            _logger.LogInformation("Received request to verify code for {PhoneNumber}", request.PhoneNumber);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedPhoneNumber = request.PhoneNumber.Trim();

            var url = $"https://verify.twilio.com/v2/Services/{ServiceSid}/VerificationCheck";
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