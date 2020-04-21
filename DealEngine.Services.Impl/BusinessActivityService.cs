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
    public class BusinessActivityService : IBusinessActivityService
    {
        IMapperSession<BusinessActivity> _businessActivityRepository;
        IMapperSession<BusinessActivityTemplate> _businessActivityTemplateRepository;

        public BusinessActivityService(IMapperSession<BusinessActivity> businessActivityRepository, IMapperSession<BusinessActivityTemplate> businessActivityTemplateRepository)
        {
            _businessActivityRepository = businessActivityRepository;
            _businessActivityTemplateRepository = businessActivityTemplateRepository;
        }

        public async Task<List<BusinessActivityTemplate>> GetBusinessActivitiesTemplates()
        {
            return await _businessActivityTemplateRepository.FindAll().OrderBy(ba => ba.AnzsciCode).ToListAsync();            
        }

        public async Task<List<BusinessActivityTemplate>> GetBusinessActivitiesByClassification(int classification)
        {
            var list = await GetBusinessActivitiesTemplates();
            return list.Where(ba => ba.Classification == classification).ToList();
        }

        public async Task CreateBusinessActivityTemplate(BusinessActivityTemplate businessActivityTemplate)
        {
            if (businessActivityTemplate.AnzsciCode != null || businessActivityTemplate.Description != null)
            {
                await _businessActivityTemplateRepository.AddAsync(businessActivityTemplate);
            }
        }

        public async Task<BusinessActivityTemplate> GetBusinessActivityTemplate(Guid Id)
        {
            return await _businessActivityTemplateRepository.GetByIdAsync(Id);
        }

        public async Task<BusinessActivity> CreateActivity(Guid guid)
        {
            var template = await _businessActivityTemplateRepository.GetByIdAsync(guid);
            BusinessActivity activity = new BusinessActivity(null)
            {
                AnzsciCode = template.AnzsciCode,
                Description = template.Description,                
            };
            await _businessActivityRepository.AddAsync(activity);
            return activity;
        }
    }
}
