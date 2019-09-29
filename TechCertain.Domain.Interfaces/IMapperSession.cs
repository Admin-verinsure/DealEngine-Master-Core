using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace TechCertain.Domain.Interfaces
{
    public interface IMapperSession<EntityBase> where EntityBase : class
    {
        void BeginTransaction();
        Task Commit();
        Task Rollback();
        void CloseTransaction();
        Task Save(EntityBase entity);
        Task Delete(EntityBase entity);

    }
}
