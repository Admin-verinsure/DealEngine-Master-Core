using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace DealEngine.Services.Impl
{
    public class ResearchHouseService : IResearchHouseService
    {
        IMapperSession<ResearchHouse> _researchHouseRepository;

        public ResearchHouseService(IMapperSession<ResearchHouse> researchHouseRepository)
        {
            _researchHouseRepository = researchHouseRepository;
        }

        public async Task<ResearchHouse> GetResearchHouseById(Guid businessContractId)
        {
            return await _researchHouseRepository.GetByIdAsync(businessContractId);
        }

        public async Task Update(ResearchHouse researchHouse)
        {
            await _researchHouseRepository.UpdateAsync(researchHouse);
        }
    }
}

