using AutoMapper;
using E_Commerce.Business.AutoMapper;
using E_Commerce.Business.Services.Implementation;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Implementation;
using E_Commerce.DataAccess.Repositories.Interfaces;
using E_Commerce.DataAccess.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Services;

namespace E_Commerce.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("HostingConnection")));

            // Register External Login
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });
                //.AddFacebook(options =>
                //{
                //    var facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
                //    options.ClientId = facebookAuthNSection["AppId"];
                //    options.ClientSecret = facebookAuthNSection["AppSecret"];
                //});

            // Register UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<FileUploadService>();

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
            });


            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<ProductMappingProfile>();
            //});

            //config.AssertConfigurationIsValid();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
