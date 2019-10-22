using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;

namespace TechCertain.Services.Impl
{
    public class MilestoneTemplateService : IMilestoneTemplateService
    {
        IMapperSession<MilestoneTemplate> _milestoneTemplateRepository;

        public MilestoneTemplateService(IMapperSession<MilestoneTemplate> milestoneTemplateRepository)
        {
            _milestoneTemplateRepository = milestoneTemplateRepository;
        }

        public async Task<MilestoneTemplate> GetMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity)
        {
            MilestoneTemplate milestoneTemplate = await _milestoneTemplateRepository.FindAll().FirstOrDefaultAsync(m => m.ClientProgramme == clientprogrammeID && m.Activity == milestoneActivity);
            if (milestoneTemplate == null)
            {
                milestoneTemplate = await CreateMilestoneTemplate(clientprogrammeID, milestoneActivity);
            }
            return milestoneTemplate;
        }

        public async Task<MilestoneTemplate> CreateMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity)
        {
            MilestoneTemplate milestoneTemplate = new MilestoneTemplate();
            milestoneTemplate.ClientProgramme = clientprogrammeID;
            milestoneTemplate.Activity = milestoneActivity;
            milestoneTemplate.Templates = new List<string>();
            milestoneTemplate.Templates.Add("Agreement Status - Not Started");
            milestoneTemplate.Templates.Add("Agreement Status - Started");
            milestoneTemplate.Templates.Add("Agreement Status - Completed");
            milestoneTemplate.Templates.Add("Agreement Status - Quoted");
            milestoneTemplate.Templates.Add("Agreement Status - Declined");
            milestoneTemplate.Templates.Add("Agreement Status - Bound and Waiting Payment");
            milestoneTemplate.Templates.Add("Agreement Status - Bound and Waiting Invoice");
            
            await _milestoneTemplateRepository.AddAsync(milestoneTemplate);

            return milestoneTemplate;
        }
    }
}
