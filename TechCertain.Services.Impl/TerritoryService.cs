using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class TerritoryService : ITerritoryService
    {      
        IMapperSession<Territory> _territoryRepository;

        public TerritoryService(IMapperSession<Territory> territoryRepository)
        {
            _territoryRepository = territoryRepository;
        }

        public async Task AddTerritory(Territory territory)
        {
            await _territoryRepository.AddAsync(territory);
        }

        public async Task CreateTerritory(string LocationName)
        {
            Territory territory = new Territory(null, null)
            {
                Ispublic = true,
                Location = LocationName,
            };

            await _territoryRepository.AddAsync(territory);
        }

        public async Task<Territory> GetTerritoryByName(string LocationName)
        {
            if(LocationName == "NZ")
            {
                var territory = await _territoryRepository.FindAll().FirstOrDefaultAsync(t => t.Location == LocationName);
                if(territory == null)
                {
                    await CreateTerritory("NZ");
                }

            }

            return await _territoryRepository.FindAll().FirstOrDefaultAsync(t => t.Location == LocationName);
        }

        public async Task UpdateTerritory(Territory territory)
        {
            await _territoryRepository.UpdateAsync(territory);
        }
    }
}

