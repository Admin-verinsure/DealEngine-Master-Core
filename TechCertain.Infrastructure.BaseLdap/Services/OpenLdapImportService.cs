using Novell.Directory.Ldap;
using System;
using TechCertain.Domain.Exceptions;
using TechCertain.Infrastructure.BaseLdap.Helpers;
using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Infrastructure.Legacy.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Services
{
    public class OpenLdapImportService : IOpenLdapImportService
    {
        ILegacyLdapExportService _exportService;
        ILdapRepository _importRepository;
        ILdapConfigService _ldapConfigService;

        public OpenLdapImportService(ILegacyLdapExportService exportService, ILdapRepository importRepository, 
			ILdapConfigService ldapConfigService)
        {
            _exportService = exportService;
            _importRepository = importRepository;
            _ldapConfigService = ldapConfigService;
        }

		public bool ImportUser(string username)
        {
			try {
	            LdapAttributeSet attributes = _exportService.Export(username);

	            LdapEntry entry = new LdapEntry(_ldapConfigService.GetUserDN(username), attributes);
	            if (entry == null)
	                return false;

				CreateDefaultOrganisation (entry, Guid.NewGuid());
	                        
				return _importRepository.AddEntry(entry);
			}
			catch (UserExportException ex) {
				throw new UserImportException (string.Format ("Unable to import user with username {0} from legacy system.", username), ex);
			}
			catch (Exception ex) {
				throw new Exception (string.Format ("Import failed for user {0}.", username), ex);
			}
		}

		public bool ImportUserByEmail(string email)
		{
			try {
				LdapAttributeSet attributes = _exportService.ExportByEmail (email);
				if (attributes == null)
					return false;

				string username = LdapHelpers.GetLdapAttributeValue (attributes, "uid");

				LdapEntry entry = new LdapEntry(_ldapConfigService.GetUserDN(username), attributes);
				if (entry == null)
					return false;

				CreateDefaultOrganisation (entry, Guid.NewGuid());

				return _importRepository.AddEntry(entry);
			}
			catch (UserExportException ex) {
				throw new UserImportException (string.Format ("Unable to import user with email {0} from legacy system.", email), ex);
			}
			catch (Exception ex) {
				throw new Exception (string.Format ("Import failed for user with email {0}.", email), ex);
			}
		}

		public bool ImportUserById(Guid userId)
		{
			LdapAttributeSet attributes = _exportService.ExportByUserId (userId);
			if (attributes == null)
				return false;

			string username = LdapHelpers.GetLdapAttributeValue (attributes, "uid");

			LdapEntry entry = new LdapEntry(_ldapConfigService.GetUserDN(username), attributes);
			if (entry == null)
				return false;

			CreateDefaultOrganisation (entry, Guid.NewGuid());

			return _importRepository.AddEntry(entry);
		}

		public bool ImportOrganisation(Guid organisationId)
		{
			LdapAttributeSet attributes = _exportService.Export(organisationId);

			LdapEntry entry = new LdapEntry(_ldapConfigService.GetOrganisationDN(organisationId), attributes);

			if (entry == null)
				return false;

			return _importRepository.AddEntry(entry);
		}


		protected bool CreateDefaultOrganisation(LdapEntry userEntry, Guid id)
		{
			if (userEntry == null)
				throw new ArgumentNullException ("userEntry");
			if (id == Guid.Empty)
				throw new ArgumentException ("Value cannot be an empty Guid.", "id");
			
			// link user to their default organisation
			LdapAttribute attr = userEntry.getAttribute ("o");
			if (attr == null)
				userEntry.getAttributeSet ().AddAttribute ("o", id.ToString ());
			else
				userEntry.getAttribute ("o").addValue (id.ToString ());

			// check to ensure that the user has the cn attribute
			if (userEntry.getAttribute ("cn") == null)
				throw new InvalidOperationException ("LdapEntry representing a User lacks a 'cn' attribute.");

			// create default organisation attributes
			LdapAttributeSet attributes = new LdapAttributeSet ();
			attributes.AddAttribute ("objectclass", new string[] { "top", "pilotOrganization" });
			attributes.AddAttribute ("buildingName",		"Default user organisation for " + userEntry.getAttribute ("cn").StringValue);
			attributes.AddAttribute ("o",					id.ToString());
			attributes.AddAttribute ("businessCategory",	"personal");
			attributes.AddAttribute ("ou",					"organisation");

			LdapEntry entry = new LdapEntry(_ldapConfigService.GetOrganisationDN(id), attributes);
			return _importRepository.AddEntry(entry);
		}
    }
}
