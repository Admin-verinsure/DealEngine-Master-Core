using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IMilestoneService
    {
        Task<Milestone> CreateMilestone(User createdBy, string programmeProcess, string activity, Programme programmeId);
        Task<Milestone> GetMilestone(string milestoneType);
        Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, string template);
        Task CreateAdvisory(Milestone milestone, string advisory);
        Task CreateUserTask(Milestone milestone, UserTask userTask);
        Task<MilestoneTemplate> GetMilestoneTemplate(Guid id, string milestoneActivity);
        Task<Milestone> GetMilestoneProcess(Guid programmeId, string programmeProcess, string Activity);
        Task CloseMileTask(Guid id, string method);
    }
    
}
