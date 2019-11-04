using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IMilestoneTemplateService
    {
        Task<MilestoneTemplate> GetMilestoneTemplate(User user);
        Task<MilestoneTemplate> CreateMilestoneTemplate(User user);
    }
}