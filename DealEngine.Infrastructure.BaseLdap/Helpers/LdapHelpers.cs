using System;
using Novell.Directory.Ldap;

namespace DealEngine.Infrastructure.BaseLdap.Helpers
{
	public class LdapHelpers
	{
		public static string GetLdapAttributeValue(LdapAttributeSet attributeSet, string attributeName)
		{
			LdapAttribute attribute = attributeSet.getAttribute (attributeName);
			if (attribute != null)
				return attribute.StringValue;

			return string.Empty;
		}

		public static string[] GetLdapAttributeValues(LdapAttributeSet attributeSet, string attributeName)
		{
			LdapAttribute attribute = attributeSet.getAttribute (attributeName);
			if (attribute != null)
				return attribute.StringValueArray;

			return new string[] {};
		}

		public static Guid GetGuidValue(LdapAttributeSet attributeSet, string attributeName)
		{
			Guid guid = Guid.Empty;
			if (!Guid.TryParse (LdapHelpers.GetLdapAttributeValue (attributeSet, attributeName), out guid))
			{
				throw new Exception (string.Format ("No valid Guid found for \"{0}\" in LdapAttributeSet", attributeName));
			}
			return guid;
		}

		public static LdapModification GetLdapModifider(int operation, string attribute, string value)
		{
			if (!string.IsNullOrWhiteSpace(attribute) && !string.IsNullOrEmpty(value))
				return new LdapModification (operation, new LdapAttribute (attribute, value));
			return null;
		}

		public static LdapModification GetLdapModifider(int operation, string attribute, string[] values)
		{
			if (!string.IsNullOrWhiteSpace(attribute) && values != null)
				return new LdapModification (operation, new LdapAttribute (attribute, values));
			return null;
		}

		public static LdapModification GetLdapModifider(int operation, string attribute, Guid value)
		{
			return GetLdapModifider(operation, attribute, value.ToString());
		}

		public static LdapModification GetLdapModifider(int operation, string attribute, Guid[] values)
		{
			string[] array = new string[values.Length];
			for (int i = 0; i < array.Length; i++)
				array [i] = values [i].ToString ();
			return GetLdapModifider(operation, attribute, array);
		}
	}
}

