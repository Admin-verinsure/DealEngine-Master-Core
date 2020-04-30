using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
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
        Task<List<User>> GetBrokerUsers();
    }
}
