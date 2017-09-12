using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Diagnostics;
using PortalBuddyWebApp.Extensions;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Client;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PortalBuddyWebApp.Controllers
{
    public class CrmSdkController : Controller
    {
        public CrmServiceClient CrmServiceClient;
        public OrganizationServiceContext ServiceContext;
        public IOrganizationService OrgService;

        public CrmSdkController(CrmServiceClient crmClient, OrganizationServiceContext context, IOrganizationService orgSevice)
        {
            CrmServiceClient = crmClient;
            ServiceContext = context;
            OrgService = orgSevice;
        }
        
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var contacts = ServiceContext.CreateQuery("contact").ToList();
            return View(model: string.Join(",", contacts.Select(a => a.GetAttributeValue<string>("fullname"))));
        }

        public IActionResult WhoAmI()
        {
            var identity = (ClaimsIdentity)User.Identity;
            WhoAmIResponse response = (WhoAmIResponse)OrgService.Execute(new WhoAmIRequest());

            string responseText = responseText = $"{response.UserId}";           
            
            return View((object)responseText);
        }

        public IActionResult MultipleCalls()
        {
            Trace.TraceInformation("Start MultipleCalls");

            WhoAmIResponse response = (WhoAmIResponse)OrgService.Execute(new WhoAmIRequest());
            Trace.TraceInformation("WhoAmI Executed");

            string responseText = $"{response.UserId} : ";

            List<string> contacts = null;
            contacts = ServiceContext.CreateQuery("contact").Select(a => a.GetAttributeValue<string>("fullname")).ToList();
            Trace.TraceInformation("Contact Query Executed");
            
            responseText += contacts != null ? string.Join(",", contacts) : "null";

            return View((object)responseText);
        }
        
        [Produces("application/json")]
        [Route("api/CrmSdk")]
        public IEnumerable<Entity> GetContacts()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var contacts = ServiceContext.CreateQuery("contact").ToList();
            return contacts;
        }
    }
}