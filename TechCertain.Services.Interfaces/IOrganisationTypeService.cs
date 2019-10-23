using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IOrganisationTypeService
    {
        void AddNew(string name);

        IEnumerable<OrganisationType> GetOrganisationTypes();

        Task<OrganisationType> CreateNewOrganisationType(User user, string organisationTypeName);

        Task<OrganisationType> GetOrganisationTypeByName(string organisationTypeName);

    }
}
