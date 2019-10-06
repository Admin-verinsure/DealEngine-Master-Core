using System;
using System.Threading.Tasks;

namespace TechCertain.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task Commit();
        IUnitOfWork BeginUnitOfWork();
    }
}
