using System;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBusinessActivityService
    {
        IQueryable<BusinessActivity> GetBusinessActivitiesByClassification (int classification);
        IQueryable<BusinessActivity> GetBusinessActivities();
        Task CreateBusinessActivity(BusinessActivity businessActivity);
        Task<BusinessActivity> GetBusinessActivity(Guid Id);
        Task AttachClientProgrammeToActivities(Programme programme, BusinessActivity businessActivity);
        Task<BusinessActivity> GetBusinessActivitiesByClientProgramme(Guid programmeId);
    }
    
}
