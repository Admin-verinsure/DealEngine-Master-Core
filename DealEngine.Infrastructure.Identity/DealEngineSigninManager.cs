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

namespace DealEngine.Infrastructure.Identity
{
    public class DealEngineSignInManager : ISignInManager
    {
        //protected IAuthenticationManager AuthenticationManager { get; set; }
        protected ILdapService LdapService { get; set; }
        protected UserManager<User> UserManager { get; set; }

        public DealEngineSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes,
            IAuthenticationManager authenticationManager, ILdapService ldapService)

        {
            UserManager = userManager;
            LdapService = ldapService;
        }

        public async void SyncUserFromAuth(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return;

            var ldapUser = LdapService.GetUser(username);
            var localUser = await UserManager.FindByNameAsync(username);
            MapUserToUser(ldapUser, localUser);
            await UserManager.UpdateAsync(localUser);
        }

        public void SyncUserToAuth(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return;
            throw new NotImplementedException();
        }

        void MapUserToUser(User user1, User user2)
        {

        }

        public Task<bool> CanSignInAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            throw new NotImplementedException();
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
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

        public Task<User> GetTwoFactorAuthenticationUserAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTwoFactorClientRememberedAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            throw new NotImplementedException();
        }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            int resultCode = -1;
            string resultMessage = "";

            LdapService.Validate(userName, password, out resultCode, out resultMessage);
            Console.WriteLine("Test");

            if (resultCode == 0)
            {                
                if (string.IsNullOrWhiteSpace(userName))
                {
                    var ldapUser = LdapService.GetUser(userName);
                    var localUser = await UserManager.FindByNameAsync(userName);
                    MapUserToUser(ldapUser, localUser);
                    await UserManager.UpdateAsync(localUser);
                }               

                return await Task.FromResult(SignInResult.Success);
            }
            return await Task.FromResult(SignInResult.Failed);
        }

        public Task RefreshSignInAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task RememberTwoFactorClientAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(User user, bool isPersistent, string authenticationMethod = null)
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

        public Task<bool> ValidateSecurityStampAsync(User user, string securityStamp)
        {
            throw new NotImplementedException();
        }

        public Task<User> ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<User> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsLockedOut(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> LockedOut(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> PreSignInCheck(User user)
        {
            throw new NotImplementedException();
        }

        public Task ResetLockout(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> SignInOrTwoFactorAsync(User user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = false)
        {
            throw new NotImplementedException();
        }
    }
}
