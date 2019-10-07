using NHibernate;
using System;
using System.Threading.Tasks;


namespace TechCertain.Domain.Interfaces
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        private ISession _session;
        private ITransaction _transaction;

        public NHibernateUnitOfWork(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
            _transaction = _session.BeginTransaction();

        }

        public async Task Commit()
        {
            try
            {               
              _transaction.Commit();
            }
            catch(Exception ex)
            {
                // log exception here
                await Rollback();
                throw new Exception(ex.Message);
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

        public void Dispose()
        {
            //_transaction.Dispose();
            //_transaction = null;
        }

        public IUnitOfWork BeginUnitOfWork()
        {
            return new NHibernateUnitOfWork(_session);
        }
    }
}
