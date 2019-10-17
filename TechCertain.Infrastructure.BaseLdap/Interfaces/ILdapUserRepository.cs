using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.BaseLdap.Interfaces
{
    public interface ILdapUserRepository
    {
        public bool Create(User user);
        public bool Update(User user);
        public bool Delete(User user);
        public User GetUser(Guid userID);
        public User GetUser(string userName);
        public User GetUser(string userName, string userPassword);
        public User GetUserByEmail(string email);
        public IEnumerable<User> GetUsers();
    }
}
