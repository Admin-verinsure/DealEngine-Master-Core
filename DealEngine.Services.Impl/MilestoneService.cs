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
                var NotifyUsers = sheet.Programme.BaseProgramme.BrokerContactUser;
                var Advisory = ProgrammeProcesse.Activities.FirstOrDefault(a => a.Name == activityName).Advisory;
                if (!NotifyUsers.UserTasks.Any(t => t.Name.Contains(activityName + "_" + sheet.Id)))
                {
                    UserTask ReferralTask = new UserTask(user, activityName + "_" + sheet.Id, null)
                    {
                        URL = "/Agreement/ViewAcceptedAgreement/" + sheet.Programme.Id.ToString(),
                        Body = "UIS Referral: " + sheet.ReferenceId + " (" + sheet.Programme.BaseProgramme.Name + " - " + sheet.Programme.Owner.Name + ")"
                    };

                    NotifyUsers.UserTasks.Add(ReferralTask);
                    await _userService.Update(NotifyUsers);
                }                           
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
                    await ReferredComplete(activityName, user, sheet);
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
                Activity = new Activity(user, activity, collection, null);
                ProgrammeProcess.Activities.Add(Activity);
            }
            else
            {
                Activity.Advisory.PopulateEntity(collection);
            }

            await Update(milestone);
        }

        private async Task Update(Milestone milestone)
        {
            await _milestoneRepository.AddAsync(milestone);
        }

        private async Task ReferredComplete(string activityName, User user, ClientInformationSheet sheet)
        {
            //close task
            var UserTask = sheet.Programme.BaseProgramme.BrokerContactUser.UserTasks.FirstOrDefault(t => t.Name == activityName + "_" + sheet.Id && t.Completed == false);
            if (UserTask != null)
            {
                UserTask.Complete(user);
                await _taskingService.Update(UserTask);
            }
            //run email
            
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
                        //JsonObjects.Add("UserTask", Activity.UserTask);
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

        public async Task CreateJoinOrganisationTask(User user, User organisationUser, Programme programme)
        {
            string URL = "/Organisation/RejoinProgramme/?ProgrammeId=" + programme.Id.ToString() + "&OrganisationId=" + organisationUser.PrimaryOrganisation.Id.ToString();
            UserTask userTask = organisationUser.UserTasks.FirstOrDefault(t => t.URL == URL && t.IsActive == true);
            if (userTask == null)
            {
                userTask = new UserTask(user, "Rejoin", null)
                {
                    URL = URL,
                    Body = organisationUser.FirstName+ " click here to rejoin " + programme.Name,
                    IsActive = true
                };

                //var programmeUser = programme.BrokerContactUser;
                var programmeUser = user;
                programmeUser.UserTasks.Add(userTask);             
                await _userService.Update(programmeUser);
            }                   
        }

        public async Task CreateAttachOrganisationTask(User user, Programme programme, Organisation organisation)
        {            
            string URL = "/Organisation/RejoinProgramme/?ProgrammeId=" + programme.Id.ToString() + "&OrganisationId=" + organisation.Id.ToString();
            //var ProgrammeUser = programme.BrokerContactUser;
            var ProgrammeUser = user;
            UserTask userTask = ProgrammeUser.UserTasks.FirstOrDefault(t => t.URL == URL && t.IsActive == true);
            if (userTask != null)
            {
                userTask.Complete(ProgrammeUser);
                user.UserTasks.Remove(userTask);

                userTask = new UserTask(ProgrammeUser, "Attach", null)
                {
                    URL = "/Organisation/AttachOrganisation/?ProgrammeId=" + programme.Id.ToString() + "&OrganisationId=" + organisation.Id.ToString(),
                    Body = "click here to rejoin "+ organisation.Name+ " to "+ programme.Name,
                    IsActive = true
                };

                ProgrammeUser.UserTasks.Add(userTask);
                await _userService.Update(ProgrammeUser);   
            }
        }
    }
}
