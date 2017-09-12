using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using PortalBuddyWebApp.Extensions;

namespace PortalBuddyWebApp.Controllers
{    
    public class AccountController : Controller
    {
        private readonly AzureAdB2COidcOptions _azureAdB2COptions;

        public AccountController(IOptions<AzureAdB2COidcOptions> authOptions)
        {
            _azureAdB2COptions = authOptions.Value;
        }

        // GET: /Account/SignIn
        [HttpGet]
        public async Task SignIn()
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                await HttpContext.Authentication.ChallengeAsync(_azureAdB2COptions.DefaultPolicy.ToLower(), authenticationProperties);
            }
        }

        // GET: /Account/LogOff
        [HttpGet]
        public async Task LogOff()
        {
            if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var scheme = (HttpContext.User.FindFirst("tfp"))?.Value;
                
                if (string.IsNullOrEmpty(scheme))
                    scheme = (HttpContext.User.FindFirst("http://schemas.microsoft.com/claims/authnclassreference"))?.Value;

                await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.Authentication.SignOutAsync(scheme.ToLower(), new AuthenticationProperties { RedirectUri = "/" });
            }
        }
    }
}