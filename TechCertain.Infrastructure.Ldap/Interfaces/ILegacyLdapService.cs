using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.Ldap.Interfaces
{
	public interface ILegacyLdapService
	{
		User GetLegacyUser (string username);
		User GetLegacyUser (Guid userId);
		User GetLegacyUserByEmail (string email);
		Organisation GetLegacyOrganisation (Guid organisationId);
	}
}

