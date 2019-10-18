using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public interface IMapperSession<TEntity> where TEntity : class
    {
        IQueryable<TEntity> FindAll();
        Task<TEntity> GetByIdAsync(string id);
        Task<TEntity> GetByIdAsync(Guid id);
        void RemoveAsync(TEntity entity);
        void UpdateAsync(TEntity entity);
        void AddAsync(TEntity entity);
        void SaveAsync(TEntity entity);
    }
}
