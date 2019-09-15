using NHibernate;
using System;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public class NHibernateUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ISession session;
        private readonly bool sessionWasProvided;
        private readonly ITransaction transaction;

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory");

            session = sessionFactory.OpenSession();
            transaction = session.BeginTransaction();
            sessionWasProvided = false;
        }

        public NHibernateUnitOfWork(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            this.session = session;
            transaction = session.BeginTransaction();
            sessionWasProvided = true;
        }

   //     public TEntity GetById<TEntity>(Guid id) where TEntity : class, IAggregateRoot
   //     {
   //         return session.Get<TEntity>(id);
   //     }

   //     public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IAggregateRoot
   //     {
   //         return session.Query<TEntity>();
   //     }

   //     public void Add<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
   //     {
   //         if (entity == null) throw new ArgumentNullException("entity");
			//entity = session.Merge(entity);
			//session.SaveOrUpdate(entity);
   //     }

   //     public void Remove<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
   //     {
   //         if (entity == null) throw new ArgumentNullException("entity");
   //         session.Delete(entity);
   //     }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Dispose()
        {
            try
            {
                if (transaction.IsActive)
                    transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();

                if (!sessionWasProvided)
                {
                    session.Dispose();
                }
            }
        }    
    }
}
