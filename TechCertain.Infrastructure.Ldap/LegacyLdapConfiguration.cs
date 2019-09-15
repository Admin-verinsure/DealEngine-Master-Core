using System.Configuration;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace TechCertain.Infrastructure.Ldap
{
	public class LegacyLdapConfiguration : ILegacyLdapConfiguration
	{

		public string AdminDn {
			get {
				return ConfigurationManager.AppSettings ["LegacyLdapBindDN"];
			}
		}

		public string AdminPassword {
			get {
				return ConfigurationManager.AppSettings ["LegacyLdapBindPW"];
			}
		}

		public string LdapHost {
			get {
				return ConfigurationManager.AppSettings ["LegacyLdapServer"];
			}
		}

		public int LdapPort {
			get {
				return int.Parse (ConfigurationManager.AppSettings ["LegacyLdapPort"]);
			}
		}

		public string BaseDn {
			get {
				return ConfigurationManager.AppSettings ["LegacyLdapBaseDN"];
			}
		}
	}
}

