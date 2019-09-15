using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Infrastructure.Ldap.Mapping;
using TechCertain.Infrastructure.Ldap.Services;

namespace DealEngine.Infrastructure.AppInitialize.ContainerServices
{
    public class LdapPackage
    {

        public static void RegisterServices(Container container)
        {
            container.Register<ILdapConfiguration, LdapConfiguration>();
            container.Register<ILegacyLdapConfiguration, LegacyLdapConfiguration>();

            container.Register(typeof(ILdapEntityMapping<Organisation>), typeof(OrganisationMapping));
            container.Register(typeof(ILdapEntityMapping<User>), typeof(UserMapping));
            container.Register<ILegacyEntityMapping, LegacyEntityMapping>();

            container.Register<ILdapService, LdapService>();
            container.Register<ILegacyLdapService, LegacyLdapService>();
        }
    }
    
}
