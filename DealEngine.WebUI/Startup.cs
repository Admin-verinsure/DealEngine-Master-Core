﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using DealEngine.Infrastructure.AppInitialize.Nhibernate;
using DealEngine.Infrastructure.AppInitialize.BaseLdapPackage;
using DealEngine.Infrastructure.AppInitialize.Services;
using DealEngine.Infrastructure.AppInitialize.Repositories;
using DealEngine.WebUI.Models;
using Microsoft.Extensions.Hosting;
using DealEngine.Infrastructure.AppInitialize;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Localization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using Newtonsoft.Json;
using FluentNHibernate.Conventions.Inspections;

namespace DealEngine.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddServices();
            services.AddControllersWithViews();
            services.AddRouting();
            services.AddRazorPages();
            services.AddNHibernate();
            services.AddIdentityExtentions();
            services.AddSingleton(MapperConfig.ConfigureMaps());
            services.AddLogging();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //https://stackoverflow.com/questions/41289737/get-the-current-culture-in-a-controller-asp-net-core
                options.DefaultRequestCulture = new RequestCulture(culture: "en-NZ", uiCulture: "en-NZ");
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                //https://github.com/dotnet/aspnetcore/issues/12166
                options.ValidationInterval = TimeSpan.FromHours(8);
            });

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddRepositories();
            services.AddBaseLdap();
            services.AddElmah(options =>
            {
                options.Path = @"c078b2de-f512-4225-90e8-90f8e17ac70b";
            });
            services.AddBaseLdapPackage();
            services.AddResponseCaching();
            services.AddMvc();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");                
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRequestLocalization();
            app.UseElmah();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();                
            });
        }
    }

}
