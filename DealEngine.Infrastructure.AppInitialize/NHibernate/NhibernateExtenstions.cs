using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.ByCode;
using System;
using TechCertain.Infrastructure.FluentNHibernate;

namespace DealEngine.Infrastructure.AppInitialize.Nhibernate
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services)
        {

            var connectionStringName = "TechCertainConnection";
            //var sessionFactory = SessionFactoryBuilder.BuildSessionFactory(connectionStringName);

            var configurationbuilder = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json")
                            .Build();
            var NpgsqlConnectionString = configurationbuilder.GetConnectionString("TechCertainConnection");

            var mapper = new ModelMapper();
            mapper.AddMappings(typeof(NHibernateExtensions).Assembly.ExportedTypes);
            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();

            configuration.DataBaseIntegration(c =>
            {
                c.Dialect<PostgreSQL82Dialect>();
                c.ConnectionString = NpgsqlConnectionString;
                c.SchemaAction = SchemaAutoAction.Validate;
                c.Driver<NpgSqlDriver>();
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

            });
            configuration.AddIdentityMappingsForPostgres();
            configuration.AddMapping(domainMapping);            
            var sessionFactory = configuration.BuildSessionFactory();
    //            .Database(PostgreSQLConfiguration.Standard.ConnectionString(NpgsqlConnectionString)
    //    .Dialect<PostgreSQL82Dialect>()
    //    .AdoNetBatchSize(10)
    //    .Driver<NpgSqlDriver>()
    //    .FormatSql()
    //    .ShowSql()
    // )
    //.CurrentSessionContext("web")
    //.ExposeConfiguration(cfg => BuildSchema(cfg, NpgsqlConnectionString))
    //.Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Organisation>(new DefaultMappingConfiguration())
    //.Conventions.Add<CascadeConvention>()
    //.UseOverridesFromAssemblyOf<OrganisationMappingOverride>())
    //).BuildConfiguration()
    //.AddIdentityMappingsForPostgres()
    //.BuildSessionFactory();



            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddTransient(typeof(IMapperSession<>), typeof(NHibernateMapperSession<>));
            services.AddTransient<IUnitOfWork, NHibernateUnitOfWork>();

            return services;
        }
    }

    
}
