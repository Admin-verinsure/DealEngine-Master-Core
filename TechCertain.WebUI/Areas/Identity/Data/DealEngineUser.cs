using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the DealEngineUser class
    public class DealEngineUser : IdentityUser
    {
        public DealEngineUser(string userName) : base(userName)
        {
        }
    }
}
