using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ThongKe.Data.Models;
using ThongKe.Data.Models_KDIB;
using ThongKe.Data.Models_KDND;
using ThongKe.Data.Models_KDOB;
using ThongKe.Data.Models_QLTour;
using ThongKe.Data.Repository;
using ThongKe.Data.Repository.KDIB;
using ThongKe.Data.Repository.QLTour;
using ThongKe.Services;

namespace ThongKe
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
            services.AddDbContext<thongkeContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))/*.EnableSensitiveDataLogging()*/);
            services.AddDbContext<qlkdtrContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultKDOB"))/*.EnableSensitiveDataLogging()*/);
            services.AddDbContext<qlkdtrnoidiaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultKDND"))/*.EnableSensitiveDataLogging()*/);
            services.AddDbContext<SaleDoanIBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultKDIB"))/*.EnableSensitiveDataLogging()*/);
            services.AddDbContext<qltourContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultQLTour"))/*.EnableSensitiveDataLogging()*/);

            // thongke
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IChiNhanhRepository, ChiNhanhRepository>();
            services.AddTransient<IDMDaiLyRepository, DMDaiLyRepository>();
            services.AddTransient<IChiNhanhRepository, ChiNhanhRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();

            // qltour
            services.AddTransient<IPhongBanRepository, PhongBanRepository>();
            services.AddTransient<IDmChiNhanhRepository, DmChiNhanhRepository>();
            services.AddTransient<ITourKindRepository, TourKindRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();

            // KDIB
            services.AddTransient<ICacNoiDungHuyTourRepository, CacNoiDungHuyTourRepository>();
            services.AddTransient<ITourKDIBRepository, TourKDIBRepository>();
            services.AddTransient<IPhanKhuCNRepository, PhanKhuCNRepository>();
            services.AddTransient<IUserIBRepository, UserIBRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // services
            services.AddTransient<IThongKeService, ThongKeService>();
            services.AddTransient<IBaoCaoService, BaoCaoService>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Shared/AccessDenied"); // Change AccessDenied route
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            var supportedCultures = new[] { new CultureInfo("en-AU") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-AU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
