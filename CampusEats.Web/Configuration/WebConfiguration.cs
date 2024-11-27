using CampusEats.Infrastructure.Configuration;
using CampusEats.Web.Hubs;
using CampusEats.Web.Interface;
using CampusEats.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;

namespace CampusEats.Web.Configuration
{
    public static class WebConfiguration
    {
        public static IServiceCollection AddWebServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add Infrastructure Layer
            services.AddInfrastructureServices(configuration);

            // Add Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                });

            // Add Cart Service
            services.AddScoped<ICartService, CartService>();

            // Add Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add MVC with options
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddSignalR();

            services.AddHttpContextAccessor();

            services.AddMvc()
                .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Admin", "/");
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = int.MaxValue; // ขนาดสูงสุดของ request
                options.ValueLengthLimit = int.MaxValue; // ขนาดสูงสุดของค่าใน form
                options.MemoryBufferThreshold = int.MaxValue; // ขนาด buffer ในหน่วยความจำ
            });

            return services;
        }

        public static WebApplication ConfigureWebApplication(this WebApplication app)
        {
            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<NotificationHub>("/notificationHub");

            return app;
        }
    }
}
