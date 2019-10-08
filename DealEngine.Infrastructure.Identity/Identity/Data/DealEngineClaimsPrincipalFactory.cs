using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineClaimsPrincipalFactory : UserClaimsPrincipalFactory<DealEngineUser>
    {
        public DealEngineClaimsPrincipalFactory(UserManager<DealEngineUser> userManager, IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {
        }

        public override Task<ClaimsPrincipal> CreateAsync(DealEngineUser user)
        {
            //this is where we load the role and claims - just defaulted for now
            var claims = new[]
            {
                new Claim("c1", "v1"),
                new Claim("c2", "v2"),
                new Claim("c3", "v3")
            };

            var claimIdentity = new ClaimsIdentity("SuperUser", "TCUser", user.UserName);
            foreach(Claim cl in claims)
            {
                claimIdentity.AddClaim(cl);
            }           
            return Task.FromResult(new ClaimsPrincipal(claimIdentity));            
        }


    }
}