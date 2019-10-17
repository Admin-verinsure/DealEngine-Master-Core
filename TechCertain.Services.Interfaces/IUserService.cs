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
        User GetUser (string username, string password);
        void Create(User user);
        void Update(User user);
    }
}
