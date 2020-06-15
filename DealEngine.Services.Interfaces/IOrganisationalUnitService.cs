using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IOrganisationalUnitService
    {
        Task<List<OrganisationalUnit>> GetAllOrganisationalUnitsNames();

        Task<OrganisationalUnit> GetOrganisationalUnit(Guid organisationalUnitId);

        Task<OrganisationalUnit> GetOrganisationalUnitByName (string organisationalUnitName);
        Task<List<string>> GetAllOrganisationalUnitsName();
        Task<OrganisationalUnit> CreateOrganisationalUnit(OrganisationalUnit organisationalUnit);
    }
}

