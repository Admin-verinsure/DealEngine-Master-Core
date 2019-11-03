using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Collections.Generic;

namespace TechCertain.Services.Impl
{
    public class MilestoneService : IMilestoneService
    {
        IMapperSession<Milestone> _milestoneRepository;
        ISystemEmailService _systemEmailRepository;
        //ITaskingService _taskingService;
        IProgrammeProcessService _programmeProcessService;
        IActivityService _activityService;

        public MilestoneService(IMapperSession<Milestone> milestoneRepository,
                                ISystemEmailService systemEmailService,
                                IProgrammeProcessService programmeProcessService,
                                IActivityService activityService
                                //ITaskingService taskingService,
                                )
        {
            _activityService = activityService;
            _programmeProcessService = programmeProcessService;
            _milestoneRepository = milestoneRepository;
            _systemEmailRepository = systemEmailService;
            //_taskingService = taskingService;
        }

        public async Task<Milestone> CreateMilestone(User createdBy, string programmeProcessName, string activityName, Programme programme)
        {
            var programmeProcess = await _programmeProcessService.GetProcess(programmeProcessName);
            var activity = await _activityService.GetActivity(activityName);

            Milestone milestone = new Milestone(createdBy);
            milestone.ProgrammeProcess = programmeProcess;
            milestone.Activity = activity;
            milestone.HasTriggered = false;
            milestone.Programme = programme;
            await _milestoneRepository.AddAsync(milestone);

            return milestone;
        }

        public async Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, string template)
        {

            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentNullException(nameof(template));
            if (string.IsNullOrWhiteSpace(emailContent))
                throw new ArgumentNullException(nameof(emailContent));

            SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType(template).Result;
            if (systemEmailTemplate != null)
            {
                await _systemEmailRepository.AddNewSystemEmail(user, milestone.Activity.Name, null, subject, emailContent, template);
            }
            else
            {
                //update function
            }

            milestone.EmailTemplates.Add(systemEmailTemplate);
            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task CreateAdvisory(Milestone milestone, string advisory)
        {
            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (string.IsNullOrWhiteSpace(advisory))
                throw new ArgumentNullException(nameof(advisory));

            milestone.Advisory = new Advisory(advisory);
            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task CreateUserTask(Milestone milestone, UserTask userTask)
        {
            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (userTask == null)
                throw new ArgumentNullException(nameof(userTask));

            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task CloseMileTask(Guid id, string method)
        {
            Milestone milestone = await _milestoneRepository.GetByIdAsync(id);
            milestone.Advisory.Method = method;
            milestone.HasTriggered = true;
            milestone.Task.IsActive = true;

            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task<Milestone> GetMilestoneProcess(Guid programmeId, ProgrammeProcess programmeProcess, Activity activity)
        {
            return await _milestoneRepository.FindAll().FirstOrDefaultAsync(m => m.Programme.Id == programmeId && m.ProgrammeProcess == programmeProcess
                                                                                            && m.Activity == activity);
        }

    }
}
