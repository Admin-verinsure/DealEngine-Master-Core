using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;
using TechCertain.Infrastructure.FluentNHibernate;

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

            return services;
        }
    }

    
}
