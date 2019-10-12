using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TechCertain.Domain.Interfaces
{
    public interface IMapperSession<TEntity> where TEntity : class
    {
        IQueryable<TEntity> FindAll();
        TEntity GetById(String id);
        TEntity GetById(Guid id);
        void Update(TEntity entity);
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
    }
}
