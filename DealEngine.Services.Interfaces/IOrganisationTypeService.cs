using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IOrganisationTypeService
    {
        void AddNew(string name);

        IEnumerable<OrganisationType> GetOrganisationTypes();

        Task<OrganisationType> CreateNewOrganisationType(User user, string organisationTypeName);

        Task<OrganisationType> GetOrganisationTypeByName(string organisationTypeName);

    }
}
