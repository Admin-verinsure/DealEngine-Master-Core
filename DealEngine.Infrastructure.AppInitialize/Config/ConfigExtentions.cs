using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DealEngine.Infrastructure.AppInitialize
{
    public static class ConfigExtentions
    {
        public static IServiceCollection AddConfig(this IServiceCollection services)
        {
            //services.AddSingleton<IAppConfigure>(new[] {
            //    typeof(ProductConfigure),
            //    typeof(AppRolesGroupsConfigure) });

            //services.AddScoped<ILogger>();           
            services.AddSingleton(typeof(ILogger));


            return services;
        }
    }
}
