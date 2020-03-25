using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.WebUI.Models;
using Microsoft.AspNetCore.Http;

namespace DealEngine.WebUI.Controllers
{
    public class InformationTemplateController : BaseController
    {
		IMapper _mapper;		
        IInformationTemplateService _informationTemplateService;
        ILogger<InformationTemplateController> _logger;
        IApplicationLoggingService _applicationLoggingService;

		public InformationTemplateController(
            IInformationTemplateService informationTemplateService,
            IUserService userService,
            IMapper mapper,
            ILogger<InformationTemplateController> logger,
            IApplicationLoggingService applicationLoggingService
            )
			: base (userService)
        {
            _applicationLoggingService = applicationLoggingService;
            _logger = logger;
            _informationTemplateService = informationTemplateService;
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

        //[HttpPost]
        //public async Task<IActionResult> CreateInformationSheet(InformationBuilderViewModel model)
        //{            
        //    InformationBuilder builder;
        //    User user = null;
        //    try
        //    {
        //        user = await CurrentUser();
        //        if (!model.Id.HasValue)
        //            builder = _informationBuilderService.CreateNewInformation(model.Name);
        //        else
        //            throw new Exception("The Id contained a value should have been null");

        //        model = _mapper.Map<InformationBuilder, InformationBuilderViewModel>(builder);

        //        return PartialView("_QuestionsPartialView", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        return RedirectToAction("Error500", "Error");
        //    }
        //}

		[HttpGet]
		public async Task<IActionResult> StagingBuilder ()
		{
			await _informationTemplateService.GetTemplate(new Guid ("95e8d973-4516-4e34-892a-a8be00f8ef3f"));

			return View (new ExperimentalInfoBuilderViewModel());
		}

        [HttpPost]
        public async Task<IActionResult> CreateSection(IFormCollection form)
        {
            User user = null;
            string title = "_Template";
            try
            {                
                user = await CurrentUser();

                if (!string.IsNullOrWhiteSpace(form["templatename"]))
                {
                    title = form["templatename"];
                }

                InformationTemplate informationTemplate = new InformationTemplate(user, title, null);
                var template = Guid.NewGuid();
                informationTemplate.Id = template;
                InformationSection section = new InformationSection(user, title, null);
                var sectionId = Guid.NewGuid();
                section.Id = sectionId;
                InformationItem item;
                List<DropdownListOption> ddOptions = new List<DropdownListOption>();
                string randomName = System.IO.Path.GetRandomFileName().Replace(".", "");
                switch (form["questiontype"])
                {
                    case "textTemplate":
                        item = new TextboxItem(user, randomName, form["question"], title, 10, "TEXTBOX");
                        break;
                    case "yesNoTemplate":                        
                        ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                        ddOptions.Add(new DropdownListOption(user, "Yes", "1"));
                        ddOptions.Add(new DropdownListOption(user, "No", "2"));
                        item = new DropdownListItem(user, randomName, form["question"], title, 10, "DROPDOWNLIST", ddOptions, "");
                        break;
                    case "dropdownTemplate":
                        ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                        var options = form["dropdownvalue"].ToList();
                        for(int j = 0; j < options.Count; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(options.ElementAt(j)))
                            {
                                ddOptions.Add(new DropdownListOption(user, options.ElementAt(j), j.ToString()));
                            }
                        }
                        item = new DropdownListItem(user, randomName, form["question"], title, 10, "DROPDOWNLIST", ddOptions, "");
                        break;
                    default:
                        throw new Exception("Unable to map element (" + form["questiontype"] + ")");
                }
                try
                {
                    var on = form["questionreferunderwriting"];
                    item.ReferUnderwriting = true;
                }
                catch (Exception ex)
                {
                    item.ReferUnderwriting = false;
                }
                try
                {
                    var on = form["Required"];
                    item.Required = true;
                }
                catch (Exception ex)
                {
                    item.Required = false;
                }

                return Json(new { templateId = informationTemplate.Id, sectionId = section.Id }); 
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return Json(new { Result = true });
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateInformationTemplate(InformationBuilderViewModel model)
        {
            User user = null;
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return View(form);
                //}
                user = await CurrentUser();
                //InformationTemplate informationTemplate = new InformationTemplate(user, model.Title, null);

                //foreach (var page in model.Pages)
                //{
                //    InformationSection section = new InformationSection(user, page.Title, null);

                //    for (int i = 0; i < page.Questions.Count(); i++)
                //    {
                //        var question = page.Questions.ElementAt(i);
                //        InformationItem item = null;
                //        string randomName = System.IO.Path.GetRandomFileName().Replace(".", "");
                //        string randomId = informationTemplate.Name;
                //        randomId = randomId + question.QuestionTitle.Substring(question.QuestionTitle.Length - 5);
                //        //var ques = question.QuestionTitle.Substring(question.QuestionTitle.Length - 6);
                //        switch (question.QuestionType)
                //        {
                //            case "text":
                //                item = new TextboxItem(user, randomName, question.QuestionTitle, randomId, 10, "TEXTBOX");
                //                break;
                //            case "radiobutton":

                //                break;
                //            case "dropdown":
                //                List<DropdownListOption> ddOptions = new List<DropdownListOption>();
                //                ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                //                for (int j = 0; j < question.OptionsArray.Length; j++)
                //                    ddOptions.Add(new DropdownListOption(user, question.OptionsArray[j], j.ToString()));
                //                item = new DropdownListItem(user, randomName, question.QuestionTitle, randomId, 10, "DROPDOWNLIST", ddOptions, "");
                //                break;
                //            case "mvRegPanelTemplate":
                //                section.CustomView = "ICIBHianzMotor";
                //                break;
                //            case "mvUnRegPanelTemplate":
                //                section.CustomView = "ICIBHianzPlant";
                //                break;
                //            default:
                //                throw new Exception("Unable to map element (" + question.QuestionType + ")");
                //        }
                //        item.EditorId = question.EditorId;
                //        item.ItemOrder = i;
                //        // set flags
                //        if (item != null)
                //        {
                //            item.NeedsReview = question.NeedsReview;
                //            item.ReferUnderwriting = question.ReferUnderWriting;
                //            item.Required = question.Required;
                //            item.NeedsMilestone = question.NeedsMilestone;
                //        }

                //        section.AddItem(item);
                        
                //    }
                //    informationTemplate.AddSection(section);
                //}

                //await _informationTemplateService.CreateInformationTemplate(user, informationTemplate.Name, informationTemplate.Sections);
                return Json(new { Result = true });

            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return Json(new { Result = true });
            }            
        }
    }

}