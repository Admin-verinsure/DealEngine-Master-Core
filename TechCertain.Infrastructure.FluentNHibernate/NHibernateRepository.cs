using NHibernate;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Infrastructure.FluentNHibernate
{
    /// <remarks>
    /// If we do a GetById or FindAll() and make changes to anything in that aggregate
    /// then completing the request should save any changes made back to the db.
    /// If we new up an entity and Add it, it should get inserted back to the db.
    /// If we Delete an entity it should be deleted from the db.
    /// No need for Update() or Save() or Write() or nonsense like that!
    /// </remarks>
    //public class NHibernateRepository<TEntity> : IMapperSession<TEntity> where TEntity : class, IAggregateRoot
    //{
    //    private readonly ISession session;

    //    public NHibernateRepository(ISession session)
    //    {
    //        if (session == null) throw new ArgumentNullException("session");
    //        this.session = session;
    //    }

    //    public async Task<TEntity> GetByIdAsync(Guid id)
    //    {
    //        return session.GetAsync<TEntity>(id).Result;
    //    }

    //    public async Task<TEntity> GetByIdAsync(string id)
    //    {
    //        return session.GetAsync<TEntity>(id).Result;
    //    }

    //    public async Task<IQueryable<TEntity>> FindAll()
    //    {
    //        Thread.Sleep(1000);
    //        return  session.Query<TEntity>();
    //    }

    //    public async Task UpdateAsync(TEntity entity)
    //    {
    //        if (entity == null) throw new ArgumentNullException("entity");
    //        await session.UpdateAsync(entity);
    //    }

    //    public async Task RemoveAsync(TEntity entity)
    //    {
    //        if (entity == null) throw new ArgumentNullException("entity");
    //        await session.DeleteAsync(entity);
    //    }

    //    public async Task AddAsync(TEntity entity)
    //    {
    //        if (entity == null) throw new ArgumentNullException("entity");           
    //        await session.SaveOrUpdateAsync(entity);
    //    }

    //    public async Task SaveAsync(TEntity entity)
    //    {
    //        if (entity == null) throw new ArgumentNullException("entity");
    //        await session.SaveAsync(entity);
    //    }
    //}
}
