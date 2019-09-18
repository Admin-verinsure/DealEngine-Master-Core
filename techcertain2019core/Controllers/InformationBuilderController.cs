using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using techcertain2019core.Models.ViewModels;

namespace techcertain2019core.Controllers
{
    public class InformationBuilderController : BaseController
    {
        private IInformationBuilderService _informationBuilderService;
		IMapper _mapper;
		IRepository<InformationTemplate> _templateRepository;
        IRepository<InformationItem> _informationItemRepository;
        IRepository<InformationSection> _informationSectionRepository;


        IUnitOfWorkFactory _unitOfWork;

        //public InformationBuilderController(IInformationBuilderService informationBuilderService)
		public InformationBuilderController(IUserService userService, IMapper mapper, IRepository<InformationSection> informationSectionRepository, IRepository<InformationItem> informationItemRepository, IRepository<InformationTemplate> templateRepository, IUnitOfWorkFactory unitOfWork)
			: base(userService)
        {
            _informationBuilderService = new InformationBuilderService(new InformationBuilderFactory());
			_mapper = mapper;
			_templateRepository = templateRepository;
            _informationItemRepository = informationItemRepository;
            _informationSectionRepository = informationSectionRepository;
            _unitOfWork = unitOfWork;
        }

        // GET: InformationBuilder
        [HttpGet]
        public ActionResult Index()
        {
            return View(new InformationBuilderViewModel());
        }        

		[HttpGet]
        public PartialViewResult _QuestionsPartialView()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateInformationSheet(InformationBuilderViewModel model)
        {
            // Should be abstracted to a service

            InformationBuilder builder;

            if (!model.Id.HasValue)
                builder = _informationBuilderService.CreateNewInformation(model.Name);
            else
                throw new Exception("The Id contained a value should have been null");

            //Mapper.CreateMap<InformationBuilder, InformationBuilderViewModel>();
            //Mapper.CreateMap<InformationBuilderViewModel, InformationBuilder>();

            model = _mapper.Map<InformationBuilder, InformationBuilderViewModel>(builder);

            return PartialView("_QuestionsPartialView", model);
        }

		[HttpGet]
		public ActionResult StagingBuilder ()
		{
			_templateRepository.GetById (new Guid ("95e8d973-4516-4e34-892a-a8be00f8ef3f"));


			return View (new ExperimentalInfoBuilderViewModel());
		}

        //[HttpGet]
        //public ActionResult SectionBuilder()
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
		public ActionResult StagingBuilder (ExperimentalInfoBuilderViewModel model)
		{
            //Console.WriteLine ("Title: " + model.Title);
            //Console.WriteLine ("Description: " + model.Description);
            //Console.WriteLine ("Pages: " + model.Pages.Count ());
            //foreach (var page in model.Pages) {
            //	Console.WriteLine ("  Questions: " + page.Questions.Count ());
            //	foreach (var question in page.Questions) {
            //		Console.WriteLine ("    QuestionType: " + question.QuestionType);
            //		Console.WriteLine ("    EditorId: " + question.EditorId);
            //		Console.WriteLine ("    Required: " + question.Required);
            //		Console.WriteLine ("    ReferUnderWriting: " + question.ReferUnderWriting);
            //		Console.WriteLine ("    NeedsReview: " + question.NeedsReview);
            //		Console.WriteLine ("    Question: " + question.QuestionTitle);
            //		Console.WriteLine ("    HorizontalLayout: " + question.HorizontalLayout);
            //		Console.WriteLine ("    Options: " + question.OptionsArray);
            //		Console.WriteLine ("    ----------");
            //	}
            //}
            //Console.WriteLine ("Conditionals: " + model.Conditionals.Count ());
            //foreach (var conditional in model.Conditionals) {
            //	Console.WriteLine ("  Conditional for: " + conditional.QuestionId);
            //	Console.WriteLine ("  Triggers on value: " + conditional.TriggerValue);
            //	Console.WriteLine ("  Show/Hide: " + (conditional.Visibility == 1 ? "Show" : "Hide"));
            //	Console.WriteLine ("  Targets: " + string.Join (", ", conditional.Controls));
            //}
            try
            {

                InformationTemplate informationTemplate = new InformationTemplate (CurrentUser, model.Title, null);
    
			foreach (var page in model.Pages) {
				InformationSection section = new InformationSection (CurrentUser, page.Title, null);

				for (int i = 0; i < page.Questions.Count(); i++) {
					var question = page.Questions.ElementAt (i);
					InformationItem item = null;
					string randomName = System.IO.Path.GetRandomFileName ().Replace (".", "");
					switch (question.QuestionType) {
					case "text":
						item = new TextboxItem (CurrentUser, randomName, question.QuestionTitle, 10, "TEXTBOX");
						break;
					case "radiobutton":

						break;
					case "dropdown":
						List<DropdownListOption> ddOptions = new List<DropdownListOption> ();
						ddOptions.Add (new DropdownListOption (CurrentUser, "-- Select --", ""));
						for (int j = 0; j < question.OptionsArray.Length; j++)
							ddOptions.Add (new DropdownListOption (CurrentUser, question.OptionsArray [j], j.ToString()));
						item = new DropdownListItem (CurrentUser, randomName, question.QuestionTitle, 10, "DROPDOWNLIST", ddOptions ,"" );
						break;
					case "mvRegPanelTemplate":
						section.CustomView = "ICIBHianzMotor";
						break;
					case "mvUnRegPanelTemplate":
						section.CustomView = "ICIBHianzPlant";
						break;
					default:
						throw new Exception ("Unable to map element (" + question.QuestionType + ")");
					}
					item.EditorId = question.EditorId;
					item.ItemOrder = i;
					// set flags
					if (item != null) {
						item.NeedsReview = question.NeedsReview;
						item.ReferUnderwriting = question.ReferUnderWriting;
						item.Required = question.Required;
                        item.NeedsMilestone = question.NeedsMilestone;
					}

					section.AddItem (item);
				}
				informationTemplate.AddSection (section);
			}

			//var items = informationTemplate.Sections.SelectMany (s => s.Items);
			//foreach (var item in items) {
			//	var editorConditional = model.Conditionals.FirstOrDefault (a => a.QuestionId.EndsWith(item.EditorId, StringComparison.CurrentCulture));
			//	if (editorConditional != null) {
			//		item.Conditional = new InformationItemConditional {
			//			TriggerValue = editorConditional.TriggerValue,
			//			VisibilityOnTrigger = editorConditional.Visibility,
			//			Targets = items.Where (c => editorConditional.Controls.Contains (c.EditorId)).ToList ()
			//		};
			//	}
			//}

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    _templateRepository.Add(informationTemplate);
                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = true });

            }


			return Json (new { Result = true });
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