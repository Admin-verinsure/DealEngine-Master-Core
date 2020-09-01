using System;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using System.Threading.Tasks;
using NHibernate.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DealEngine.Services.Impl
{
    public class MilestoneService : IMilestoneService
    {
        IMapperSession<Milestone> _milestoneRepository;
        IProgrammeService _programmeService;
        ITaskingService _taskingService;
        IUserService _userService;

        public MilestoneService(
            IUserService userService,
            IMapperSession<Milestone> milestoneRepository,
            IProgrammeService programmeService,
            ITaskingService taskingService
            )
        {
            _userService = userService;
            _taskingService = taskingService;
            _programmeService = programmeService;
            _milestoneRepository = milestoneRepository;
        }

        public async Task CreateMilestone(string Type)
        {
            if(Type == "Rejoin")
            {
                await CreateReJoinMilestone();
            }
        }

        private Task CreateReJoinMilestone()
        {
            throw new NotImplementedException();
        }

        public async Task CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, Guid activityId, Guid programmeProcessId)
        {
            //var activity = await _activityService.GetActivityId(activityId);
            //var programmeProcess = await _programmeProcessService.GetProcessId(programmeProcessId);
            //SystemEmail systemEmailTemplate = await _systemEmailService.GetSystemEmailByType(activity.Name);
            //if (systemEmailTemplate == null)
            //{
            //    systemEmailTemplate = new SystemEmail(user, activity.Name, "", subject, emailContent, programmeProcess.Name);
            //    await _systemEmailService.AddNewSystemEmail(user, activity.Name, "", subject, emailContent, programmeProcess.Name);
            //}

            //systemEmailTemplate.Milestone = milestone;
            //systemEmailTemplate.Activity = activity;
            //await _systemEmailService.UpdateSystemEmailTemplate(systemEmailTemplate);
        }

        public async Task<Milestone> GetMilestoneProgrammeId(Guid programmeId)
        {
            return await _milestoneRepository.FindAll().FirstOrDefaultAsync(m => m.Programme.Id == programmeId);
        }

        public async Task<string> SetMilestoneFor(string activityName, User user, ClientInformationSheet sheet)
        {
            var milestone = await GetMilestoneProgrammeId(sheet.Programme.BaseProgramme.Id);
            string Discription = "";
            if (milestone != null)
            {
                if (activityName == "Agreement Status - Not Started")
                {
                    Discription = await NotStartedMilestone(activityName, user, milestone);
                    await CompleteMilestoneFor("Agreement Status - Not Started", user, sheet);
                }
                if (activityName == "Agreement Status - Declined")
                {
                    Discription = await DeclinedMilestone(activityName, user, milestone);                    
                }
                if (activityName == "Agreement Status – Referred")
                {
                    Discription = await ReferredMilestone(activityName, user, milestone, sheet);
                }
            }
            return Discription;
        }

        private async Task<string> DeclinedMilestone(string activityName, User user, Milestone milestone)
        {
            var ProgrammeProcesse = milestone.ProgrammeProcesses.FirstOrDefault(p => p.Activities.Any(a => a.Name == activityName));
            if (ProgrammeProcesse != null)
            {
                //run task
                //run email
                return ProgrammeProcesse.Activities.FirstOrDefault(a => a.Name == activityName).Advisory.Description;
            }
            return "";
        }

        private async Task<string> NotStartedMilestone(string activityName, User user, Milestone milestone)
        {
            var ProgrammeProcesse = milestone.ProgrammeProcesses.FirstOrDefault(p => p.Activities.Any(a => a.Name == activityName));
            if (ProgrammeProcesse != null)
            {
                //run task
                //run email
                return ProgrammeProcesse.Activities.FirstOrDefault(a => a.Name == activityName).Advisory.Description;
            }
            return "";
        }

        private async Task<string> ReferredMilestone(string activityName, User user, Milestone milestone, ClientInformationSheet sheet)
        {
            var ProgrammeProcesse = milestone.ProgrammeProcesses.FirstOrDefault(p => p.Activities.Any(a => a.Name == activityName));
            if (ProgrammeProcesse != null)
            {
                //run task
                var Advisory = ProgrammeProcesse.Activities.FirstOrDefault(a => a.Name == activityName).Advisory;
                var UserTask = ProgrammeProcesse.Activities.FirstOrDefault(a => a.Name == activityName).UserTask;
                if (string.IsNullOrWhiteSpace(UserTask.URL))
                {
                    UserTask.URL = "/Agreement/ViewAcceptedAgreement/" + sheet.Programme.Id.ToString();
                    UserTask.Body = "UIS Referral: " + sheet.ReferenceId + " (" + sheet.Programme.BaseProgramme.Name + " - " + sheet.Programme.Owner.Name + ")";                    
                }
                sheet.Programme.BaseProgramme.AgreementReferNotifyUsers
                await _userService.AssignTaskToUser(user, UserTask);
                                
                //run email
                return Advisory.Description;
            }
            return "";
        }

        public async Task CompleteMilestoneFor(string activityName, User user, ClientInformationSheet sheet)
        {
            var milestone = await GetMilestoneProgrammeId(sheet.Programme.BaseProgramme.Id);
            if(milestone!= null)
            {
                if (activityName == "Agreement Status - Not Started")
                {
                    await NotStartedCompleted(activityName, user, sheet);
                }
                if (activityName == "Agreement Status – Referred")
                {
                    await ReferredComplete(activityName, user, milestone);
                }
            }

        }

        private async Task NotStartedCompleted(string activityName, User user, ClientInformationSheet sheet)
        {
            if(!sheet.ClientInformationSheetAuditLogs.Any(l=>l.AuditLogDetail.Contains(activityName)))
            {
                string log = "User: " + user.UserName + " closed " + activityName + " Advisory on " + DateTime.Now;
                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(user, sheet, null, log));
            }
        }

        public async Task CreateMilestone(User user, IFormCollection collection)
        {
            Guid.TryParse(collection["MilestoneViewModel.Programme"].ToString(), out Guid ProgrammeId);
            string programmeProcess = collection["MilestoneViewModel.ProgrammeProcesses"].ToString();
            string activity = collection["MilestoneViewModel.Activity"].ToString();
            Programme programme =  await _programmeService.GetProgramme(ProgrammeId);
            Milestone milestone =  await GetMilestoneProgrammeId(programme.Id);
            if (milestone == null)
            {
                milestone = new Milestone(user, programme);
            }
            var ProgrammeProcess = milestone.ProgrammeProcesses.FirstOrDefault(p => p.Name == programmeProcess);
            if (ProgrammeProcess == null)
            {
                ProgrammeProcess = new ProgrammeProcess(user, programmeProcess);
                milestone.ProgrammeProcesses.Add(ProgrammeProcess);
            }
            var Activity = ProgrammeProcess.Activities.FirstOrDefault(a => a.Name == activity);
            if (Activity == null)
            {
                Activity = new Activity(user, activity, collection);
                ProgrammeProcess.Activities.Add(Activity);
            }

            await Update(milestone);
        }

        private async Task Update(Milestone milestone)
        {
            await _milestoneRepository.AddAsync(milestone);
        }

        private async Task ReferredComplete(string activityName, User user, Milestone milestone)
        {
            //close task
            var ProgrammeProcesse = milestone.ProgrammeProcesses.FirstOrDefault(p => p.Activities.Any(a => a.Name == activityName));
            if (ProgrammeProcesse != null)
            {
                //run task
                var UserTask = user.UserTasks.FirstOrDefault(t => t.Name == activityName && t.Completed == false);                
                if(UserTask != null)
                {
                    UserTask.Complete(user);
                    _taskingService.Update(UserTask);
                }
                //run email
            }
        }

        public async Task<string> GetMilestone(IFormCollection collection)
        {
            Dictionary<string, object> JsonObjects = new Dictionary<string, object>();
            Guid.TryParse(collection["MilestoneViewModel.Programme"].ToString(), out Guid ProgrammeId);
            string programmeProcess = collection["MilestoneViewModel.ProgrammeProcesses"].ToString();
            string activity = collection["MilestoneViewModel.Activity"].ToString();
            Milestone Milestone = await GetMilestoneProgrammeId(ProgrammeId);
            if (Milestone != null)
            {
                var ProgrammeProcess = Milestone.ProgrammeProcesses.FirstOrDefault(p => p.Name == programmeProcess);
                if (ProgrammeProcess != null)
                {
                    var Activity = ProgrammeProcess.Activities.FirstOrDefault(a => a.Name == activity);
                    if (Activity != null)
                    {
                        JsonObjects.Add("Advisory", Activity.Advisory);
                        JsonObjects.Add("UserTask", Activity.UserTask);
                        return GetSerializedModel(JsonObjects);
                    }
                }
            }
            return string.Empty;
        }

        public string GetSerializedModel(object model)
        {
            try
            {
                return JsonConvert.SerializeObject(model,
                    new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        FloatFormatHandling = FloatFormatHandling.DefaultValue,
                        DateParseHandling = DateParseHandling.DateTime
                    });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public Task DeveloperTool()
        {
            //var list = _taskingService.GetAllActiveTasks();
            throw new NotImplementedException();

        }
    }
}
