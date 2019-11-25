
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ITerritoryService
    {
        Task<Territory> GetTerritoryByName(string LocationName);
        Task UpdateTerritory(Territory territory);
        Task AddTerritory(Territory territory);
    }
}

 
