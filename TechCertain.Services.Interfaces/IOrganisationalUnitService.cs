using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IOrganisationalUnitService
    {
        Task<List<OrganisationalUnit>> GetAllOrganisationalUnits();

        Task<OrganisationalUnit> GetOrganisationalUnit(Guid organisationalUnitId);

        Task<OrganisationalUnit> GetOrganisationalUnitByName (string organisationalUnitName);

    }
}

