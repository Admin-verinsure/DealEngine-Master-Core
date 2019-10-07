using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Converters;
using TechCertain.Infrastructure.BaseLdap.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Repositories
{
    public class LdapUserRepository : IUserRepository
    {
        ILdapConfigService _ldapConfigService;
        ILdapRepository _ldapRepository;
		IOrganisationRepository _organisationRepository;
        IOpenLdapImportService _ldapImportService;

        public LdapUserRepository(ILdapConfigService ldapConfigService,
	        ILdapRepository ldapRepository,
			IOrganisationRepository organisationRepository,
	        IOpenLdapImportService ldapImportService)
        {
            _ldapConfigService = ldapConfigService;
            _ldapRepository = ldapRepository;
			_organisationRepository = organisationRepository;
            _ldapImportService = ldapImportService;
        }

        public bool Create(User user)
		{
			if (user == null)
				throw new ArgumentNullException ("user");
			
			bool result = false;
			try {
	            string userDn = _ldapConfigService.GetUserDN(user.UserName);
	            LdapEntry entry = LdapConverter.ToEntry(user, userDn);
				result = _ldapRepository.AddEntry(entry);
			}
			catch (Exception ex) {
				throw new Exception ("Unable to create User in ldap.", ex);
			}
			return result;
		}

		public bool Update(User user)
		{
			if (user == null)
				throw new ArgumentNullException ("user");

			bool update;
			try {
				//TODO Update all child elements
				var mods = LdapConverter.ToModificationArray(user);
				string userDN = _ldapConfigService.GetUserDN(user.UserName);
				update = _ldapRepository.UpdateEntry(userDN, mods);
			}
			catch (Exception ex) {
				throw new Exception ("Unable to update User in Ldap.", ex);
			}
			return update;
			//return _ldapRepository.UpdateEntry(userDN, mods);
		}

        public bool Delete(User user)
		{
			if (user == null)
				throw new ArgumentNullException ("user");

			bool result = false;
			try {
	            var mods = LdapConverter.ToModificationArray(user);
	            string userDN = _ldapConfigService.GetUserDN(user.UserName);
				result = _ldapRepository.UpdateEntry(userDN, mods);
			}
			catch (Exception ex) {
				throw new Exception ("Unable to mark User as deleted in ldap.", ex);
			}
			return result;
        }

        public User GetUser(Guid userID)
		{
			if (userID == Guid.Empty)
				throw new ArgumentException ("Value cannot be an empty Guid.", "userID");
			
			User user;
			try {
	            string userSearch = _ldapConfigService.GetUsersByIdSearch(userID);
				LdapEntry entry = _ldapRepository.SearchFor(_ldapConfigService.BaseUserDN, userSearch, _ldapConfigService.SearchAttributes);

				if (entry == null && _ldapImportService.ImportUserById(userID))
					entry = _ldapRepository.SearchFor(_ldapConfigService.BaseUserDN, userSearch, _ldapConfigService.SearchAttributes);

				user = LdapConverter.ToUser (entry);
			}
			catch (Exception ex) {
				throw new Exception ("Unable to retrieve User by id from ldap.", ex);
			}
			if (user != null)
				return GetOrganisations(user);
            return null;
        }

        public User GetUser(string userName)
        {
            return GetUser(userName, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
        }

        public User GetUser(string userName, string userPassword)
        {
            string userDN = _ldapConfigService.GetUserDN(userName);
            return GetUser(userName, userDN, userPassword);
		}

		public User GetUserByEmail(string email)
		{
			if (string.IsNullOrWhiteSpace (email))
				throw new ArgumentException ("Value cannot be null, empty or whitespace.", "email");
			
			try {
				string userSearch = _ldapConfigService.GetUsersByEmailSearch(email);
				LdapEntry entry = _ldapRepository.SearchFor(_ldapConfigService.BaseUserDN, userSearch, _ldapConfigService.SearchAttributes);

				if (entry == null && _ldapImportService.ImportUserByEmail(email))
					entry = _ldapRepository.SearchFor(_ldapConfigService.BaseUserDN, userSearch, _ldapConfigService.SearchAttributes);

				User user = LdapConverter.ToUser (entry);
				user = GetOrganisations(user);
				return user;
			}
//			catch (UserImportException ex) {
//				throw;
//			}
			catch (Exception ex) {
				throw new Exception ("Unable to retrieve User by email from ldap.", ex);
			}
   		}

		public IEnumerable<User> GetUsers ()
		{
			throw new NotImplementedException ();
		}


        User GetUser(string userName, string bindDN, string bindPassword)
		{
			if (string.IsNullOrWhiteSpace (userName))
				throw new ArgumentException ("Value cannot be null, empty or whitespace.", "userName");
			if (string.IsNullOrWhiteSpace (bindDN))
				throw new ArgumentException ("Value cannot be null, empty or whitespace.", "bindDN");
			if (string.IsNullOrWhiteSpace (bindPassword))
				throw new ArgumentException ("Value cannot be null, empty or whitespace.", "bindPassword");

			User user;
			try {
	            string userDN = _ldapConfigService.GetUserDN(userName);
	            LdapEntry entry = GetLdapEntry(userDN, bindDN, bindPassword);

	            if (entry == null && _ldapImportService.ImportUser(userName))
	                entry = GetLdapEntry(userDN);

				if (entry == null)
					return null;
				user = GetOrganisations(LdapConverter.ToUser(entry));
			}
			catch (Exception ex) {
				throw new Exception ("Unable to retrieve User by id from ldap.", ex);
			}
			return user;
        }

        LdapEntry GetLdapEntry(string dn)
        {
            return GetLdapEntry(dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
        }

        LdapEntry GetLdapEntry(string dn, string bindDN, string password)
        {
			LdapEntry entry;
			try {
				entry = _ldapRepository.GetEntry(dn, bindDN, password);
			}
			catch (Exception ex) {
				throw new Exception ("Unable to load entry from ldap.", ex);
			}
			return entry;
        }

		User GetOrganisations(User user)
		{
			for (int i = 0; i < (user.Organisations as List<Organisation>).Count; i++) {
				(user.Organisations as List<Organisation>) [i] = _organisationRepository.Get ((user.Organisations as List<Organisation>) [i].Id);
			}
			return user;
		}

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
