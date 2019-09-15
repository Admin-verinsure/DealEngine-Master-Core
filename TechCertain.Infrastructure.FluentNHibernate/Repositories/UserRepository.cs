using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Infrastructure.FluentNHibernate.Repositories
{
    public class UserRepository : IUserRepository
    {
        IRepository<User> _userRepository;
        IUnitOfWorkFactory _unitOfWorkFactory;

        public UserRepository(IRepository<User> userRepository, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _userRepository = userRepository;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public bool Create(User user)
        {
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                _userRepository.Add(user);
                //uow.Add<User>(user);
                uow.Commit();
            }
            return true;
        }

        public bool Delete(User user)
        {
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
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

       
        public IEnumerable<User> GetUsers ()
		{
			return _userRepository.FindAll ();
		}

        public bool Update(User user)
        {
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
				try
				{
                    _userRepository.Add(user);
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
