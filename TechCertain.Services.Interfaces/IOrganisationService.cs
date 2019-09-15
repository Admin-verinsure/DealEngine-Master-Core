using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IOrganisationService
    {
		Organisation CreateNewOrganisation (Organisation organisation);

		Organisation CreateNewOrganisation (string p1, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail);

		bool DeleteOrganisation (User deletedBy, Organisation organisation);

		IQueryable<Organisation> GetAllOrganisations ();

		Organisation GetOrganisation(Guid organisationId);

		Organisation GetOrganisationByName (string organisationName);

        Organisation GetOrganisationByEmail(string organisationEmail);

        bool UpdateOrganisation(Organisation organisation);
       // void Create(User user);


    }
}

