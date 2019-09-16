using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Infrastructure.AppInitialize.ContainerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Lifestyles;
using TechCertain.Domain.Entities;
using TechCertain.Services.Impl;

namespace techcertain2019core
{
    public class Startup
    {
        private Container container = new Container();
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }      

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();
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

            //services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
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

            InitializeContainer();

            container.Verify();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            System.Web.Mvc.DependencyResolver.SetResolver(
                new SimpleInjectorDependencyResolver(container));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //System.Web.Mvc.DependencyResolver.SetResolver(
            //    new SimpleInjectorDependencyResolver(container));

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}");
            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{controller=Home}/{action=Index}/{id?}",
            //    //    //defaults: new { controller = "Home", action = "Index" });
            //});

        }

        private void InitializeContainer()
        {
            RepositoryPackage.RegisterServices(container);            
            BaseLdapPackage.RegisterServices(container);
            LdapPackage.RegisterServices(container);
            LoggingPackage.RegisterServices(container);
            ConfigurePackage.RegisterServices(container);
            IdentityPackage.RegisterServices(container);


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
        }

    }
}

