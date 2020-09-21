using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DealEngine.Infrastructure.AppInitialize
{
    public static class ConfigExtentions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services)
        {
            //services.AddSingleton<IAppConfigure>(new[] {
            //    typeof(ProductConfigure),
            //    typeof(AppRolesGroupsConfigure) });

            //services.AddScoped<ILogger>();           
            //services.AddSingleton(typeof(ILogger));

            services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 10000;
            });
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                //https://github.com/dotnet/aspnetcore/issues/12166
                options.ValidationInterval = TimeSpan.FromHours(8);
            });

            return services;
        }
    }
}
