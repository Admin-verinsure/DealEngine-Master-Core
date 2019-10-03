using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCertain.Domain.Entities.Abstracts;
using TechCertain.Domain.Interfaces;
using TechCertain.Domain.Services.Factories;

namespace DealEngine.Infrastructure.AppInitialize
{
    public static class FactoriesExtenstions
    {
        public static IServiceCollection AddFactories(this IServiceCollection services)
        {
            services.AddSingleton<IEntityFactory, InformationItemFactory>();
            services.AddSingleton<>();
            services.AddSingleton<>()
                services.AddSingleton<>()
                services.AddSingleton<>()

            return services;
        }
    }

    
}
