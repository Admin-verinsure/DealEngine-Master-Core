using Microsoft.Extensions.DependencyInjection;
using TechCertain.Domain.Services.Factories;

namespace DealEngine.Infrastructure.AppInitialize
{
    public static class FactoriesExtenstions
    {
        public static IServiceCollection AddFactories(this IServiceCollection services)
        {
            services.AddSingleton<IEntityFactory, InformationTemplateFactory>();
            services.AddSingleton<IEntityFactory, InformationSectionFactory>();
            services.AddSingleton<IEntityFactory, InformationItemFactory>();
            services.AddSingleton<IEntityFactory, ProposalTemplateFactory>();
            services.AddSingleton<IUWMFactory, UWMFactory>();

            return services;
        }
    }

    
}
