using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Domain.Exceptions;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Infrastructure.Ldap.Interfaces;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{

	public class AuthenticationService : IAuthenticationService
	{
		IUserService _userService;
		IMapperSession<SingleUseToken> _singleTokenRepository;
		ILdapService _ldapService;		

		public AuthenticationService (IUserService userRepository, IMapperSession<SingleUseToken> singleTokenRepository, ILdapService ldapService)
		{
            _userService = userRepository;
			_singleTokenRepository = singleTokenRepository;
			_ldapService = ldapService;		
		}

		#region IAuthenticationService implementation

		public User ValidateUser (string username, string password)
		{
			int resultCode = -1;
			string resultMessage = string.Empty;

			_ldapService.Validate (username, password, out resultCode, out resultMessage);

			if (resultCode == 0)
				return _userService.GetUser(username).Result;
			
			try {
				if (resultCode == 49)
					throw new AuthenticationException ("Unable to authenticate user") { ErrorCode = 49, User = username };
				throw new AuthenticationException (resultMessage) { ErrorCode = resultCode, User = username };
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}			
		}

		public async Task<SingleUseToken> GenerateSingleUseToken(string email)
		{
			if (string.IsNullOrWhiteSpace (email))
				throw new ArgumentException ("Parameter cannot be null, empty or whitespace.", "email");

			SingleUseToken request = null;
			try
			{
				User user = await _userService.GetUserByEmail(email);
				if (user == null)
					throw new ObjectNotFoundException("Email may be incorrect in ldap and/or application database.");

				request = new SingleUseToken (null, user.Id, "password");
				if (request == null)
					throw new Exception("Exception while creating token.");

	         await _singleTokenRepository.AddAsync(request);

			}
			catch (Exception ex) {
				throw new Exception (string.Format ("Unable to generate single use token for {0}.", email), ex);
			}

			return request;
		}

		public SingleUseToken GetToken(Guid token)
		{
			return _singleTokenRepository.GetByIdAsync(token).Result;
		}

		public async Task<bool> ValidSingleUseToken(Guid token)
		{
			SingleUseToken request = await _singleTokenRepository.GetByIdAsync(token);
			if (request == null)
				return false;

			User user = await _userService.GetUserById(request.UserID);
			if (user == null)
				return false;

			bool valid = request.TokenIsValid(user, "password");
			return request.TokenIsValid(user, "password");
		}

		public async Task<bool> UseSingleUseToken(Guid token)
		{
			SingleUseToken request = await _singleTokenRepository.GetByIdAsync(token);
			if (request == null)
				return false;

			request.SetUsed ();
            await _singleTokenRepository.AddAsync(request);            

			return true;
		}

		public IEnumerable<SingleUseToken> GetTokensFor (Guid userId)
		{
			return _singleTokenRepository.FindAll().Where(t => t.UserID == userId);
		}

        #endregion
    }
}

