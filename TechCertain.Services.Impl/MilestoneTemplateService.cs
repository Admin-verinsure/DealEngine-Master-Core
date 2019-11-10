using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class MilestoneTemplateService : IMilestoneTemplateService
    {
        IMapperSession<MilestoneTemplate> _milestoneTemplateRepository;

        public MilestoneTemplateService(IMapperSession<MilestoneTemplate> milestoneTemplateRepository)
        {
            _milestoneTemplateRepository = milestoneTemplateRepository;
        }

        public async Task<MilestoneTemplate> GetMilestoneTemplate(User user)
        {
            MilestoneTemplate milestoneTemplate = await _milestoneTemplateRepository.FindAll().FirstOrDefaultAsync(m => m.CreatedBy == user);
            if (milestoneTemplate == null)
            {
                milestoneTemplate = await CreateMilestoneTemplate(user);
            }
            return milestoneTemplate;
        }

        public async Task<MilestoneTemplate> CreateMilestoneTemplate(User user)
        {
            MilestoneTemplate milestoneTemplate = new MilestoneTemplate(user);
            string[] programmeProcess = new[] { "Process New Agreement", "Change Agreement", "Process Renewal Agreement", "Process Cancel Agreement" };
            string[] activity = new[] { "Agreement Status - Not Started", "Agreement Status - Started", "Agreement Status - Completed", "Agreement Status - Quoted", "Agreement Status – Referred", "Agreement Status - Declined", "Agreement Status - Bound and Waiting Payment", "Agreement Status - Bound and Waiting Invoice" };

            var programmeProcessList = new List<ProgrammeProcess>();
            foreach(string pp in programmeProcess)
            {
                var process = new ProgrammeProcess(user)
                {
                    Name = pp
                };

                programmeProcessList.Add(process);
            }

            var activityList = new List<Activity>();
            foreach (string act in activity)
            {
                var active = new Activity(user)
                {
                    Name = act
                };

                activityList.Add(active);
            }

            milestoneTemplate.Activities = activityList;
            milestoneTemplate.ProgrammeProcesses = programmeProcessList;
            await _milestoneTemplateRepository.AddAsync(milestoneTemplate);

            return milestoneTemplate;
        }
    }
}
