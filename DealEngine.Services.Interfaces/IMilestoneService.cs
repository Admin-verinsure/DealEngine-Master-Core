using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Services.Interfaces
{
    public interface IMilestoneService
    {
        Task<Milestone> CreateMilestone(User createdBy, Guid programmeProcessId, Guid activityId, Programme programmeId);
        Task CreateMilestone(string type);
        Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, Guid activityId, Guid programmeProcessId);
        //Task CreateAdvisory(User user, Milestone milestone, Activity activity, string advisory);
        //Task CreateMilestoneUserTask(User user, Organisation createdFor, DateTime dueDate, Milestone milestone, Activity activity, int priority, string description, string details);
        Task<Milestone> GetMilestoneProgrammeId(Guid programmeId);
        //Task UpdateMilestone(Milestone milestone);
        //Task<string> SetMilestoneFor(string activityName, User user, ClientInformationSheet sheet);
        Task CompleteMilestoneFor(string activityType, User user, ClientInformationSheet sheet);
        Task CreateMilestone(User user, IFormCollection collection);
        Task<string> SetMilestoneFor(string activity, User user, ClientInformationSheet sheet);
        Task<string> GetMilestone(IFormCollection collection);
    }
    
}
