using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBusinessActivityService
    {
        Task<List<BusinessActivityTemplate>> GetBusinessActivitiesByClassification (int classification);
        Task<List<BusinessActivityTemplate>> GetBusinessActivitiesTemplate();
        //Task<BusinessActivityTemplate> GetBusinessActivitiesTemplate(BusinessActivityTemplate businessActivity);
        Task<BusinessActivity> GetBusinessActivity(Guid Id);
        Task<BusinessActivityTemplate> GetBusinessActivityTemplate(Guid Id);
        Task AttachClientProgrammeToActivities(Programme programme, BusinessActivityTemplate businessActivity);
        Task<BusinessActivity> GetBusinessActivityByCode(string AnzsciCode);
        Task UpdateBusinessActivity(BusinessActivity businessActivity);
        Task CreateBusinessActivityTemplate(BusinessActivityTemplate businessActivity);
        Task CreateBusinessActivity(BusinessActivity newBusinessActivity);
        Task<BusinessActivityTemplate> GetBusinessActivityTemplateByCode(string anzsciCode);
    }
    
}
