//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TechCertain.Domain.Entities;
//using TechCertain.Domain.Interfaces;
//using TechCertain.Infrastructure.BaseLdap.Repositories;

//namespace TechCertain.Infrastructure.FluentNHibernate.Repositories
//{
//    public class CompositeUserRepository : IUserRepository
//    {
//        readonly List<IUserRepository> _userRepositories;

//        public CompositeUserRepository(params IUserRepository[] userRepositories)
//        {
//            _userRepositories = new List<IUserRepository>();

//            _userRepositories.AddRange(userRepositories);
//        }

//        public bool Create(User user)
//        {
//            bool sucess = false;

//			foreach (var repo in _userRepositories) {

//				bool result = repo.Create (user);
//				sucess = sucess && result;
//				//sucess = sucess && repo.Create (user);
//			}

//            return sucess;
//        }

//        public bool Delete(User user)
//        {
//            bool sucess = false;

//            foreach (var repo in _userRepositories)
//                sucess = sucess && repo.Delete(user);

//            return sucess;
//        }

//        public User GetUser(Guid userID)
//        {
//			User user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).GetUser(userID);
//			if (user == null)
//			{
//				user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(LdapUserRepository)).GetUser(userID);
//				if (user != null)
//					ImportFromLdapToLocal(user);
//			}

//            return user;
//        }

//        public User GetUser(string userName)
//		{
//			User user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).GetUser(userName);
//			if (user == null)
//			{
//				user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(LdapUserRepository)).GetUser(userName);
//				if (user != null)
//					ImportFromLdapToLocal(user);
//			}

//            return user;
//        }

//        public User GetUser(string userName, string userPassword)
//        {
//			User user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).GetUser(userName, userPassword);
//			if (user == null)
//			{
//				user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(LdapUserRepository)).GetUser(userName, userPassword);
//				if (user != null)
//					ImportFromLdapToLocal(user);
//			}

//            return user;
//        }

//		public User GetUserByEmail(string email)
//		{
//			User user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).GetUserByEmail(email);
//			if (user == null)
//			{
//				user = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(LdapUserRepository)).GetUserByEmail(email);
//				if (user != null)
//					ImportFromLdapToLocal(user);
//			}

//			return user;
//		}

//		public IEnumerable<User> GetUsers()
//		{
//			var users = _userRepositories.FirstOrDefault (c => c.GetType () == typeof (UserRepository)).GetUsers ();
//			if (users == null)
//				users = new List<User> ();
//			return users;
//		}

//        public bool Update(User user)
//        {
//            bool sucess = false;

//			foreach (var repo in _userRepositories)
//			{
//				bool result = repo.Update (user);
//				sucess = sucess && result;
//				//sucess = sucess && repo.Update (user);
//			}

//            return sucess;
//        }

//        public bool ImportFromLdapToLocal(User user)
//        {
//            if (user != null)
//            {
//                User localUser = _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).GetUser(user.Id);

//                if (localUser == null)
//                {
//                    _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).Create(user);

//                    return true;
//                }
//            }

//            return false;
//        }

//        public Task<User> GetUserByEmail(string email)
//        {
//            return _userRepositories.FirstOrDefault(c => c.GetType() == typeof(UserRepository)).GetUserByEmail(email);
//        }
//    }
//}
