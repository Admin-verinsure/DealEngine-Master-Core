using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;
using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Infrastructure.BaseLdap.Services;
using TechCertain.Infrastructure.Legacy.Interfaces;
using TechCertain.Infrastructure.Legacy.Repositories;
using TechCertain.Infrastructure.Legacy.Services;

namespace DealEngine.Infrastructure.AppInitialize.ContainerServices
{
    public class BaseLdapPackage
    {
        public static void RegisterServices(Container container)
        {
            container.Register<ILegacyLdapConfigService, LegacyLdapConfigService>();
            container.Register<ILegacyLdapExportService, LegacyLdapExportService>();
            container.Register<ILegacyLdapRepository, LegacyLdapRepository>();

            container.RegisterConditional<ISessionService, LegacySessionService>(
                c => c.Consumer.ImplementationType.Namespace.Contains("Legacy"));

            container.RegisterConditional<ILdapConfigService, LegacyLdapConfigService>(
                c => c.Consumer.ImplementationType == typeof(LegacySessionService) || c.Consumer.ImplementationType.Namespace.Contains("Legacy"));


            container.RegisterConditional<ISessionService, SessionService>(
                c => c.Consumer.ImplementationType.Namespace.Contains("BaseLdap"));

            container.RegisterConditional<ILdapConfigService, LdapConfigService>(
                c => c.Consumer.ImplementationType == typeof(SessionService) || c.Consumer.ImplementationType.Namespace.Contains("BaseLdap"));

            container.Register<IOpenLdapImportService, OpenLdapImportService>();
            container.Register<IOpenLdapService, OpenLdapService>();
            container.Register<IOpenLdapOrganisationService, OpenLdapOrganisationService>();
            container.Register<ILdapRepository, LdapRepository>();
        }
    }
}
