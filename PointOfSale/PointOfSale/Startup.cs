using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Admin.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using POS.DataAccessLayer;
using POS.DataAccessLayer.IServices;
using POS.DataAccessLayer.Models.Security;
using POS.DataAccessLayer.Services;

namespace PointOfSale
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
            services.AddDbContextPool<AppDbContext>
            (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 0;
            }).AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders()
               .AddRoleManager<RoleManager<IdentityRole>>();

            services.AddLocalization(opts =>
            {
                opts.ResourcesPath = "Resources";
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; })
              .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
              .AddDataAnnotationsLocalization().SetCompatibilityVersion(CompatibilityVersion.Version_3_0); ;


            services.Configure<RequestLocalizationOptions>(option =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-SA")
                };
                option.DefaultRequestCulture = new RequestCulture("en-US");
                option.SupportedCultures = supportedCultures;
                option.SupportedUICultures = supportedCultures;
                DateTimeFormatInfo engDateFormat = new CultureInfo("en-US").DateTimeFormat;
                option.SupportedCultures.Last().DateTimeFormat = engDateFormat;
            });


            services.AddScoped<IProductServices, ProductServices>();
            services.AddScoped<IDropdownsServices, DropdownsServices>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=index}/{id?}");
            });


            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});
        }
    }
}
