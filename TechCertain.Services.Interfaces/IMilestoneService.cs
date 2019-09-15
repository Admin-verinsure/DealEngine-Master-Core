using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IMilestoneService
    {
        Milestone CreateMilestone(User createdBy, string programmeProcess, string activity, Programme programmeId);
        Milestone GetMilestone(string milestoneType);
        void CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, string template);
        void CreateAdvisory(Milestone milestone, string advisory);
        void CreateUserTask(Milestone milestone, UserTask userTask);
        MilestoneTemplate GetMilestoneTemplate(Guid id, string milestoneActivity);
        Milestone GetMilestoneProcess(Guid programmeId, string programmeProcess, string Activity);
        Task CloseMileTask(Guid id, string method);
    }
    
}
