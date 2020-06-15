using System;
using System.Collections.Generic;
using Bismuth.Ldap;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.Ldap.Interfaces;

namespace DealEngine.Infrastructure.Ldap.Mapping
{
	public class OrganisationMapping : BaseEntityMapping, ILdapEntityMapping<Organisation>
	{
		public Organisation FromLdap (LdapEntry entry)
		{
			Guid id = Guid.Parse (entry.GetAttributeValue ("o"));					// Need to swap this to use 'uniqueIdentifier'
			string orgName = entry.GetAttributeValue ("buildingName");				// Need to swap this to use 'o'

			Organisation organisation = new Organisation (null, id, orgName);
			organisation.Domain = entry.GetAttributeValue ("associatedDomain");
			organisation.Phone = entry.GetAttributeValue ("telephoneNumber");
			organisation.Description = entry.GetAttributeValue ("description");
			string organisationType = entry.GetAttributeValue ("businessCategory");

			return organisation;
		}

		public string GetDn (Organisation entity, string baseDn)
		{
			return "o=" + entity.Id + ",ou=organisations," + baseDn;
		}

		public LdapEntry ToLdap (Organisation entity, string baseDn)
		{
			LdapEntry entry = new LdapEntry (GetDn (entity, baseDn))
				.AddAttribute ("o", entity.Id.ToString ())
				.AddAttribute ("objectClass", "top", "pilotOrganization", "domainRelatedObject");
				//.AddAttribute ("uniqueIdentifier", entity.Id.ToString());       // Removed in RFC4524 schema

			AddNonNullAttribute (entry, "ou", "organisation");
			AddNonNullAttribute (entry, "buildingName", entity.Name);
			AddNonNullAttribute (entry, "description", entity.Description);
			AddNonNullAttribute (entry, "telephoneNumber", entity.Phone);
			AddDefaultAttribute (entry, "associatedDomain", entity.Domain, "#");
			if (entity.OrganisationType != null)
				AddNonNullAttribute (entry, "businessCategory", entity.OrganisationType.Name);

			return entry;
		}

		public List<ModifyAttribute> ToModify (Organisation entity)
		{
			var orgMods = new List<ModifyAttribute> ();
			AddNonNullAttribute (orgMods, "telephoneNumber", ModificationType.Replace, entity.Phone);
			AddNonNullAttribute (orgMods, "description", ModificationType.Replace, entity.Description);
			AddNonNullAttribute (orgMods, "associatedDomain", ModificationType.Replace, entity.Domain);
			if (entity.OrganisationType != null)
				AddNonNullAttribute (orgMods, "businessCategory", ModificationType.Replace, entity.OrganisationType.Name);
			return orgMods;
		}
	}
}

