using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class BusinessActivityService : IBusinessActivityService
    {
        IMapperSession<BusinessActivity> _businessActivityRepository;

        public BusinessActivityService(IMapperSession<BusinessActivity> businessActivityRepository)
        {
            _businessActivityRepository = businessActivityRepository;
        }

        public async Task AttachClientProgrammeToActivities(Programme programme, BusinessActivity businessActivity)
        {
            businessActivity.Programme = programme;
            await Update(businessActivity);
        }

        public async Task Update(BusinessActivity businessActivity)
        {
            await _businessActivityRepository.UpdateAsync(businessActivity);
        }

        public async Task CreateBusinessActivity(BusinessActivity businessActivity)
        {
            if (businessActivity.AnzsciCode != null || businessActivity.Description != null)
            {
                await _businessActivityRepository.AddAsync(businessActivity);
            }
        }

        public async Task<List<BusinessActivity>> GetBusinessActivities()
        {
            return await _businessActivityRepository.FindAll().OrderBy(ba => ba.AnzsciCode).ToListAsync();            
        }

        public async Task<List<BusinessActivity>> GetBusinessActivitiesByClassification(int classification)
        {
            var list = await GetBusinessActivities();
            return list.Where(ba => ba.Classification == classification).ToList();
        }

        public async Task<BusinessActivity> GetBusinessActivity(Guid Id)
        {
            return await _businessActivityRepository.GetByIdAsync(Id);
        }

        public async Task<BusinessActivity> GetBusinessActivitiesByClientProgramme(Guid programmeId)
        {
            return await _businessActivityRepository.FindAll().FirstOrDefaultAsync(t => t.Programme.Id == programmeId);
        }
    }
}
