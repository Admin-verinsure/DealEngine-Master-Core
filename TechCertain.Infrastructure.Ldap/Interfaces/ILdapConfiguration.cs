

namespace TechCertain.Infrastructure.Ldap.Interfaces
{
	public interface ILdapConfiguration
	{
		string LdapHost { get; }
		int LdapPort { get; }

		string AdminDn { get; }
		string AdminPassword { get; }

		string BaseDn { get; }
	}

	public interface ILegacyLdapConfiguration : ILdapConfiguration
	{

	}
}

