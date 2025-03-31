using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Services;
using MicroCredit.Data;
using MicroCredit.Models;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/testauth")]
    public class AuthController : ControllerBase
    {
        private readonly UDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtTokenService _jwtTkService;
        private readonly FingerprintService _fpService;
        private readonly IUCService _ucs;
        private readonly IConfiguration _configuration;

        public AuthController
        (
            UDbContext context,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            IJwtTokenService jwtTkService,
            FingerprintService fpService,
            IUCService ucs
        )
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _jwtTkService = jwtTkService;
            _fpService = fpService;
            _ucs = ucs;
        }

        [HttpPost("send")]
        public IActionResult SendSMS([FromBody] SMSRequest request)
        {
            if (string.IsNullOrEmpty(request.Phone) || !request.Phone.StartsWith("+"))
            {
                _logger.LogWarning("Invalid phone format");
                return BadRequest(new { message = "Phone number must be in E.164 format" });
            }

            _logger.LogInformation("Code request sent for phone: {Phone}", request.Phone);
            return Ok(new { message = "Code request sent" });
        }

        [HttpPost("verify")]
        public IActionResult VerifySMS([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Simulating verification");

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request model" });
            }

            if (request.Code != "123456")
            {
                return BadRequest(new { message = "Invalid code" });
            }

            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);
                if (existingUser != null)
                {
                    // Generate a token for an existing user
                    var token = GenerateToken(
                        request.Phone,
                        _fpService.GenerateFingerprint(HttpContext),
                        existingUser.Id, // Pass the userId
                        TimeSpan.FromHours(1) // 1-hour expiration
                    );

                    return Ok(new
                    {
                        message = "Login successful",
                        token = token,
                        loginProviders = existingUser.LoginProviders
                    });
                }

                // Create a new user and generate a token
                var newUser = new User
                {
                    Phone = request.Phone,
                    Name = "Usuario",
                    RegDate = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var newToken = GenerateToken(
                    request.Phone,
                    _fpService.GenerateFingerprint(HttpContext),
                    newUser.Id, // Pass the userId
                    TimeSpan.FromHours(2) // 2-hour expiration for new users
                );

                _logger.LogInformation("Signup successful");

                return Ok(new
                {
                    message = "Signup successful",
                    token = newToken,
                    loginProviders = newUser.LoginProviders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during verification");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("login-providers")]
        public IActionResult GetLoginProviders()
        {
            var phone = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phone))
            {
                _logger.LogWarning("Phone number not found in token");
                return Unauthorized(new { message = "Unauthorized access" });
            }

            var user = _context.Users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { loginProviders = user.LoginProviders });
        }

        [HttpPost("add-login-provider")]
        public IActionResult AddLoginProvider([FromBody] LoginProviderReq request)
        {
            var phone = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phone))
            {
                _logger.LogWarning("Phone number not found");
                return Unauthorized(new { message = "Unauthorized access" });
            }

            var user = _context.Users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (user.LoginProviders.Contains(request.Provider))
            {
                return BadRequest(new { message = "Provider already exists" });
            }

            user.LoginProviders.Add(request.Provider);
            _context.SaveChanges();

            _logger.LogInformation("Added login provider");

            return Ok(new
            {
                message = "Login provider added",
                loginProviders = user.LoginProviders
            });
        }

        private string GenerateToken(string phoneNumber, string fingerprint, Guid userId, TimeSpan expiration)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            {
                throw new ArgumentException("JWT Key must be at least 32 characters long.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, phoneNumber),
                new Claim("PhoneNumber", phoneNumber),
                new Claim("Fingerprint", fingerprint),
                new Claim("Id", userId.ToString()) // Add the UserId claim
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Optional: Log the generated token
            _logger.LogInformation("Generated JWT Token: {Token}", tokenString);

            return tokenString;
        }
    }

    public class LoginProviderReq
    {
        [Required]
        [MaxLength(50)]
        public string Provider { get; set; }
    }

    public class SMSRequest
    {
        [Required]
        [RegularExpression(@"^\+\d{10,15}$", ErrorMessage = "Phone number must be in E.164 format")]
        public string Phone { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits")]
        public string Code { get; set; }

        [MaxLength(500)]
        public string Token { get; set; }
    }
}