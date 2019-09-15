using System;
using System.Collections.Generic;
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
using SimpleInjector.Lifestyles;
using TechCertain.Domain.Entities;


namespace techcertain2015rebuildcore
{
    public class Startup
    {
        private Container container = new Container();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSimpleInjector(container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope.
                options.AddAspNetCore();                
            });           

            //System.Web.Mvc.DependencyResolver.SetResolver(
            //new SimpleInjectorDependencyResolver(container));

            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
          
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
            InitializeContainer();

            container.Verify();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
         
        }

        private void InitializeContainer()
        {
            RepositoryPackage.RegisterServices(container);            
            BaseLdapPackage.RegisterServices(container);
            LdapPackage.RegisterServices(container);
            LoggingPackage.RegisterServices(container);
            ConfigurePackage.RegisterServices(container);
            IdentityPackage.RegisterServices(container);
        }

    }
}

