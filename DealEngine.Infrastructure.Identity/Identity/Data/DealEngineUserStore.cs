//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using TechCertain.Domain.Entities;
//using TechCertain.Domain.Interfaces;
//using Claim = System.Security.Claims.Claim;

//namespace DealEngine.Infrastructure.Identity.Data
//{
//    public class DealEngineUserStore : IUserStore<DealEngineUser>, IUserEmailStore<DealEngineUser>, IUserPasswordStore<DealEngineUser>, IUserClaimStore<DealEngineUser>
//    {
//        IUnitOfWork _unitOfWork;
//		IMapperSession<User> _userRepository;
//        DealEngineDBContext _dealEngine;

//		public DealEngineUserStore(IUnitOfWork unitOfWork, IMapperSession<User> userRepository, DealEngineDBContext dealEngine)
//		{
//			if (unitOfWork == null) {
//				throw new ArgumentNullException (nameof (unitOfWork));
//			}
//			if (userRepository == null) {
//				throw new ArgumentNullException (nameof (userRepository));
//			}

//			_unitOfWork = unitOfWork;
//			_userRepository = userRepository;
//            _dealEngine = dealEngine;

//        }

//        public IQueryable<User> Users {
//			get {
//				return _userRepository.FindAll ();
//			}
//		}

//        public Task AddClaimsAsync(DealEngineUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task CreateAsync (User user)
//		{          
//            return UpdateAsync (user);
//		}

//        public Task<IdentityResult> CreateAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            _dealEngine.Add(user);
//            _dealEngine.SaveChanges();

//            return Task.FromResult(IdentityResult.Success);
//        }

//        public Task DeleteAsync (User user)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//            _userRepository.Remove(user);            
//			return Task.FromResult<object> (null);
//		}

//        public Task<IdentityResult> DeleteAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose ()
//		{
			
//		}

//		public Task<User> FindByEmailAsync (string email)
//		{
//			if (string.IsNullOrWhiteSpace (email)) {
//				throw new ArgumentNullException (nameof (email));
//			}
//			var user = _userRepository.FindAll ().FirstOrDefault (u => u.Email == email);

//			return Task.FromResult(user);
//		}

//        public Task<DealEngineUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<User> FindByIdAsync (Guid userId)
//		{
//			if (userId == Guid.Empty) {
//				throw new ArgumentNullException (nameof (userId));
//			}

//			return Task.FromResult (_userRepository.GetById (userId));
//		}

//        public Task<DealEngineUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<User> FindByNameAsync (string userName)
//		{
//			if (string.IsNullOrWhiteSpace(userName)) {
//				throw new ArgumentNullException (nameof (userName));
//			}
//			var user = _userRepository.FindAll ().FirstOrDefault (u => u.UserName == userName);

//			return Task.FromResult (user);
//		}

//        public Task<DealEngineUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
//        {
//            if (string.IsNullOrWhiteSpace(normalizedUserName))
//            {
//                throw new ArgumentNullException(nameof(normalizedUserName));
//            }

//            var deUser = _dealEngine.Users.FirstOrDefault(x => x.NormalizedUserName == normalizedUserName);
//            return Task.FromResult(deUser);
//        }

//        public Task<IList<Claim>> GetClaimsAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();

//        }

//        public Task<string> GetEmailAsync (User user)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//			if (!string.IsNullOrWhiteSpace (user.Email)) {
//				return Task.FromResult (user.Email);
//			}
//			return Task.FromResult (FindByIdAsync(user.Id).Result.Email);
//		}

//        public Task<string> GetEmailAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(user.Email);
//        }

//        public Task<bool> GetEmailConfirmedAsync (User user)
//		{
//			throw new NotImplementedException ();
//		}

//        public Task<bool> GetEmailConfirmedAsync(DealEngineUser deUser, CancellationToken cancellationToken)
//        {
//            var user = _userRepository.FindAll().FirstOrDefault(u => u.UserName.ToUpper() == deUser.NormalizedUserName);
//            return Task.FromResult(user.Email == deUser.Email);
//        }

//        public Task<string> GetNormalizedEmailAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<string> GetNormalizedUserNameAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<string> GetPasswordHashAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IList<string>> GetRolesAsync (User user)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//			IList<string> roles = user.Groups.Select (g => g.Name).ToList();
//			return Task.FromResult (roles);
//		}

//        public Task<string> GetUserIdAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(user.Id.ToString());            
//        }

//        public Task<string> GetUserNameAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(user.UserName);           
//        }

//        public Task<IList<DealEngineUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> HasPasswordAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> IsInRoleAsync (User user, string roleName)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//			if (string.IsNullOrWhiteSpace (roleName)) {
//				throw new ArgumentNullException (nameof (roleName));
//			}
//			return Task.FromResult (user.Groups.FirstOrDefault (g => g.Name == roleName) != null);
//		}

//        public Task RemoveClaimsAsync(DealEngineUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveFromRoleAsync (User user, string roleName)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//			if (string.IsNullOrWhiteSpace (roleName)) {
//				throw new ArgumentNullException (nameof (roleName));
//			}

//			user.Groups.Remove (user.Groups.FirstOrDefault (g => g.Name == roleName));
//			return UpdateAsync (user);
//		}

//        public Task ReplaceClaimAsync(DealEngineUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SetEmailAsync (User user, string email)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//			user.Email = email;
//			return UpdateAsync (user);
//		}

//        public Task SetEmailAsync(DealEngineUser user, string email, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SetEmailConfirmedAsync (User user, bool confirmed)
//		{
//			throw new NotImplementedException ();
//		}

//        public Task SetEmailConfirmedAsync(DealEngineUser user, bool confirmed, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SetNormalizedEmailAsync(DealEngineUser user, string normalizedEmail, CancellationToken cancellationToken)
//        {
//            user.NormalizedEmail = normalizedEmail;
//            return Task.CompletedTask;
//        }

//        public Task SetNormalizedUserNameAsync(DealEngineUser user, string normalizedName, CancellationToken cancellationToken)
//        {
//            user.NormalizedUserName = normalizedName;
//            return Task.CompletedTask;            
//        }

//        public Task SetPasswordHashAsync(DealEngineUser user, string passwordHash, CancellationToken cancellationToken)
//        {
//            user.PasswordHash = passwordHash;
//            //_dealEngine.Update(user);

//            return Task.CompletedTask;
//        }

//        public Task SetUserNameAsync(DealEngineUser user, string userName, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateAsync (User user)
//		{
//			if (user == null) {
//				throw new ArgumentNullException (nameof (user));
//			}
//            _unitOfWork.BeginUnitOfWork();
//			_userRepository.Add (user);
//            _unitOfWork.Commit ();
			
//			return Task.FromResult<object> (null);
//		}

//        public Task<IdentityResult> UpdateAsync(DealEngineUser user, CancellationToken cancellationToken)
//        {
//            _dealEngine.Update(user);
//            _dealEngine.SaveChanges();

//            return Task.FromResult(IdentityResult.Success);
//        }
//    }
//}

