using System;
using Bismuth.Ldap;
using Bismuth.Ldap.Requests;
using Bismuth.Ldap.Responses;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.Ldap.Interfaces;

namespace DealEngine.Infrastructure.Ldap.Services
{
	public class LegacyLdapService : ILegacyLdapService
	{
		ILegacyLdapConfiguration _ldapConfiguration;

		ILegacyEntityMapping _entityMapping;

		public LegacyLdapService (ILegacyLdapConfiguration ldapConfiguration, ILegacyEntityMapping legacyEntityMapping)
		{
			_ldapConfiguration = ldapConfiguration;
			_entityMapping = legacyEntityMapping;
		}

		public User GetLegacyUser (string username)
		{
			using (LdapClient client = GetLdapServer ()) {
				LdapEntry entry = SearchForSingle (client, "(uid=" + username + ")");
				if (entry != null)
					return _entityMapping.UserFromLdap (entry);
			}
			return null;
		}

		public User GetLegacyUser (Guid userId)
		{
			using (LdapClient client = GetLdapServer ()) {
				LdapEntry entry = SearchForSingle (client, "(tcUserID=" + userId + ")");
				if (entry != null)
					return _entityMapping.UserFromLdap (entry);
			}
			return null;
		}

		public User GetLegacyUserByEmail (string email)
		{
			using (LdapClient client = GetLdapServer ()) {
				LdapEntry entry = SearchForSingle (client, "(mail=" + email + ")");
				if (entry != null)
					return _entityMapping.UserFromLdap (entry);
			}
			return null;
		}

		public Organisation GetLegacyOrganisation (Guid organisationId)
		{
			using (LdapClient client = GetLdapServer ()) {
				LdapEntry entry = SearchForSingle (client, "(tcOrganisationID=" + organisationId + ")");
				if (entry != null)
					return _entityMapping.OrganisationFromLdap (entry);
			}
			return null;
		}

		LdapClient GetLdapServer ()
		{
			LdapClient client = new LdapClient (_ldapConfiguration.LdapHost, _ldapConfiguration.LdapPort);
			client.Bind (_ldapConfiguration.AdminDn, _ldapConfiguration.AdminPassword, BindAuthentication.Simple);
			return client;
		}

		LdapEntry SearchForSingle (LdapClient client, string searchDN)
		{
			var searchResponse = client.Send<SearchResponse> (new SearchRequest (client.CurrentMessageId) {
				Attributes = new string [] { "*", "+" },
				BaseObject = _ldapConfiguration.BaseDn,
				SearchFilter = searchDN,
				Scope = SearchScope.Subtree
			});
			if (searchResponse.HasResults)
				return searchResponse.Results [0];
			return null;
		}
	}
}

