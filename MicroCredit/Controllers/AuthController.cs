using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Services;
using MicroCredit.Data;
using MicroCredit.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/testauth")]
    public class TestAuthController : ControllerBase
    {
        private readonly ILogger<TestAuthController> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ApplicationDbContext _context;
        private readonly FingerprintService _fingerprintService;
        private readonly IUserContextService _userContextService;

        public TestAuthController(
            ILogger<TestAuthController> logger,
            IJwtTokenService jwtTokenService,
            ApplicationDbContext context,
            FingerprintService fingerprintService,
            IUserContextService userContextService)
        {
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _context = context;
            _fingerprintService = fingerprintService;
            _userContextService = userContextService;
        }

        [HttpPost("send")]
        public IActionResult SendSMS([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Simulating sending verification SMS to {PhoneNumber}", request.Phone);

            if (string.IsNullOrEmpty(request.Phone) || !request.Phone.StartsWith("+"))
            {
                _logger.LogWarning("Invalid phone number format: {PhoneNumber}", request.Phone);
                return BadRequest(new { message = "Phone number must be in E.164 format (e.g., +1234567890)" });
            }

            _logger.LogInformation("Verification SMS simulated for {PhoneNumber}", request.Phone);
            return Ok(new { message = "Verification SMS simulated" });
        }

        [HttpPost("verify")]
        public IActionResult VerifySMS([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Simulating verification for {PhoneNumber}", request.Phone);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Invalid model request");
                return BadRequest(new { message = "Invalid model request" });
            }

            if (request.Code != "123456") // Simulate a fixed verification code
            {
                return BadRequest(new { message = "Invalid verification code" });
            }

            if (request.Action == "signup")
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
                if (existingUser != null)
                {
                    _logger.LogInformation("User with phone number {PhoneNumber} already exists", request.Phone);
                    return BadRequest(new { message = "User already exists" });
                }

                var fingerprint = _fingerprintService.GenerateFingerprint(HttpContext);
                _logger.LogInformation("User fingerprint generated successfully for {PhoneNumber}", request.Phone);

                var newUser = new User
                {
                    Phone = request.Phone,
                    Fingerprint = fingerprint,
                    Phase = 1,
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                _logger.LogInformation("User created and saved successfully for {PhoneNumber}", request.Phone);

                var token = GenerateToken(request.Phone, fingerprint);
                _logger.LogInformation("Token generated successfully for {PhoneNumber}", request.Phone);

                return Ok(new { message = "Signup successful", token });
            }
            else if (request.Action == "login")
            {
                if (!string.IsNullOrEmpty(request.Token))
                {
                    // Validate the provided token
                    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    try
                    {
                        var jwtToken = tokenHandler.ReadJwtToken(request.Token);
                        var phoneNumber = jwtToken.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
                        var tokenFingerprint = jwtToken.Claims.FirstOrDefault(c => c.Type == "Fingerprint")?.Value;

                        _logger.LogInformation("Token Claims - PhoneNumber: {PhoneNumber}, Fingerprint: {Fingerprint}", phoneNumber, tokenFingerprint);

                        if (phoneNumber != request.Phone)
                        {
                            _logger.LogWarning("Token phone number does not match request phone number");
                            return BadRequest(new { message = "Invalid token" });
                        }

                        var user = _context.Users.FirstOrDefault(u => u.Phone == phoneNumber);
                        if (user == null)
                        {
                            _logger.LogInformation("User with phone number {PhoneNumber} not found", request.Phone);
                            return NotFound(new { message = "User not found" });
                        }

                        var fingerprint = _fingerprintService.GenerateFingerprint(HttpContext);
                        if (fingerprint != tokenFingerprint)
                        {
                            _logger.LogWarning("Invalid fingerprint. Expected: {Expected}, Actual: {Actual}", fingerprint, tokenFingerprint);
                            return BadRequest(new { message = "Invalid fingerprint" });
                        }

                        _logger.LogInformation("Token validated successfully for {PhoneNumber}", request.Phone);
                        return Ok(new { message = "Login successful", token = request.Token });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Invalid token");
                        return BadRequest(new { message = "Invalid token" });
                    }
                }
                else
                {
                    var user = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
                    if (user == null)
                    {
                        _logger.LogInformation("User with phone number {PhoneNumber} not found", request.Phone);
                        return NotFound(new { message = "User not found" });
                    }

                    var fingerprint = _fingerprintService.GenerateFingerprint(HttpContext);
                    _logger.LogInformation("User fingerprint generated successfully for {PhoneNumber}", request.Phone);

                    var token = GenerateToken(request.Phone, fingerprint);
                    _logger.LogInformation("Token generated successfully for {PhoneNumber}", request.Phone);

                    return Ok(new { message = "Login successful", token });
                }
            }

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
        public string Action { get; set; }

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
}