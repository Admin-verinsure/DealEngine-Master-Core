using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class BoatUseService : IBoatUseService
    {
        IUnitOfWorkFactory _unitOfWork;
        ILogger _logging;
        IRepository<BoatUse> _boatUseRepository;

        public BoatUseService(IUnitOfWorkFactory unitOfWork, ILogger logging, IRepository<BoatUse> boatUseRepository)
        {
            _unitOfWork = unitOfWork;
            _logging = logging;
            _boatUseRepository = boatUseRepository;            
        }

        public BoatUse CreateNewBoatUse(BoatUse boatUse)
        {
            UpdateBoatUse(boatUse);
            return boatUse;
        }

        public bool DeleteBoatUse(User deletedBy, BoatUse boatUse)
        {
            boatUse.Delete(deletedBy);
            return UpdateBoatUse(boatUse);
        }

        public IQueryable<BoatUse> GetAllBoatUses()
        {
            // we don't want to query ldap. That way lies timeouts. Or Dragons.
            return _boatUseRepository.FindAll();
        }

        public BoatUse GetBoatUse(Guid boatUseId)
        {
            BoatUse boatUse = _boatUseRepository.GetById(boatUseId);
            // have a repo boatUse? Return it
            if (boatUse != null)
                return boatUse;
            // have a ldap boatUse but no repo? Update NHibernate & return
            if (boatUse != null)
            {
                UpdateBoatUse(boatUse);
                return boatUse;
            }
            // no boatUse at all? Throw exception
            throw new Exception("BoatUse with id [" + boatUseId + "] does not exist in the system");
        }

        public bool UpdateBoatUse(BoatUse boatUse)
        {
            _boatUseRepository.Add(boatUse);
            return true;
        }

/*
public BoatUse GetBoatUseById(string boatUseId)
         {
             return _boatUseRepository.FindAll().FirstOrDefault(bus => bus.Id == boatUseId);
         }
*/
/*
public BoatUse GetBoatUseByEmail(string boatUseEmail)
        {
            return _boatUseRepository.FindAll().FirstOrDefault(o => o.Email == boatUseEmail);
        }
*/
/*
public BoatUse CreateNewBoatUse(string p1, BoatUseType boatUseType, string ownerFirstName, string ownerLastName, string ownerEmail)
{
    BoatUse boatUse = new BoatUse(null, Guid.NewGuid(), p1, boatUseType);
    // TODO - finish this later since I need to figure out what calls the controller function that calls this service function
    throw new NotImplementedException();
}
*/

    }
}
