using Microsoft.Extensions.DependencyInjection;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Domain.Services;

namespace DealEngine.Infrastructure.AppInitialize.Repositories
{
    public static class RespositoriesExtentions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IOrganisationRepository, LdapOrganisationRepository>();                      

            return services;
        }
    }

    
}
