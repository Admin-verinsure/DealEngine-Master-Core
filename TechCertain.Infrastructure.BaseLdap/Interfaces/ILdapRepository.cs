using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;

namespace TechCertain.Infrastructure.BaseLdap.Interfaces
{
	public interface ILdapRepository
	{
        LdapEntry GetEntry(string dn);

        LdapEntry GetEntry(string dn, string userDN, string password);

		LdapEntry SearchFor(string searchDN, string filter, string[] attributes);

		LdapEntry SearchFor(string searchDN, string filter, string[] attributes, string userDN, string password);

		LdapEntry[] FindAll(string searchDN, string filter, string[] attributes);

		LdapEntry[] FindAll(string searchDN, string filter, string[] attributes, string userDN, string password);

		bool AddEntry (LdapEntry entry);

		bool UpdateEntry(string dn, LdapModification[] modifiers);

		bool UpdateEntry(string dn, List<LdapModification> modifiers);
	}
}

