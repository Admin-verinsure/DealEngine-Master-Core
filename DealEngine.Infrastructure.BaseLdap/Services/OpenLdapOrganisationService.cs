using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.BaseLdap.Helpers;
using DealEngine.Infrastructure.BaseLdap.Interfaces;

namespace DealEngine.Infrastructure.BaseLdap.Services
{
	public class OpenLdapOrganisationService : OpenLdapServiceBase, IOpenLdapOrganisationService
	{
		public OpenLdapOrganisationService (ILdapRepository ldapRepository, ILdapConfigService ldapConfigService)
			: base (ldapRepository, ldapConfigService)
		{
		}

		#region ILdapOrganisationService implementation

		public Organisation GetOrganisation (Guid organisationID)
		{
			return null;
		}

		public bool Create (Organisation organisation)
		{
			LdapAttributeSet attributes = new LdapAttributeSet ();
			attributes.AddAttribute ("objectclass", new string[] { "top", "uidObject", "pilotOrganization" });
			attributes.AddAttribute ("o", organisation.Name);
			attributes.AddAttribute ("ou", organisation.Id.ToString());
			attributes.AddAttribute ("uid",	organisation.Id.ToString());
			attributes.AddAttribute ("businessCategory", organisation.OrganisationType.Name);

			string organisationDN = GetOrganisationDN (organisation.Id);

			LdapEntry newOrganisationEntry = new LdapEntry (organisationDN, attributes);

			return _ldapRepository.AddEntry (newOrganisationEntry);
		}

		public bool Update (Organisation organisation)
		{
			List<LdapModification> mods = new List<LdapModification> ()
			{
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "o", organisation.Name),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "businessCategory", organisation.OrganisationType.Name),
			};

			string userDN = GetOrganisationDN (organisation.Id);

			return _ldapRepository.UpdateEntry(userDN, mods);
		}

		public bool Delete (Organisation organisation)
		{
			return false;
		}

		public Organisation OrganisationFrom (LdapEntry entry)
		{
			if (entry == null)
				return null;
			
			LdapAttributeSet attributes = entry.getAttributeSet ();

			Guid organisationID = new Guid (attributes.getAttribute ("ou").StringValue);
			string organisationName = attributes.getAttribute ("o").StringValue;
			string organisationType = attributes.getAttribute ("businessCategory").StringValue;

			Organisation organisation = new Organisation (null, organisationID, organisationName, new OrganisationType (null, organisationType));
			
            //organisation.ChangeId (organisationID);

			return organisation;
		}

		public string GetOrganisationDN(Guid organisationID)
		{
			return _ldapConfigService.GetOrganisationDN (organisationID);
		}

		#endregion
	}
}

