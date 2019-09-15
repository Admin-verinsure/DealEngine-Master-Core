using System;
using System.Configuration;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace TechCertain.Infrastructure.Ldap
{
	public class LdapConfiguration : ILdapConfiguration
	{
		public string AdminDn {
			get {
				return string.Format (ConfigurationManager.AppSettings ["OpenLdapBindDN"], BaseDn);
			}
		}

		public string AdminPassword {
			get {
				return ConfigurationManager.AppSettings ["OpenLdapBindPW"];
			}
		}

		public string LdapHost {
			get {
				return ConfigurationManager.AppSettings ["OpenLdapServer"];
			}
		}

		public int LdapPort {
			get {
				return int.Parse (ConfigurationManager.AppSettings ["OpenLdapPort"]);
			}
		}

		public string BaseDn {
			get {
				return ConfigurationManager.AppSettings ["OpenLdapBaseDN"];
			}
		}
	}
}

