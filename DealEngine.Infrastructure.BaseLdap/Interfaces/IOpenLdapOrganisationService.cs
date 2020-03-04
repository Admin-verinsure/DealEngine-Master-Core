using System;
using Novell.Directory.Ldap;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.BaseLdap.Interfaces
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

