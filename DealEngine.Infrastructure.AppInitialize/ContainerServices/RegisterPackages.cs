using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealEngine.Infrastructure.AppInitialize.ContainerServices
{
    public interface IRegisterPackages
    {
        public static void InitializeContainer(Container container)
        {
            RepositoryPackage.RegisterServices(container);
            IdentityPackage.RegisterServices(container);
            BaseLdapPackage.RegisterServices(container);
            LdapPackage.RegisterServices(container);
            LoggingPackage.RegisterServices(container);
            ConfigurePackage.RegisterServices(container);
        }
    }
}
