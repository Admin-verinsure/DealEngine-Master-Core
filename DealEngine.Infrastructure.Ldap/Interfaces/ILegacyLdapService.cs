using System;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.Ldap.Interfaces
{
	public interface ILegacyLdapService
	{
		User GetLegacyUser (string username);
		User GetLegacyUser (Guid userId);
		User GetLegacyUserByEmail (string email);
		Organisation GetLegacyOrganisation (Guid organisationId);
	}
}

