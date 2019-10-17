using Novell.Directory.Ldap;
using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.BaseLdap.Converters;
using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Repositories
{
	public class LdapOrganisationRepository : IOrganisationService
    {
		ILdapConfigService _ldapConfigService;
		ILdapRepository _ldapRepository;
		IOpenLdapImportService _ldapImportService;

		public LdapOrganisationRepository (ILdapConfigService ldapConfigService,
			ILdapRepository ldapRepository,
			IOpenLdapImportService ldapImportService)
		{
			_ldapConfigService = ldapConfigService;
			_ldapRepository = ldapRepository;
			_ldapImportService = ldapImportService;
		}

		#region IOrganisationRepository implementation

		public Organisation GetOrganisation(Guid organisationID)
		{
			string organisationDn = _ldapConfigService.GetOrganisationDN (organisationID);
			LdapEntry entry = GetLdapEntry(organisationDn);

			if (entry == null && _ldapImportService.ImportOrganisation(organisationID))
				entry = GetLdapEntry(organisationDn);

			return LdapConverter.ToOrganisation(entry);
		}

		public Organisation CreateNewOrganisation(Organisation organisation)
		{
			string organisationDn = _ldapConfigService.GetOrganisationDN (organisation.Id);
			LdapEntry entry = LdapConverter.ToEntry (organisation, organisationDn);

			return organisation;
			//throw new NotImplementedException ();
		}

		public bool Update (Organisation organisation)
		{
//			var mods = LdapConverter.ToModificationArray(organisation);
//			string organisationDn = _ldapConfigService.GetOrganisationDN (organisation.Id);
//
//			return _ldapRepository.UpdateEntry(organisationDn, mods);
			throw new NotImplementedException ();
		}

        public Organisation GetOrganisation(string organisationName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteOrganisation(User deletedBy, Organisation organisation)
		{
			throw new NotImplementedException ();
		}

		#endregion

		LdapEntry GetLdapEntry(string dn)
		{
			return _ldapRepository.GetEntry(dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

        public Organisation CreateNewOrganisation(string p1, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Organisation> GetAllOrganisations()
        {
            throw new NotImplementedException();
        }

        public Organisation GetOrganisationByName(string organisationName)
        {
            throw new NotImplementedException();
        }

        public Organisation GetOrganisationByEmail(string organisationEmail)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrganisation(Organisation organisation)
        {
            throw new NotImplementedException();
        }
    }
}

