using System;
using System.Threading.Tasks;

namespace DealEngine.Infrastructure.FluentNHibernate
{
    public interface IUnitOfWork : IDisposable
    {
        Task Commit();
        IUnitOfWork BeginUnitOfWork();
        Task Rollback();
    }
}
