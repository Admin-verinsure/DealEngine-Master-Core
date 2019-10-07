using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class UserService : IUserService
    {
        IUnitOfWork _unitOfWork;
		IUserRepository _userRepository;
		ILdapService _ldapService;
		ILegacyLdapService _legacyLdapService;
        IOrganisationTypeService _organisationTypeService;


        public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository, ILdapService ldapService, ILegacyLdapService legacyLdapService, IOrganisationTypeService organisationTypeService)
        {
            _unitOfWork = unitOfWork;
			_userRepository = userRepository;
			_ldapService = ldapService;
			_legacyLdapService = legacyLdapService;
            _organisationTypeService = organisationTypeService;
        }

        //public async Task<User> GetCurrentUserAsync()
        //{
        //    //return await _userRepository.GetByIdAsync(_currentUserGuid);
        //}

        public User GetUser (string username)
		{
            User user = null;
            try
            {
                user = _userRepository.GetUser(username);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
			// have a repo user? Return them
			if (user != null)
				return user;
			user = _ldapService.GetUser (username);
			// have a ldap user but no repo? Update NHibernate & return them
			if (user != null) {
				// postgres is case sensitive, while Ldap is case insensitive. So a valid lowercase username will fail the 1st condition, but get here and reimport the user, which isn't what we want to have happen
				// in this case, we'll get the ldap user, and only if the uppercase'd ldap username doesn't exist in postgres, we'll add the user.
				var localUser = _userRepository.GetUser (user.UserName);
				if (localUser == null)
					Update (user);
				return user;
			}
			user = _legacyLdapService.GetLegacyUser (username);
			// have a legacy ldap user only? Create them in Ldap & NHibernate & return them
			if (user != null) {
				Create (user);
				return user;
			}
			// no user at all? Throw exception
			throw new Exception ("User with username '" + username + "' does not exist in the system");
		}

		public User GetUser (Guid userId)
		{
			User user = _userRepository.GetUser (userId);
			// have a repo user? Return them
			if (user != null)
				return user;
			user = _ldapService.GetUser (userId);
			// have a ldap user but no repo? Update NHibernate & return them
			if (user != null) {
				Update (user);
				return user;
			}
			user = _legacyLdapService.GetLegacyUser (userId);
			// have a legacy ldap user only? Create them in Ldap & NHibernate & return them
			if (user != null) {
				Create (user);
				return user;
			}
			throw new Exception ("User with Id '" + userId + "' does not exist in the system");
		}

		public async Task<User> GetUserByEmailAsync (string email)
		{
            User user = null;
            try
            {
                user = await _userRepository.GetUserByEmailAsync(email);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
			
			// have a repo user? Return them
			if (user != null)
				return user;
			user = _ldapService.GetUserByEmail (email);
			// have a ldap user but no repo? Update NHibernate & return them
			if (user != null) {
				Update (user);
				return user;
			}
			//user = _legacyLdapService.GetLegacyUserByEmail (email);
			// have a legacy ldap user only? Create them in Ldap & NHibernate & return them
			if (user != null) {
				Create (user);				
			}
            return user;            
		}

        public User GetUserByEmail(string email)
        {
            User user = null;
            try
            {
                user = _userRepository.GetUserByEmail(email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // have a repo user? Return them
            if (user != null)
                return user;
            user = _ldapService.GetUserByEmail(email);
            // have a ldap user but no repo? Update NHibernate & return them
            if (user != null)
            {
                Update(user);
                return user;
            }
            //user = _legacyLdapService.GetLegacyUserByEmail (email);
            // have a legacy ldap user only? Create them in Ldap & NHibernate & return them
            if (user != null)
            {
                Create(user);
                return user;
            }
            throw new Exception("User with email '" + email + "' does not exist in the system");
        }

        public IEnumerable<User> GetAllUsers ()
		{
			return _userRepository.GetUsers ();
		}

		public void Create (User user)
		{
            CreateDefaultUserOrganisation (user);
			_ldapService.Create (user);
			Update (user);
		}

		public void Update (User user)
		{
			//using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork ()) {
				_userRepository.Update (user);
			//	uow.Commit ();
			//}
			_ldapService.Update (user);
		}

		public void Delete (User user, User authorizingUser)
		{
            user.Delete(authorizingUser, DateTime.UtcNow);
            Update(user);
        }

		public void SetPasswordPolicyFor (User user, string passwordPolicyName)
		{
			_ldapService.SetPasswordPolicyFor (user, passwordPolicyName);
		}

		public void IssueLocalBan (User user, User banningUser)
		{
			user.Lock ();
			Update (user);
		}

		public void RemoveLocalban (User user, User banningUser)
		{
			user.Unlock ();
			Update (user);
		}

		public bool IsUserLocalBanned (User user)
		{
			return user.Locked;
		}

		public void IssueGlobalBan (User user, User banningUser)
		{
			_ldapService.GlobalBan (user);
		}

		public void RemoveGlobalBan (User user, User banningUser)
		{
			_ldapService.RemoveGlobalBan (user);
		}

		public User GetUser (string username, string password)
		{
			return GetUser (username);
		}

		public bool IsUserGlobalBanned (User user)
		{
			throw new NotImplementedException();
		}

        protected void CreateDefaultUserOrganisation (User user)
		{
            OrganisationType personalOrganisationType = null;
            personalOrganisationType = _organisationTypeService.GetOrganisationTypeByName("personal");
            if (personalOrganisationType == null)
            {
                personalOrganisationType = new OrganisationType(user, "personal");
            }
			Organisation defaultOrganisation = Organisation.CreateDefaultOrganisation (user, user, personalOrganisationType);
			user.Organisations.Add (defaultOrganisation);
		}

        User IUserService.GetUserByEmailAsync(string email)
        {
            return GetUserByEmailAsync(email).Result;
        }
    }
}