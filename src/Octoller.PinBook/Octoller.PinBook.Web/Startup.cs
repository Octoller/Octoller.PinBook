using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octoller.PinBook.Web.Data;
using Octoller.PinBook.Web.Data.Model;
using Octoller.PinBook.Web.Data.Stores;
using Octoller.PinBook.Web.Kernel;
using Octoller.PinBook.Web.Kernel.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Octoller.PinBook.Web
{
    public class Startup
    {

        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseAppContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DevelopmentDb"));
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<DatabaseAppContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddOAuth(AppData.ExternalAuthProvider.VkProviderName, "VKontakte", ConfigureOptions);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Users", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppData.RolesData.Roles);
                });

                options.AddPolicy("Administration", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppData.RolesData.AdministratorRoleName);
                });
            });

            services.AddScoped<AccountStore>();
            services.AddScoped<VkontakteApiService>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void ConfigureOptions(OAuthOptions options)
        {
            options.ClaimsIssuer = "Vkontakte";
            options.ClientId = Configuration["VkOptionsData:ClientId"];
            options.ClientSecret = Configuration["VkOptionsData:ClientSecret"];
            options.CallbackPath = new PathString(Configuration["VkOptionsData:CallbackPathAccount"]);
            options.AuthorizationEndpoint = Configuration["VkOptionsData:AuthorizationEndpoint"];
            options.TokenEndpoint = Configuration["VkOptionsData:TokenEndpoint"];
            options.Scope.Add("email");
            options.Scope.Add("offline");
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
            options.SaveTokens = true;

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = context =>
                {
                    context.RunClaimActions(context.TokenResponse.Response.RootElement);
                    return Task.CompletedTask;
                }
            };
        }
    }
}
