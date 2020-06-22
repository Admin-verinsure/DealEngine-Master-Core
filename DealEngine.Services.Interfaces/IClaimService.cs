using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IClaimService 
    {
        Task<Domain.Entities.Claim> GetTemplateByName(string claimName);
        Task<List<Domain.Entities.Claim>> GetClaimsAllClaimsList();
        Task AddClaim(Domain.Entities.Claim claim);
        Task RemoveClaim(string claim);
    }
}

