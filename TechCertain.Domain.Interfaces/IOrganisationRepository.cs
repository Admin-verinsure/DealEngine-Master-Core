using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Domain.Interfaces
{
	public interface IOrganisationRepository
	{
		Organisation Get(Guid organisationID);

        Organisation GetOrganisation(string organisationName);

        bool Create (Organisation organisation);

		bool Update (Organisation organisation);

		bool Delete (Organisation organisation);
	}
}

