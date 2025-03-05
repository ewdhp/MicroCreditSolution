using MicroCredit.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCredit.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtMiddleware> _logger;
        private readonly FingerprintService _fingerprintService;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger, FingerprintService fingerprintService)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
            _fingerprintService = fingerprintService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                _logger.LogInformation("Token found in request: {Token}", token);
                AttachUserToContext(context, token);
            }
            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Issuer"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;
                var fingerprint = jwtToken.Claims.First(x => x.Type == "Fingerprint").Value;

                // Validate fingerprint
                var requestFingerprint = _fingerprintService.GenerateFingerprint(context);
                _logger.LogInformation("Token Fingerprint: {TokenFingerprint}, Request Fingerprint: {RequestFingerprint}", fingerprint, requestFingerprint);
                if (fingerprint != requestFingerprint)
                {
                    _logger.LogWarning("Invalid fingerprint. Expected: {Expected}, Actual: {Actual}", fingerprint, requestFingerprint);
                    throw new SecurityTokenException("Invalid fingerprint");
                }

                _logger.LogInformation("Token validated successfully. UserId: {UserId}", userId);
                context.Items["UserId"] = userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                // Do nothing if JWT validation fails
                // User is not attached to context so request won't have access to secure routes
            }
        }
    }
}