﻿using System;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.Tasking;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Collections.Generic;

namespace DealEngine.Services.Impl
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
            var advisoryList = await _advisoryService.GetAdvisorysByMilestone(milestone);
            var advisory = advisoryList.FirstOrDefault(a => a.DateDeleted == null && a.Activity == activity);
            
            if (advisory != null)
            {
                advisory.DateDeleted = DateTime.Now;
                advisory.DeletedBy = user;
                await _advisoryService.UpdateAdvisory(advisory);
            }

            advisory = new Advisory(advisoryString)
            {
                Milestone = milestone,
                Activity = activity,
                Description = advisoryString
            };

            await _advisoryService.CreateAdvisory(advisory);
        }

        public async Task CreateMilestoneUserTask(User user, Organisation createdFor, DateTime dueDate, 
            Milestone milestone, Activity activity, int priority, string description, string details)
        {
            var userTaskList = await _taskingService.GetUserTasksByMilestone(milestone);
            var userTask = userTaskList.FirstOrDefault(t => t.DateDeleted == null && t.Activity == activity);
            if (userTask != null)
            {
                userTask.DateDeleted = DateTime.Now;
                userTask.DeletedBy = user;
                await _taskingService.UpdateUserTask(userTask);                
            }

            userTask = new UserTask(user, createdFor)
            {
                Description = description,
                Details = details,
                Milestone = milestone,
                Activity = activity
            };

            await _taskingService.CreateTask(userTask);
        }

        public async Task<Milestone> GetMilestoneByBaseProgramme(Guid programmeId)
        {
            return await _milestoneRepository.FindAll().FirstOrDefaultAsync(m => m.Programme.Id == programmeId);
        }

        public async Task UpdateMilestone(Milestone milestone)
        {
            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task SetMilestoneFor(string activityType, User user, ClientInformationSheet sheet)
        {
            if(activityType == "Agreement Status – Referred")
            {
                await ReferredMilestone(activityType, user, sheet);
            }
        }

        private async Task ReferredMilestone(string activityType, User user, ClientInformationSheet sheet)
        {
            var milestone = await GetMilestoneByBaseProgramme(sheet.Programme.BaseProgramme.Id);
            var activity = await _activityService.GetActivityByName(activityType);
            var task = new UserTask(user, user.PrimaryOrganisation);
            if (milestone == null)
            {
                milestone = new Milestone(user);
                milestone.Programme = sheet.Programme.BaseProgramme;
            }

            //task process      
            task.Milestone = milestone;
            task.Activity = activity;
            task.Details = "UIS Referral: " + sheet.ReferenceId + " (" + sheet.Programme.BaseProgramme.Name + " - " + sheet.Programme.Owner.Name + ")";
            task.Description = "/Agreement/ViewAcceptedAgreement/" + sheet.Programme.Id.ToString();

            await _taskingService.CreateTask(task);            
        }

        public async Task CompleteMilestoneFor(string activityType, User user, ClientInformationSheet sheet)
        {
            if (activityType == "Agreement Status – Referred")
            {
                await ReferredComplete(activityType, user, sheet);
            }
        }

        private async Task ReferredComplete(string activityType, User user, ClientInformationSheet sheet)
        {
            var milestone = await GetMilestoneByBaseProgramme(sheet.Programme.BaseProgramme.Id);
            var activity = await _activityService.GetActivityByName(activityType);
            var tasks = await _taskingService.GetUserTasksByMilestone(milestone);
            var task = tasks.FirstOrDefault(t => t.Activity == activity && t.Completed == false);

            task.Complete(user);
            await _taskingService.UpdateUserTask(task);
        }
    }
}
