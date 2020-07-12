using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IResearchHouseService
    {
        Task<ResearchHouse> GetResearchHouseById(Guid researchHouseId);
        Task Update(ResearchHouse researchHouse);
    }
}
