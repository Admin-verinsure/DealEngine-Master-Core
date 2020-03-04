using System;
using Novell.Directory.Ldap;
using DealEngine.Infrastructure.BaseLdap.Entities;
using DealEngine.Infrastructure.BaseLdap.Interfaces;
using DealEngine.Infrastructure.BaseLdap.Converters;

namespace DealEngine.Infrastructure.BaseLdap.Services
{
	public class OpenLdapService : OpenLdapServiceBase, IOpenLdapService
	{
		IOpenLdapImportService _ldapImportService;
		//LdapConverter _ldapConverter;

		public OpenLdapService (ILdapRepository ldapRepository, IOpenLdapImportService ldapImportService, ILdapConfigService ldapConfigService)
			: base (ldapRepository, ldapConfigService)
		{
			_ldapImportService = ldapImportService;
		}

		#region ILdapService implementation

		public LdapUser GetUser (string userName)
		{
			return GetUser (userName, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

		public LdapUser GetUser (string userName, string userPassword)
		{
			string userDN = _ldapConfigService.GetUserDN (userName);
			return GetUser (userName, userDN, userPassword);
		}

		public LdapUser GetUser (Guid userID)
		{
			string userSearch = _ldapConfigService.GetUsersByIdSearch (userID);
			LdapEntry entry = _ldapRepository.SearchFor (_ldapConfigService.BaseUserDN, userSearch, new string[] { "+", "*" });
			if (entry != null)
				return LdapConverter.ToUser_Old (entry);
			return null;
		}

		public LdapOrganisation GetOrganisation (Guid organisationID)
		{
			return GetOrganisation (organisationID, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

		public LdapOrganisation GetOrganisation (Guid organisationID, string orgPassword)
		{
			string organisationDN = _ldapConfigService.GetOrganisationDN (organisationID);

			return GetOrganisation (organisationID, organisationDN, orgPassword);
		}

		public LdapRole GetRole (string roleName)
		{
			string roleDN = _ldapConfigService.GetRoleDN (roleName);
			LdapEntry entry = GetLdapEntry (roleDN,  _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);

			return LdapConverter.ToRole(entry);
		}

		public bool Create (LdapUser user)
		{
			string userDn = _ldapConfigService.GetUserDN (user.UserName);
			LdapEntry entry = LdapConverter.ToEntry (user, userDn);

			return _ldapRepository.AddEntry (entry);
		}

		public bool Create (LdapOrganisation organisation)
		{
			string organisationDN = _ldapConfigService.GetOrganisationDN (organisation.ID);
			LdapEntry entry = LdapConverter.ToEntry (organisation, organisationDN);

			return _ldapRepository.AddEntry (entry);
		}

		public bool Create (LdapDepartment department)
		{
			return false;
		}

		public bool Create (LdapBranch branch)
		{
			return false;
		}

		public bool Update (LdapUser user)
		{
			var mods = LdapConverter.ToModificationArray (user);
			string userDN = _ldapConfigService.GetUserDN (user.UserName);
			return _ldapRepository.UpdateEntry(userDN, mods);
		}

		public bool Update (LdapOrganisation organisation)
		{
			return false;
		}

		public bool Delete (LdapUser user)
		{
			return false;
		}

		public bool Delete (LdapOrganisation organisation)
		{
			return false;
		}

		public LdapUser[] GetUsersWithOrganisation (Guid organisationID)
		{
			return null;
		}

		#endregion

		LdapUser GetUser(string userName, string bindDN, string bindPassword)
		{
			string userDN = _ldapConfigService.GetUserDN (userName);
			LdapEntry entry = GetLdapEntry (userDN,  bindDN, bindPassword);

            if (entry == null && _ldapImportService.ImportUser(userName))
                entry = GetLdapEntry(userDN);            

			return LdapConverter.ToUser_Old (entry);
		}

		LdapOrganisation GetOrganisation(Guid organisationID, string bindDN, string bindPassword)
		{
			string organisationDN = _ldapConfigService.GetOrganisationDN (organisationID);
			LdapEntry entry = GetLdapEntry (organisationDN,  bindDN, bindPassword);

			if (entry == null && _ldapImportService.ImportOrganisation(organisationID))
				entry = GetLdapEntry (organisationDN);

			return LdapConverter.ToOrganisation_Old (entry);
		}
	}
}

