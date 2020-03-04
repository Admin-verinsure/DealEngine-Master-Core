using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using DealEngine.Infrastructure.BaseLdap.Interfaces;
using DealEngine.Infrastructure.BaseLdap.Repositories;
using DealEngine.Infrastructure.BaseLdap.Services;
using DealEngine.Infrastructure.Legacy.Interfaces;
using DealEngine.Infrastructure.Legacy.Repositories;
using DealEngine.Infrastructure.Legacy.Services;

namespace DealEngine.Infrastructure.AppInitialize.BaseLdapPackage
{
    public static class BaseLdapExtentions
    {
        public static IServiceCollection AddBaseLdap(this IServiceCollection services)
        {
            services.AddTransient<ILegacyLdapConfigService, LegacyLdapConfigService>();
            services.AddTransient<ILegacyLdapExportService, LegacyLdapExportService>();
            services.AddTransient<ILegacyLdapRepository, LegacyLdapRepository>();

            services.AddTransient<ISessionService, LegacySessionService>();

            services.AddTransient<ILdapConfigService, LegacyLdapConfigService>();
            services.AddTransient<ISessionService, SessionService>();

            services.AddTransient<IOpenLdapImportService, OpenLdapImportService>();
            services.AddTransient<IOpenLdapService, OpenLdapService>();
            services.AddTransient<IOpenLdapOrganisationService, OpenLdapOrganisationService>();
            services.AddTransient<ILdapRepository, LdapRepository>();

            return services;
        }
    }
}
