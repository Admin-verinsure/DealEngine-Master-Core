using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using DealEngine.Infrastructure.BaseLdap.Interfaces;

namespace DealEngine.Infrastructure.BaseLdap.Repositories
{
	public class LdapRepository : RepositoryBase, ILdapRepository 
	{
        ISessionService _ldapSession;

		public LdapRepository (ISessionService ldapSession, ILdapConfigService ldapConfigService)
			: base (ldapConfigService)
		{
			_ldapSession = ldapSession;
		}

        #region ILdapRepository implementation

        public LdapEntry GetEntry(string dn)
        {
            return GetEntry(dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
        }

        public LdapEntry GetEntry (string dn, string userDN, string password)
		{
			LdapConnection connection = _ldapSession.GetConnection ();
			LdapEntry entry = null;
			try
			{
				connection.Bind (userDN, password);

                entry = connection.Read(dn, new string[] { "*", "+" });
            }
			catch (LdapException ex)
			{
				// TODO Map to error logger
				Console.WriteLine("ILdapRepository.GetEntry");
				Console.WriteLine ("{0} {1} {2}", dn, userDN, password);
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				if (connection.Connected)
					connection.Disconnect ();
			}
			return entry;
		}

		public LdapEntry SearchFor (string searchDN, string filter, string[] attributes)
		{
			return SearchFor (searchDN, filter, attributes, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

		public LdapEntry SearchFor (string searchDN, string filter, string[] attributes, string userDN, string password)
		{
			LdapConnection connection = _ldapSession.GetConnection ();
			LdapEntry entry = null;
			try
			{
				connection.Bind (userDN, password);

				var searchResults = connection.Search (searchDN, LdapConnection.SCOPE_ONE, filter, attributes, false);

				while (searchResults.hasMore ())
				{
					entry = searchResults.next ();
					break;
				}
			}
			catch (LdapException ex)
			{
				// TODO Map to error logger
				Console.WriteLine("ILdapRepository.SearchFor");
				Console.WriteLine ("{0} {1} {2} {3} {4}", searchDN, filter, attributes, userDN, password);
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				if (connection.Connected)
					connection.Disconnect ();
			}
			return entry;
		}

		public LdapEntry[] FindAll (string searchDN, string filter, string[] attributes)
		{
			return FindAll (searchDN, filter, attributes, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

		public LdapEntry[] FindAll (string searchDN, string filter, string[] attributes, string userDN, string password)
		{
			LdapConnection connection = _ldapSession.GetConnection ();
			List<LdapEntry> entries = new List<LdapEntry> ();
			try
			{
				connection.Bind (userDN, password);

				var searchResults = connection.Search (searchDN, LdapConnection.SCOPE_ONE, filter, attributes, false);

				while (searchResults.hasMore ())
					entries.Add(searchResults.next ());
			}
			catch (LdapException ex)
			{
				// TODO Map to error logger
				Console.WriteLine("ILdapRepository.FindAll");
				Console.WriteLine ("{0} {1} {2} {3} {4}", searchDN, filter, attributes, userDN, password);
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				if (connection.Connected)
					connection.Disconnect ();
			}
			return entries.ToArray();
		}

		public bool AddEntry (LdapEntry entry)
		{
			LdapConnection connection = _ldapSession.GetConnection ();
			try
			{
				connection.Bind (_ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
				connection.Add(entry);
			}
			catch (LdapException ex)
			{
				// TODO Map to error logger
				Console.WriteLine ("ILdapRepository.AddEntry");
				Console.WriteLine ("{0}", entry);
				Console.WriteLine (ex);
				Console.WriteLine (ex.LdapErrorMessage);
				return false;
			}
			finally
			{
				if (connection.Connected)
					connection.Disconnect ();
			}
			return true;
		}

		public bool UpdateEntry(string dn, LdapModification[] modifiers)
		{
			LdapConnection connection = _ldapSession.GetConnection ();
			try
			{
				connection.Bind (_ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);

				connection.Modify(dn, modifiers);
			}
			catch (LdapException ex)
			{
				// TODO Map to error logger
				Console.WriteLine("ILdapRepository.UpdateEntry");
				Console.WriteLine ("{0}", dn);
				Console.WriteLine(ex.ToString());
				return false;
			}
			finally
			{
				if (connection.Connected)
					connection.Disconnect ();
			}
			return true;
		}

		public bool UpdateEntry (string dn, List<LdapModification> modifiers)
		{
			return UpdateEntry (dn, modifiers.ToArray ());
		}       

        #endregion
    }
}

