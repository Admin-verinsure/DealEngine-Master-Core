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
        IInformationSectionService _informationSectionService;
        IProductService _productService;

		public InformationTemplateController(
            IProductService productService,
            IInformationSectionService informationSectionService,
            IInformationTemplateService informationTemplateService,
            IUserService userService,
            IMapper mapper,
            ILogger<InformationTemplateController> logger,
            IApplicationLoggingService applicationLoggingService
            )
			: base (userService)
        {
            _productService = productService;
            _informationSectionService = informationSectionService;
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
                InformationTemplate informationTemplate;
                if (!string.IsNullOrWhiteSpace(form["templatename"]))
                {
                    title = form["templatename"];
                }
                if (!string.IsNullOrWhiteSpace(form["templateId"]))
                {
                    informationTemplate = await _informationTemplateService.GetTemplate(Guid.Parse(form["templateId"]));
                }
                else
                {
                    informationTemplate = new InformationTemplate(user, title, null);
                    informationTemplate.Name = title;
                    //await _informationTemplateService.CreateInformationTemplate(informationTemplate);
                }

                InformationSection section = new InformationSection(user, title, null);                
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
                //await _informationSectionService.CreateNewSection(section);                

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
                var form = HttpContext.Request.Form;
                user = await CurrentUser();
                var templateId = form["templateId"];
                var templateName = form["templatename"];
                InformationTemplate informationTemplate;
                if (!string.IsNullOrWhiteSpace(templateId))
                {
                    informationTemplate = await _informationTemplateService.GetTemplate(Guid.Parse(templateId));
                }
                else
                {
                    informationTemplate = new InformationTemplate(user, "", null);
                }
                var existingTemplates = form["existingtemplates"].ToList();                 
                var sectionList = form["sectionId"].ToList();
                //for(int i = 0; i < sectionList.Count; i++)
                //{
                //    var section = await _informationSectionService.GetSection(Guid.Parse(sectionList.ElementAt(i)));
                //    section.Name = templateName;
                //    section.Position = i + 2;
                //    informationTemplate.AddSection(section);
                //}

                //foreach(var existingId in existingTemplates)
                //{
                //    var template = await _informationTemplateService.GetTemplate(Guid.Parse(existingId));
                //    foreach(var section in template.Sections)
                //    {
                //        informationTemplate.AddSection(section);
                //    }
                //}

                //informationTemplate.Name = templateName;
                var products = await _productService.GetAllProducts();
                var product = products.LastOrDefault();
                //product.InformationTemplate = informationTemplate;
                //await _informationTemplateService.UpdateInformationTemplate(informationTemplate);
                //await _productService.UpdateProduct(product);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return Json(new { Result = true });
            }            
        }
    }

}