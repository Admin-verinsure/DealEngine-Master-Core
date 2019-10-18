using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    public interface IMapperSession<TEntity> where TEntity : class
    {
        IQueryable<TEntity> FindAll();
        Task<TEntity> GetById(string id);
        Task<TEntity> GetById(Guid id);
        Task RemoveAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);        
        Task AddAsync(TEntity entity);        
        Task SaveAsync(TEntity entity);
    }
}
