using System;
using DealEngine.Infrastructure.BaseLdap.Entities;

namespace DealEngine.Infrastructure.BaseLdap.Interfaces
{
	public interface IOpenLdapService
	{
		//bool Authenticate (Guid organisationID, string orgPassword);

		LdapUser GetUser (string userName);

		LdapUser GetUser (string userName, string userPassword);

		LdapUser GetUser (Guid userID);

		LdapOrganisation GetOrganisation (Guid organisationID);

		LdapOrganisation GetOrganisation (Guid organisationID, string orgPassword);

		LdapRole GetRole (string roleName);

		bool Create (LdapUser user);

		bool Create (LdapOrganisation organisation);

		bool Create (LdapDepartment department);

		bool Create (LdapBranch branch);

		bool Update (LdapUser user);

		bool Update (LdapOrganisation organisation);

		bool Delete (LdapUser user);

		bool Delete (LdapOrganisation organisation);

		LdapUser[] GetUsersWithOrganisation (Guid organisationID);
	}
}

