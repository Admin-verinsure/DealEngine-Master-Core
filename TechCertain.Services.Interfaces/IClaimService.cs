using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IClaimService 
    {
        Task<Claim> GetTemplateByName(string claimName);
        Task<List<Claim>> GetClaimsAllClaimsList();
    }
}

