using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Infrastructure.FluentNHibernate.Repositories
{
    public class UserRepository : IUserRepository
    {
        IMapperSession<User> _userRepository;
        IUnitOfWork _unitOfWork;

        public UserRepository(IMapperSession<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Create(User user)
        {
            using (var uow = _unitOfWork.BeginUnitOfWork())
            {
                _userRepository.Add(user);
                //uow.Add<User>(user);
                uow.Commit();
            }
            return true;
        }

        public bool Delete(User user)
        {
            using (var uow = _unitOfWork.BeginUnitOfWork())
            {
                _userRepository.Remove(user);
                //uow.Remove(user);
                uow.Commit();
            }
            return true;
        }

        public User GetUser(Guid userID)
        {
            return _userRepository.GetById(userID);
        }

        public User GetUser(string userName)
		{
			return _userRepository.FindAll ().FirstOrDefault (u => u.UserName == userName);
        }

        public User GetUser(string userName, string userPassword)
        {
			User user = GetUser (userName);

			return (user != null && user.Password == userPassword) ? user : null;
        }

		public User GetUserByEmail(string email)
		{
			return _userRepository.FindAll ().FirstOrDefault (u => u.Email == email);
		}

        public Task<User> GetUserByEmailAsync(string email)
        {
            var user = _userRepository.FindAll().FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        }

        public IEnumerable<User> GetUsers ()
		{
			return _userRepository.FindAll ();
		}

        public bool Update(User user)
        {
            using (var uow = _unitOfWork.BeginUnitOfWork())
            {
				try
				{
                    _userRepository.Update(user);
	                //uow.Add(user);
	                uow.Commit();
				}
				catch (Exception e)
				{
					Console.WriteLine (e.ToString ());
				}
            }

            return true;
        }
    }
}
