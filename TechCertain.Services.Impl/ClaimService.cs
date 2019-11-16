using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ClaimService : IClaimService
    {
        IMapperSession<Claim> _claimsRepository;

        public ClaimService(IMapperSession<Claim> claimsRepository)
        {
            _claimsRepository = claimsRepository;
        }

        public Task<List<Claim>> GetClaimsByOrganisation(Organisation organisation)
        {
            return _claimsRepository.FindAll().Where(c => c.Organisation == organisation).ToListAsync();
        }
    }
}
