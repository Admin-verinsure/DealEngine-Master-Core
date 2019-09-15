using NHibernate;
using System;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public class NHibernateIsolatedUnitOfWorkFactory : IIsolatedUnitOfWorkFactory
    {
        private readonly ISessionFactory sessionFactory;

        public NHibernateIsolatedUnitOfWorkFactory(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");
            this.sessionFactory = sessionFactory;
        }

        public IUnitOfWork BeginUnitOfWork()
        {
            return new NHibernateUnitOfWork(sessionFactory);
        }
    }
}
