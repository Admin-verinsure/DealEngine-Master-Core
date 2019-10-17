
using Microsoft.Extensions.Logging;
using NHibernate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public class NHibernateMapperSession<TEntity> : IMapperSession<TEntity> where TEntity : class
    {
        private readonly ISession _session;
        //ILogger _logger;

        public NHibernateMapperSession(ISession session)
        {
            //_logger = logger;
            _session = session;
        }

        public IQueryable<TEntity> FindAll()
        {
            return _session.Query<TEntity>();
        }

        public Task<TEntity> GetByIdAsync(string id)
        {
            return _session.GetAsync<TEntity>(id);
        }

        public Task<TEntity> GetByIdAsync(Guid id)
        {
            return _session.GetAsync<TEntity>(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            var transaction = _session.BeginTransaction();
            try
            {
                await _session.SaveAsync(entity);
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

        public async Task SaveAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            var transaction = _session.BeginTransaction();
            try
            {
                await _session.SaveAsync(entity);
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

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
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