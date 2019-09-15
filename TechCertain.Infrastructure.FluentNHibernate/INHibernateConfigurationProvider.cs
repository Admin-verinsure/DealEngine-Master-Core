using NHibernate.Cfg;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public interface INHibernateConfigurationProvider
    {
        Configuration GetDatabaseConfiguration();
    }
}
