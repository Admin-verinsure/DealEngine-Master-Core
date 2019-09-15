using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Impl
{
    public interface IMilestoneTemplateService
    {
        MilestoneTemplate GetMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity);
        MilestoneTemplate CreateMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity);
    }
}