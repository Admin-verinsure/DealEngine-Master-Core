using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Collections.Generic;
using System;

namespace DealEngine.Services.Impl
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

        public async Task<List<RiskCategory>> CreateAllRiskCategories()
        {
            var riskArray = new List<string>();
            riskArray.Add("People");
            riskArray.Add("Business");
            riskArray.Add("Associations");
            riskArray.Add("Professions");
            riskArray.Add("Goods");
            riskArray.Add("Agriculture");
            riskArray.Add("Transport");
            riskArray.Add("Marine");

            var list = new List<RiskCategory>();
            foreach(var risk in riskArray)
            {
                RiskCategory riskCategory = new RiskCategory(null, risk, "");
                await _riskCategoryRepository.AddAsync(riskCategory);
                list.Add(riskCategory);
            }

            return list;
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

