using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IMilestoneTemplateService
    {
        Task<MilestoneTemplate> GetMilestoneTemplate(User user);
        Task<MilestoneTemplate> CreateMilestoneTemplate(User user);
    }
}