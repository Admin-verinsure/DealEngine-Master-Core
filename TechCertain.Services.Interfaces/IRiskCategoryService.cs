using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IRiskCategoryService
    {
        Task<List<RiskCategory>> GetAllRiskCategories();
        Task AddRiskCategory(RiskCategory risk);
        Task<RiskCategory> GetRiskCategoryById(Guid Id);
    }
}
