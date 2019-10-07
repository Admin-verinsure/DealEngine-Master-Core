using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCertain.Domain.Entities.Abstracts;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.AppInitialize.Nhibernate
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services)
        {

            var connectionStringName = "TechCertainConnection";
            var sessionFactory = SessionFactoryBuilder.BuildSessionFactory(connectionStringName);

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddTransient(typeof(IMapperSession<>), typeof(NHibernateMapperSession<>));
            services.AddTransient<IUnitOfWork, NHibernateUnitOfWork>();
            services.AddTransient<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();

            return services;
        }
    }

    
}
