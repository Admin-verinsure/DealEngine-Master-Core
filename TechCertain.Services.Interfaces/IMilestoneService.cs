using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IMilestoneService
    {
        Task<Milestone> CreateMilestone(User createdBy, Guid programmeProcessId, Guid activityId, Programme programmeId);
        Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, Guid activityId, Guid programmeProcessId);
        Task CreateAdvisory(Milestone milestone, Activity activity, string advisory);
        Task CreateMilestoneUserTask(User createdBy, Organisation createdFor, DateTime dueDate, Milestone milestone, Activity activity, int priority, string description, string details);
        Task CloseMileTask(Guid id, string method);
        Task<Milestone> GetMilestoneByBaseProgramme(Guid programmeId);
        Task UpdateMilestone(Milestone milestone);
    }
    
}
