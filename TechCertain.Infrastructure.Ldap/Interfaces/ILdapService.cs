using Bismuth.Ldap;
using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.Ldap.Interfaces
{
	public interface ILdapService
	{
		User GetUser (string username);
		User GetUser (Guid userId);
		User GetUserByEmail (string email);

		void Validate (string username, string password, out int resultCode, out string resultMessage);

		Organisation GetOrganisation (string organisationName);
		Organisation GetOrganisation (Guid organisationId);

		//PasswordPolicy GetPasswordPolicy (string passwordPolicyName);
		void SetPasswordPolicyFor (User user, string passwordPolicyName);
		void Create (User user);
		void Create (Organisation organisation);
		//void Create (PasswordPolicy passwordPolicy)
		void Update (User user);
		void Update (Organisation organisation);
		//void Update (PasswordPolicy passwordPolicy)
		void GlobalBan (User user);
		void RemoveGlobalBan (User user);
        bool ChangePassword(string username, string oldPassword, string newPassword);
        //LdapClient GetLdapConnection();
        //LdapClient GetLdapConnection(string ldapUser, string password);
        string GetUsernameDN(string username);
    }
}

