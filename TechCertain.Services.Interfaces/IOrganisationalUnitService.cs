using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IOrganisationalUnitService
    {
		IQueryable<OrganisationalUnit> GetAllOrganisationalUnits();

		OrganisationalUnit GetOrganisationalUnit(Guid organisationalUnitId);

        OrganisationalUnit GetOrganisationalUnitByName (string organisationalUnitName);

    }
}

