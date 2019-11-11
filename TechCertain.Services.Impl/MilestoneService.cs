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

        public MilestoneService(IMapperSession<Milestone> milestoneRepository,
                                ISystemEmailService systemEmailService,
                                IProgrammeProcessService programmeProcessService,
                                IActivityService activityService,
                                ITaskingService taskingService
                                )
        {
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
            milestone.ProgrammeProcess = programmeProcess;
            milestone.Activity = activity;
            milestone.HasTriggered = false;
            milestone.Programme = programme;
            await _milestoneRepository.AddAsync(milestone);

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
            else
            {
                await _systemEmailService.UpdateSystemEmailTemplate(systemEmailTemplate);
            }

            milestone.SystemEmailTemplate = systemEmailTemplate;
            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task CreateAdvisory(Milestone milestone, string advisory)
        {
            milestone.Advisory = new Advisory(advisory);
            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task CreateMilestoneUserTask(User createdBy, Organisation createdFor, DateTime dueDate, 
            Milestone milestone, int priority, string description, string details)
        {
            var userTask = await _taskingService.GetMilestoneTask(milestone.Id);
            if(userTask == null)
            {
                userTask = await _taskingService.CreateTaskForMilestone(createdBy, createdFor, dueDate, milestone);
            }

            userTask.Priority = priority;
            userTask.Description = description;
            userTask.Details = details;
            userTask.IsActive = false;
            userTask.DueDate = dueDate;
            await _taskingService.UpdateUserTask(userTask);

            milestone.UserTask = userTask;
            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task CloseMileTask(Guid id, string method)
        {
            Milestone milestone = await _milestoneRepository.GetByIdAsync(id);
            milestone.Method = method;
            milestone.HasTriggered = true;
            milestone.UserTask.IsActive = true;

            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task<Milestone> GetMilestoneProcess(Guid programmeId, ProgrammeProcess programmeProcess, Activity activity)
        {
            return await _milestoneRepository.FindAll().FirstOrDefaultAsync(m => m.Programme.Id == programmeId && m.ProgrammeProcess == programmeProcess
                                                                                            && m.Activity == activity);
        }

        public async Task<Milestone> GetMilestoneActivity(string activity)
        {
            return await _milestoneRepository.FindAll().FirstOrDefaultAsync(m => m.Activity.Name == activity);
        }

        public async Task<List<Milestone>> GetMilestones(Guid programmeId)
        {
            return await _milestoneRepository.FindAll().Where(m => m.Programme.Id == programmeId).ToListAsync();
        }

        public async Task UpdateMilestone(Milestone milestone)
        {
            await _milestoneRepository.UpdateAsync(milestone);
        }
    }
}
