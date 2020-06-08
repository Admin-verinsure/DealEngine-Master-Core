using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IOrganisationService
    {
		Task<Organisation> CreateNewOrganisation (Organisation organisation);
		Organisation CreateNewOrganisation (string p1, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail);
		Task DeleteOrganisation (User deletedBy, Organisation organisation);
        Task<List<Organisation>> GetAllOrganisations ();
        Task<Organisation> GetOrganisation(Guid organisationId);
        Task<Organisation> GetOrganisationByName(string organisationName);
        Task<Organisation> GetOrganisationByEmail(string organisationEmail);
        Task<Organisation> GetExistingOrganisationByEmail(string organisationEmail);
        Task UpdateOrganisation(Organisation organisation);
        Task<List<Organisation>> GetOrganisationPrincipals(ClientInformationSheet sheet);
        Task<List<Organisation>> GetSubsystemOrganisationPrincipals(ClientInformationSheet sheet);
        Task<List<Organisation>> GetAllOrganisationsByEmail(string email);
    }
}

