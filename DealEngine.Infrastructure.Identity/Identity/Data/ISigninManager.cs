using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DealEngine.Infrastructure.Identity.Data
{
    public interface ISignInManager<TUser> where TUser : class
    {
        public  Task<bool> CanSignInAsync(TUser user);
        public  Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);
        public  AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);
        public  Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user);
        public  Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor);
        public  Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        public  Task ForgetTwoFactorClientAsync();
        public  Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        public  Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null);
        public  Task<TUser> GetTwoFactorAuthenticationUserAsync();
        public  bool IsSignedIn(ClaimsPrincipal principal);
        public  Task<bool> IsTwoFactorClientRememberedAsync(TUser user);
        public  Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        public  Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure);
        public  Task RefreshSignInAsync(TUser user);
        public Task<string> GetHttpContext();
        public  Task RememberTwoFactorClientAsync(TUser user);
        public  Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null);
        public  Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null);
        public  Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims);
        public  Task SignInWithClaimsAsync(TUser user, bool isPersistent, IEnumerable<Claim> additionalClaims);
        public  Task SignOutAsync();
        public  Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient);        
        public  Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);
        public  Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
        public  Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin);
        public  Task<TUser> ValidateSecurityStampAsync(ClaimsPrincipal principal);
        public  Task<bool> ValidateSecurityStampAsync(TUser user, string securityStamp);
        public  Task<TUser> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal);

    }
}
