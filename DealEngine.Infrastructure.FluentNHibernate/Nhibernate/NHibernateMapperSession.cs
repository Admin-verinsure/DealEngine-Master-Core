using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealEngine.Infrastructure.FluentNHibernate
{
    public class NHibernateMapperSession<TEntity> : IMapperSession<TEntity> where TEntity : class
    {
        private ISession _session;
        private readonly ISessionFactory _sessionFactory;

        public NHibernateMapperSession(ISession session, ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _session = session;
        }

        public IQueryable<TEntity> FindAll()
        {
            return _session.Query<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return await _session.GetAsync<TEntity>(id);
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _session.GetAsync<TEntity>(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            if (!_session.IsOpen)
            {
                _session = _sessionFactory.OpenSession();
            }

            var transaction = _session.BeginTransaction();
            try
            {
                await _session.SaveOrUpdateAsync(entity);
                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                //_logger.LogDebug(ex.Message);
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }

            transaction.Dispose();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            var transaction = _session.BeginTransaction();
            try
            {
                await _session.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                //_logger.LogDebug(ex.Message);
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }

            transaction.Dispose();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (!_session.IsOpen)
            {
                _session = _sessionFactory.OpenSession();
            }

            var transaction = _session.BeginTransaction();
            try
            {
                await _session.SaveOrUpdateAsync(entity);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                //_logger.LogDebug(ex.Message);
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }

            transaction.Dispose();
        }

    }
}