using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using SimpleInjector.Lifestyles;
using SimpleInjector;
using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Services;
using TechCertain.Infrastructure.BaseLdap.Repositories;

using TechCertain.Infrastructure.Legacy.Interfaces;
using TechCertain.Infrastructure.Legacy.Services;
using TechCertain.Infrastructure.Legacy.Repositories;


namespace TechCertain.Infrastructure.DependecyResolution
{
    /// <summary>
    /// Register the Base LDAP Implementation into IoC
    /// </summary>
    public class BaseLdapPackage// : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.Register<ILegacyLdapConfigService, LegacyLdapConfigService>();
            container.Register<ILegacyLdapExportService, LegacyLdapExportService>();
            container.Register<ILegacyLdapRepository, LegacyLdapRepository>();

            // New
            //container.RegisterConditional<
            //                    ILdapConfigService,
            //                    OpenLdap.Services.LdapConfigService>(
            //                    c => c.Consumer.ImplementationType.Namespace.Contains("OpenLdap"));

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

            // Base
            //container.Register<ILdapConfigService, LdapConfigService>();
            //container.Register<ISessionService, SessionService>();
            container.Register<ILdapRepository, LdapRepository>();
        }
    }
}
