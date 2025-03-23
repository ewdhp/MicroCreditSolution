using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MicroCredit.Models;
using MicroCredit.Data;

namespace MicroCredit.Services
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(string phone, string fingerprint);
        void InvalidateToken(string userId);
        bool IsTokenInvalidated(string userId);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly UDbContext _context;
        private readonly ConcurrentDictionary<string, string>
        _invalidatedTokens = new ConcurrentDictionary<string, string>();

        public JwtTokenService(
            IConfiguration configuration,
            ILogger<JwtTokenService> logger,
            UDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public string GenerateJwtToken(string phone, string fingerprint)
        {
            _logger.LogInformation("Generating JWT token");

            var user = _context.Users
                .FirstOrDefault(u => u.Phone == phone);

            if (user == null)
                throw new Exception("User not found");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, phone),
                new Claim("PhoneNumber", phone),
                new Claim("Id", user.Id.ToString()),
                new Claim("Fingerprint", fingerprint),
                new Claim(JwtRegisteredClaimNames.Jti, 
                Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var tokenString = new 
            JwtSecurityTokenHandler()
            .WriteToken(token);

            _logger.LogInformation("Generated JWT token");

            return tokenString;
        }

        public void InvalidateToken(string userId)
        {
            _invalidatedTokens[userId] = userId;
        }

        public bool IsTokenInvalidated(string userId)
        {
            return _invalidatedTokens.ContainsKey(userId);
        }
    }

    public class FingerprintService
    {
        public string GenerateFingerprint(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var fingerprint = $"{ipAddress}-{userAgent}";
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(fingerprint);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}