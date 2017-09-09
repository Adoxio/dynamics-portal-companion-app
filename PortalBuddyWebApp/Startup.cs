using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortalBuddyWebApp.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;

namespace PortalBuddyWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            });

            services.Configure<DynS2SOptions>(Configuration.GetSection("Dynamics:dynS2S"));
            services.Configure<DynConnStringOptions>(Configuration.GetSection("Dynamics:dynConnString"));

            services.AddSingleton<CrmCoreServiceClient>();

            // build CrmCoreServiceClient instance on application init
            var instance = services.BuildServiceProvider().GetService<CrmCoreServiceClient>();

            // add transient services that are re-initialized on each request
            services.AddTransient(typeof(OrganizationServiceContext), i => instance.ServiceContext);
            services.AddTransient(typeof(IOrganizationService), i => instance.OrgService);
            services.AddTransient(typeof(CrmServiceClient), i => instance.CrmServiceClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors("CorsPolicy");

            // Setup Azure AD B2C OpenID Connect Authentication
            //app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            //{
            //    Authority = authOptions.Value.AzureAD.Authority,
            //    MetadataAddress = authOptions.Value.AzureAD.Metadata,
            //    AuthenticationScheme = authOptions.Value.AzureAD.SignInOrSignUpPolicy.ToLower(),
            //    ClientId = authOptions.Value.AzureAD.Audience,
            //    PostLogoutRedirectUri = authOptions.Value.AzureAD.RedirectUri,
            //    Events = new OpenIdConnectEvents
            //    {
            //        OnRemoteFailure = RemoteFailure,
            //        OnRedirectToIdentityProvider = context =>
            //        {
            //            if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            //            {
            //                context.SkipToNextMiddleware();
            //            }
            //            return Task.FromResult(0);
            //        }
            //    },
            //    ResponseType = OpenIdConnectResponseType.IdToken,
            //    TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //    }
            //});

            //// Setup Azure AD B2C JWT Bearer Authentication
            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    MetadataAddress = authOptions.Value.AzureAD.Metadata,
            //    Audience = authOptions.Value.AzureAD.Audience,
            //    Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            return Task.FromResult(0);
            //        },
            //        OnChallenge = context =>
            //        {
            //            return Task.FromResult(0);
            //        },
            //        OnMessageReceived = context =>
            //        {
            //            return Task.FromResult(0);
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            return Task.FromResult(0);
            //        },
            //    }
            //});
        }
    }
}
