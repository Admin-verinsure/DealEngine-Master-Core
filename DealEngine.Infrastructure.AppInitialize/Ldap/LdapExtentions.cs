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
    public static class LdapExtentions
    {
        public static IServiceCollection AddBaseLdapPackage(this IServiceCollection services)
        {
            services.AddTransient<ILdapConfiguration, LdapConfiguration>();
            services.AddTransient<ILegacyLdapConfiguration, LegacyLdapConfiguration>();

            services.AddTransient(typeof(ILdapEntityMapping<Organisation>), typeof(OrganisationMapping));
            services.AddTransient(typeof(ILdapEntityMapping<User>), typeof(UserMapping));
            services.AddTransient<ILegacyEntityMapping, LegacyEntityMapping>();

            services.AddTransient<ILdapService, LdapService>();
            services.AddTransient<ILegacyLdapService, LegacyLdapService>();

            return services;
        }
    }
}
