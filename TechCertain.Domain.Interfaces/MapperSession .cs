
using NHibernate;
using System.Threading.Tasks;


namespace TechCertain.Domain.Interfaces
{
    public class NHibernateMapperSession<TEntity> : IMapperSession<TEntity> where TEntity : class
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public NHibernateMapperSession(ISession session)
        {
            _session = session;
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public async Task Commit()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                // log exception here
                await Rollback();
            }
            finally
            {
                CloseTransaction();
            }
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public void CloseTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task Save(TEntity entity)
        {           
            try
            {
                BeginTransaction();
                await _session.SaveOrUpdateAsync(entity);
                await Commit();
            }
            catch
            {
                // log exception here
                await Rollback();
            }
            finally
            {
                CloseTransaction();
            }
        }

        public async Task Delete(TEntity entity)
        {
            try
            {
                BeginTransaction();
                await _session.DeleteAsync(entity);
                await Commit();
            }
            catch
            {
                // log exception here
                await Rollback();
            }
            finally
            {
                CloseTransaction();
            }            
        }
    }
}