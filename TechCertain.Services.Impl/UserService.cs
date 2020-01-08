using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Services.Interfaces;
using NHibernate.Linq;
using System.Threading;

namespace TechCertain.Services.Impl
{
    public class UserService : IUserService
    {
		IMapperSession<User> _userRepository;
		ILdapService _ldapService;
		ILegacyLdapService _legacyLdapService;
        IOrganisationTypeService _organisationTypeService;


        public UserService(IMapperSession<User> userRepository, ILdapService ldapService, ILegacyLdapService legacyLdapService, IOrganisationTypeService organisationTypeService)
        {
			_userRepository = userRepository;
			_ldapService = ldapService;
			_legacyLdapService = legacyLdapService;
            _organisationTypeService = organisationTypeService;
        }

        public async Task<User> GetUser (string username)
		{
            User user = null;
            try
            {
                user = await _userRepository.FindAll().FirstOrDefaultAsync(u => u.UserName == username);
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
                var localUser = _userRepository.FindAll().FirstOrDefault(u => u.UserName == user.UserName);
                if (localUser == null)
                    await Update(user);
				return user;
			}
			user = _legacyLdapService.GetLegacyUser (username);
			// have a legacy ldap user only? Create them in Ldap & NHibernate & return them
			if (user != null) {
                await Create(user);
				return user;
			}
			// no user at all? Throw exception
			throw new Exception ("User with username '" + username + "' does not exist in the system");
		}

		public async Task<User> GetUser (Guid userId)
		{
			User user = await _userRepository.GetByIdAsync(userId);
			// have a repo user? Return them
			if (user != null)
				return user;
			user = _ldapService.GetUser (userId);
			// have a ldap user but no repo? Update NHibernate & return them
			if (user != null) {
                await Update(user);
				return user;
			}
			user = _legacyLdapService.GetLegacyUser (userId);
			// have a legacy ldap user only? Create them in Ldap & NHibernate & return them
			if (user != null) {
                await Create(user);
				return user;
			}
			throw new Exception ("User with Id '" + userId + "' does not exist in the system");
		}

		public async Task<User> GetUserByEmail(string email)
		{
            User user = null;
            try
            {
                user = await _userRepository.FindAll().FirstOrDefaultAsync(u => u.Email == email);

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
				await Update (user);
                return user;
            }
			//user = _legacyLdapService.GetLegacyUserByEmail (email);
			// have a legacy ldap user only? Create them in Ldap & NHibernate & return them
			if (user != null) {
				await Create (user);				
			}
            return user;
        }        

        public async Task<List<User>> GetAllUsers ()
		{
			return await _userRepository.FindAll().ToListAsync();
		}

		public async Task Create (User user)
		{
            await CreateDefaultUserOrganisation (user);
            await _userRepository.AddAsync(user);
            _ldapService.Create (user);
            await Update (user);
		}

		public async Task Update (User user)
		{
		    await _userRepository.UpdateAsync(user);
			_ldapService.Update (user);
		}

        public async Task Delete (User user, User authorizingUser)
		{
            user.Delete(authorizingUser, DateTime.UtcNow);
            await _userRepository.RemoveAsync(user);
        }

		public void SetPasswordPolicyFor (User user, string passwordPolicyName)
		{
			_ldapService.SetPasswordPolicyFor (user, passwordPolicyName);
		}

		public async Task IssueLocalBan (User user, User banningUser)
		{
			user.Lock ();
			await Update (user);
		}

		public async Task RemoveLocalban (User user, User banningUser)
		{
			user.Unlock ();
			await Update (user);
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

		public bool IsUserGlobalBanned (User user)
		{
			throw new NotImplementedException();
		}

        protected async Task CreateDefaultUserOrganisation (User user)
		{
            OrganisationType personalOrganisationType = null;
            personalOrganisationType = await _organisationTypeService.GetOrganisationTypeByName("personal");
            if (personalOrganisationType == null)
            {
                personalOrganisationType = new OrganisationType(user, "personal");
            }
			Organisation defaultOrganisation = Organisation.CreateDefaultOrganisation (user, user, personalOrganisationType);
			user.Organisations.Add (defaultOrganisation);
		}

	}
}