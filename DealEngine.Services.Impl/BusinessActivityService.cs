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

        public async Task AttachClientProgrammeToActivities(Programme programme, BusinessActivityTemplate businessActivity)
        {
            businessActivity.Programme = programme;
            await Update(businessActivity);
        }

        public async Task Update(BusinessActivityTemplate businessActivity)
        {
            await _businessActivityTemplateRepository.UpdateAsync(businessActivity);
        }

        public async Task CreateBusinessActivity(BusinessActivity businessActivity)
        {
            if (businessActivity.AnzsciCode != null || businessActivity.Description != null)
            {
                await _businessActivityRepository.AddAsync(businessActivity);
            }
        }

        public async Task<List<BusinessActivityTemplate>> GetBusinessActivitiesTemplate()
        {
            return await _businessActivityTemplateRepository.FindAll().OrderBy(ba => ba.AnzsciCode).ToListAsync();            
        }

        public async Task<List<BusinessActivityTemplate>> GetBusinessActivitiesByClassification(int classification)
        {
            var list = await GetBusinessActivitiesTemplate();
            return list.Where(ba => ba.Classification == classification).ToList();
        }

        public async Task<BusinessActivity> GetBusinessActivity(Guid Id)
        {
            return await _businessActivityRepository.GetByIdAsync(Id);
        }

        public async Task<BusinessActivity> GetBusinessActivityByCode(string AnzsciCode)
        {
            return await _businessActivityRepository.FindAll().FirstOrDefaultAsync(a => a.AnzsciCode == AnzsciCode);
        }

        public async Task UpdateBusinessActivity(BusinessActivity businessActivity)
        {
            await _businessActivityRepository.UpdateAsync(businessActivity);
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

        public async Task<BusinessActivityTemplate> GetBusinessActivityTemplateByCode(string anzsciCode)
        {
            return await _businessActivityTemplateRepository.FindAll().FirstOrDefaultAsync(bat => bat.AnzsciCode == anzsciCode);
        }
    }
}
