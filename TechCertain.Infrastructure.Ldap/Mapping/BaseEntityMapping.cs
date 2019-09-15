using System;
using System.Collections.Generic;
using Bismuth.Ldap;

namespace TechCertain.Infrastructure.Ldap.Mapping
{
	public class BaseEntityMapping
	{
		protected void AddNonNullAttribute (LdapEntry entry, string attributeName, string value)
		{
			if (entry == null)
				throw new ArgumentNullException (nameof (entry));
			if (string.IsNullOrWhiteSpace (attributeName))
				throw new ArgumentNullException (nameof (attributeName));
			if (string.IsNullOrEmpty (value))
				return;
			entry.AddAttribute (attributeName, value);
		}

		protected void AddNonNullAttribute (List<ModifyAttribute> modificationsList, string type, ModificationType modification, string value)
		{
			if (modificationsList == null)
				throw new ArgumentNullException (nameof (modification));
			if (string.IsNullOrEmpty (type))
				throw new ArgumentNullException (nameof (type));
			if (string.IsNullOrEmpty (value))
				return;
			modificationsList.Add (new ModifyAttribute (type, modification, value));
		}

		protected void AddDefaultAttribute (LdapEntry entry, string attributeName, string value, string defaultValue)
		{
			if (entry == null)
				throw new ArgumentNullException (nameof (entry));
			if (string.IsNullOrWhiteSpace (attributeName))
				throw new ArgumentNullException (nameof (attributeName));
			if (string.IsNullOrWhiteSpace (defaultValue))
				throw new ArgumentNullException (nameof (defaultValue));

			if (string.IsNullOrEmpty (value))
				entry.AddAttribute (attributeName, defaultValue);
			else
				entry.AddAttribute (attributeName, value);
		}
	}
}

