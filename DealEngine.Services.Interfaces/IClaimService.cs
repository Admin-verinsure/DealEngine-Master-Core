using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IClaimService 
    {
        Task<Claim> GetTemplateByName(string claimName);
        Task<List<Claim>> GetClaimsAllClaimsList();

    }
}

