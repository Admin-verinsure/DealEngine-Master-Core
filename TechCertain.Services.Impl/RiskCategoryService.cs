using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Collections.Generic;
using System;

namespace TechCertain.Services.Impl
{
    public class RiskCategoryService : IRiskCategoryService
    {
        IMapperSession<RiskCategory> _riskCategoryRepository;

        public RiskCategoryService(IMapperSession<RiskCategory> riskCategoryRepository)
        {
            _riskCategoryRepository = riskCategoryRepository;
        }

        public async Task AddRiskCategory(RiskCategory risk)
        {
            await _riskCategoryRepository.AddAsync(risk);
        }

        public async Task<List<RiskCategory>> GetAllRiskCategories()
        {
            return await _riskCategoryRepository.FindAll().ToListAsync();
        }

        public async Task<RiskCategory> GetRiskCategoryById(Guid Id)
        {
            return await _riskCategoryRepository.GetByIdAsync(Id);
        }
    }
}

