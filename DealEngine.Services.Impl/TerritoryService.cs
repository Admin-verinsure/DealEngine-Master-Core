using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class TerritoryService : ITerritoryService
    {      
        IMapperSession<Territory> _territoryRepository;
        IMapperSession<TerritoryTemplate> _territoryTemplateRepository;

        public TerritoryService(IMapperSession<Territory> territoryRepository,  IMapperSession<TerritoryTemplate> territoryTemplateRepository)
        {
            _territoryRepository = territoryRepository;
            _territoryTemplateRepository = territoryTemplateRepository;
        }

        public async Task AddTerritory(Territory territory)
        {
            await _territoryRepository.AddAsync(territory);
        }

        public async Task AddTerritoryTemplate(TerritoryTemplate territoryTemplate)
        {
            await _territoryTemplateRepository.AddAsync(territoryTemplate);
        }

        public async Task CreateTerritoryTemplate(string LocationName)
        {
            TerritoryTemplate territoryTemplate = new TerritoryTemplate(null, LocationName);
            territoryTemplate.Ispublic = true;
            await _territoryTemplateRepository.AddAsync(territoryTemplate);
        }

        public async Task<List<TerritoryTemplate>> GetAllTerritoryTemplates()
        {
            return await _territoryTemplateRepository.FindAll().ToListAsync();
        }

        public async Task<TerritoryTemplate> GetTerritoryTemplateById(Guid territoryTemplateId)
        {
            return await _territoryTemplateRepository.GetByIdAsync(territoryTemplateId);
        }

        public async Task<TerritoryTemplate> GetTerritoryTemplateByName(string LocationName)
        {
            if(LocationName == "NZ")
            {
                var territory = await _territoryTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Location == LocationName);
                if(territory == null)
                {
                    await CreateTerritoryTemplate("NZ");
                }

            }

            return await _territoryTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Location == LocationName);
        }

        public async Task UpdateTerritory(Territory territory)
        {
            await _territoryRepository.UpdateAsync(territory);
        }

        public async Task<Territory> GetTerritoryById(Guid territoryId)
        {
            return await _territoryRepository.GetByIdAsync(territoryId);
        }

        public async Task<Territory> GetTerritoryByName(string location)
        {
            return await _territoryRepository.FindAll().FirstOrDefaultAsync(t => t.Location == location);
        }

        public async Task<Territory> GetTerritoryByTemplateId(Guid Id)
        {
            return await _territoryRepository.FindAll().FirstOrDefaultAsync(t => t.TerritoryTemplateId == Id);
        }

        public async Task RemoveTerritory(Territory territory)
        {
            await _territoryRepository.RemoveAsync(territory);
        }
    }
}

