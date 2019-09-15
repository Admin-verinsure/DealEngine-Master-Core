using System;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Interfaces
{
    /// <summary>
    /// Simple traditional repository. 
    /// May go the way of the dodo if query objects takes off here.
    /// </summary>
    /// <remarks>
    /// We're being DDD strict here; can't get at entities and value objects except
    /// by walking down from their aggregate root.
    /// </remarks>
    public interface IRepository<EntityBase> where EntityBase : class , IAggregateRoot
    {
        EntityBase GetById(Guid id);
        EntityBase GetById(String id);
        void Update(EntityBase entity);

        IQueryable<EntityBase> FindAll();

        void Add(EntityBase entity);

        void Remove(EntityBase entity);

    }
}
