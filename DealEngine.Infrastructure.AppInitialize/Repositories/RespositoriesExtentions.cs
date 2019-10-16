using Microsoft.Extensions.DependencyInjection;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Services.Interfaces;

namespace DealEngine.Infrastructure.AppInitialize.Repositories
{
    public static class RespositoriesExtentions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IOrganisationService, LdapOrganisationRepository>();                      

            return services;
        }
    }

    
}
