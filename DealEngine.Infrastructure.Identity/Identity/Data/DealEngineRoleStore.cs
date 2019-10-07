using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineRoleStore : IRoleStore<DealEngineIdentityRole>
    {
        public Task<IdentityResult> CreateAsync(DealEngineIdentityRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(DealEngineIdentityRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task<DealEngineIdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<DealEngineIdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(DealEngineIdentityRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(DealEngineIdentityRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(DealEngineIdentityRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(DealEngineIdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetRoleNameAsync(DealEngineIdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(DealEngineIdentityRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

