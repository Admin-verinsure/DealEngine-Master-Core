using NHibernate;
using SimpleInjector;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Infrastructure.FluentNHibernate.ConfigurationProvider;
using TechCertain.Infrastructure.FluentNHibernate.Repositories;

namespace TechCertain.Infrastructure.DependecyResolution
{
	public class RepositoryPackage
	{
		public void RegisterServices (Container container)
		{

            // register per webrequest is now obsolete https://stackoverflow.com/questions/42766577/registerperwebrequest-is-obsolete-can-i-use-lifestyle-scoped use Lifestyle.Scoped
            container.Register(() =>
            {
                return container.GetInstance<INHibernateConfigurationProvider>().GetDatabaseConfiguration();
            }, Lifestyle.Scoped);

            container.Register(() =>
            {
                return container.GetInstance<NHibernate.Cfg.Configuration>().BuildSessionFactory();
            }, Lifestyle.Scoped);

            container.Register(() =>
            {
                return container.GetInstance<ISessionFactory>().OpenSession();
            }, Lifestyle.Scoped);

            container.Register(typeof(IRepository<>), typeof(NHibernateRepository<>));
            container.Register<IIsolatedUnitOfWorkFactory, NHibernateIsolatedUnitOfWorkFactory>();
            container.Register<IUnitOfWorkFactory, NHibernateUnitOfWorkFactory>();            
            container.Register<INHibernateConfigurationProvider, PostgreSQLServerNHibernateConfigurationProvider>(Lifestyle.Singleton);
			container.Register<IUserRepository, UserRepository> ();
			container.Register<IOrganisationRepository, LdapOrganisationRepository> ();
        }        
    }
}
