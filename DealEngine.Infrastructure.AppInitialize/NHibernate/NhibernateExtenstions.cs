using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.ByCode;
using System;
using System.Reflection;
using DealEngine.Infrastructure.FluentNHibernate;

namespace DealEngine.Infrastructure.AppInitialize.Nhibernate
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services)
        {

            var connectionStringName = "DealEngineConnection";
            var sessionFactory = SessionFactoryBuilder.BuildSessionFactory(connectionStringName);
            
            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddTransient(typeof(IMapperSession<>), typeof(NHibernateMapperSession<>));
            services.AddTransient<IUnitOfWork, NHibernateUnitOfWork>();

            return services;
        }
    }

    
}
