using System;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class BoatUseService : IBoatUseService
    {       
        IMapperSession<BoatUse> _boatUseRepository;

        public BoatUseService(IMapperSession<BoatUse> boatUseRepository)
        {          
            _boatUseRepository = boatUseRepository;            
        }

        public async Task<BoatUse> CreateNewBoatUse(BoatUse boatUse)
        {
            await UpdateBoatUse(boatUse);
            return boatUse;
        }

        public async Task DeleteBoatUse(User deletedBy, BoatUse boatUse)
        {
            boatUse.Delete(deletedBy);
            await UpdateBoatUse(boatUse);
        }

        public IQueryable<BoatUse> GetAllBoatUses()
        {
            // we don't want to query ldap. That way lies timeouts. Or Dragons.
            return _boatUseRepository.FindAll();
        }

        public async Task<BoatUse> GetBoatUse(Guid boatUseId)
        {
            BoatUse boatUse = _boatUseRepository.GetByIdAsync(boatUseId).Result;
            // have a repo boatUse? Return it
            if (boatUse != null)
                return boatUse;
            // have a ldap boatUse but no repo? Update NHibernate & return
            if (boatUse != null)
            {
                await UpdateBoatUse(boatUse);
                return boatUse;
            }
            // no boatUse at all? Throw exception
            throw new Exception("BoatUse with id [" + boatUseId + "] does not exist in the system");
        }

        public async Task UpdateBoatUse(BoatUse boatUse)
        {
            await _boatUseRepository.AddAsync(boatUse);
        }
    }
}
