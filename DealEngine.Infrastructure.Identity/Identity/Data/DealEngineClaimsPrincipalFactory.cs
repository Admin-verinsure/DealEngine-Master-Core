using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineClaimsPrincipalFactory : IUserClaimsPrincipalFactory<DealEngineUser>
    {
        public Task<ClaimsPrincipal> CreateAsync(DealEngineUser user)
        {
            //this is where we load the role and claims - just defaulted for now
            var claimIdentity = new ClaimsIdentity("SuperUser", "TCUser", user.UserName);
            return Task.FromResult(new ClaimsPrincipal(claimIdentity));            
        }
    }
}