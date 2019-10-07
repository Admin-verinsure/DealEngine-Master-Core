using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IUserService
    {
		User GetUser (string username);
		User GetUser (Guid userId);
		User GetUserByEmail (string email);
        IEnumerable<User> GetAllUsers ();
		void Create (User user);
		void Update (User user);
		void Delete (User user, User authorizingUser);
		void SetPasswordPolicyFor (User user, string passwordPolicyName);
		void IssueLocalBan (User user, User banningUser);
		void RemoveLocalban (User user, User banningUser);
		bool IsUserLocalBanned (User user);
		void IssueGlobalBan (User user, User banningUser);
		void RemoveGlobalBan (User user, User banningUser);
		bool IsUserGlobalBanned (User user);

        User GetUser (string username, string password);
        User GetUserByEmailAsync(string email);
    }
}
