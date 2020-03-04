
using Bismuth.Ldap;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.Ldap.Interfaces
{
	public interface ILegacyEntityMapping
	{
		User UserFromLdap (LdapEntry entry);
		Organisation OrganisationFromLdap (LdapEntry entry);
		string GetDn (User entity, string baseDn);
		string GetDn (Organisation entity, string baseDn);
	}
}

