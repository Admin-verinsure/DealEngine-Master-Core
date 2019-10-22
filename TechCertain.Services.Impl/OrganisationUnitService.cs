﻿using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class OrganisationalUnitService : IOrganisationalUnitService
    {
		IMapperSession<OrganisationalUnit> _organisationUnitRepository;


        public OrganisationalUnitService(IMapperSession<OrganisationalUnit> organisationUnitRepository)
        {
            _organisationUnitRepository = organisationUnitRepository;
        }

        public async Task<List<OrganisationalUnit>> GetAllOrganisationalUnits()
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
    }
}

