using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.BaseLdap.Converters;
using TechCertain.Infrastructure.BaseLdap.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Repositories
{
	public class LdapOrganisationRepository : ILdapOrganisationRepository
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

		LdapEntry GetLdapEntry(string dn)
		{
			return _ldapRepository.GetEntry(dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

        public Organisation Get(Guid organisationID)
        {
            string organisationDn = _ldapConfigService.GetOrganisationDN(organisationID);
            LdapEntry entry = GetLdapEntry(organisationDn);

            if (entry == null && _ldapImportService.ImportOrganisation(organisationID))
                entry = GetLdapEntry(organisationDn);

            return LdapConverter.ToOrganisation(entry);
        }

        public bool Create(Organisation organisation)
        {
            string organisationDn = _ldapConfigService.GetOrganisationDN(organisation.Id);
            LdapEntry entry = LdapConverter.ToEntry(organisation, organisationDn);

            return _ldapRepository.AddEntry(entry);
        }

        #endregion
    }
}

