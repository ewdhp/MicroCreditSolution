using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroCredit.Services;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MicroCredit.Data;
using MicroCredit.Models;

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
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserFingerprintService _userFingerprintService;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration,
            ILogger<AuthController> logger, IJwtTokenService jwtTokenService,
            UserFingerprintService userFingerprintService, ApplicationDbContext context)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _serviceSid = configuration["Twilio:ServiceSid"];
            var authBytes = Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}");
            _twilioAuthHeader = Convert.ToBase64String(authBytes);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _twilioAuthHeader);
            _jwtTokenService = jwtTokenService;
            _userFingerprintService = userFingerprintService;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Signup request received for {PhoneNumber}", request.Phone);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model request");
                return BadRequest(new { message = "Invalid model request" });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
            if (existingUser != null)
            {
                _logger.LogInformation("User with phone number {PhoneNumber} already exists", request.Phone);
                return BadRequest(new { message = "User already exists" });
            }

            var result = await SendVerificationSms(request.Phone);

            if (result is OkObjectResult)
            {
                var fingerprint = _userFingerprintService.GenerateUserFingerprint(HttpContext);
                var token = GenerateToken(request.Phone, fingerprint);

                var newUser = new User
                {
                    Phone = request.Phone,
                    EncryptedPhase = "encryptedPhaseValue"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Verification SMS sent successfully. Please verify the code.", token });
            }

            return Ok(new { Message = "Signup successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Login request received for {PhoneNumber}", request.Phone);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model request");
                return BadRequest(new { message = "Invalid model request" });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
            if (existingUser == null)
            {
                _logger.LogInformation("User with phone number {PhoneNumber} does not exist", request.Phone);
                return BadRequest(new { message = "User does not exist" });
            }

            var result = await SendVerificationSms(request.Phone);

            if (result is OkObjectResult)
            {
                var fingerprint = _userFingerprintService.GenerateUserFingerprint(HttpContext);
                var token = GenerateToken(request.Phone, fingerprint);
                return Ok(new { message = "Verification SMS sent successfully. Please verify the code.", token });
            }

            return Ok(new { Message = "Login successful" });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Verification request received for {PhoneNumber}", request.Phone);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model request");
                return BadRequest(new { message = "Invalid model request" });
            }

            var url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/VerificationCheck";
            var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("To", request.Phone),
                new KeyValuePair<string, string>("Code", request.Code)
            });

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = payload
                };
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}")));

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Failed to verify code" });
                }

                var verificationResponse = JsonSerializer.Deserialize<TwilioVerificationResponse>(responseContent);
                if (verificationResponse.valid)
                {
                    return Ok(new { message = "Code verified successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Invalid verification code" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying the code");
                return StatusCode(500, new { message = "An error occurred while verifying the code" });
            }
        }

        private async Task<IActionResult> SendVerificationSms(string phoneNumber)
        {
            var sanitizedPhoneNumber = phoneNumber.Trim();
            var url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/Verifications";
            var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("To", sanitizedPhoneNumber),
                new KeyValuePair<string, string>("Channel", "sms")
            });

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = payload
                };
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}")));

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Failed to send verification SMS" });
                }

                var fingerprint = _userFingerprintService.GenerateUserFingerprint(HttpContext);
                var token = GenerateToken(sanitizedPhoneNumber, fingerprint);

                return Ok(new { message = "Verification SMS sent successfully. Please verify the code.", token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending verification SMS");
                return StatusCode(500, new { message = "An error occurred while sending verification SMS" });
            }
        }

        private string GenerateToken(string phoneNumber, string fingerprint)
        {
            var tokenData = $"{phoneNumber}:{fingerprint}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenData));
        }
    }

    public class SMSRequest
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^(signup|login|verify)$", ErrorMessage = "Action must be either 'signup', 'login', or 'verify'")]
        public string action { get; set; }

        [RegularExpression(@"^\+\d{10,15}$", ErrorMessage = "Phone number must be in E.164 format")]
        public string Phone { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")]
        public string Code { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Token { get; set; }
    }

    public class TwilioVerificationResponse
    {
        public string status { get; set; }
        public bool valid { get; set; }
    }
}