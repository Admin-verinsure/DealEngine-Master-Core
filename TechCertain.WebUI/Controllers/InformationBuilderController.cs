using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using TechCertain.WebUI.Models;


namespace TechCertain.WebUI.Controllers
{
    public class InformationBuilderController : BaseController
    {
        IInformationBuilderService _informationBuilderService;
		IMapper _mapper;		
        IInformationTemplateService _informationTemplateService;
        ILogger _logger;
        IApplicationLoggingService _applicationLoggingService;

		public InformationBuilderController(
            IInformationTemplateService informationTemplateService,
            IUserService userService,
            IMapper mapper,
            ILogger logger,
            IApplicationLoggingService applicationLoggingService
            )
			: base (userService)
        {
            _applicationLoggingService = applicationLoggingService;
            _logger = logger;
            _informationTemplateService = informationTemplateService;
            _informationBuilderService = new InformationBuilderService(new InformationBuilderFactory());
			_mapper = mapper;			
        }

        // GET: InformationBuilder
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new InformationBuilderViewModel());
        }        

		[HttpGet]
        public PartialViewResult _QuestionsPartialView()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInformationSheet(InformationBuilderViewModel model)
        {            
            InformationBuilder builder;
            User user = null;
            try
            {
                user = await CurrentUser();
                if (!model.Id.HasValue)
                    builder = _informationBuilderService.CreateNewInformation(model.Name);
                else
                    throw new Exception("The Id contained a value should have been null");

                model = _mapper.Map<InformationBuilder, InformationBuilderViewModel>(builder);

                return PartialView("_QuestionsPartialView", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpGet]
		public async Task<IActionResult> StagingBuilder ()
		{
			await _informationTemplateService.GetTemplate(new Guid ("95e8d973-4516-4e34-892a-a8be00f8ef3f"));

			return View (new ExperimentalInfoBuilderViewModel());
		}

        //[HttpGet]
        //public async Task<IActionResult> SectionBuilder()
        //{
        //    // //  _informationItemRepository.FindAll().Where(p => p.infor)
        //    InformationSection informationSection = _informationSectionRepository.GetById(new Guid("3b2ba8c1-48bc-4ec2-b8ef-aaa200bc5376"));
        //    InformationItemViewModel model = new InformationItemViewModel();
        //    var options = new List<SelectListItem>();
        //    //informationSection.InformationTemplate
        //    foreach (var item in informationSection.Items)
        //    {
        //        //if (item.Type == "TEXTBOX")
        //        //{
        //        options.Add(new SelectListItem { Text = item.Type, Value = "nz" });
        //        // }
        //    }
        //        model.Options = options;

        //        //     // _templateRepository.GetById(new Guid("95e8d973-4516-4e34-892a-a8be00f8ef3f"));

        //        return View(model);
        //}



        [HttpPost]
        public async Task<IActionResult> StagingBuilder(ExperimentalInfoBuilderViewModel model)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                InformationTemplate informationTemplate = new InformationTemplate(user, model.Title, null);

                foreach (var page in model.Pages)
                {
                    InformationSection section = new InformationSection(user, page.Title, null);

                    for (int i = 0; i < page.Questions.Count(); i++)
                    {
                        var question = page.Questions.ElementAt(i);
                        InformationItem item = null;
                        string randomName = System.IO.Path.GetRandomFileName().Replace(".", "");
                        string randomId = informationTemplate.Name;
                        randomId = randomId + question.QuestionTitle.Substring(question.QuestionTitle.Length - 5);
                        //var ques = question.QuestionTitle.Substring(question.QuestionTitle.Length - 6);
                        switch (question.QuestionType)
                        {
                            case "text":
                                item = new TextboxItem(user, randomName, question.QuestionTitle, randomId, 10, "TEXTBOX");
                                break;
                            case "radiobutton":

                                break;
                            case "dropdown":
                                List<DropdownListOption> ddOptions = new List<DropdownListOption>();
                                ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                                for (int j = 0; j < question.OptionsArray.Length; j++)
                                    ddOptions.Add(new DropdownListOption(user, question.OptionsArray[j], j.ToString()));
                                item = new DropdownListItem(user, randomName, question.QuestionTitle, randomId, 10, "DROPDOWNLIST", ddOptions, "");
                                break;
                            case "mvRegPanelTemplate":
                                section.CustomView = "ICIBHianzMotor";
                                break;
                            case "mvUnRegPanelTemplate":
                                section.CustomView = "ICIBHianzPlant";
                                break;
                            default:
                                throw new Exception("Unable to map element (" + question.QuestionType + ")");
                        }
                        item.EditorId = question.EditorId;
                        item.ItemOrder = i;
                        // set flags
                        if (item != null)
                        {
                            item.NeedsReview = question.NeedsReview;
                            item.ReferUnderwriting = question.ReferUnderWriting;
                            item.Required = question.Required;
                            item.NeedsMilestone = question.NeedsMilestone;
                        }

                        section.AddItem(item);
                        
                    }
                    informationTemplate.AddSection(section);
                }

                await _informationTemplateService.CreateInformationTemplate(user, informationTemplate.Name, informationTemplate.Sections);
                return Json(new { Result = true });

            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return Json(new { Result = true });
            }            
        }
    }

    /// <summary>
    /// Service Interface
    /// </summary>
    public interface IInformationBuilderService
    {
        InformationBuilder CreateNewInformation(string informationSheetName);
    }


    /// <summary>
    /// Service Implementation
    /// </summary>
    public class InformationBuilderService : IInformationBuilderService
    {
        private InformationBuilderFactory _informationBuilderFactory;

        public InformationBuilderService(InformationBuilderFactory informationBuilderFactory)
        {
            _informationBuilderFactory = informationBuilderFactory;
        }

        public InformationBuilder CreateNewInformation(string informationSheetName)
        {
            InformationBuilder builder = _informationBuilderFactory.Create(informationSheetName);

            return builder;
        }
    }

    /// <summary>
    /// Factory
    /// </summary>
    public class InformationBuilderFactory
    {
        public InformationBuilder Create(string informationNme)
        {
            return new InformationBuilder(informationNme);
        }
    }

    /// <summary>
    /// Entity
    /// </summary>
    public class InformationBuilder
    {
        public InformationBuilder(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; protected set; }

    }
}