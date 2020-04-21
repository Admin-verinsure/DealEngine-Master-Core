using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IBusinessActivityService
    {
        Task<List<BusinessActivityTemplate>> GetBusinessActivitiesByClassification (int classification);
        Task<List<BusinessActivityTemplate>> GetBusinessActivitiesTemplates();
        Task<BusinessActivityTemplate> GetBusinessActivityTemplate(Guid Id);
        Task CreateBusinessActivityTemplate(BusinessActivityTemplate businessActivity);
        Task<BusinessActivity> CreateActivity(Guid guid);
    }
    
}
