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
using TechCertain.WebUI.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using DealEngine.Infrastructure.Identity.Data;

namespace TechCertain.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddRouting();
            services.AddMvc();
            services.AddRazorPages();
            services.AddNHibernate();
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //        .AddEntityFrameworkStores<DealEngineDBContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(MapperConfig.ConfigureMaps());
            services.AddLogging();
            services.AddRepositories();
            services.AddBaseLdap();
            services.AddBaseLdapPackage();
            services.AddResponseCaching();
            services.AddServices();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
            //});
            //services.AddTransient(IRolePermissionsService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute("default", "{controller=Account}/{action=Login}/{id?}");
            //});

            //app.UseResponseCaching();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

        }
    }

}
