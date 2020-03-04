using Novell.Directory.Ldap;

namespace DealEngine.Infrastructure.BaseLdap.Helpers
{
	public static class LdapExtensionHelper
	{
		public static void AddAttribute(this LdapAttributeSet obj, string attributeName, string value)
		{
			obj.Add (new LdapAttribute (attributeName, value));
		}

		public static void AddAttribute(this LdapAttributeSet obj, string attributeName, string[] values)
		{
			obj.Add (new LdapAttribute (attributeName, values));
		}
	}
}

