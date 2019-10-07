using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Infrastructure.FluentNHibernate.Repositories;

namespace DealEngine.Infrastructure.AppInitialize.Repositories
{
    public static class RespositoriesExtentions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IOrganisationRepository, LdapOrganisationRepository>();                      

            return services;
        }
    }

    
}
