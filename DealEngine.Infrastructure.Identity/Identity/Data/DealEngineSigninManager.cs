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
    public partial class DealEngineSignInManager : SignInManager<DealEngineUser>
    {        
        //protected ILdapService _ldapService;
        UserManager<DealEngineUser> _userManager;
        IUserClaimsPrincipalFactory<DealEngineUser> _claimsFactory;

        public DealEngineSignInManager(UserManager<DealEngineUser> userManager, IHttpContextAccessor contextAccessor,
                IUserClaimsPrincipalFactory<DealEngineUser> claimsFactory, 
                IOptions<IdentityOptions> optionsAccessor, 
                ILogger<SignInManager<DealEngineUser>> logger, 
                IAuthenticationSchemeProvider schemes)
                //ILdapService ldapService) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
            //_ldapService = ldapService;
            _claimsFactory = claimsFactory;
            _userManager = userManager;
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            throw new Exception();
            //int resultCode = -1;
            //string resultMessage = "";

            //_ldapService.Validate(userName, password, out resultCode, out resultMessage);
            //Console.WriteLine("Test");

            //if (resultCode == 0)
            //{
            //    if (string.IsNullOrWhiteSpace(userName))
            //    {
            //        var ldapUser = _ldapService.GetUser(userName);
            //        var localUser = await UserManager.FindByNameAsync(userName);
            //        MapUserToUser(ldapUser, localUser);
            //        await UserManager.UpdateAsync(localUser);
            //    }

            //    return await Task.FromResult(SignInResult.Success);
            //}
            //return await Task.FromResult(SignInResult.Failed);
        }

        //void MapUserToUser(User applicationUser, DealEngineUser dealEngineUser)
        //{
        //    dealEngineUser.ApplicationUser = applicationUser;
        //}


    }
}
