
using NHibernate;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace TechCertain.Domain.Interfaces
{
    public class NHibernateMapperSession<TEntity> : IMapperSession<TEntity> where TEntity : class
    {
        private readonly ISession _session;
        //private ITransaction _transaction;

        public NHibernateMapperSession(ISession session)
        {
            _session = session;
        }
      
        public void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            // not, not SaveOrUpdate as we don't need Update if we use Unit of Work semantics            
            _session.Save(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            // not, not SaveOrUpdate as we don't need Update if we use Unit of Work semantics            
            await _session.SaveAsync(entity);
        }

        public IQueryable<TEntity> FindAll()
        {
            return _session.Query<TEntity>();
        }

        public TEntity GetById(string id)
        {
            return _session.Get<TEntity>(id);
        }

        public TEntity GetById(Guid id)
        {
            return _session.Get<TEntity>(id);
        }

        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _session.DeleteAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _session.SaveOrUpdateAsync(entity);
        }
    }
}