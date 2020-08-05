
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.Tasking;
using System;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models.Milestone;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
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
        IApplicationLoggingService _applicationLoggingService;
        ILogger<MilestoneController> _logger;

        public MilestoneController(
            ILogger<MilestoneController> logger,
            IApplicationLoggingService applicationLoggingService,
            IAdvisoryService advisoryService,
            ISystemEmailService systemEmailService,
            IUserService userRepository,
            IProgrammeService programmeService,
            ITaskingService taskingService,
            IMilestoneService milestoneService,
            IActivityService activityService,
            IMilestoneTemplateService milestoneTemplateService,
            IProgrammeProcessService programmeProcess)
            : base(userRepository)
        {
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _systemEmailService = systemEmailService;
            _advisoryService = advisoryService;
            _activityService = activityService;
            _programmeService = programmeService;
            _taskingService = taskingService;
            _milestoneService = milestoneService;
            _milestoneTemplateService = milestoneTemplateService;
            _programmeProcess = programmeProcess;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                //var templates = await _milestoneTemplateService.GetMilestoneTemplate(user);
                var programmeList = await _programmeService.GetAllProgrammes();
                MilestoneViewModel model = new MilestoneViewModel(programmeList);

                
                //var avaliableProgrammes = programmeList.Where(p => p.IsPublic == true || p.Owner.Id == user.PrimaryOrganisation.Id);

                //if (user.PrimaryOrganisation.IsTC)
                //{
                //    avaliableProgrammes = programmeList.Where(d => !d.DateDeleted.HasValue);
                //}

                //IList<Programme> programmes = new List<Programme>();
                //foreach (Programme programme in avaliableProgrammes)
                //{
                //    programmes.Add(programme);
                //}

                //foreach (var programmeProcesses in templates.ProgrammeProcesses)
                //{
                //    MilestoneConfigurationViewModel milestoneViewModel = new MilestoneConfigurationViewModel();
                //    milestoneViewModel.ProgrammeProcess = programmeProcesses;
                //    milestoneViewModel.Programmes = programmes;
                //    model.MilestoneVM.Add(milestoneViewModel);
                //}

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMilestone(IFormCollection collection)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Guid.TryParse(collection["MilestoneViewModel.Programme"].ToString(), out Guid ProgrammeId);
                string programmeProcess = collection["MilestoneViewModel.ProgrammeProcesses"].ToString();
                Programme programme = await _programmeService.GetProgramme(ProgrammeId);
                Milestone milestone = await _milestoneService.GetMilestoneProgrammeId(programme.Id);
                if(milestone == null)
                {
                    milestone = new Milestone(user, programme);
                }
                var ProgrammeProcess = milestone.ProgrammeProcesses.FirstOrDefault(p => p.Name == programmeProcess);
                if(ProgrammeProcess == null)
                {
                    ProgrammeProcess = new ProgrammeProcess(user, programmeProcess);
                    milestone.ProgrammeProcesses.Add(ProgrammeProcess);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> MilestoneTemplate(string strProgrammeId, string strProgrammeProcessId)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));

                MilestoneBuilderViewModel model = new MilestoneBuilderViewModel()
                {
                    ProgrammeId = programme.Id,
                    ProgrammeProcessId = strProgrammeProcessId,
                };

                return Content("/Milestone/MilestoneBuilder/?strProgrammeId=" + programme.Id + "&strProgrammeProcessId=" + strProgrammeProcessId);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> MilestoneBuilder(string strProgrammeId, string strProgrammeProcessId)
        //{
        //    User user = null;

        //    try
        //    {
        //        user = await CurrentUser();
        //        Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));
        //        var templates = await _milestoneTemplateService.GetMilestoneTemplate(user);

        //        MilestoneTemplateVM milestoneTemplateVM = new MilestoneTemplateVM
        //        {
        //            Activities = templates.Activities,
        //            ProgrammeProcesses = templates.ProgrammeProcesses
        //        };

        //        MilestoneBuilderViewModel model = new MilestoneBuilderViewModel();
        //        model.MilestoneTemplate = milestoneTemplateVM;
        //        model.ProgrammeId = programme.Id;
        //        model.ProgrammeProcessId = strProgrammeProcessId;

        //        return View("MilestoneBuilder", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        return RedirectToAction("Error500", "Error");
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> MilestoneSelect(string strProgrammeId, string strMilestoneActivityId, string strProgrammeProcessId, string type)
        //{
        //    User user = null;

        //    try
        //    {
        //        user = await CurrentUser();
        //        Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));
        //        MilestoneBuilderViewModel model = new MilestoneBuilderViewModel();
        //        model.ProgrammeId = programme.Id;
        //        model.ProgrammeProcessId = strProgrammeProcessId;
        //        model.ActivityId = strMilestoneActivityId;

        //        return Content("/Milestone/MilestoneType/?strProgrammeId=" + strProgrammeId + "&strMilestoneActivityId=" + model.ActivityId
        //            + "&strProgrammeProcessId=" + model.ProgrammeProcessId + "&type=" + type);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        return RedirectToAction("Error500", "Error");
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> MilestoneType(string strProgrammeId, string strMilestoneActivityId, string strProgrammeProcessId, string type)
        //{
        //    IList<string> emailTo = new List<string>();
        //    User user = null;

        //    try
        //    {
        //        user = await CurrentUser();
        //        Programme programme = await _programmeService.GetProgramme(Guid.Parse(strProgrammeId));

        //        var emailUsers = new List<SelectListItem> {
        //            new SelectListItem { Text = "Email Broker User", Value = "Broker" },
        //            new SelectListItem { Text = "Email Insurer User", Value = "Insurer" },
        //    };

        //        var priorityTypes = new List<SelectListItem> {
        //            new SelectListItem { Text = "Important", Value = "1" },
        //            new SelectListItem { Text = "Critical", Value = "2" },
        //    };

        //        var programmeProcess = await _programmeProcess.GetProcessId(Guid.Parse(strProgrammeProcessId));
        //        var milestoneActivity = await _activityService.GetActivityId(Guid.Parse(strMilestoneActivityId));

        //        MilestoneBuilderViewModel model = new MilestoneBuilderViewModel();
        //        model.UserTask = new UserTaskVM();
        //        model.EmailTemplate = new EmailTemplateVM();
        //        model.EmailAddresses = emailUsers;
        //        model.AdvisoryContent = new AdvisoryVM();
        //        model.Priorities = priorityTypes;
        //        model.ProgrammeId = programme.Id;
        //        model.ProgrammeProcessId = programmeProcess.Id.ToString();

        //        if (type == "Advisory")
        //        {
        //            model.IsAdvisory = true;
        //        }
        //        else if (type == "Email")
        //        {
        //            model.IsEmail = true;
        //        }
        //        else
        //            model.IsUserTask = true;

        //        var milestone = await _milestoneService.GetMilestoneProgrammeId(programme.Id);

        //        if (milestone != null)
        //        {
        //            var advisoryList = await _advisoryService.GetAdvisorysByMilestone(milestone);
        //            var advisory = advisoryList.LastOrDefault(a => a.Activity.Name == milestoneActivity.Name && a.DateDeleted == null);

        //            var systemEmailTemplateList = await _systemEmailService.GetEmailTemplatesByMilestone(milestone);
        //            var systemEmailTemplate = systemEmailTemplateList.LastOrDefault(s => s.Activity == milestoneActivity && s.DateDeleted == null);
        //            var userTaskList = await _taskingService.GetUserTasksByMilestone(milestone);
        //            var userTask = userTaskList.LastOrDefault(t => t.Activity == milestoneActivity && t.DateDeleted == null);
        //            if (advisory != null)
        //            {
        //                model.AdvisoryContent.Advisory = advisory.Description;
        //            }
        //            if (systemEmailTemplate != null)
        //            {
        //                model.EmailTemplate.Subject = systemEmailTemplate.Subject;
        //                model.EmailTemplate.Body = systemEmailTemplate.Body;
        //            }
        //            if (userTask != null)
        //            {
        //                model.UserTask.Details = userTask.Details;
        //                model.UserTask.Description = userTask.Description;
        //            }
        //        }

        //        return PartialView("_MilestoneTypeList", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        return RedirectToAction("Error500", "Error");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SubmitMilestone(MilestoneBuilderViewModel model)
        {
            Milestone milestone;
            User user = null;

            try
            {
                user = await CurrentUser();
                Programme programme = await _programmeService.GetProgramme(model.ProgrammeId);
                Activity activity = await _activityService.GetActivityId(Guid.Parse(model.ActivityId));
                milestone = await _milestoneService.GetMilestoneProgrammeId(model.ProgrammeId);

                if (milestone == null)
                {
                    milestone = await _milestoneService.CreateMilestone(user, Guid.Parse(model.ProgrammeProcessId), Guid.Parse(model.ActivityId), programme);
                }

                //if (model.EmailTemplate != null)
                //{
                //    await _milestoneService.CreateEmailTemplate(user, milestone, model.EmailTemplate.Subject, System.Net.WebUtility.HtmlDecode(model.EmailTemplate.Body), Guid.Parse(model.ActivityId), Guid.Parse(model.ProgrammeProcessId));
                //}

                if (model.AdvisoryContent != null)
                {
                    await _milestoneService.CreateAdvisory(user, milestone, activity, System.Net.WebUtility.HtmlDecode(model.AdvisoryContent.Advisory));
                }

                //if (model.UserTask != null)
                //{
                //    var dateDue = model.UserTask.DueDate;
                //    DateTime date = DateTime.Now;
                //    var milestoneDueDate = date.AddDays(dateDue);
                //    await _milestoneService.CreateMilestoneUserTask(user, user.PrimaryOrganisation, milestoneDueDate, milestone, activity,
                //        model.UserTask.Priority, model.UserTask.Description, model.UserTask.Details);
                //}

                return Ok();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

    }
}
