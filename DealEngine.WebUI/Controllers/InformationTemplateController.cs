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
        IInformationItemService _informationItemService;
        IInformationTemplateService _informationTemplateService;
        ILogger<InformationTemplateController> _logger;
        IApplicationLoggingService _applicationLoggingService;
        IInformationSectionService _informationSectionService;
        IProductService _productService;

		public InformationTemplateController(
            IInformationItemService informationItemService,
            IProductService productService,
            IInformationSectionService informationSectionService,
            IInformationTemplateService informationTemplateService,
            IUserService userService,
            ILogger<InformationTemplateController> logger,
            IApplicationLoggingService applicationLoggingService
            )
			: base (userService)
        {
            _informationItemService = informationItemService;
            _productService = productService;
            _informationSectionService = informationSectionService;
            _applicationLoggingService = applicationLoggingService;
            _logger = logger;
            _informationTemplateService = informationTemplateService;			
        }

        [HttpPost]
        public async Task<IActionResult> CreateConditionalItem(IFormCollection form)
        {

            User user = null;
            try
            {
                user = await CurrentUser();
                var itemId = form["ItemId"];
                var conditionalform = form["Form"].ToString();
                var formsplit = conditionalform.Split('&').ToList();
                var item = await _informationItemService.GetItemById(Guid.Parse(itemId));

                InformationItemConditional itemConditional = null;
                List<DropdownListOption> ddOptions = new List<DropdownListOption>();
                string randomName = System.IO.Path.GetRandomFileName().Replace(".", "");
                if (formsplit.Contains("questiontype=textTemplate"))
                {
                    var question = formsplit.ElementAt(4);
                    var split = question.Split('=');
                    itemConditional = (InformationItemConditional)await _informationItemService.CreateTextboxItem(user, randomName, split[1], 10, "TEXTBOX");
                }
                else if (formsplit.Contains("questiontype=dropdownTemplate"))
                {
                    var question = formsplit.ElementAt(4);
                    var split = question.Split('=');
                    ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                    var options = form["dropdownvalue"].ToList();
                    for (int j = 0; j < options.Count; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(options.ElementAt(j)))
                        {
                            ddOptions.Add(new DropdownListOption(user, options.ElementAt(j), j.ToString()));
                        }
                    }
                    itemConditional = (InformationItemConditional)await _informationItemService.CreateDropdownListItem(user, randomName, form["question"], item.Name, ddOptions, 10, "DROPDOWNLIST");
                }
                else if (formsplit.Contains("questiontype=yesNoTemplate"))
                {
                    var question = formsplit.ElementAt(4);
                    var split = question.Split('=');
                    ddOptions.Add(new DropdownListOption(user, "-- Select --", ""));
                    ddOptions.Add(new DropdownListOption(user, "Yes", "0"));
                    ddOptions.Add(new DropdownListOption(user, "No", "1"));
                    itemConditional = (InformationItemConditional)await _informationItemService.CreateDropdownListItem(user, randomName, form["question"], item.Name, ddOptions, 10, "DROPDOWNLIST");
                }

                try
                {
                    var on = form["questionreferunderwriting"];
                    itemConditional.ReferUnderwriting = true;
                }
                catch (Exception ex)
                {
                    itemConditional.ReferUnderwriting = false;
                }
                try
                {
                    var on = form["Required"];
                    itemConditional.Required = true;
                }
                catch (Exception ex)
                {
                    itemConditional.Required = false;
                }

                item.Conditional = itemConditional;

                await _informationItemService.UpdateItem(item);


            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return Json(new { Result = true });
            }

            return NoContent();
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
                    await _informationTemplateService.CreateInformationTemplate(informationTemplate);
                }

                InformationSection section = new InformationSection(user, title, null);
                var item = await _informationItemService.CreateItemFromForm(form, user, title);

                section.AddItem(item);
                await _informationSectionService.CreateNewSection(section);

                return Json(new { templateId = informationTemplate.Id, sectionId = section.Id, itemId = item.Id });
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
                for (int i = 0; i < sectionList.Count; i++)
                {
                    var section = await _informationSectionService.GetSection(Guid.Parse(sectionList.ElementAt(i)));
                    section.Name = templateName;
                    section.Position = i + 2;
                    informationTemplate.AddSection(section);
                }

                foreach (var existingId in existingTemplates)
                {
                    var template = await _informationTemplateService.GetTemplate(Guid.Parse(existingId));
                    if(template != null)
                    {
                        foreach (var section in template.Sections)
                        {
                            informationTemplate.AddSection(section);
                        }
                    }                    
                }

                informationTemplate.Name = templateName;
                var products = await _productService.GetAllProducts();
                var product = products.LastOrDefault();
                informationTemplate.Product = product;
                product.InformationTemplate = informationTemplate;
                await _informationTemplateService.UpdateInformationTemplate(informationTemplate);
                await _productService.UpdateProduct(product);

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