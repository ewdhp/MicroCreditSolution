using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using MicroCredit.Services;

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
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly UserFingerprintService _userFingerprintService;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, UserFingerprintService userFingerprintService, JwtTokenService jwtTokenService)
        {
            _configuration = configuration;
            _logger = logger;
            _userFingerprintService = userFingerprintService;
            _jwtTokenService = jwtTokenService;
            _httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            var authBytes = Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}");
            _twilioAuthHeader = Convert.ToBase64String(authBytes);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _twilioAuthHeader);
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

            var sanitizedPhoneNumber = request.PhoneNumber.Trim();

            var url = $"https://verify.twilio.com/v2/Services/{ServiceSid}/Verifications";

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", sanitizedPhoneNumber),
                    new KeyValuePair<string, string>("Channel", "sms")
                });

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send verification SMS: {ResponseContent}", responseContent);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to send verification SMS" });
                }

                _logger.LogInformation("Verification SMS sent to {PhoneNumber}", sanitizedPhoneNumber);
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

            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Code))
            {
                _logger.LogWarning("Phone number and code are required");
                return BadRequest(new { message = "Phone number and code are required" });
            }

            var sanitizedPhoneNumber = request.PhoneNumber.Trim();
            var sanitizedCode = request.Code.Trim();

            var url = $"https://verify.twilio.com/v2/Services/{ServiceSid}/VerificationCheck";

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", sanitizedPhoneNumber),
                    new KeyValuePair<string, string>("Code", sanitizedCode)
                });

                var response = await _httpClient.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Verification failed for {PhoneNumber}: {ResponseContent}", sanitizedPhoneNumber, responseContent);
                    return StatusCode((int)response.StatusCode, new { message = "Verification failed" });
                }

                // Generate user fingerprint
                var fingerprint = _userFingerprintService.GenerateUserFingerprint(HttpContext);

                // Generate JWT token upon successful verification
                var token = _jwtTokenService.GenerateJwtToken(sanitizedPhoneNumber, fingerprint);
                _logger.LogInformation("Verification successful for {PhoneNumber}", sanitizedPhoneNumber);
                return Ok(new { token });
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
        public required string PhoneNumber { get; set; }
    }

    public class VerificationRequest
    {
        public required string PhoneNumber { get; set; }
        public required string Code { get; set; }
    }

    // ✅ Response model
    public class TwilioVerificationResponse
    {
        public string Sid { get; set; }
        public string ServiceSid { get; set; }
        public string AccountSid { get; set; }
        public string To { get; set; }
        public string Channel { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}