using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Impl
{
    public interface IMilestoneTemplateService
    {
        Task<MilestoneTemplate> GetMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity);
        Task<MilestoneTemplate> CreateMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity);
    }
}