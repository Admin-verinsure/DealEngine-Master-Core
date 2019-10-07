using NHibernate;
using System;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Domain.Interfaces
{
    public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISession session;

        public NHibernateUnitOfWorkFactory(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            this.session = session;
        }

        public IUnitOfWork BeginUnitOfWork()
        {
            return new NHibernateUnitOfWork(session);
        }
    }
}
