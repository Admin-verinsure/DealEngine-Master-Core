using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class ClaimService : IClaimService
    {
        IMapperSession<Claim> _claimsRepository;

        public ClaimService(IMapperSession<Claim> claimsRepository)
        {
            _claimsRepository = claimsRepository;
        }

        public async Task<List<Claim>> GetClaimsAllClaimsList()
        {
            return await _claimsRepository.FindAll().ToListAsync();
        }

        public async Task<Claim> GetTemplateByName(string claimName)
        {
            return await _claimsRepository.FindAll().FirstOrDefaultAsync(c => c.Value == claimName);
        }
    }
}
