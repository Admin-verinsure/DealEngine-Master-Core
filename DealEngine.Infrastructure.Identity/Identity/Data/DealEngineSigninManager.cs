using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineSignInManager : ISignInManager<DealEngineUser>
    {        
        protected ILdapService _ldapService;
        UserManager<DealEngineUser> _userManager;        
        IHttpContextAccessor _contextAccessor;
        IUserClaimsPrincipalFactory<DealEngineUser> _claimsFactory;
        IOptions<IdentityOptions> _optionsAccessor;
        ILogger<SignInManager<DealEngineUser>> _logger;
        IAuthenticationSchemeProvider _schemes;

        public DealEngineSignInManager(UserManager<DealEngineUser> userManager, IHttpContextAccessor contextAccessor,
                IUserClaimsPrincipalFactory<DealEngineUser> claimsFactory, 
                IOptions<IdentityOptions> optionsAccessor, 
                ILogger<SignInManager<DealEngineUser>> logger, 
                IAuthenticationSchemeProvider schemes,      
                ILdapService ldapService) 
        {
            _ldapService = ldapService;
            _claimsFactory = claimsFactory;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
            _schemes = schemes;
        }

        public Task<bool> CanSignInAsync(DealEngineUser user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> CheckPasswordSignInAsync(DealEngineUser user, string password, bool lockoutOnFailure)
        {
            throw new NotImplementedException();
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimsPrincipal> CreateUserPrincipalAsync(DealEngineUser user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            throw new NotImplementedException();
        }

        public Task ForgetTwoFactorClientAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            throw new NotImplementedException();
        }

        public Task<DealEngineUser> GetTwoFactorAuthenticationUserAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTwoFactorClientRememberedAsync(DealEngineUser user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {

            int resultCode = -1;
            string resultMessage = "";

            _ldapService.Validate(userName, password, out resultCode, out resultMessage);
            Console.WriteLine("Test");

            if (resultCode == 0)
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    var ldapUser = _ldapService.GetUser(userName);
                    var localUser = _userManager.FindByNameAsync(userName).Result;
                    if(localUser == null)
                    {
                        localUser = new DealEngineUser { UserName = userName };
                        _userManager.CreateAsync(localUser, password);
                    }
                    MapUserToUser(ldapUser, localUser);                    
                }

                 return Task.FromResult(SignInResult.Success);
            }
            return Task.FromResult(SignInResult.Failed);
        }

        private void MapUserToUser(User ldapUser, DealEngineUser localUser)
        {
            localUser.ApplicationUser = ldapUser;
        }

        public Task<SignInResult> PasswordSignInAsync(DealEngineUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            throw new NotImplementedException();
        }

        public Task RefreshSignInAsync(DealEngineUser user)
        {
            throw new NotImplementedException();
        }

        public Task RememberTwoFactorClientAsync(DealEngineUser user)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(DealEngineUser user, bool isPersistent, string authenticationMethod = null)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(DealEngineUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            throw new NotImplementedException();
        }

        public Task SignInWithClaimsAsync(DealEngineUser user, AuthenticationProperties authenticationProperties, IEnumerable<System.Security.Claims.Claim> additionalClaims)
        {
            throw new NotImplementedException();
        }

        public Task SignInWithClaimsAsync(DealEngineUser user, bool isPersistent, IEnumerable<System.Security.Claims.Claim> additionalClaims)
        {
            throw new NotImplementedException();
        }

        public Task SignOutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin)
        {
            throw new NotImplementedException();
        }

        public Task<DealEngineUser> ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateSecurityStampAsync(DealEngineUser user, string securityStamp)
        {
            throw new NotImplementedException();
        }

        public Task<DealEngineUser> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
