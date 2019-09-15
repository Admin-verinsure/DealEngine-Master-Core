using System;
using Novell.Directory.Ldap;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.BaseLdap.Interfaces
{
	public interface IOpenLdapOrganisationService
    {
		Organisation GetOrganisation(Guid organisationID);

		bool Create (Organisation organisation);

		bool Update (Organisation organisation);

		bool Delete (Organisation organisation);

		Organisation OrganisationFrom (LdapEntry entry);
	}
}

