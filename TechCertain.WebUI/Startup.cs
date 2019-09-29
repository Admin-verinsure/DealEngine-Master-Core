﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleInjector;
using TechCertain.Services.Impl;
using DealEngine.Infrastructure.AppInitialize.ContainerServices;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using TechCertain.WebUI.Models;
using DealEngine.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

namespace TechCertain.WebUI
{
    public class Startup
    {
        private Container container = new Container();
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

            //services.AddDbContext<DealEngineDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))
            //    .AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<DealEngineDBContext>();


            //services.AddIdentity<IdentityUser>()
            //        .AddSignInManager<DealEngineSignInManager>()
            //        .AddClaimsPrincipalFactory<IdentityUser>();

            // services.AddDbContext<ApplicationDbContext>().AddEntityFrameworkNpgsql().AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

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

            services.ConfigureApplicationCookie(options =>
            {
                //options.LoginPath = "/Identity/Account/Login";
                //options.LogoutPath = "/Identity/Account/Logout";
                //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });


            services.AddSimpleInjector(container, options =>
            {
                // Wraps web requests in a Simple Injector scope.
                options.AddAspNetCore()
                    // Ensure activation of a specific framework type to be created by Simple Injector instead of the built-in configuration system.
                    .AddControllerActivation()
                    .AddViewComponentActivation()
                    .AddPageModelActivation()
                    .AddTagHelperActivation();

            });

            services.EnableSimpleInjectorCrossWiring(container);
            services.AddResponseCaching();
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

            app.UseSimpleInjector(container, options =>
            {
                options.UseLogging();
                options.CrossWire<IOptions<IdentityOptions>>();
                options.CrossWire<IOptions<PasswordHasherOptions>>();
                options.CrossWire<IServiceProvider>();
                options.CrossWire<IEnumerable<IUserValidator<User>>>();
                options.CrossWire<IEnumerable<IPasswordValidator<User>>>();
                options.CrossWire<ILogger<UserManager<User>>>();
            });

            container.AutoCrossWireAspNetComponents(app);
            container.RegisterInstance(MapperConfig.ConfigureMaps());

            InitializeContainer();

            container.Verify();

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

        private void InitializeContainer()
        {
            var repositoryAssembly = typeof(EmailService).Assembly;
            var registrations =
                from type in repositoryAssembly.GetExportedTypes()
                where type.Namespace == "TechCertain.Services.Impl"
                where type.GetInterfaces().Any()
                select new { Service = type.GetInterfaces().Single(), Implementation = type };

            foreach (var reg in registrations)
            {
                container.Register(reg.Service, reg.Implementation);
            }

            RepositoryPackage.RegisterServices(container);
            BaseLdapPackage.RegisterServices(container);
            LdapPackage.RegisterServices(container);
            LoggingPackage.RegisterServices(container);
            ConfigurePackage.RegisterServices(container);
            IdentityPackage.RegisterServices(container);
        }
    }

}
