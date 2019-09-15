using DealEngine.Infrastructure.AppInitialize;
using SimpleInjector;

namespace DealEngine.Infrastructure.AppInitialize.ContainerServices
{
	public class ConfigurePackage
	{
		public static void RegisterServices (Container container)
		{
            container.RegisterCollection<IAppConfigure> (new[] {
				typeof(ProductConfigure),
				typeof(AppRolesGroupsConfigure)
			});
        }
	}
}

