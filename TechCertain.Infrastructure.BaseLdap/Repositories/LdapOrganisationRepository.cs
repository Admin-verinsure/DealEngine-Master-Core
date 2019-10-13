using Novell.Directory.Ldap;
using System;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Services;
using TechCertain.Infrastructure.BaseLdap.Converters;
using TechCertain.Infrastructure.BaseLdap.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Repositories
{
	public class LdapOrganisationRepository : IOrganisationRepository
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

		public Organisation Get (Guid organisationID)
		{
			string organisationDn = _ldapConfigService.GetOrganisationDN (organisationID);
			LdapEntry entry = GetLdapEntry(organisationDn);

			if (entry == null && _ldapImportService.ImportOrganisation(organisationID))
				entry = GetLdapEntry(organisationDn);

			return LdapConverter.ToOrganisation(entry);
		}

		public bool Create (Organisation organisation)
		{
			string organisationDn = _ldapConfigService.GetOrganisationDN (organisation.Id);
			LdapEntry entry = LdapConverter.ToEntry (organisation, organisationDn);

			return _ldapRepository.AddEntry(entry);
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

        public bool Delete (Organisation organisation)
		{
			throw new NotImplementedException ();
		}

		#endregion

		LdapEntry GetLdapEntry(string dn)
		{
			return _ldapRepository.GetEntry(dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

        LdapEntry IOrganisationRepository.GetLdapEntry(string dn)
        {
            return _ldapRepository.GetEntry(dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
        }
    }
}

