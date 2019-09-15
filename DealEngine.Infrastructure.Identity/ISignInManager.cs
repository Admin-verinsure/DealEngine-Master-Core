using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity
{
    public interface ISignInManager 
	{

        //
        // Summary:
        //     Returns a flag indicating whether the specified user can sign in.
        //
        // Parameters:
        //   user:
        //     The user whose sign-in status should be returned.
        //
        // Returns:
        //     The task object representing the asynchronous operation, containing a flag that
        //     is true if the specified user can sign-in, otherwise false.

        public Task<bool> CanSignInAsync(User user);
        //
        // Summary:
        //     Attempts a password sign in for a user.
        //
        // Parameters:
        //   user:
        //     The user to sign in.
        //
        //   password:
        //     The password to attempt to sign in with.
        //
        //   lockoutOnFailure:
        //     Flag indicating if the user account should be locked if the sign in fails.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);
        //
        // Summary:
        //     Configures the redirect URL and user identifier for the specified external login
        //     provider.
        //
        // Parameters:
        //   provider:
        //     The provider to configure.
        //
        //   redirectUrl:
        //     The external login URL users should be redirected to during the login flow.
        //
        //   userId:
        //     The current user's identifier, which will be used to provide CSRF protection.
        //
        // Returns:
        //     A configured Microsoft.AspNetCore.Authentication.AuthenticationProperties.
        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);
        //
        // Summary:
        //     Creates a System.Security.Claims.ClaimsPrincipal for the specified user, as an
        //     asynchronous operation.
        //
        // Parameters:
        //   user:
        //     The user to create a System.Security.Claims.ClaimsPrincipal for.
        //
        // Returns:
        //     The task object representing the asynchronous operation, containing the ClaimsPrincipal
        //     for the specified user.
        public  Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user);
        //
        // Summary:
        //     Signs in a user via a previously registered third party login, as an asynchronous
        //     operation.
        //
        // Parameters:
        //   loginProvider:
        //     The login provider to use.
        //
        //   providerKey:
        //     The unique provider identifier for the user.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        //
        // Summary:
        //     Signs in a user via a previously registered third party login, as an asynchronous
        //     operation.
        //
        // Parameters:
        //   loginProvider:
        //     The login provider to use.
        //
        //   providerKey:
        //     The unique provider identifier for the user.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   bypassTwoFactor:
        //     Flag indicating whether to bypass two factor authentication.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor);
        //
        // Summary:
        //     Clears the "Remember this browser flag" from the current browser, as an asynchronous
        //     operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public Task ForgetTwoFactorClientAsync();
        //
        // Summary:
        //     Gets a collection of Microsoft.AspNetCore.Authentication.AuthenticationSchemes
        //     for the known external login providers.
        //
        // Returns:
        //     A collection of Microsoft.AspNetCore.Authentication.AuthenticationSchemes for
        //     the known external login providers.
        public Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        //
        // Summary:
        //     Gets the external login information for the current login, as an asynchronous
        //     operation.
        //
        // Parameters:
        //   expectedXsrf:
        //     Flag indication whether a Cross Site Request Forgery token was expected in the
        //     current request.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the ExternalLoginInfo
        //     for the sign-in attempt.
        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null);
        //
        // Summary:
        //     Gets the TUser for the current two factor authentication login, as an asynchronous
        //     operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the TUser
        //     for the sign-in attempt.
        public Task<User> GetTwoFactorAuthenticationUserAsync();
        //
        // Summary:
        //     Returns true if the principal has an identity with the application cookie identity
        //
        // Parameters:
        //   principal:
        //     The System.Security.Claims.ClaimsPrincipal instance.
        //
        // Returns:
        //     True if the user is logged in with identity.
        public bool IsSignedIn(ClaimsPrincipal principal);
        //
        // Summary:
        //     Returns a flag indicating if the current client browser has been remembered by
        //     two factor authentication for the user attempting to login, as an asynchronous
        //     operation.
        //
        // Parameters:
        //   user:
        //     The user attempting to login.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing true if the
        //     browser has been remembered for the current user.
        public Task<bool> IsTwoFactorClientRememberedAsync(User user);
        //
        // Summary:
        //     Attempts to sign in the specified user and password combination as an asynchronous
        //     operation.
        //
        // Parameters:
        //   user:
        //     The user to sign in.
        //
        //   password:
        //     The password to attempt to sign in with.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   lockoutOnFailure:
        //     Flag indicating if the user account should be locked if the sign in fails.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure);
        //
        // Summary:
        //     Attempts to sign in the specified userName and password combination as an asynchronous
        //     operation.
        //
        // Parameters:
        //   userName:
        //     The user name to sign in.
        //
        //   password:
        //     The password to attempt to sign in with.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   lockoutOnFailure:
        //     Flag indicating if the user account should be locked if the sign in fails.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        //
        // Summary:
        //     Regenerates the user's application cookie, whilst preserving the existing AuthenticationProperties
        //     like rememberMe, as an asynchronous operation.
        //
        // Parameters:
        //   user:
        //     The user whose sign-in cookie should be refreshed.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public Task RefreshSignInAsync(User user);
        //
        // Summary:
        //     Sets a flag on the browser to indicate the user has selected "Remember this browser"
        //     for two factor authentication purposes, as an asynchronous operation.
        //
        // Parameters:
        //   user:
        //     The user who choose "remember this browser".
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public Task RememberTwoFactorClientAsync(User user);
        //
        // Summary:
        //     Signs in the specified user.
        //
        // Parameters:
        //   user:
        //     The user to sign-in.
        //
        //   authenticationProperties:
        //     Properties applied to the login and authentication cookie.
        //
        //   authenticationMethod:
        //     Name of the method used to authenticate the user.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string authenticationMethod = null);
        //
        // Summary:
        //     Signs in the specified user.
        //
        // Parameters:
        //   user:
        //     The user to sign-in.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   authenticationMethod:
        //     Name of the method used to authenticate the user.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public Task SignInAsync(User user, bool isPersistent, string authenticationMethod = null);
        //
        // Summary:
        //     Signs the current user out of the application.
        public Task SignOutAsync();
        //
        // Summary:
        //     Validates the sign in code from an authenticator app and creates and signs in
        //     the user, as an asynchronous operation.
        //
        // Parameters:
        //   code:
        //     The two factor authentication code to validate.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   rememberClient:
        //     Flag indicating whether the current browser should be remember, suppressing all
        //     further two factor authentication prompts.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient);
        //
        // Summary:
        //     Signs in the user without two factor authentication using a two factor recovery
        //     code.
        //
        // Parameters:
        //   recoveryCode:
        //     The two factor recovery code.
        public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);
        //
        // Summary:
        //     Validates the two factor sign in code and creates and signs in the user, as an
        //     asynchronous operation.
        //
        // Parameters:
        //   provider:
        //     The two factor authentication provider to validate the code against.
        //
        //   code:
        //     The two factor authentication code to validate.
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   rememberClient:
        //     Flag indicating whether the current browser should be remember, suppressing all
        //     further two factor authentication prompts.
        //
        // Returns:
        //     The task object representing the asynchronous operation containing the SignInResult
        //     for the sign-in attempt.
        public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
        //
        // Summary:
        //     Stores any authentication tokens found in the external authentication cookie
        //     into the associated user.
        //
        // Parameters:
        //   externalLogin:
        //     The information from the external login provider.
        //
        // Returns:
        //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
        //     the Microsoft.AspNetCore.Identity.IdentityResult of the operation.
        public Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin);
        //
        // Summary:
        //     Validates the security stamp for the specified user. Will always return false
        //     if the userManager does not support security stamps.
        //
        // Parameters:
        //   user:
        //     The user whose stamp should be validated.
        //
        //   securityStamp:
        //     The expected security stamp value.
        //
        // Returns:
        //     True if the stamp matches the persisted value, otherwise it will return false.
        public Task<bool> ValidateSecurityStampAsync(User user, string securityStamp);
        //
        // Summary:
        //     Validates the security stamp for the specified principal against the persisted
        //     stamp for the current user, as an asynchronous operation.
        //
        // Parameters:
        //   principal:
        //     The principal whose stamp should be validated.
        //
        // Returns:
        //     The task object representing the asynchronous operation. The task will contain
        //     the TUser if the stamp matches the persisted value, otherwise it will return
        //     false.
        public Task<User> ValidateSecurityStampAsync(ClaimsPrincipal principal);
        //
        // Summary:
        //     Validates the security stamp for the specified principal from one of the two
        //     factor principals (remember client or user id) against the persisted stamp for
        //     the current user, as an asynchronous operation.
        //
        // Parameters:
        //   principal:
        //     The principal whose stamp should be validated.
        //
        // Returns:
        //     The task object representing the asynchronous operation. The task will contain
        //     the TUser if the stamp matches the persisted value, otherwise it will return
        //     false.
        public Task<User> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal);
        //
        // Summary:
        //     Used to determine if a user is considered locked out.
        //
        // Parameters:
        //   user:
        //     The user.
        //
        // Returns:
        //     Whether a user is considered locked out.
        public Task<bool> IsLockedOut(User user);
        //
        // Summary:
        //     Returns a locked out SignInResult.
        //
        // Parameters:
        //   user:
        //     The user.
        //
        // Returns:
        //     A locked out SignInResult
        public Task<SignInResult> LockedOut(User user);
        //
        // Summary:
        //     Used to ensure that a user is allowed to sign in.
        //
        // Parameters:
        //   user:
        //     The user
        //
        // Returns:
        //     Null if the user should be allowed to sign in, otherwise the SignInResult why
        //     they should be denied.
        public Task<SignInResult> PreSignInCheck(User user);
        //
        // Summary:
        //     Used to reset a user's lockout count.
        //
        // Parameters:
        //   user:
        //     The user
        //
        // Returns:
        //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
        //     the Microsoft.AspNetCore.Identity.IdentityResult of the operation.
        public Task ResetLockout(User user);
        //
        // Summary:
        //     Signs in the specified user if bypassTwoFactor is set to false. Otherwise stores
        //     the user for use after a two factor check.
        //
        // Parameters:
        //   user:
        //
        //   isPersistent:
        //     Flag indicating whether the sign-in cookie should persist after the browser is
        //     closed.
        //
        //   loginProvider:
        //     The login provider to use. Default is null
        //
        //   bypassTwoFactor:
        //     Flag indicating whether to bypass two factor authentication. Default is false
        //
        // Returns:
        //     Returns a Microsoft.AspNetCore.Identity.SignInResult
        public Task<SignInResult> SignInOrTwoFactorAsync(User user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = false);
    }


}

