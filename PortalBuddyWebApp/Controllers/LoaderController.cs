using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PortalBuddyWebApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalBuddyWebApp.Controllers
{
    public class LoaderController : Controller
    {
        public AzureAdB2CJwtOptions B2COptions;

        public LoaderController(IOptions<AzureAdB2CJwtOptions> b2cOptions)
        {
            B2COptions = b2cOptions.Value;
        }

        [ResponseCache(Duration = 100)]
        public IActionResult Index()
        {
            return View(B2COptions);
        }
    }
}
