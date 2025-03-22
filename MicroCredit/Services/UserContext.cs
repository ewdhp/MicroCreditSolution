using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace MicroCredit.Services
{
    public interface IUCService
    {
        Guid GetUserId();
    }

    public class UserContextService
    (IHttpContextAccessor hca) : IUCService
    {
        private readonly IHttpContextAccessor _hca = hca;

        public Guid GetUserId()
        {
            var userIdClaim = _hca.HttpContext?.User?
            .Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException
                ("Invalid or missing User ID claim.");

            return userId;
        }
    }
}