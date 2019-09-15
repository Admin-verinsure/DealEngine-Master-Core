using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Exceptions;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{

	public class AuthenticationService : IAuthenticationService
	{
		IUnitOfWorkFactory _unitOfWork;
		IUserService _userService;
		IRepository<SingleUseToken> _singleTokenRepository;
		ILdapService _ldapService;
		ILogger _logger;

		public AuthenticationService (IUnitOfWorkFactory unitOfWork, IUserService userRepository, IRepository<SingleUseToken> singleTokenRepository, ILdapService ldapService, ILogger logger)
		{
			_unitOfWork = unitOfWork;
            _userService = userRepository;
			_singleTokenRepository = singleTokenRepository;
			_ldapService = ldapService;
			_logger = logger;
		}

		#region IAuthenticationService implementation

		public User ValidateUser (string username, string password)
		{
			int resultCode = -1;
			string resultMessage = string.Empty;

			_ldapService.Validate (username, password, out resultCode, out resultMessage);

			if (resultCode == 0)
				return _userService.GetUser (username);
			
			try {
				if (resultCode == 49)
					throw new AuthenticationException ("Unable to authenticate user") { ErrorCode = 49, User = username };
				throw new AuthenticationException (resultMessage) { ErrorCode = resultCode, User = username };
			}
			catch (Exception ex) {
				_logger.Error (ex);
			}

			return null;
		}

		public SingleUseToken GenerateSingleUseToken(string email)
		{
			if (string.IsNullOrWhiteSpace (email))
				throw new ArgumentException ("Parameter cannot be null, empty or whitespace.", "email");

			SingleUseToken request = null;
			try
			{
				User user = _userService.GetUserByEmail (email);
				if (user == null)
					throw new ObjectNotFoundException("Email may be incorrect in ldap and/or application database.");

				request = new SingleUseToken (null, user.Id, "password");
				if (request == null)
					throw new Exception("Exception while creating token.");

				using (var uow = _unitOfWork.BeginUnitOfWork())
				{
					//uow.Add<SingleUseToken>(request);
	                _singleTokenRepository.Add(request);
					uow.Commit();
				}
			}
			catch (Exception ex) {
				throw new Exception (string.Format ("Unable to generate single use token for {0}.", email), ex);
			}

			return request;
		}

		public SingleUseToken GetToken(Guid token)
		{
			return _singleTokenRepository.GetById (token);
		}

		public bool ValidSingleUseToken(Guid token)
		{
			SingleUseToken request = _singleTokenRepository.GetById (token);
			if (request == null)
				return false;

			User user = _userService.GetUser (request.UserID);
			if (user == null)
				return false;

			bool valid = request.TokenIsValid(user, "password");
			return request.TokenIsValid(user, "password");
		}

		public bool UseSingleUseToken(Guid token)
		{
			SingleUseToken request = _singleTokenRepository.GetById (token);
			if (request == null)
				return false;

			request.SetUsed ();

			using (var uow = _unitOfWork.BeginUnitOfWork())
			{
				//uow.Add<SingleUseToken>(request);
                _singleTokenRepository.Add(request);
				uow.Commit();
			}

			return true;
		}

		public IEnumerable<SingleUseToken> GetTokensFor (Guid userId)
		{
			return _singleTokenRepository.FindAll ().Where (t => t.UserID == userId);
		}

        #endregion
    }
}

