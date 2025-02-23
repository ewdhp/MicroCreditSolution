using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System;

namespace MicroCredit.Services
{
    public class UserFingerprintService
    {
        public string GenerateUserFingerprint(HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
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