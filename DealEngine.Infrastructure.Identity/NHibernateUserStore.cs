using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.Identity
{
    public class NHibernateUserStore : IDealEngineUserStore
	{
		IUnitOfWorkFactory _unitOfWorkFactory;
		IRepository<User> _userRepository;
		IRepository<ApplicationGroup> _groupRepository;

		public NHibernateUserStore (IUnitOfWorkFactory unitOfWorkFactory, IRepository<User> userRepository, IRepository<ApplicationGroup> groupRepository)
		{
			if (unitOfWorkFactory == null) {
				throw new ArgumentNullException (nameof (unitOfWorkFactory));
			}
			if (userRepository == null) {
				throw new ArgumentNullException (nameof (userRepository));
			}

			_unitOfWorkFactory = unitOfWorkFactory;
			_userRepository = userRepository;
			_groupRepository = groupRepository;
		}

        public IQueryable<User> Users {
			get {
				return _userRepository.FindAll ();
			}
		}

		public Task AddToRoleAsync (User user, string roleName)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			if (string.IsNullOrWhiteSpace (roleName)) {
				throw new ArgumentNullException (nameof (roleName));
			}

			user.Groups.Add (_groupRepository.FindAll ().FirstOrDefault (g => g.Name == roleName));

			return UpdateAsync (user);
		}

		public Task CreateAsync (User user)
		{
			return UpdateAsync (user);
		}

		public Task DeleteAsync (User user)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork ()) {
				_userRepository.Remove (user);
				uow.Commit ();
			}
			return Task.FromResult<object> (null);
		}

		public void Dispose ()
		{
			
		}

		public Task<User> FindByEmailAsync (string email)
		{
			if (string.IsNullOrWhiteSpace (email)) {
				throw new ArgumentNullException (nameof (email));
			}
			var user = _userRepository.FindAll ().FirstOrDefault (u => u.Email == email);

			return Task.FromResult (user);
		}

		public Task<User> FindByIdAsync (Guid userId)
		{
			if (userId == Guid.Empty) {
				throw new ArgumentNullException (nameof (userId));
			}

			return Task.FromResult (_userRepository.GetById (userId));
		}

		public Task<User> FindByNameAsync (string userName)
		{
			if (string.IsNullOrWhiteSpace(userName)) {
				throw new ArgumentNullException (nameof (userName));
			}
			var user = _userRepository.FindAll ().FirstOrDefault (u => u.UserName == userName);

			return Task.FromResult (user);
		}

		public Task<string> GetEmailAsync (User user)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			if (!string.IsNullOrWhiteSpace (user.Email)) {
				return Task.FromResult (user.Email);
			}
			return Task.FromResult (FindByIdAsync(user.Id).Result.Email);
		}

		public Task<bool> GetEmailConfirmedAsync (User user)
		{
			throw new NotImplementedException ();
		}

		public Task<IList<string>> GetRolesAsync (User user)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			IList<string> roles = user.Groups.Select (g => g.Name).ToList();
			return Task.FromResult (roles);
		}

		public Task<bool> IsInRoleAsync (User user, string roleName)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			if (string.IsNullOrWhiteSpace (roleName)) {
				throw new ArgumentNullException (nameof (roleName));
			}
			return Task.FromResult (user.Groups.FirstOrDefault (g => g.Name == roleName) != null);
		}

		public Task RemoveFromRoleAsync (User user, string roleName)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			if (string.IsNullOrWhiteSpace (roleName)) {
				throw new ArgumentNullException (nameof (roleName));
			}

			user.Groups.Remove (user.Groups.FirstOrDefault (g => g.Name == roleName));
			return UpdateAsync (user);
		}

		public Task SetEmailAsync (User user, string email)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			user.Email = email;
			return UpdateAsync (user);
		}

		public Task SetEmailConfirmedAsync (User user, bool confirmed)
		{
			throw new NotImplementedException ();
		}

		public Task UpdateAsync (User user)
		{
			if (user == null) {
				throw new ArgumentNullException (nameof (user));
			}
			using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork ()) {
				_userRepository.Add (user);
				uow.Commit ();
			}
			return Task.FromResult<object> (null);
		}

		public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync (User user)
		{
			throw new NotImplementedException ();
		}

		public Task AddClaimAsync (User user, System.Security.Claims.Claim claim)
		{
			throw new NotImplementedException ();
		}

		public Task RemoveClaimAsync (User user, System.Security.Claims.Claim claim)
		{
			throw new NotImplementedException ();
		}

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

