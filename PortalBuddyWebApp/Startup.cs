using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortalBuddyWebApp.Extensions;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PortalBuddyWebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // set global cors policy to allow anything for any origin
            // -- should be modified for production --
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

            services.Configure<AzureAdB2COptions>(Configuration.GetSection("Authentication:AzureADB2C"));

            services.AddSingleton<CrmCoreServiceClient>();

            // build CrmCoreServiceClient instance on application init
            var sp = services.BuildServiceProvider();
            var instance = sp.GetService<CrmCoreServiceClient>();

            // add transient services that are re-initialized on each request
            services.AddTransient(typeof(OrganizationServiceContext), i => instance.ServiceContext);
            services.AddTransient(typeof(IOrganizationService), i => instance.OrgService);
            services.AddTransient(typeof(CrmServiceClient), i => instance.CrmServiceClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<AzureAdB2COptions> azureAdB2COptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Account/SignIn"),
                LogoutPath = new PathString("/Account/LogOff"),
                CookieName = Configuration["Authentication:CookieName"]
            });

            app.UseCors("CorsPolicy");

            // Setup Azure AD B2C OpenID Connect Authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                Authority = azureAdB2COptions.Value.Authority,
                MetadataAddress = azureAdB2COptions.Value.Metadata,
                SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                AuthenticationScheme = azureAdB2COptions.Value.DefaultPolicy.ToLower(),
                ClientId = azureAdB2COptions.Value.ClientId,
                PostLogoutRedirectUri = azureAdB2COptions.Value.RedirectUri,
                Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = RemoteFailure,
                    OnRedirectToIdentityProvider = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                        {
                            context.SkipToNextMiddleware();
                        }
                        return Task.FromResult(0);
                    }
                },
                ResponseType = OpenIdConnectResponseType.IdToken,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                }
            });

            // Setup Azure AD B2C JWT Bearer Authentication
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                MetadataAddress = azureAdB2COptions.Value.Metadata,
                Audience = azureAdB2COptions.Value.ClientId,
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.FromResult(0);
                    },
                    OnChallenge = context =>
                    {
                        return Task.FromResult(0);
                    },
                    OnMessageReceived = context =>
                    {
                        return Task.FromResult(0);
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.FromResult(0);
                    },
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private Task RemoteFailure(FailureContext context)
        {
            context.HandleResponse();
            if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
            {
                context.Response.Redirect("/");
            }
            else
            {
                context.Response.Redirect("/Home/Error?message=" + context.Failure.Message);
            }

            return Task.FromResult(0);
        }
    }
}
