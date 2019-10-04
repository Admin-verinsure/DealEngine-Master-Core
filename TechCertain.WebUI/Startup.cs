﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.HttpOverrides;
using DealEngine.Infrastructure.AppInitialize.Nhibernate;
using DealEngine.Infrastructure.Identity.Data;
using DealEngine.Infrastructure.AppInitialize.BaseLdapPackage;
using DealEngine.Infrastructure.AppInitialize.Services;
using DealEngine.Infrastructure.AppInitialize.Repositories;
using Microsoft.Extensions.Logging;
using TechCertain.WebUI.Models;

namespace TechCertain.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            // Note: The default connection string assumes that you have 'LocalDb' installed on your machine (either through SQL Server or Visual Studio installer)
            // If you followed the instructions in 'README.MD' and installed SQL Express then change the 'DefaultConnection' value in 'appSettings.json' with
            // "Server=localhost\\SQLEXPRESS;Database=aspnet-smartadmin;Trusted_Connection=True;MultipleActiveResultSets=true"


            services.AddControllersWithViews();
            services.AddRouting(options =>
                {
                    options.LowercaseUrls = true;
                    options.LowercaseQueryStrings = true;
                })
                .AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Latest)                
                .AddRazorPagesOptions(options =>
                {
                    //options.AllowAreas = true;
                    //options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    //options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            //start of removing simpleinjector
            services.AddNHibernate();
            services.AddSingleton(MapperConfig.ConfigureMaps());
            services.AddLogging();
            services.AddServices();
            services.AddRepositories();
            services.AddBaseLdap();
            services.AddBaseLdapPackage();
            services.AddResponseCaching();

            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseAuthentication();
            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    endpoints.MapRazorPages();
            //});

            app.UseMvc(routes =>
            {
                //routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Account}/{action=Login}/{id?}");
            });

            app.UseResponseCaching();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

        }
    }

}
