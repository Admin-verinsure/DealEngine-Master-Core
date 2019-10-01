using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Infrastructure.Ldap.Mapping;
using TechCertain.Infrastructure.Ldap.Services;

namespace DealEngine.Infrastructure.AppInitialize.BaseLdapPackage
{
    public static class ConfigExtentions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services)
        {
            //services.AddSingleton<IAppConfigure>(new[] {
            //    typeof(ProductConfigure),
            //    typeof(AppRolesGroupsConfigure) });

            //services.AddScoped<ILogger>();

            return services;
        }
    }
}
