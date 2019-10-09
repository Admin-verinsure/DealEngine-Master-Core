using System;
using TechCertain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Proposal;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Controllers
{
    public class ProposalController : BaseController
    {
        IProposalBuilderService _proposalBuilderService;
        IHttpContextAccessor _httpContextAccessor;
        public ProposalController(IUserService userRepository, 
            DealEngineDBContext dealEngineDBContext, IHttpContextAccessor httpContextAccessor, SignInManager<DealEngineUser> signInManager) 
            : base(userRepository, dealEngineDBContext, signInManager, httpContextAccessor)

        {

        }


		[HttpGet]
        public ActionResult Index()
        {
            return View();
        }

		[HttpGet]
        public ActionResult ProposalBuilder()
        {
            ProposalBuilderViewModel model = new ProposalBuilderViewModel();

            return View(model);
        }

        [HttpPost]
        public JsonResult CreateProposal(CreateProposalTemplateViewModel createProposalTemplateViewModel)
        {
            if (createProposalTemplateViewModel.Id == Guid.Empty)
            {
                //Organisation organisation = CurrentUser.Organisations.First();

                //ProposalTemplate proposalTemplate = _proposalTemplateFactory.CreateProposalTemplate(CurrentUser as Owner, createProposalTemplateViewModel.Name, false, organisation);

                //createProposalTemplateViewModel.Id = proposalTemplate.Id;

                createProposalTemplateViewModel.Id = Guid.NewGuid();
            }

            return Json(createProposalTemplateViewModel);
        }

        //public JsonResult AddQuestion(ProposalBuilderViewModel proposalViewModel, string label)
        //{
        //    ProposalTemplate proposalTemplate = _proposalBuilderService.GetProposalTemplate(proposalViewModel.Id);

        //    // use first section, we will reorder later.
        //    var proposalsection = proposalTemplate.Sections.First();

        //    proposalTemplate.AddQuestion(proposalsection, label);



        //    AddQuestionToProposalTemplate(proposalTemplate.Id, proposalQuestion);

        //    return Json(proposalViewModel);
        //}


        //   public class ProposalController : BaseController
        //{
        //       private readonly IProposalBuilderService _proposalBuilderService;

        //       List<ProposalViewAllViewModel.ProposalItem> proposalItems = new List<ProposalViewAllViewModel.ProposalItem>();

        //       public ProposalController(IAccountService userService, IProposalBuilderService proposalBuilderService) : base(userService) 
        //       {
        //           _proposalBuilderService = proposalBuilderService;

        //           //SetUpItems_Guid();
        //       }

        //       //public void SetUpItems_Int()
        //       //{
        //       //    ProposalViewAllViewModel.ProposalItem section1 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 1,
        //       //        Name = "Section 1",
        //       //        Label = "Section 1",
        //       //        Type = "Section"
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem section2 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 2,
        //       //        Name = "Section 2",
        //       //        Label = "Section 2",
        //       //        Type = "Section"
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem section3 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 3,
        //       //        Name = "Section 3",
        //       //        Label = "Section 3",
        //       //        Type = "Section"
        //       //    };

        //       //    ProposalViewAllViewModel.ProposalItem element11 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 4,
        //       //        Name = "Element 1.1",
        //       //        Label = "Element 1.1",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element12 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 5,
        //       //        Name = "Element 1.2",
        //       //        Label = "Element 1.2",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element13 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 6,
        //       //        Name = "Element 1.3",
        //       //        Label = "Element 1.3",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element14 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = 7,
        //       //        Name = "Element 1.4",
        //       //        Label = "Element 1.4",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };

        //       //    ProposalViewAllViewModel.ProposalItem element21 = new ProposalViewAllViewModel.ProposalItem() { Id = 8, Name = "Element 2.1", Label = "Element 2.1", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element22 = new ProposalViewAllViewModel.ProposalItem() { Id = 9, Name = "Element 2.2", Label = "Element 2.2", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element23 = new ProposalViewAllViewModel.ProposalItem() { Id = 10, Name = "Element 2.3", Label = "Element 2.3", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element24 = new ProposalViewAllViewModel.ProposalItem() { Id = 11, Name = "Element 2.4", Label = "Element 2.4", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };

        //       //    ProposalViewAllViewModel.ProposalItem element31 = new ProposalViewAllViewModel.ProposalItem() { Id = 12, Name = "Element 3.1", Label = "Element 3.1", Type = "Question", InputType = "Textbox", Parent = section3, ParentId = section3.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element32 = new ProposalViewAllViewModel.ProposalItem() { Id = 13, Name = "Element 3.2", Label = "Element 3.2", Type = "Question", InputType = "Textbox", Parent = section3, ParentId = section3.Id };

        //       //    proposalItems.Add(section1);
        //       //    proposalItems.Add(section2);
        //       //    proposalItems.Add(section3);

        //       //    proposalItems.Add(element11);
        //       //    proposalItems.Add(element12);
        //       //    proposalItems.Add(element13);
        //       //    proposalItems.Add(element14);

        //       //    proposalItems.Add(element21);
        //       //    proposalItems.Add(element22);
        //       //    proposalItems.Add(element23);
        //       //    proposalItems.Add(element24);

        //       //    proposalItems.Add(element31);
        //       //    proposalItems.Add(element32);

        //       //    model.Items = proposalItems;

        //       //    ViewData["items"] = proposalItems;
        //       //}

        //       //public void SetUpItems_Guid()
        //       //{
        //       //    ProposalViewAllViewModel.ProposalItem section1 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Section 1",
        //       //        Label = "Section 1",
        //       //        Type = "Section"
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem section2 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Section 2",
        //       //        Label = "Section 2",
        //       //        Type = "Section"
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem section3 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Section 3",
        //       //        Label = "Section 3",
        //       //        Type = "Section"
        //       //    };

        //       //    ProposalViewAllViewModel.ProposalItem element11 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Element 1.1",
        //       //        Label = "Element 1.1",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element12 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Element 1.2",
        //       //        Label = "Element 1.2",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element13 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Element 1.3",
        //       //        Label = "Element 1.3",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element14 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid(),
        //       //        Name = "Element 1.4",
        //       //        Label = "Element 1.4",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };

        //       //    ProposalViewAllViewModel.ProposalItem element21 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid(), Name = "Element 2.1", Label = "Element 2.1", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element22 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid(), Name = "Element 2.2", Label = "Element 2.2", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element23 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid(), Name = "Element 2.3", Label = "Element 2.3", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element24 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid(), Name = "Element 2.4", Label = "Element 2.4", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };

        //       //    ProposalViewAllViewModel.ProposalItem element31 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid(), Name = "Element 3.1", Label = "Element 3.1", Type = "Question", InputType = "Textbox", Parent = section3, ParentId = section3.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element32 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid(), Name = "Element 3.2", Label = "Element 3.2", Type = "Question", InputType = "Textbox", Parent = section3, ParentId = section3.Id };


        //       //    proposalItems.Add(section1);
        //       //    proposalItems.Add(section2);
        //       //    proposalItems.Add(section3);

        //       //    proposalItems.Add(element11);
        //       //    proposalItems.Add(element12);
        //       //    proposalItems.Add(element13);
        //       //    proposalItems.Add(element14);

        //       //    proposalItems.Add(element21);
        //       //    proposalItems.Add(element22);
        //       //    proposalItems.Add(element23);
        //       //    proposalItems.Add(element24);

        //       //    proposalItems.Add(element31);
        //       //    proposalItems.Add(element32);

        //       //    model.Items = proposalItems;

        //       //    ViewData["items"] = proposalItems;
        //       //}

        //       //public void SetUpItems_GuidString()
        //       //{
        //       //    ProposalViewAllViewModel.ProposalItem section1 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Section 1",
        //       //        Label = "Section 1",
        //       //        Type = "Section"
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem section2 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Section 2",
        //       //        Label = "Section 2",
        //       //        Type = "Section"
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem section3 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Section 3",
        //       //        Label = "Section 3",
        //       //        Type = "Section"
        //       //    };

        //       //    ProposalViewAllViewModel.ProposalItem element11 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Element 1.1",
        //       //        Label = "Element 1.1",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element12 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Element 1.2",
        //       //        Label = "Element 1.2",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element13 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Element 1.3",
        //       //        Label = "Element 1.3",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };
        //       //    ProposalViewAllViewModel.ProposalItem element14 = new ProposalViewAllViewModel.ProposalItem()
        //       //    {
        //       //        Id = Guid.NewGuid().ToString(),
        //       //        Name = "Element 1.4",
        //       //        Label = "Element 1.4",
        //       //        Type = "Question",
        //       //        InputType = "Textbox",
        //       //        Parent = section1,
        //       //        ParentId = section1.Id
        //       //    };

        //       //    ProposalViewAllViewModel.ProposalItem element21 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid().ToString(), Name = "Element 2.1", Label = "Element 2.1", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element22 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid().ToString(), Name = "Element 2.2", Label = "Element 2.2", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element23 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid().ToString(), Name = "Element 2.3", Label = "Element 2.3", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element24 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid().ToString(), Name = "Element 2.4", Label = "Element 2.4", Type = "Question", InputType = "Textbox", Parent = section2, ParentId = section2.Id };

        //       //    ProposalViewAllViewModel.ProposalItem element31 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid().ToString(), Name = "Element 3.1", Label = "Element 3.1", Type = "Question", InputType = "Textbox", Parent = section3, ParentId = section3.Id };
        //       //    ProposalViewAllViewModel.ProposalItem element32 = new ProposalViewAllViewModel.ProposalItem() { Id = Guid.NewGuid().ToString(), Name = "Element 3.2", Label = "Element 3.2", Type = "Question", InputType = "Textbox", Parent = section3, ParentId = section3.Id };


        //       //    proposalItems.Add(section1);
        //       //    proposalItems.Add(section2);
        //       //    proposalItems.Add(section3);

        //       //    proposalItems.Add(element11);
        //       //    proposalItems.Add(element12);
        //       //    proposalItems.Add(element13);
        //       //    proposalItems.Add(element14);

        //       //    proposalItems.Add(element21);
        //       //    proposalItems.Add(element22);
        //       //    proposalItems.Add(element23);
        //       //    proposalItems.Add(element24);

        //       //    proposalItems.Add(element31);
        //       //    proposalItems.Add(element32);

        //       //    model.Items = proposalItems;

        //       //    ViewData["items"] = proposalItems;
        //       //}

        //       public ActionResult Index()
        //       {
        //           return View();
        //       }

        //       public ActionResult Add()
        //       {
        //           return View();
        //       }

        //       //public ActionResult ViewAll()
        //       //{
        //       //    return View(model);
        //       //}

        //       public ActionResult ProposalBuilder()
        //       {
        //           ProposalBuilderViewModel model = new ProposalBuilderViewModel();

        //           return View(model);
        //       }

        //       [HttpPost]
        //       public JsonResult CreateProposal(ProposalBuilderViewModel proposalBuilderViewModel)
        //       {
        //           //insuredOrganisationGridView.AddInsuredOrganisation(insuredOrganisation);

        //           //ProposalTemplate proposalTemplate = _proposalBuilderService.CreateProposalTemplate(proposalBuilderViewModel.Title);
        //           if (proposalBuilderViewModel.Id == Guid.Empty)
        //           {
        //               var proposalTemplate = _proposalBuilderService.CreateProposalTemplate(CurrentUser as Owner, proposalBuilderViewModel.Title, false);

        //               proposalBuilderViewModel.Id = proposalTemplate.Id;
        //           }

        //           return Json(proposalBuilderViewModel);
        //       }


        //       public JsonResult GetParents()
        //       {
        //           return Json(proposalItems, JsonRequestBehavior.AllowGet);
        //       }

        //	public ActionResult ManageElements()
        //	{
        //		ProposalViewAllViewModel model = new ProposalViewAllViewModel () {
        //			Items = new List<ProposalViewAllViewModel.ProposalItem>()
        //		};
        //		model.Items.Add(new ProposalViewAllViewModel.ProposalItem () {
        //			Name = "Name",
        //			Label = "Label",
        //			Type = "Type",
        //			InputType = "InputType"
        //		});

        //		ViewData["items"] = proposalItems;

        //		return View(model);
        //	}

        //       //public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        //       //{
        //       //    return Json(proposalItems.ToDataSourceResult(request));
        //       //}

        //       //        [AcceptVerbs(HttpVerbs.Post)]
        //       //        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ProposalViewAllViewModel.ProposalItem proposalItem)
        //       //        {
        //       //            if (proposalItem != null && ModelState.IsValid)
        //       //            {                //productService.Create(product);
        //       //
        //       //                var last = proposalItems.LastOrDefault();
        //       //
        //       //                proposalItem.Id = last != null? last.Id + 1 : 1;
        //       //                //proposalItem.Id = Guid.NewGuid();
        //       //                proposalItems.Add(proposalItem);
        //       //
        //       //                ViewData["items"] = proposalItems;
        //       //            }
        //       //
        //       //            return Json(new[] { proposalItem }.ToDataSourceResult(request, ModelState));
        //       //        }

        //       //[AcceptVerbs(HttpVerbs.Post)]
        //       //public ActionResult Update([DataSourceRequest] DataSourceRequest request, ProposalViewAllViewModel.ProposalItem proposalItem)
        //       //{
        //       //    if (proposalItem != null && ModelState.IsValid)
        //       //    {
        //       //        //productService.Update(product);
        //       //    }

        //       //    return Json(new[] { proposalItem }.ToDataSourceResult(request, ModelState));
        //       //}

        //       //[AcceptVerbs(HttpVerbs.Post)]
        //       //public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, ProposalViewAllViewModel.ProposalItem proposalItem)
        //       //{
        //       //    if (proposalItem != null)
        //       //    {
        //       //        //productService.Destroy(product);
        //       //    }

        //       //    return Json(new[] { proposalItem }.ToDataSourceResult(request, ModelState));
        //       //}

        //       // This should build the proposal based on a binder

        //       public ActionResult MNZNLP()
        //       {
        //           #region MyRegion

        //           #endregion

        //           return View();
        //       }

        //       InsuredOrganisationGridView insuredOrganisationGridView;

        //       [HttpGet]
        //       public JsonResult GetOrganisations(int? page, int? limit, string sortBy, string direction, string searchString = null)
        //       {
        //           int total;
        //           insuredOrganisationGridView = new InsuredOrganisationGridView();
        //           var records = insuredOrganisationGridView.GetInsuredOrganisations(page, limit, sortBy, direction, searchString, out total);
        //           return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        //       }

        //       [HttpPost]
        //       public JsonResult Save(InsuredOrganisation insuredOrganisation)
        //       {
        //           insuredOrganisationGridView.AddInsuredOrganisation(insuredOrganisation);
        //           return Json(true);
        //       }

        //       [HttpPost]
        //       public JsonResult Remove(int id)
        //       {
        //           insuredOrganisationGridView.RemoveInsuredOrganisation(id);
        //           return Json(true);
        //       }
        //   }   
    }
}