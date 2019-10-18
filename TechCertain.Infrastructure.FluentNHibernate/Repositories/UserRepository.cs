using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;

namespace TechCertain.Infrastructure.FluentNHibernate.Repositories
{
    public class UserRepository : IUserService
    {
        IMapperSession<User> _userRepository;
        public UserRepository(IMapperSession<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async void Create(User user)
        {
            _userRepository.AddAsync(user);            
        }

        public async void Delete(User user)
        {
            _userRepository.RemoveAsync(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.FindAll();
        }

        public User GetUser(Guid userID)
        {
            return _userRepository.GetByIdAsync(userID).Result;
        }

        public User GetUser(string userName)
		{
			return _userRepository.FindAll().FirstOrDefault(u => u.UserName == userName);
        }

        public User GetUser(string userName, string userPassword)
        {
			User user = GetUser(userName);

			return (user != null && user.Password == userPassword) ? user : null;
        }

		public User GetUserByEmail(string email)
		{
			return _userRepository.FindAll().FirstOrDefault(u => u.Email == email);
		}

        public IEnumerable<User> GetUsers ()
		{
			return _userRepository.FindAll();
		}

        public async void Update(User user)
        {
            _userRepository.UpdateAsync(user);
        }
    }
}
