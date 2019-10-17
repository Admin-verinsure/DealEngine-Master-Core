using System;
using System.Linq;
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

        public void AttachClientProgrammeToActivities(Programme programme, BusinessActivity businessActivity)
        {
            businessActivity.Programme = programme;
            Update(businessActivity);
        }

        public void Update(BusinessActivity businessActivity)
        {
            _businessActivityRepository.UpdateAsync(businessActivity);
        }

        public void CreateBusinessActivity(BusinessActivity businessActivity)
        {
            if (businessActivity.AnzsciCode != null || businessActivity.Description != null)
            {
                _businessActivityRepository.AddAsync(businessActivity);
            }
        }

        public IQueryable<BusinessActivity> GetBusinessActivities()
        {
            return _businessActivityRepository.FindAll().OrderBy(ba => ba.AnzsciCode);
        }

        public IQueryable<BusinessActivity> GetBusinessActivitiesByClassification(int classification)
        {
            return GetBusinessActivities().Where(ba => ba.Classification == classification);
        }

        public BusinessActivity GetBusinessActivity(Guid Id)
        {
            return _businessActivityRepository.GetByIdAsync(Id).Result;
        }

        public object GetBusinessActivitiesByClientProgramme(Guid programmeId)
        {
            return _businessActivityRepository.FindAll().Where(t => t.Programme.Id == programmeId);
        }
    }
}
