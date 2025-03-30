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

        public AuthController
        (
            UDbContext context,
            ILogger<AuthController> logger,
            IJwtTokenService jwtTkService,
            FingerprintService fpService,
            IUCService ucs
            )
        {
            _context = context;
            _logger = logger;
            _jwtTkService = jwtTkService;
            _fpService = fpService;
            _ucs = ucs;
        }

        [HttpPost("send")]
        public IActionResult SendSMS([FromBody] SMSRequest request)
        {
            if (string.IsNullOrEmpty(request.Phone) ||
                !request.Phone.StartsWith("+"))
            {
                _logger.LogWarning("Invalid phone format");
                return BadRequest(new
                {
                    message = "Phone number must be in E.164 format"
                });
            }

            _logger.LogInformation
            ("Code request sent for phone: {Phone}", request.Phone);
            return Ok(new { message = "Code request sent" });
        }

        [HttpPost("verify")]
        public IActionResult VerifySMS([FromBody] SMSRequest request)
        {
            _logger.LogInformation("Simulating verification");

            if (!ModelState.IsValid)
            {
                return BadRequest
                (new { message = "Invalid request model" });
            }

            if (request.Code != "123456")
            {
                return BadRequest
                (new { message = "Invalid code" });
            }

            try
            {
                var existingUser = _context
                .Users.FirstOrDefault
                (u => u.Phone == request.Phone);
                if (existingUser != null)
                {
                    if (!string.IsNullOrEmpty(request.Token))
                    {
                        try
                        {
                            var tokenHandler = new System
                            .IdentityModel.Tokens.Jwt
                            .JwtSecurityTokenHandler();
                            var jwtToken = tokenHandler
                            .ReadJwtToken(request.Token);
                            var phone = jwtToken.Claims
                            .FirstOrDefault
                            (c => c.Type == "PhoneNumber")?
                            .Value;

                            if (phone != request.Phone)
                            {
                                return BadRequest(new
                                {
                                    message = "Phone does not match"
                                });
                            }

                            var expClaim = jwtToken
                            .Claims.FirstOrDefault
                            (c => c.Type == "exp")?.Value;
                            if (expClaim != null && long
                            .TryParse(expClaim, out var exp))
                            {
                                var expDate = DateTimeOffset
                                .FromUnixTimeSeconds(exp).UtcDateTime;
                                if (expDate < DateTime.UtcNow)
                                {
                                    throw new
                                    SecurityTokenExpiredException
                                    ("Token has expired");
                                }
                            }

                            var fingerprint = _fpService
                            .GenerateFingerprint(HttpContext);
                            var tokenFingerprint = jwtToken.Claims
                            .FirstOrDefault(c => c.Type == "Fingerprint")?
                            .Value;

                            if (fingerprint != tokenFingerprint)
                            {
                                return BadRequest(new
                                {
                                    message = "Invalid fingerprint"
                                });
                            }
                            return Ok(new
                            {
                                message = "Login successful",
                                token = request.Token,
                                loginProviders = existingUser.LoginProviders
                            });
                        }
                        catch (SecurityTokenExpiredException ex)
                        {
                            _logger.LogWarning
                            (ex, "Token has expired");
                            return BadRequest
                            (new { message = "Token has expired" });
                        }
                    }

                    return Ok(new
                    {
                        message = "Login successful",
                        token = GenerateToken(request.Phone,
                        _fpService.GenerateFingerprint(HttpContext))
                    });
                }

                var newUser = new User
                {
                    Phone = request.Phone,
                    Name = "Usuario",
                    RegDate = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var token = GenerateToken(request.Phone,
                _fpService.GenerateFingerprint(HttpContext));
                _logger.LogInformation("Signup successful");

                return Ok(new
                {
                    message = "Signup successful",
                    token,
                    loginProviders = newUser.LoginProviders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError
                (ex, "Error during verification");
                return StatusCode(StatusCodes
                .Status500InternalServerError, new
                {
                    message = "Internal server error"
                });
            }
        }

        [HttpGet("login-providers")]
        public IActionResult GetLoginProviders()
        {
            var phone = HttpContext
            .User.Claims.FirstOrDefault
            (c => c.Type == "PhoneNumber")?.Value;
            if (string.IsNullOrEmpty(phone))
            {
                _logger.LogWarning
                ("Phone number not found in token");
                return Unauthorized(new
                {
                    message = "Unauthorized access"
                });
            }
            var user = _context
            .Users.FirstOrDefault
            (u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            return Ok(new
            {
                loginProviders = user.LoginProviders
            });
        }

        [HttpPost("add-login-provider")]
        public IActionResult AddLoginProvider([FromBody] string provider)
        {
            var phone = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;

            if (string.IsNullOrEmpty(phone))
            {
                _logger.LogWarning("Phone number not found in token");
                return Unauthorized(new
                {
                    message = "Unauthorized access"
                });
            }

            var user = _context.Users
            .FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            if (user.LoginProviders
                .Contains(provider))
            {
                return BadRequest(new
                {
                    message = "Provider already exists"
                });
            }

            user.LoginProviders.Add(provider);
            _context.SaveChanges();

            _logger.LogInformation("Added login provider");

            return Ok(new
            {
                message = "Login provider added",
                loginProviders = user.LoginProviders
            });
        }
        private string GenerateToken
        (string phone, string fingerprint)
        {
            return _jwtTkService
            .GenerateJwtToken(phone, fingerprint);
        }
    }

    public class SMSRequest
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^(signup|login)$",
        ErrorMessage = "Action must be 'signup' or 'login'")]
        public string Action { get; set; }

        [Required]
        [RegularExpression(@"^\+\d{10,15}$",
        ErrorMessage = "Phone number must be in E.164 format")]
        public string Phone { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z]+$",
        ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }

        [RegularExpression(@"^\d{6}$",
        ErrorMessage = "Code must be exactly 6 digits")]
        public string Code { get; set; }

        [MaxLength(500)]
        public string Token { get; set; }
    }
}