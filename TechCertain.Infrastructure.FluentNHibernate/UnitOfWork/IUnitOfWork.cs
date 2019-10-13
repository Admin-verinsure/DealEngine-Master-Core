using System;
using System.Threading.Tasks;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public interface IUnitOfWork : IDisposable
    {
        Task Commit();
        IUnitOfWork BeginUnitOfWork();
    }
}
