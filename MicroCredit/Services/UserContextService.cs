using System;
using Microsoft.AspNetCore.Http;

namespace MicroCredit.Services
{
    public interface IUserContextService
    {
        Guid GetUserId();
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpca;

        public UserContextService(IHttpContextAccessor httpca)
        {
            _httpca = httpca;
        }

        public Guid GetUserId()
        {
            var user = _httpca
            .HttpContext?.User;
            var userIdClaim = user?
            .FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim,
                out var userId))
            {
                throw new UnauthorizedAccessException
                ("Invalid or missing User ID claim.");
            }

            return userId;
        }
    }

}