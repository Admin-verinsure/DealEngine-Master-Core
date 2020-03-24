﻿using System;
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
        //Task<BusinessActivityTemplate> GetBusinessActivitiesTemplate(BusinessActivityTemplate businessActivity);
        Task<BusinessActivity> GetBusinessActivity(Guid Id);
        Task<BusinessActivityTemplate> GetBusinessActivityTemplate(Guid Id);
        Task AttachClientProgrammeToActivities(Programme programme, BusinessActivityTemplate businessActivity);
        Task<BusinessActivity> GetBusinessActivityByCode(string AnzsciCode);
        Task UpdateBusinessActivity(BusinessActivity businessActivity);
        Task CreateBusinessActivityTemplate(BusinessActivityTemplate businessActivity);
        Task CreateBusinessActivity(BusinessActivity newBusinessActivity);
        Task<BusinessActivityTemplate> GetBusinessActivityTemplateByCode(string anzsciCode);
        Task RemoveBusinessActivity(BusinessActivityTemplate businessActivityTemplate);
    }
    
}
