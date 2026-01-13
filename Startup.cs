using CPhoneS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CPhoneS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Thêm các dịch vụ cơ bản
            services.AddControllersWithViews();

            // Cấu hình DbContext để kết nối với SQL Server
            services.AddDbContext<CPhoneSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CPhoneSConnect")));

            // Thêm dịch vụ lưu trữ Session
            services.AddDistributedMemoryCache();  // Cấu hình bộ nhớ đệm
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);  // Đặt thời gian hết hạn cho session
                options.Cookie.HttpOnly = true;  // Giới hạn quyền truy cập session từ client
                options.Cookie.IsEssential = true;  // Đảm bảo cookie cần thiết cho session
            });

            // Cấu hình dịch vụ Authentication (nếu dùng cookie authentication)
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";  // Đường dẫn đến trang đăng nhập
                    options.LogoutPath = "/Account/Logout";  // Đường dẫn đến trang đăng xuất
                    options.AccessDeniedPath = "/Account/AccessDenied";  // Đường dẫn đến trang không có quyền truy cập
                });

            // Nếu bạn dùng Identity (comment nếu không dùng)
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<CPhoneSContext>()
            //    .AddDefaultTokenProviders();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Cấu hình môi trường phát triển và sản xuất
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Bật tính năng HTTPS và sử dụng file tĩnh
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Bật sử dụng session
            app.UseSession();

            // Cấu hình routing
            app.UseRouting();

            // Cấu hình middleware cho authentication (nếu sử dụng Identity)
            app.UseAuthentication(); // Đảm bảo gọi UseAuthentication khi sử dụng authentication
            app.UseAuthorization();

            // Cấu hình endpoint mặc định cho các controller và action
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
