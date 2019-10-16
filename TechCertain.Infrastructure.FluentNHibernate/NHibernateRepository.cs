using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    /// <remarks>
    /// If we do a GetById or FindAll() and make changes to anything in that aggregate
    /// then completing the request should save any changes made back to the db.
    /// If we new up an entity and Add it, it should get inserted back to the db.
    /// If we Delete an entity it should be deleted from the db.
    /// No need for Update() or Save() or Write() or nonsense like that!
    /// </remarks>
    public class NHibernateRepository<TEntity> : IMapperSession<TEntity> where TEntity : class, IAggregateRoot
    {
        private readonly ISession session;

        public NHibernateRepository(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            this.session = session;
        }

        public void Update(TEntity entity)
        {
            session.Update(entity);
            session.Save(entity);
        }

        public TEntity GetById(Guid id)
        {
            return session.Get<TEntity>(id);
        }

        public TEntity GetById(String id)
        {
            return session.Get<TEntity>(id);
        }

        public IQueryable<TEntity> FindAll()
        {
            return session.Query<TEntity>();
        }

        public void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            // not, not SaveOrUpdate as we don't need Update if we use Unit of Work semantics            
           
            session.SaveOrUpdate(entity);
        }

        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            session.Delete(entity);
        }
    }
}
