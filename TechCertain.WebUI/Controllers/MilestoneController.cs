
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using System;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models.Milestone;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using NHibernate.Linq;
using TechCertain.WebUI.Helpers;

namespace TechCertain.WebUI.Controllers
{
    public class MilestoneController : BaseController
    {

        ITaskingService _taskingService;
        IMilestoneService _milestoneService;
        IActivityService _activityService;
        ISystemEmailService _systemEmailService;
        IAdvisoryService _advisoryService;
        IMilestoneTemplateService _milestoneTemplateService;
        IProgrammeProcessService _programmeProcess;
        IProgrammeService _programmeService;
        IMapperSession<Programme> _programmeRepository;

        public MilestoneController(
            IAdvisoryService advisoryService,
            ISystemEmailService systemEmailService,
            IUserService userRepository,
            IProgrammeService programmeService,
            IMapperSession<Programme> programmeRepository,
            ITaskingService taskingService,
            IMilestoneService milestoneService,
            IActivityService activityService,
            IMilestoneTemplateService milestoneTemplateService,
            IProgrammeProcessService programmeProcess)
            : base(userRepository)
        {
            _systemEmailService = systemEmailService;
            _advisoryService = advisoryService;
            _activityService = activityService;
            _programmeService = programmeService;
            _programmeRepository = programmeRepository;
            _taskingService = taskingService;
            _milestoneService = milestoneService;
            _milestoneTemplateService = milestoneTemplateService;
            _programmeProcess = programmeProcess;
        }



        [HttpGet]
        public async Task<IActionResult> MilestoneList()
        {
            var user = await CurrentUser();
            var templates = await _milestoneTemplateService.GetMilestoneTemplate(user);
            MilestoneListViewModel model = new MilestoneListViewModel
            {
                MilestoneVM = new List<MilestoneConfigurationViewModel>(),
            };

            var avaliableProgrammes = await _programmeRepository.FindAll().Where(p => p.IsPublic == true || p.Owner.Id == user.PrimaryOrganisation.Id).ToListAsync();

            if (user.PrimaryOrganisation.IsTC)
            {
                avaliableProgrammes = await _programmeRepository.FindAll().Where(d => !d.DateDeleted.HasValue).ToListAsync();
            }

            IList<Programme> programmes = new List<Programme>();
            foreach (Programme programme in avaliableProgrammes)
            {
                programmes.Add(programme);
            }

            foreach (var programmeProcesses in templates.ProgrammeProcesses)
            {
                MilestoneConfigurationViewModel milestoneViewModel = new MilestoneConfigurationViewModel();
                milestoneViewModel.ProgrammeProcess = programmeProcesses;
                milestoneViewModel.Programmes = programmes;
                model.MilestoneVM.Add(milestoneViewModel);
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> MilestoneTemplate(string strProgrammeId, string strProgrammeProcessId)
        {
            Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));

            MilestoneBuilderViewModel model = new MilestoneBuilderViewModel()
            {
                ProgrammeId = programme.Id,
                ProgrammeProcessId = strProgrammeProcessId,
            };

            return Content("/Milestone/MilestoneBuilder/?strProgrammeId=" + programme.Id + "&strProgrammeProcessId=" + strProgrammeProcessId);
        }

        [HttpGet]
        public async Task<IActionResult> MilestoneBuilder(string strProgrammeId, string strProgrammeProcessId)
        {
            var user = await CurrentUser();
            Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));
            var templates = await _milestoneTemplateService.GetMilestoneTemplate(user);

            MilestoneTemplateVM milestoneTemplateVM = new MilestoneTemplateVM
            {
                Activities = templates.Activities,
                ProgrammeProcesses = templates.ProgrammeProcesses
            };

            MilestoneBuilderViewModel model = new MilestoneBuilderViewModel();
            model.MilestoneTemplate = milestoneTemplateVM;
            model.ProgrammeId = programme.Id;
            model.ProgrammeProcessId = strProgrammeProcessId;

            return View("MilestoneBuilder", model);
        }

        [HttpPost]
        public async Task<IActionResult> MilestoneSelect(string strProgrammeId, string strMilestoneActivityId, string strProgrammeProcessId, string type)
        {
            Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));

            MilestoneBuilderViewModel model = new MilestoneBuilderViewModel();
            model.ProgrammeId = programme.Id;
            model.ProgrammeProcessId = strProgrammeProcessId;
            model.ActivityId = strMilestoneActivityId;


            return Content("/Milestone/MilestoneType/?strProgrammeId=" + strProgrammeId + "&strMilestoneActivityId=" + model.ActivityId
                + "&strProgrammeProcessId=" + model.ProgrammeProcessId + "&type=" + type);
        }

        [HttpGet]
        public async Task<IActionResult> MilestoneType(string strProgrammeId, string strMilestoneActivityId, string strProgrammeProcessId, string type)
        {
            IList<string> emailTo = new List<string>();
            Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));

            var emailUsers = new List<SelectListItem> {
                    new SelectListItem { Text = "Email Broker User", Value = "Broker" },
                    new SelectListItem { Text = "Email Insurer User", Value = "Insurer" },
            };

            var priorityTypes = new List<SelectListItem> {
                    new SelectListItem { Text = "Important", Value = "1" },
                    new SelectListItem { Text = "Critical", Value = "2" },
            };

            var programmeProcess = await _programmeProcess.GetProcessId(Guid.Parse(strProgrammeProcessId));
            var milestoneActivity = await _activityService.GetActivityId(Guid.Parse(strMilestoneActivityId));

            MilestoneBuilderViewModel model = new MilestoneBuilderViewModel();
            model.UserTask = new UserTaskVM();
            model.EmailTemplate = new EmailTemplateVM();
            model.EmailAddresses = emailUsers;
            model.AdvisoryContent = new AdvisoryVM();
            model.Priorities = priorityTypes;
            model.ProgrammeId = programme.Id;
            model.ProgrammeProcessId = programmeProcess.Id.ToString();

            if (type == "Advisory")
            {
                model.IsAdvisory = true;
            }
            else if (type == "Email")
            {
                model.IsEmail = true;
            }
            else
                model.IsUserTask = true;

            var milestone = await _milestoneService.GetMilestoneByBaseProgramme(programme.Id);

            if (milestone != null)
            {
                var advisory = await _advisoryService.GetAdvisoryByMilestone(milestone, milestoneActivity);
                var systemEmailTemplate = await _systemEmailService.GetEmailTemplateByMilestone(milestone, milestoneActivity);
                var userTask = await _taskingService.GetUserTaskByMilestone(milestone, milestoneActivity);
                if (advisory != null)
                {
                    model.AdvisoryContent.Advisory = advisory.Description;
                }
                if (systemEmailTemplate != null)
                {
                    model.EmailTemplate.Subject = systemEmailTemplate.Subject;
                    model.EmailTemplate.Body = systemEmailTemplate.Body;
                }
                if (userTask != null)
                {
                    model.UserTask.Details = userTask.Details;
                    model.UserTask.Description = userTask.Description;
                }
            }

            return PartialView("_MilestoneTypeList", model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitMilestone(MilestoneBuilderViewModel model)
        {
            var user = await CurrentUser();
            Milestone milestone;
            Programme programme = await _programmeService.GetProgramme(model.ProgrammeId);
            Activity activity = await _activityService.GetActivityId(Guid.Parse(model.ActivityId));
            milestone = await _milestoneService.GetMilestoneByBaseProgramme(model.ProgrammeId);

            if (milestone == null)
            {
                milestone = await _milestoneService.CreateMilestone(user, Guid.Parse(model.ProgrammeProcessId), Guid.Parse(model.ActivityId), programme);
            }

            if (model.EmailTemplate != null)
            {
                await _milestoneService.CreateEmailTemplate(user, milestone, model.EmailTemplate.Subject, System.Net.WebUtility.HtmlDecode(model.EmailTemplate.Body), Guid.Parse(model.ActivityId), Guid.Parse(model.ProgrammeProcessId));
            }

            if (model.AdvisoryContent != null)
            {
                await _milestoneService.CreateAdvisory(user, milestone, activity, System.Net.WebUtility.HtmlDecode(model.AdvisoryContent.Advisory));
            }

            if (model.UserTask != null)
            {
                var dateDue = model.UserTask.DueDate;
                DateTime date = DateTime.Now;
                var milestoneDueDate = date.AddDays(dateDue);
                await _milestoneService.CreateMilestoneUserTask(user, user.PrimaryOrganisation, milestoneDueDate, milestone, activity,
                    model.UserTask.Priority, model.UserTask.Description, model.UserTask.Details);
            }

            return Ok();
        }

    }
}
