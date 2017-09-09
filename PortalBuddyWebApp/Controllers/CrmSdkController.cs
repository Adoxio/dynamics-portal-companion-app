using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Diagnostics;
using PortalBuddyWebApp.Extensions;

namespace PortalBuddyWebApp.Controllers
{
    public class CrmSdkController : Controller
    {
        private readonly CrmCoreServiceClient _crmClient;

        public CrmSdkController(CrmCoreServiceClient crmClient)
        {
            _crmClient = crmClient;
        }

        public IActionResult Index()
        {
            var contacts = _crmClient.ServiceContext.CreateQuery("contact").ToList();
            return View(model: string.Join(",", contacts.Select(a => a.GetAttributeValue<string>("fullname"))));
        }

        public IActionResult WhoAmI()
        {
            WhoAmIResponse response = (WhoAmIResponse)_crmClient.OrgService.Execute(new WhoAmIRequest());

            string responseText = responseText = $"{response.UserId}";           
            
            return View((object)responseText);
        }

        public IActionResult MultipleCalls()
        {
            Trace.TraceInformation("Start MultipleCalls");

            WhoAmIResponse response = (WhoAmIResponse)_crmClient.OrgService.Execute(new WhoAmIRequest());
            Trace.TraceInformation("WhoAmI Executed");

            string responseText = $"{response.UserId} : ";

            List<string> contacts = null;
            contacts = _crmClient.ServiceContext.CreateQuery("contact").Select(a => a.GetAttributeValue<string>("fullname")).ToList();
            Trace.TraceInformation("Contact Query Executed");
            
            responseText += contacts != null ? string.Join(",", contacts) : "null";

            return View((object)responseText);
        }

        [Produces("application/json")]
        [Route("api/CrmSdk")]
        public IEnumerable<Entity> GetContacts()
        {
            var contacts = _crmClient.ServiceContext.CreateQuery("contact").ToList();
            return contacts;
        }
    }
}