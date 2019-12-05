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
        ISystemEmailService _systemEmailService;
        ITaskingService _taskingService;
        IProgrammeProcessService _programmeProcessService;
        IActivityService _activityService;
        IAdvisoryService _advisoryService;

        public MilestoneService(IMapperSession<Milestone> milestoneRepository,
                                IAdvisoryService advisoryService,
                                ISystemEmailService systemEmailService,
                                IProgrammeProcessService programmeProcessService,
                                IActivityService activityService,
                                ITaskingService taskingService
                                )
        {
            _advisoryService = advisoryService;
            _activityService = activityService;
            _programmeProcessService = programmeProcessService;
            _milestoneRepository = milestoneRepository;
            _systemEmailService = systemEmailService;
            _taskingService = taskingService;
        }

        public async Task<Milestone> CreateMilestone(User createdBy, Guid programmeProcessId, Guid activityId, Programme programme)
        {
            var programmeProcess = await _programmeProcessService.GetProcessId(programmeProcessId);
            var activity = await _activityService.GetActivityId(activityId);

            Milestone milestone = new Milestone(createdBy);
            milestone.HasTriggered = false;
            milestone.Programme = programme;
            await _milestoneRepository.AddAsync(milestone);

            activity.Milestone = milestone;
            await _activityService.UpdateActivity(activity);

            programmeProcess.Milestone = milestone;
            await _programmeProcessService.UpdateProgrammeProcess(programmeProcess);

            return milestone;
        }

        public async Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, Guid activityId, Guid programmeProcessId)
        {
            var activity = await _activityService.GetActivityId(activityId);
            var programmeProcess = await _programmeProcessService.GetProcessId(programmeProcessId);
            SystemEmail systemEmailTemplate = await _systemEmailService.GetSystemEmailByType(activity.Name);
            if (systemEmailTemplate == null)
            {
                systemEmailTemplate = new SystemEmail(user, activity.Name, "", subject, emailContent, programmeProcess.Name);
                await _systemEmailService.AddNewSystemEmail(user, activity.Name, "", subject, emailContent, programmeProcess.Name);
            }
            //else
            //{
            //    systemEmailTemplate.DateDeleted = DateTime.Now;
            //    systemEmailTemplate.DeletedBy = user;
            //    await _systemEmailService.UpdateSystemEmailTemplate(systemEmailTemplate);
            //}


            systemEmailTemplate.Milestone = milestone;
            systemEmailTemplate.Activity = activity;
            await _systemEmailService.UpdateSystemEmailTemplate(systemEmailTemplate);
        }

        public async Task CreateAdvisory(User user, Milestone milestone, Activity activity, string advisoryString)
        {
            Advisory advisory = await _advisoryService.GetAdvisoryByMilestone(milestone, activity);
            if (advisory == null)
            {
                advisory = new Advisory(advisoryString);                
                await _advisoryService.CreateAdvisory(advisory);
            }
            //else
            //{
            //    advisory.DateDeleted = DateTime.Now;
            //    advisory.DeletedBy = user;

            //}

            advisory.Milestone = milestone;
            advisory.Activity = activity;
            await _advisoryService.UpdateAdvisory(advisory);
        }

        public async Task CreateMilestoneUserTask(User createdBy, Organisation createdFor, DateTime dueDate, 
            Milestone milestone, Activity activity, int priority, string description, string details)
        {
            var userTask = await _taskingService.GetUserTaskByMilestone(milestone, activity);
            if(userTask == null)
            {
                userTask = await _taskingService.CreateTaskForMilestone(createdBy, createdFor, dueDate, milestone);
            }

            userTask.Priority = priority;
            userTask.Description = description;
            userTask.Details = details;
            userTask.IsActive = false;
            userTask.DueDate = dueDate;
            userTask.Milestone = milestone;
            userTask.Activity = activity;
            await _taskingService.UpdateUserTask(userTask);
        }

        public async Task CloseMileTask(Guid id, string method)
        {
            Milestone milestone = await _milestoneRepository.GetByIdAsync(id);
            milestone.Method = method;
            milestone.HasTriggered = true;

            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task<Milestone> GetMilestoneByBaseProgramme(Guid programmeId)
        {
            return await _milestoneRepository.FindAll().FirstOrDefaultAsync(m => m.Programme.Id == programmeId);
        }

        public async Task UpdateMilestone(Milestone milestone)
        {
            await _milestoneRepository.UpdateAsync(milestone);
        }
    }
}
