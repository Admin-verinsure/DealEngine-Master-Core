
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface ITerritoryService
    {
        Task<TerritoryTemplate> GetTerritoryTemplateByName(string LocationName);
        Task UpdateTerritory(Territory Territory);
        Task AddTerritory(Territory Territory);
        Task AddTerritoryTemplate(TerritoryTemplate TerritoryTemplate);
        Task<List<TerritoryTemplate>> GetAllTerritoryTemplates();
        Task<TerritoryTemplate> GetTerritoryTemplateById(Guid TerritoryTemplateId);
        Task<Territory> GetTerritoryById(Guid TerritoryId);
        Task<Territory> GetTerritoryByName(string Location);
        Task RemoveTerritory(Territory territory);
    }
}

 
