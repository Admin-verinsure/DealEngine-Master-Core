using Microsoft.AspNetCore.Identity;


namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineIdentityRole : IdentityRole<string>
    {
        public DealEngineIdentityRole(string roleName) : base(roleName)
        {
        }
    }
}

