using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IMilestoneService
    {
        Task<Milestone> CreateMilestone(User createdBy, Guid programmeProcessId, Guid activityId, Programme programmeId);
        Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, Guid activityId, Guid programmeProcessId);
        Task CreateAdvisory(User user, Milestone milestone, Activity activity, string advisory);
        Task CreateMilestoneUserTask(User user, Organisation createdFor, DateTime dueDate, Milestone milestone, Activity activity, int priority, string description, string details);
        Task<Milestone> GetMilestoneByBaseProgramme(Guid programmeId);
        Task UpdateMilestone(Milestone milestone);
        Task SetMilestoneFor(string activityType, User user, ClientInformationSheet sheet);
        Task CompleteMilestoneFor(string activityType, User user, ClientInformationSheet sheet);
    }
    
}
