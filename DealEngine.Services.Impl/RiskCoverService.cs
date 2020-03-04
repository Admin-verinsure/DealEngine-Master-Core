using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using NHibernate.Linq;

namespace DealEngine.Services.Impl
{
    public class RiskCoverService : IRiskCoverService
    {
        IMapperSession<RiskCover> _riskCoverRepository;

        public RiskCoverService(IMapperSession<RiskCover> riskCoverRepository)
        {
            _riskCoverRepository = riskCoverRepository;
        }

        public async Task<List<RiskCover>> GetAllRiskCovers()
        {
            return await _riskCoverRepository.FindAll().ToListAsync();
        }
    }
}

