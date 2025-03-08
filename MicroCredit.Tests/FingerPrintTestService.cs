using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MicroCredit.Tests.Services
{
    public class FingerprintTestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FingerprintTestService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateFingerprint()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("No HTTP context available");
            }

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(userAgent))
            {
                throw new InvalidOperationException("IP address or User-Agent is missing");
            }

            var fingerprintSource = $"{ipAddress}-{userAgent}";
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fingerprintSource));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}