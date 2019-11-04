using System;
using Npgsql;
using System.Text.RegularExpressions;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
using FluentNHibernate.Automapping;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate.MappingConventions;
using TechCertain.Infrastructure.FluentNHibernate.MappingOverrides;
using NHibernate.Extensions.NpgSql;
using System.Reflection;

namespace DealEngine.Infrastructure.AppInitialize.Nhibernate
{
    public class SessionFactoryBuilder
    {

        private static string NpgsqlConnectionString;
        public static ISessionFactory BuildSessionFactory(string connectionStringName)
        {

            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                        .AddJsonFile("appsettings.json")
                                        .Build();

            NpgsqlConnectionString = configuration.GetConnectionString("TechCertainConnection");

            var session = Fluently.Configure()
                .Database(PostgreSQLConfiguration.Standard.ConnectionString(NpgsqlConnectionString)
                    .Dialect<PostgreSQL82Dialect>()
                    .AdoNetBatchSize(10)
                    .Driver<NpgSqlDriver>()
                    .FormatSql()
                    .ShowSql()
                 )
                .CurrentSessionContext("web")
                .ExposeConfiguration(cfg => BuildSchema(cfg, NpgsqlConnectionString))
                .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Organisation>(new DefaultMappingConfiguration())                    
                    .Conventions.Add<CascadeConvention>()
                    .AddMappingsFromAssembly(Assembly.GetExecutingAssembly())
                .UseOverridesFromAssemblyOf<OrganisationMappingOverride>())
                ).BuildConfiguration()
                .AddIdentityMappingsForPostgres()
                .BuildSessionFactory();

            return session;

            }
            /// <summary>  
            /// Build the schema of the database.  
            /// </summary>  
            /// <param name="config">Configuration.</param>  
            private static void BuildSchema(NHibernate.Cfg.Configuration config, string connectionStringName)
        {

            using (var connection = new NpgsqlConnection(connectionStringName))
            {
                try
                {
                    connection.Open();
                }
                catch (NpgsqlException)
                {
                    CreateDatabase(connectionStringName);
                    CreateLoggingTable(connectionStringName);
                }
                finally
                {
                    try
                    {
                        new SchemaUpdate(config).ExecuteAsync(false, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    
                }
            }          
        }
       
        private static void CreateDatabase(string connectionStringName)
        {
            var masterConnectionString = Regex.Replace(connectionStringName, "(Database|Initial Catalog)=[^;]+", "Database=postgres", RegexOptions.IgnoreCase);
            using (var connection = new NpgsqlConnection(masterConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "CREATE DATABASE " + "\"" + GetDatabaseName(connectionStringName) + "\"";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateLoggingTable(string connectionStringName)
        {
            using (var connection = new NpgsqlConnection(connectionStringName))
            {
                connection.Open();

                string strSql = @" CREATE SEQUENCE ELMAH_Error_SEQUENCE;

                                        CREATE TABLE ELMAH_Error
                                        (
                                            ErrorId     UUID NOT NULL,
                                            Application VARCHAR(60) NOT NULL,
                                            Host        VARCHAR(50) NOT NULL,
                                            Type        VARCHAR(100) NOT NULL,
                                            Source      VARCHAR(60)  NOT NULL,
                                            Message     VARCHAR(500) NOT NULL,
                                            ""User""      VARCHAR(50)  NOT NULL,
                                            StatusCode  INT NOT NULL,
                                            TimeUtc     TIMESTAMP NOT NULL,
                                            Sequence    INT NOT NULL DEFAULT NEXTVAL('ELMAH_Error_SEQUENCE'),
                                            AllXml      TEXT NOT NULL
                                        );

                                        ALTER TABLE ELMAH_Error ADD CONSTRAINT PK_ELMAH_Error PRIMARY KEY (ErrorId);

                                        CREATE INDEX IX_ELMAH_Error_App_Time_Seq ON ELMAH_Error USING BTREE
                                        (
                                            Application   ASC,
                                            TimeUtc       DESC,
                                            Sequence      DESC
                                        );";

                using (var cmd = new NpgsqlCommand(strSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static string GetDatabaseName(string connectionStringName)
        {
            using (var connection = new NpgsqlConnection(connectionStringName))
            {
                return connection.Database;
            }
        }
    }
}
