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
        Task CreateAdvisory(Milestone milestone, string advisory);
        Task CreateMilestoneUserTask(User createdBy, Organisation createdFor, DateTime dueDate, Milestone milestone, int priority, string description, string details);
        Task<Milestone> GetMilestoneProcess(Guid programmeId, ProgrammeProcess programmeProcess, Activity activity);
        Task CloseMileTask(Guid id, string method);
        Task<Milestone> GetMilestoneActivity(string activity);
        Task<List<Milestone>> GetMilestones(Guid programmeId);
        Task UpdateMilestone(Milestone milestone);
    }
    
}
