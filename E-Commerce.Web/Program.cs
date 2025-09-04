using E_Commerce.Business.Services.Implementation;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Seeding;
using E_Commerce.Web.Filters;
using E_Commerce.Web.Helpers;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });
            builder.Services.AddSession();


            // Register External Login
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    var facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
                    options.ClientId = facebookAuthNSection["AppId"];
                    options.ClientSecret = facebookAuthNSection["AppSecret"];

                    options.SaveTokens = true;
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.CallbackPath = "/signin-facebook"; // default

                });

            // Register Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
           .AddEntityFrameworkStores<AppDbContext>()
           .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Authentication/Account/Login";
                options.AccessDeniedPath = "/Authentication/Account/AccessDenied";
                options.Cookie.SameSite = SameSiteMode.None; // App auth cookie
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            });
            builder.Services.ConfigureExternalCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None; // Must be None for OAuth redirects
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Register custom services
            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseRouting();
            seedData();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
               name: "default",
               pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();

            void seedData()
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                IdentitySeeder.SeedAsync(userManager, roleManager).Wait();
            }
        }
    }
}

