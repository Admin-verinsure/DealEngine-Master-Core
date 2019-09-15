using System;
using Bismuth.Ldap;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace TechCertain.Infrastructure.Ldap.Mapping
{
	public class LegacyEntityMapping : ILegacyEntityMapping
	{
		public string GetDn (Organisation entity, string baseDn)
		{
			throw new NotImplementedException ();
		}

		public string GetDn (User entity, string baseDn)
		{
			return "uid=" + entity.UserName + ",ou=techcertain users," + baseDn;
		}

		public Organisation OrganisationFromLdap (LdapEntry entry)
		{
			Guid id = Guid.Parse (entry.GetAttributeValue ("tcOrganisationID"));
			string orgName = entry.GetAttributeValue ("ou");

			Organisation organisation = new Organisation (null, id, orgName);
			organisation.Domain = entry.GetAttributeValue ("tcinstanceurl");

			return organisation;
		}

		public User UserFromLdap (LdapEntry entry)
		{
			Guid id = Guid.Parse (entry.GetAttributeValue ("tcUserID"));
			string userName = entry.GetAttributeValue ("uid");

			User user = new User (null, id, userName);
			user.FirstName = entry.GetAttributeValue ("givenName");
			user.LastName = entry.GetAttributeValue ("sn");
			user.FullName = entry.GetAttributeValue ("cn");
			user.Email = entry.GetAttributeValue ("mail");
			user.Phone = entry.GetAttributeValue ("telephoneNumber");
			user.Description = entry.GetAttributeValue ("description");

			return user;
		}
	}
}

