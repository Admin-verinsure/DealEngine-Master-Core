using DealEngine.Infrastructure.AppInitialize;
using SimpleInjector;

namespace TechCertain.Infrastructure.DependecyResolution
{
	public class ConfigurePackage //: IPackage
	{
		public void RegisterServices (Container container)
		{
            container.Collection.Register<IAppConfigure> (new[] {
				typeof(ProductConfigure),
				typeof(AppRolesGroupsConfigure)
			});
        }
	}
}

