using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBusinessActivityService
    {
        IQueryable<BusinessActivity> GetBusinessActivitiesByClassification (int classification);
        IQueryable<BusinessActivity> GetBusinessActivities();
        void CreateBusinessActivity(BusinessActivity businessActivity);
        BusinessActivity GetBusinessActivity(Guid Id);
        void AttachClientProgrammeToActivities(Programme programme, BusinessActivity businessActivity);
        object GetBusinessActivitiesByClientProgramme(Guid programmeId);
    }
    
}
