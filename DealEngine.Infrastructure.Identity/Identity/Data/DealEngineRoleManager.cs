using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineRoleManager : RoleManager<DealEngineIdentityRole>
    {
        public DealEngineRoleManager(IRoleStore<DealEngineIdentityRole> store, IEnumerable<IRoleValidator<DealEngineIdentityRole>> roleValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            ILogger<RoleManager<DealEngineIdentityRole>> logger) 
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}

