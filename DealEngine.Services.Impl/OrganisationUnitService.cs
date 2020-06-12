using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
	public class OrganisationalUnitService : IOrganisationalUnitService
    {
		IMapperSession<OrganisationalUnit> _organisationUnitRepository;

        public OrganisationalUnitService(IMapperSession<OrganisationalUnit> organisationUnitRepository)
        {
            _organisationUnitRepository = organisationUnitRepository;
        }

        public async Task<List<OrganisationalUnit>> GetAllOrganisationalUnitsNames()
        {
            return await _organisationUnitRepository.FindAll().ToListAsync();
        }

        public async Task<OrganisationalUnit> GetOrganisationalUnit(Guid organisationalUnitId)
        {
            OrganisationalUnit organisationalUnit = await _organisationUnitRepository.GetByIdAsync(organisationalUnitId);
            // have a repo organisation? Return it
            if (organisationalUnit != null)
                return organisationalUnit;
            throw new Exception("Organisation with id [" + organisationalUnitId + "] does not exist in the system");
        }

        public async Task<OrganisationalUnit> GetOrganisationalUnitByName(string organisationalUnitName)
        {
            return await _organisationUnitRepository.FindAll().FirstOrDefaultAsync(o => o.Name == organisationalUnitName);
        }

        public async Task<List<OrganisationalUnit>> GetAllOrganisationalUnitsByOrg(Organisation org)
        {
            return await _organisationUnitRepository.FindAll().Where(o => o.Company == org).ToListAsync();
        }

        public async Task<List<string>> GetAllOrganisationalUnitsName()
        {
            return await _organisationUnitRepository.FindAll().Select(ou => ou.Name).ToListAsync();
        }

        public async Task<OrganisationalUnit> CreateOrganisationalUnit(OrganisationalUnit organisationalUnit)
        {
            await _organisationUnitRepository.AddAsync(organisationalUnit);
            return organisationalUnit;
        }
    }
}

