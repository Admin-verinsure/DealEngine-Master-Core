using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IOrganisationalUnitService
    {
        Task<List<OrganisationalUnit>> GetAllOrganisationalUnitsNames();

        Task<OrganisationalUnit> GetOrganisationalUnit(Guid organisationalUnitId);

        Task<OrganisationalUnit> GetOrganisationalUnitByName (string organisationalUnitName);
        Task<List<OrganisationalUnit>> GetAllOrganisationalUnitsByOrg(Organisation org);
        Task<List<string>> GetAllOrganisationalUnitsName();
    }
}

