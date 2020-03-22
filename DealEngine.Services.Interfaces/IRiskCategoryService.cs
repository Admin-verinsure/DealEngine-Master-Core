using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IRiskCategoryService
    {
        Task<List<RiskCategory>> GetAllRiskCategories();
        Task AddRiskCategory(RiskCategory risk);
        Task<RiskCategory> GetRiskCategoryById(Guid Id);
        Task<List<RiskCategory>> CreateAllRiskCategories();
    }
}
