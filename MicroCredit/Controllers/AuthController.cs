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
        private HttpClient _httpClient;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _serviceSid;
        private readonly string _twilioAuthHeader;
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly FingerprintService _userFingerprintService;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public AuthController(
            IConfiguration configuration,
            ILogger<AuthController> logger,
            IJwtTokenService jwtTokenService,
            FingerprintService userFingerprintService,
            ApplicationDbContext context, HttpClient httpClient)
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _serviceSid = Environment.GetEnvironmentVariable("TWILIO_SERVICE_SID");
            _twilioAuthHeader = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}"));
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _userFingerprintService = userFingerprintService;
            _context = context;
            _httpClient = httpClient;

            // Log the Twilio credentials for debugging
            _logger.LogInformation("Twilio AccountSid: {AccountSid}", _accountSid);
            _logger.LogInformation("Twilio AuthToken: {AuthToken}", _authToken);
            _logger.LogInformation("Twilio ServiceSid: {ServiceSid}", _serviceSid);

            // Store the connection string as a class member property
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("Connection string 'DefaultConnection' is not configured.");
            }
            else
            {
                _logger.LogInformation("Using connection string: {ConnectionString}", _connectionString);
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSMS([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Received request to send verification SMS to {PhoneNumber}", request.Phone);

            if (string.IsNullOrEmpty(request.Phone) || !request.Phone.StartsWith("+"))
            {
                _logger.LogWarning("Invalid phone number format: {PhoneNumber}", request.Phone);
                return BadRequest(new { message = "Phone number must be in E.164 format (e.g., +1234567890)" });
            }

            var sanitizedPhoneNumber = request.Phone.Trim();
            var url = $"https://verify.twilio.com/v2/Services/{_serviceSid}/Verifications";

            // Log Twilio credentials and URL
            _logger.LogInformation("Twilio Credentials: AccountSid = {AccountSid}, AuthToken = {AuthToken}, ServiceSid = {ServiceSid}", _accountSid, _authToken, _serviceSid);
            _logger.LogInformation("Sending request to Twilio API with URL: {Url}", url);

            try
            {
                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", sanitizedPhoneNumber),
                    new KeyValuePair<string, string>("Channel", "sms")
                });

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = requestData
                };
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}"))
                );

                var response = await _httpClient.SendAsync(requestMessage);

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

        [HttpPost("verify")]
        public async Task<IActionResult> VerifySMS([FromBody] SMSRequest request)
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
                    if (request.action == "signup")
                    {
                        var existingUser = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
                        if (existingUser != null)
                        {
                            _logger.LogInformation("User with phone number {PhoneNumber} already exists", request.Phone);
                            return BadRequest(new { message = "User already exists" });
                        }

                        var fingerprint = _userFingerprintService.GenerateFingerprint(HttpContext);
                        _logger.LogInformation("User fingerprint generated successfully for {PhoneNumber}", request.Phone);

                        var token = GenerateToken(request.Phone, fingerprint);
                        _logger.LogInformation("Token generated successfully for {PhoneNumber}", request.Phone);

                        var newUser = new User
                        {
                            Phone = request.Phone,
                            Fingerprint = fingerprint,
                            Phase = 1,
                        };
                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User created and saved successfully for {PhoneNumber}", request.Phone);

                        return Ok(new { message = "Signup successful", token });
                    }
                    else if (request.action == "login")
                    {
                        var user = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
                        if (user == null)
                        {
                            _logger.LogInformation("User with phone number {PhoneNumber} not found", request.Phone);
                            return NotFound(new { message = "User not found" });
                        }

                        var fingerprint = _userFingerprintService.GenerateFingerprint(HttpContext);
                        _logger.LogInformation("User fingerprint generated successfully for {PhoneNumber}", request.Phone);

                        var token = GenerateToken(request.Phone, fingerprint);
                        _logger.LogInformation("Token generated successfully for {PhoneNumber}", request.Phone);

                        // Decode the token to retrieve user data
                        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(token);
                        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                        var tokenFingerprint = jwtToken.Claims.FirstOrDefault(c => c.Type == "Fingerprint")?.Value;

                        _logger.LogInformation("Decoded Token Details - UserId: {UserId}, Fingerprint: {Fingerprint}", userId, tokenFingerprint);

                        return Ok(new { message = "Login successful", token });
                    }
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

            // Default return in case no other return statement is hit
            return BadRequest(new { message = "Unexpected error occurred" });
        }

        private string GenerateToken(string phoneNumber, string fingerprint)
        {
            return _jwtTokenService.GenerateJwtToken(phoneNumber, fingerprint);
        }
    }

    public class SMSRequest
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^(signup|login|verify)$",
        ErrorMessage = "Action must be either 'signup', 'login', or 'verify'")]
        public string action { get; set; }

        [RegularExpression(@"^\+\d{10,15}$",
        ErrorMessage = "Phone number must be in E.164 format")]
        public string Phone { get; set; }

        [RegularExpression(@"^\d{6}$",
        ErrorMessage = "Code must be exactly 6 digits")]
        public string Code { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$",
        ErrorMessage = "Name must contain only letters")]
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