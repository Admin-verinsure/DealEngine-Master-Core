using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using System;
using TechCertain.Infrastructure.FluentNHibernate.MappingConventions;
using TechCertain.Infrastructure.FluentNHibernate.MappingOverrides;
using TechCertain.Domain.Entities;
using NHibernate;


namespace TechCertain.Infrastructure.FluentNHibernate.ConfigurationProvider
{
    public abstract class NHibernateConfigurationProvider : INHibernateConfigurationProvider
    {
        public abstract Configuration GetDatabaseConfiguration();

        public Configuration CreateCoreDatabaseConfiguration(
            IPersistenceConfigurer databaseDriver,
            Action<Configuration> databaseBuilder = null)
        {
            var fluentConfiguration =
                Fluently.Configure()
                .Database(databaseDriver)                
                .CurrentSessionContext("web")
                .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Organisation>(new DefaultMappingConfiguration())                
                .Conventions.Add<CascadeConvention>()                 
                     .UseOverridesFromAssemblyOf<OrganisationMappingOverride>()));
                     
                

            if (databaseBuilder != null)
            {
                fluentConfiguration.ExposeConfiguration(databaseBuilder);
            }

            return fluentConfiguration.BuildConfiguration();
        }
    }

    public class SqlStatementInterceptor : EmptyInterceptor
    {
        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            //Trace.WriteLine(sql.ToString());
            return sql;
        }
    }
}
