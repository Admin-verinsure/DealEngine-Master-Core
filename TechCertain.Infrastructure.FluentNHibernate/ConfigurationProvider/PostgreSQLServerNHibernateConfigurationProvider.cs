using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate.Tool.hbm2ddl;
using Npgsql;
using System;
using System.Text.RegularExpressions;
using Configuration = NHibernate.Cfg.Configuration;

namespace TechCertain.Infrastructure.FluentNHibernate.ConfigurationProvider
{
    public class PostgreSQLServerNHibernateConfigurationProvider : NHibernateConfigurationProvider
    {
        private static string NpgsqlConnectionString; //= ConfigurationManager.ConnectionStrings["TechCertainConnection"].ConnectionString;

        // VNext example http://druss.co/2015/04/vnext-use-postgresql-fluent-nhibernate-from-asp-net-5-dnx-on-ubuntu/
       
        public override Configuration GetDatabaseConfiguration()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            NpgsqlConnectionString = configuration.GetConnectionString("TechCertainConnection");

            return CreateCoreDatabaseConfiguration(
                PostgreSQLConfiguration.Standard.ConnectionString(NpgsqlConnectionString).
					//Dialect("NHibernate.Dialect.PostgreSQL82Dialect")
					Dialect<PostgreSQL82DialectIncreasedAlias> (),
                
                    //.ShowSql(),
                BuildDatabase);
        }

        private static void BuildDatabase(Configuration configuration)
        {
            using (var connection = new NpgsqlConnection(NpgsqlConnectionString))
            {
                try
                {
                    connection.Open();                  
                }
                catch (NpgsqlException)
                {
                    CreateDatabase();
                    CreateLoggingTable();
                }
				finally
				{                    
                    ExportSchemaToDatabase(configuration);                    
                }
            }
        }

        private static void CreateDatabase()
        {
            var masterConnectionString = Regex.Replace(NpgsqlConnectionString, "(Database|Initial Catalog)=[^;]+", "Database=postgres", RegexOptions.IgnoreCase);
            using (var connection = new NpgsqlConnection(masterConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "CREATE DATABASE " + "\"" + GetDatabaseName() + "\"";
                command.ExecuteNonQuery();
            }
        }

        private static void CreateLoggingTable()
        {
            using (var connection = new NpgsqlConnection(NpgsqlConnectionString))
            {
                connection.Open();

                string strSql =  @" CREATE SEQUENCE ELMAH_Error_SEQUENCE;
                                    
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

        private static void ExportSchemaToDatabase(Configuration configuration)
        {
            //new SchemaExport(configuration).Create(script => System.Diagnostics.Debug.WriteLine(script), true);

            new SchemaUpdate(configuration).Execute(false, true);
        }

        private static string GetDatabaseName()
        {
            using (var connection = new NpgsqlConnection(NpgsqlConnectionString))
            {
                return connection.Database;
            }
        }
    }
}
