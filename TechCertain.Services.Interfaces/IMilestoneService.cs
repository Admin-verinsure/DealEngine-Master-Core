using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IMilestoneService
    {
        Task<Milestone> CreateMilestone(User createdBy, Guid programmeProcessId, Guid activityId, Programme programmeId);
        Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, string template);
        Task CreateAdvisory(Milestone milestone, string advisory);
        Task CreateUserTask(Milestone milestone, UserTask userTask);
        Task<Milestone> GetMilestoneProcess(Guid programmeId, ProgrammeProcess programmeProcess, Activity activity);
        Task CloseMileTask(Guid id, string method);
        Task<Milestone> GetMilestoneActivity(string activity);
        Task<List<Milestone>> GetMilestones(Guid programmeId);
    }
    
}
