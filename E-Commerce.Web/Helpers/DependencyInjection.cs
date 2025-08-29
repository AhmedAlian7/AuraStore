using E_Commerce.Business.Configuration;
using E_Commerce.Business.Services.Implementation;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Repositories.Implementation;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Services;

namespace E_Commerce.Web.Helpers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Database
            services.AddDbContext<AppDbContext>(op =>
                op.UseSqlServer(config.GetConnectionString("HostingConnection")));

            // Stripe / Email config
            services.Configure<StripeSettings>(config.GetSection("Stripe"));
            services.Configure<EmailSettings>(config.GetSection("Smtp"));

            // Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<FileUploadService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderManagementService, OrderManagementService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IPromoCodeService, PromoCodeService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IProductNotificationService, ProductNotificationService>();
            // AutoMapper
            services.AddAutoMapper(typeof(Program));

            return services;
        }
    }
}
