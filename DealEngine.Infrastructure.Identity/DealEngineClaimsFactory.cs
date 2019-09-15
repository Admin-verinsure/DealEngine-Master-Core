using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace DealEngine.Infrastructure.Identity
{
    public class DealEngineClaimsFactory : IUserClaimsPrincipalFactory<User>
    {
        public DealEngineClaimsFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor) 
        {
            
        }

        public Task<ClaimsPrincipal> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}

