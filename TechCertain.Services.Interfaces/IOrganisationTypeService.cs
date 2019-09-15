using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IOrganisationTypeService
    {
        void AddNew(string name);

        IEnumerable<OrganisationType> GetOrganisationTypes();

        OrganisationType CreateNewOrganisationType(User user, string organisationTypeName);

        OrganisationType GetOrganisationTypeByName(string organisationTypeName);

    }
}
