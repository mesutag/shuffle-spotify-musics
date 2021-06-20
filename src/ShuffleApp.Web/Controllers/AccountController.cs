using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication6.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            var action = Url.Action(nameof(LoginCallback));
            return new ChallengeResult(
                AspNet.Security.OAuth.Spotify.SpotifyAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = action
                });
        }

        public async Task<IActionResult> LoginCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");

            if (!authenticateResult.Succeeded)
                return BadRequest(); // TODO: Handle this better.

            var claimsIdentity = new ClaimsIdentity("Application");

            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Name));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));

            await HttpContext.SignInAsync(
                "Application",
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect("~/");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                "Application");

            return LocalRedirect("~/");
        }
    }
}
