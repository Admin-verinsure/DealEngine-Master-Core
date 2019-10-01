using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Infrastructure.BaseLdap.Services;
using TechCertain.Infrastructure.Legacy.Interfaces;
using TechCertain.Infrastructure.Legacy.Repositories;
using TechCertain.Infrastructure.Legacy.Services;

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
