using System;
using Novell.Directory.Ldap;
using DealEngine.Infrastructure.BaseLdap.Helpers;
using DealEngine.Infrastructure.Legacy.Interfaces;
using DealEngine.Domain.Exceptions;

namespace DealEngine.Infrastructure.Legacy.Services
{
	public class LegacyLdapExportService : ILegacyLdapExportService
    {
        ILegacyLdapConfigService _ldapConfigService;
        ILegacyLdapRepository _ldapRepository;

		public LegacyLdapExportService(ILegacyLdapConfigService ldapConfigService, ILegacyLdapRepository ldapRepository)
		{
            _ldapConfigService = ldapConfigService;
			_ldapRepository = ldapRepository;
		}

		#region ILdapImportService implementation

		public LdapAttributeSet Export (string username)
		{
			string oldUserDN = _ldapConfigService.GetUserDN (username);
			LdapEntry oldUserEntry = _ldapRepository.GetEntry (oldUserDN);

			if (oldUserEntry == null)
				throw new NoUserFoundException (string.Format("No user found with the username {0}.", username));

			return GetImportedUserAttributes (oldUserEntry);
		}

		public LdapAttributeSet ExportByEmail(string email)
		{
			string searchDN = _ldapConfigService.GetUsersByEmailSearch (email);
			LdapEntry[] entries = _ldapRepository.FindAll (_ldapConfigService.BaseUserDN, searchDN, _ldapConfigService.SearchAttributes);

			// more than one user with a given email address? Throw an import error
			if (entries.Length > 1)
				throw new MultipleUsersFoundException (string.Format ("Found {0} possible users with the email {1}.", entries.Length, email));

			LdapEntry oldUserEntry = (entries.Length > 0) ? entries [0] : null;
			if (oldUserEntry == null)
				throw new NoUserFoundException (string.Format("No user found with the email address {0}.", email));

			//LdapEntry oldUserEntry = _ldapRepository.SearchFor (_ldapConfigService.BaseUserDN, searchDN, _ldapConfigService.SearchAttributes);

			return GetImportedUserAttributes (oldUserEntry);
		}

		public LdapAttributeSet ExportByUserId(Guid userId)
		{
			string searchDN = _ldapConfigService.GetUsersByIdSearch (userId);
			LdapEntry oldUserEntry = _ldapRepository.SearchFor (_ldapConfigService.BaseUserDN, searchDN, _ldapConfigService.SearchAttributes);

			if (oldUserEntry == null)
				throw new UserExportException (string.Format("Unable to export user for id {0}: No user found.", userId));
			
			return GetImportedUserAttributes (oldUserEntry);
		}

		public LdapAttributeSet Export(Guid organisationID)
		{
			string oldOrganisationDN = _ldapConfigService.GetOrganisationSearchDN (organisationID);
			//LdapEntry oldOrganisationEntry = _ldapImportRepository.GetEntry (oldOrganisationDN);
			LdapEntry oldOrganisationEntry = _ldapRepository.SearchFor(_ldapConfigService.BaseOrganisationDN, oldOrganisationDN, _ldapConfigService.OrganisationAttributes);

			if (oldOrganisationEntry != null) {
				LdapAttributeSet oldAttributes = oldOrganisationEntry.getAttributeSet ();

				LdapAttributeSet attributes = new LdapAttributeSet ();
				attributes.AddAttribute ("objectclass", new string[] { "top", "pilotOrganization", "domainRelatedObject" });
				attributes.AddAttribute ("buildingName",		LdapHelpers.GetLdapAttributeValue(oldAttributes, "ou"));
				attributes.AddAttribute ("o",					LdapHelpers.GetLdapAttributeValue(oldAttributes, "tcOrganisationID"));
				attributes.AddAttribute ("businessCategory",	LdapHelpers.GetLdapAttributeValue(oldAttributes, "tcOrganisationType"));
				attributes.AddAttribute ("associatedDomain",	LdapHelpers.GetLdapAttributeValue(oldAttributes, "tcinstanceurl"));
				attributes.AddAttribute ("ou",					"organisation");
                
				return attributes;
			}
			return null;
		}

		#endregion

		LdapAttributeSet GetImportedUserAttributes(LdapEntry userEntry)
		{
			if (userEntry == null)
				throw new ArgumentNullException ("userEntry");

			LdapAttributeSet oldAttributes = userEntry.getAttributeSet ();
			if (oldAttributes == null)
				throw new NullReferenceException ("LdapEntry doesn't contain any attributes.");

			LdapAttributeSet attributes = new LdapAttributeSet ();
			attributes.AddAttribute ("objectclass", new string[] { "top", "person", "organizationalPerson", "inetOrgPerson" });
			attributes.AddAttribute ("uid", 			LdapHelpers.GetLdapAttributeValue(oldAttributes, "uid"));
			attributes.AddAttribute ("cn", 				LdapHelpers.GetLdapAttributeValue(oldAttributes, "cn"));
			attributes.AddAttribute ("sn", 				LdapHelpers.GetLdapAttributeValue(oldAttributes, "sn"));
			attributes.AddAttribute ("givenName", 		LdapHelpers.GetLdapAttributeValue(oldAttributes, "givenName"));
			attributes.AddAttribute ("mail", 			LdapHelpers.GetLdapAttributeValue(oldAttributes, "mail"));
			//attributes.AddAttribute ("telephoneNumber", LDAPHelpers.GetLdapAttributeValue(oldAttributes, "telephoneNumber"));
			attributes.AddAttribute ("employeeNumber", 	LdapHelpers.GetLdapAttributeValue(oldAttributes, "tcuserid"));
			attributes.AddAttribute ("userPassword", 	LdapHelpers.GetLdapAttributeValue(oldAttributes, "userPassword"));
			//attributes.AddAttribute ("description", 	LDAPHelpers.GetLdapAttributeValue(oldAttributes, "description"));
			string organisationId = LdapHelpers.GetLdapAttributeValue(oldAttributes, "tcOrganisationID");
			if (!string.IsNullOrWhiteSpace(organisationId))
				attributes.AddAttribute ("o", 			organisationId);

			return attributes;
		}
	}
}

