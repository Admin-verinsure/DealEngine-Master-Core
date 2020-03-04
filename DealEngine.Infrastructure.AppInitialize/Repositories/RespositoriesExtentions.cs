using Microsoft.Extensions.DependencyInjection;
using DealEngine.Infrastructure.BaseLdap.Repositories;
using DealEngine.Services.Interfaces;

namespace DealEngine.Infrastructure.AppInitialize.Repositories
{
    public static class RespositoriesExtentions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ILdapOrganisationRepository, LdapOrganisationRepository>();                      

            return services;
        }
    }

    
}
