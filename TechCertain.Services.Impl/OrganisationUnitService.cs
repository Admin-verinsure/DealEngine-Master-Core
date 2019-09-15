using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class OrganisationalUnitService : IOrganisationalUnitService
    {
		IUnitOfWorkFactory _unitOfWork;
		IRepository<OrganisationalUnit> _organisationUnitRepository;


        public OrganisationalUnitService(IUnitOfWorkFactory unitOfWork, IRepository<OrganisationalUnit> organisationUnitRepository)
        {
            _organisationUnitRepository = organisationUnitRepository;
            _unitOfWork = unitOfWork;
        }

        public IQueryable<OrganisationalUnit> GetAllOrganisationalUnits()
        {
            return _organisationUnitRepository.FindAll();
        }

        public OrganisationalUnit GetOrganisationalUnit(Guid organisationalUnitId)
        {
            OrganisationalUnit organisationalUnit = _organisationUnitRepository.GetById(organisationalUnitId);
            // have a repo organisation? Return it
            if (organisationalUnit != null)
                return organisationalUnit;
            throw new Exception("Organisation with id [" + organisationalUnitId + "] does not exist in the system");
        }

        public OrganisationalUnit GetOrganisationalUnitByName(string organisationalUnitName)
        {
            return _organisationUnitRepository.FindAll().FirstOrDefault(o => o.Name == organisationalUnitName);
        }
    }
}

