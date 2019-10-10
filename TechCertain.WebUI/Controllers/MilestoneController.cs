
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models.Milestone;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.WebUI.Models;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Controllers
{
    //[Authorize]
    public class MilestoneController : BaseController
    {

        IUWMService _uWMService;
        //ITaskingService _taskingService;
        IEmailService _emailService;
        IMilestoneService _milestoneService;
        IProgrammeService _programmeService;
        IMapperSession<Programme> _programmeRepository;
        //IPaymentGatewayService _paymentGatewayService;
        IHttpContextAccessor _httpContextAccessor;

        public MilestoneController(
            IUserService userRepository,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            IProgrammeService programmeService,
            IMapperSession<Programme> programmeRepository,
            IUWMService uWMService,
            DealEngineDBContext dealEngineDBContext,
            SignInManager<DealEngineUser> signInManager,
            IMilestoneService milestoneService)
            : base (userRepository)
        {
            _programmeService = programmeService;
            _programmeRepository = programmeRepository;
            _uWMService = uWMService;
            //_taskingService = taskingService;
            _milestoneService = milestoneService;
            _emailService = emailService;
        }

        [HttpGet]
        public ActionResult MilestoneList()
        {
            string[] events = new[] { "Process New Agreement", "Change Agreement", "Process Renewal Agreement", "Process Cancel Agreement" };

            MilestoneListViewModel model = new MilestoneListViewModel
            {
                MilestoneVM = new List<MilestoneConfigurationViewModel>(),                
            };

            foreach(string milestone in events)
            {
                MilestoneConfigurationViewModel milestoneViewModel = new MilestoneConfigurationViewModel();
                milestoneViewModel.Process = milestone;
                IList<Programme> programmes = new List<Programme>();
                var avaliableProgrammes = _programmeRepository.FindAll().Where(p => p.IsPublic == true || p.Owner.Id == CurrentUser.PrimaryOrganisation.Id);

                if (CurrentUser.PrimaryOrganisation.IsTC)
                {
                    avaliableProgrammes = _programmeRepository.FindAll().Where(d => !d.DateDeleted.HasValue);
                }

                foreach (Programme programme in avaliableProgrammes)
                {
                    programmes.Add(programme);

                }

                milestoneViewModel.Programmes = programmes;
                model.MilestoneVM.Add(milestoneViewModel);
            }
           
            return View(model);
        }


        [HttpPost]
        public ActionResult CreateMilestone (MilestoneListViewModel listModel)
        {
            IList<string> emailTo = new List<string>();

            Programme programme = _programmeService.GetProgramme(Guid.Parse(listModel.ProgrammeId));            
            emailTo.Add(programme.BrokerContactUser.Address);            

            var templates = _milestoneService.GetMilestoneTemplate(programme.Id, listModel.MilestoneActivity);

            MilestoneTemplateVM milestoneTemplateVM = new MilestoneTemplateVM
            {
                Activity = templates.Activity,
                Templates = templates.Templates
            };

            var priorityTypes = new List<SelectListItem> {
                    new SelectListItem { Text = "Important", Value = "1" },
                    new SelectListItem { Text = "Critical", Value = "2" },
            };

            MilestoneBuilderViewModel model = new MilestoneBuilderViewModel()
            {
                UserTasks = new List<UserTask>(),
                Actions = new List<string>(),
                EmailTemplates = new List<EmailTemplate>(),
                Advisories = new List<string>(),
                UserTask = new UserTaskVM(),
                EmailTemplate = new EmailTemplateVM(),
                MilestoneTemplate = milestoneTemplateVM,
                EmailAddresses = emailTo,
                AdvisoryContent = new AdvisoryVM(),
                Priorities = priorityTypes,
                ProgrammeId = programme.Id,
            };

            TempData["MilestoneBuilderViewModel"] = model;
            return Json(data: model);
        }

        [HttpGet]
        public ActionResult MilestoneBuilder()
        {
            MilestoneBuilderViewModel model = (MilestoneBuilderViewModel)TempData["MilestoneBuilderViewModel"];
            return View("MilestoneBuilder", model);
        }


        [HttpPost]
        public ActionResult SubmitMilestone(MilestoneBuilderViewModel model)
        {

            Programme programme = _programmeService.GetProgramme(model.ProgrammeId);
            Milestone milestone = _milestoneService.CreateMilestone(CurrentUser, "ProgrammeChange", "Quoted", programme);

            if (model.EmailTemplate.Body != null)
            {
                _milestoneService.CreateEmailTemplate(CurrentUser, milestone, model.EmailTemplate.Subject, System.Net.WebUtility.HtmlDecode(model.EmailTemplate.Body), model.Type);
            }

            if (model.AdvisoryContent.Advisory != null)
            {
                _milestoneService.CreateAdvisory(milestone, System.Net.WebUtility.HtmlDecode(model.AdvisoryContent.Advisory));
            }

            if (model.UserTask.DueDate != null)
            {
                UserTask userTask = new UserTask(CurrentUser, CurrentUser.PrimaryOrganisation, "", model.UserTask.DueDate)
                {
                    Priority = model.UserTask.Priority,
                    Description = model.UserTask.Description,
                    Details = model.UserTask.Details,
                    IsActive = false,
                };

                _milestoneService.CreateUserTask(milestone, userTask);
            }
            return null;
        
        }
        
    }
}
