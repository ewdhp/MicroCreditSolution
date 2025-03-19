using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("{provider}")]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action
            (
                nameof(ExternalLoginCallback),
                "Auth", new { provider }
            );
            var properties = new AuthenticationProperties
            { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [HttpGet("{provider}/callback")]
        public async Task<IActionResult> ExternalLoginCallback(string provider)
        {
            var result = await HttpContext.AuthenticateAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest("External authentication error");
            }

            var token = GenerateToken(result.Principal);
            return Ok(new
            {
                success = true,
                message = "Authentication successful",
                token
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true, message = "Logged out" });
        }

        private string GenerateToken(ClaimsPrincipal principal)
        {
            // Implement your token generation logic here
            return "generated_token";
        }
    }
}