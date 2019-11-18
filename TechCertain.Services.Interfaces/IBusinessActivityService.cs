using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBusinessActivityService
    {
        Task<List<BusinessActivity>> GetBusinessActivitiesByClassification (int classification);
        Task<List<BusinessActivity>> GetBusinessActivities();
        Task CreateBusinessActivity(BusinessActivity businessActivity);
        Task<BusinessActivity> GetBusinessActivity(Guid Id);
        Task AttachClientProgrammeToActivities(Programme programme, BusinessActivity businessActivity);
        Task<BusinessActivity> GetBusinessActivitiesByClientProgramme(Guid programmeId);
    }
    
}
