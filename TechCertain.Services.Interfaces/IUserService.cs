using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser (string username);
        Task<User> GetUserById (Guid userId);
        Task<User> GetUserByEmail (string email);
        Task<List<User>> GetAllUsers ();
        Task ApplicationCreateUser(User user);
        Task Create(User user);
        Task Update(User user);
        Task<List<User>> GetLockedUsers();
        Task<User> GetUserByOrganisation(Organisation org);
    }
}
