using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public interface INHibernateConfigurationProvider
    {
        Configuration GetDatabaseConfiguration();
    }
}
