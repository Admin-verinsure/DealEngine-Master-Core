using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using Elmah;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.WebUI.Models.ControlModels;
using TechCertain.WebUI.Models.Policy;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Controllers
{
    public class PolicyController : BaseController
    {
		IDocumentService _policyDocumentService;
		ITermBuilderService _termBuilderService;
		IMapperSession<RiskCategory> _riskRepository;
        IHttpContextAccessor _httpContextAccessor;
		IUnitOfWork _unitOfWork;

		IMapper _mapper;

		public PolicyController(IUserService userRepository,
                                SignInManager<DealEngineUser> signInManager,
                                IHttpContextAccessor httpContextAccessor,
                                DealEngineDBContext dealEngineDBContext,
                                IDocumentService policyDocumentService,
								ITermBuilderService termBuilderService,
								IMapperSession<RiskCategory> riskRepository,
								IUnitOfWork unitOfWork,
								IMapper mapper)
			: base (userRepository)
		{
			_policyDocumentService = policyDocumentService;
			_termBuilderService = termBuilderService;

			_riskRepository = riskRepository;

			_unitOfWork = unitOfWork;

			_mapper = mapper;

			// Maps
			//Mapper.CreateMap<IQueryable<CategoryRisk>, IList<InsuranceRiskCategory>> ().ReverseMap();
			//Mapper.CreateMap<RiskCategory, InsuranceRiskCategory> ().ReverseMap();
		}

		[HttpGet]
		public ActionResult PolicyIndex()
        {
			PolicyDocumentListViewModel documents = new PolicyDocumentListViewModel ();

			documents.Documents = new List<PolicyDocumentViewModel> ();

			foreach (Old_PolicyDocumentTemplate doc in _policyDocumentService.GetDocumentTemplates())
			{
				PolicyDocumentViewModel model = new PolicyDocumentViewModel ();
				model.Creator = doc.Creator;
				model.CustomJurisdiction = doc.CustomJurisdiction;
				model.CustomTerritory = doc.CustomTerritory;
				model.Jurisdiction = doc.Jurisdiction;
				model.Owner = doc.Owner;
				model.Revision = doc.Revision;
				model.Territory = doc.Territory;
				model.Text = doc.Text;
				model.Title = doc.Title;
				model.DocumentType = doc.DocumentType;
				model.Version = doc.Version;

				documents.Documents.Add (model);
			}

			return View(documents);
		}

		[Obsolete]
		[HttpGet]
		public ActionResult DocumentBuilder()
		{
//			List<PolicyDocumentViewModel> documents = new List<PolicyDocumentViewModel> () {
//				new PolicyDocumentViewModel() { Title = "Wording",  Type = "Wording 1",  Owner = "Broker A", Creator = "Broker A" },
//				new PolicyDocumentViewModel() { Title = "Policy",   Type = "Policy 1",   Owner = "Broker B", Creator = "Broker B" },
//				new PolicyDocumentViewModel() { Title = "Schedule", Type = "Schedule 1", Owner = "Broker C", Creator = "Broker C" }
//			};

			PolicyDocumentViewModel policyDocument = new PolicyDocumentViewModel ();

			return View(policyDocument);
		}

		[Obsolete]
		[HttpPost]
		public JsonResult DocumentBuilder(PolicyDocumentViewModel model)
		{
			return Json(model);
		}

		[Obsolete]
		[HttpPost]
		public ActionResult SubmitCompletedDocument(PolicyDocumentViewModel model)
		{
			Old_PolicyDocumentTemplate document = new Old_PolicyDocumentTemplate (CurrentUser, model.Title);
			document.ChangeCreator(CurrentUser.FullName);
			document.ChangeOwner(CurrentUser.FullName);
			document.ChangeDescription (model.Description);
			document.SetRevision(model.Revision);
			document.ChangeText(model.Text);
			document.ChangeType(new PolicyDocumentType(model.DocumentType));
			document.ChangeVersion(model.Version);
			document.ChangeTerritory (model.Territory);
			document.ChangeJurisdiction (model.Jurisdiction);
			document.SetCustomTerritory (model.CustomTerritory);
			document.SetCustomJurisdiction (model.CustomJurisdiction);

			_policyDocumentService.SaveDocumentTemplate (document);

			// TODO enable below when the Mono framework gets updated
			//return RedirectToAction ("DocumentBuilder");
			return Redirect("~/Policy/DocumentBuilder");
		}

		[Obsolete]
		[HttpGet]
		public ActionResult CreateDocuments()
		{
			return View();
		}

		[Obsolete]
		[HttpGet]
		public ActionResult CreateDocumentSections()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Risks ()
		{
			IQueryable<RiskCategory> risks = _riskRepository.FindAll ();

			IList<InsuranceRiskCategory> riskCategories = _mapper.Map<IList<InsuranceRiskCategory>> (risks);
			IList<SelectListItem> selectList = new List<SelectListItem> ();
			foreach (RiskCategory risk in risks)
				selectList.Add (new SelectListItem { Text = risk.Name, Value = risk.Name });

			InsuranceRiskCategories model = new InsuranceRiskCategories {
				CategoriesList = riskCategories,
				CategoryItems = selectList
			};

			// TODO - load from service
			//InsuranceRiskCategories riskCategories = new InsuranceRiskCategories () {
			//	new InsuranceRiskCategory ("People", ""),
			//	new InsuranceRiskCategory ("Assets", ""),
			//	new InsuranceRiskCategory ("Liabilities", ""),
			//	new InsuranceRiskCategory ("Accidents", ""),
			//	new InsuranceRiskCategory ("Credit", ""),
			//	new InsuranceRiskCategory ("Transport", ""),
			//	new InsuranceRiskCategory ("Animal", ""),
			//	new InsuranceRiskCategory ("Crop", ""),
			//	new InsuranceRiskCategory ("Gap", ""),
			//	new InsuranceRiskCategory ("Income Protection", ""),
			//	new InsuranceRiskCategory ("Bailman", "")
			//};

			return View (model);
		}

		public ActionResult Risks (InsuranceRiskCategory category)
		{
			if (_riskRepository.FindAll ().FirstOrDefault (r => r.Name == category.Name) != null)
				return Risks ();

			RiskCategory risk = new RiskCategory (CurrentUser, category.Name, category.Description);
			using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork ()) {
				_riskRepository.Add (risk);

				uow.Commit ();
			}

			return Risks ();
		}

		/// <summary>
		/// Gets the documents.
		/// </summary>
		/// <returns>The documents.</returns>
		/// <param name="_search">Search.</param>
		/// <param name="nd">Nd.</param>
		/// <param name="rows">Number of rows per page.</param>
		/// <param name="page">Number of pages.</param>
		/// <param name="sidx">Field to order by.</param>
		/// <param name="sord">Direction to order in (asc or desc).</param>
		[HttpGet]
		public ActionResult GetDocuments(string _search, string nd, string rows, string page, string sidx, string sord)
		{
			XDocument document = null;
			try 
			{
				// FIX - fields don't map nicely.
				PolicyTermSection[] sections = _termBuilderService.GetTerms (sidx, sord);
				int perPage = Convert.ToInt32 (rows);

				JqGridViewModel model = new JqGridViewModel ();
				model.Page = Convert.ToInt32 (page);
				model.TotalRecords = sections.Length;
				model.TotalPages = ((model.TotalRecords - 1) / perPage) + 1;

				foreach (PolicyTermSection doc in sections)
				{
					try
					{
						JqGridRow row = new JqGridRow(doc.Id);
						row.AddValue(doc.Name);
						row.AddValue(_userService.GetUser(doc.Creator).FullName);
						row.AddValue(_userService.GetUser(doc.Creator).FullName);
						row.AddValue(doc.Description);
						row.AddValue("");
						row.AddValue(doc.Version);
						row.AddValue(doc.Revision);
						row.AddValue("View");
						row.AddValue("Edit");
						if (doc.DateDeleted == null)
							row.AddValue("Deprecate");
						model.AddRow(row);
					}
					catch (Exception ex) {
						// log stuff here
					}
				}
				// convert model to XDocument for rendering.
				document = model.ToXml ();
			}
			catch (Exception ex) {
				// log stuff here
				ErrorSignal.FromCurrentContext().Raise(new Exception("Unable to load Policy Document Templates.", ex));
			}
			//Console.WriteLine (document.ToString ());
			return this.Xml (document);
		}

		[HttpGet]
		public ActionResult ViewDocumentTemplate(string id)
		{
			Guid documentId = new Guid (id);
			Old_PolicyDocumentTemplate document = _policyDocumentService.GetDocumentTemplate (documentId);

			//Mapper.CreateMap<Old_PolicyDocumentTemplate, PolicyDocumentViewModel>();

			PolicyDocumentViewModel model = _mapper.Map<PolicyDocumentViewModel>(document);

			Console.WriteLine (id);

			return View (model);
		}

		[HttpGet]
		public ActionResult EditDocument(string id)
		{
			Guid documentId = new Guid (id);
			Old_PolicyDocumentTemplate document = _policyDocumentService.GetDocumentTemplate (documentId);
			// increment revision
			document.SetRevision(document.Revision + 1);

			//Mapper.CreateMap<Old_PolicyDocumentTemplate, PolicyDocumentViewModel>();

			PolicyDocumentViewModel model = _mapper.Map<PolicyDocumentViewModel>(document);

			return View ("DocumentBuilder", model);
		}

		[HttpPost]
		public bool DeprecateDocument(string id)
		{
			Guid documentId = new Guid (id);
			Old_PolicyDocumentTemplate document = _policyDocumentService.GetDocumentTemplate (documentId);
			document.SetDeprecated (true);
			_policyDocumentService.SaveDocumentTemplate (document);
			return true;
		}

		[HttpGet]
		public string RenderDocument()
		{
			// TODO - just renders a pre selected document. Finish later when not pressed for time to finish the demo.

			// Get Proposal Information here
			List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
			// Demo stuff here
			mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", 			"TestClientName"));
			mergeFields.Add(new KeyValuePair<string, string>("[[InceptionDate]]", 			"1 April 2016"));
			mergeFields.Add(new KeyValuePair<string, string>("[[ExpiryDate]]", 				"1 April 2017"));
			mergeFields.Add(new KeyValuePair<string, string>("[[RetroactiveDate_MedMal]]", 	"Unlimited excluding claims and circumstances/Policy Inception for New Business"));
			mergeFields.Add(new KeyValuePair<string, string>("[[RetroactiveDate_SL]]", 		"Unlimited excluding claims and circumstances/Policy Inception for New Business"));
			mergeFields.Add(new KeyValuePair<string, string>("[[RetroactiveDate_PL]]", 		"N/A"));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundLimit_MedMal]]", 		"$500,000"));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundLimit_SL]]", 			"$500,000"));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundLimit_PL]]", 			"$1,000,000"));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundExcess_MedMal]]", 		"$2,000"));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundExcess_SL]]", 			"$500"));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundExcess_PL]]", 			"$500"));
			mergeFields.Add(new KeyValuePair<string, string>("[[NameOfInsured]]", 			"TestClientName"));
			mergeFields.Add(new KeyValuePair<string, string>("[[TableStart:Endorsements]]", ""));
			mergeFields.Add(new KeyValuePair<string, string>("[[Endorsement]]", 			""));
			mergeFields.Add(new KeyValuePair<string, string>("[[EndorsementText]]", 		""));
			mergeFields.Add(new KeyValuePair<string, string>("[[TableEnd:Endorsements]]", 	""));
			mergeFields.Add(new KeyValuePair<string, string>("[[BrokerSignature]]", 		""));
			mergeFields.Add(new KeyValuePair<string, string>("[[BoundOrQuoteDate]]", 		"18 Febuary 2016"));

			// gets the most recent revision of the specified document and produces a complete version based off the merge fields
			string document = _policyDocumentService.RenderDocument ("WAHAP Certificate", mergeFields);

			return document;
		}

		[HttpGet]
		public ActionResult SectionBuilder()
		{
			string[] locales = new string[] {
				"Worldwide",
				"Worldwide ex USA/Canada",
				"New Zealand",
				"New Zealand, UK and Europe",
				"NZ/Australia",
				"NZ, Australia and Asia/Pacific region excluding USA or Canadian territories",
				"NZ, Australia and Pacific Islands excluding USA and Canada and their territories",
				"NZ, Pacific Islands excluding USA and Canada and their territories",
				"NZ, Pacific Islands including American Samoa",
				"Let me add a custom Territory"
			};
			IEnumerable<SelectListItem> localeList = locales.Select((r,index) => new SelectListItem{Text = r, Value = index.ToString()});
			string[] clauses = new string[] {
				"Entire Policy Clause",
				"Optional Inclusion Clause",
				"Optional Exclusion Clause",
				"Individual Clause"
			};
			IEnumerable<SelectListItem> clauseList = clauses.Select((r,index) => new SelectListItem{Text = r, Value = index.ToString()});

			PolicySectionVM model = new PolicySectionVM () {
				Content = new ContentSectionVM () {
					//Content = "Insert some text here"
				},
				Description = new DescriptionSectionVM () {
					Creator = CurrentUser.Id,
					//Description = "A sample description",
					Owner = CurrentUser.Id,
					//Revision = 0,
					//Name = "Sample Policy Section (Term)",
					//Version = "1"
				},
				Draft = true,
				Options = new OptionsSectionVM () {
					//Clause = "2",
					Clauses = clauseList,
					CustomJurisdiction = "",
					CustomTerritory = "",
					//Jurisdiction = "3",
					Jurisdictions = localeList,
					Territories = localeList,
					Territory = "0"
				}
			};

			return View (model);
		}

		[HttpPost]
		public ActionResult SubmitTermSection(PolicySectionVM model)
		{
			_termBuilderService.Create (
				CurrentUser,
				model.Description.Name,
				model.Description.Description,
				model.Description.Version,
				model.Description.Revision,
				model.Content.Content,
				model.Description.Creator,
				Guid.Empty,		//model.Options.Territory,
				Guid.Empty);	//model.Options.Jurisdiction);

			return null;
		}

		[HttpGet]
		public ActionResult ViewSectionTemplate(string id)
		{
			string[] locales = new string[] {
				"Worldwide",
				"Worldwide ex USA/Canada",
				"New Zealand",
				"New Zealand, UK and Europe",
				"NZ/Australia",
				"NZ, Australia and Asia/Pacific region excluding USA or Canadian territories",
				"NZ, Australia and Pacific Islands excluding USA and Canada and their territories",
				"NZ, Pacific Islands excluding USA and Canada and their territories",
				"NZ, Pacific Islands including American Samoa",
				"Let me add a custom Territory"
			};
			IEnumerable<SelectListItem> localeList = locales.Select((r,index) => new SelectListItem{Text = r, Value = index.ToString()});
			string[] clauses = new string[] {
				"Entire Policy Clause",
				"Optional Inclusion Clause",
				"Optional Exclusion Clause",
				"Individual Clause"
			};
			IEnumerable<SelectListItem> clauseList = clauses.Select((r,index) => new SelectListItem{Text = r, Value = index.ToString()});

			Guid termId = new Guid (id);
			PolicyTermSection section = _termBuilderService.GetTerms ().FirstOrDefault (t => t.Id == termId);

			PolicySectionVM model = new PolicySectionVM ();
			model.Description = new DescriptionSectionVM () {
				Creator = section.Creator,
				CreatorName = _userService.GetUser(section.Creator).FullName,
				Description = section.Description,
				Name = section.Name,
				Owner = section.Owner,
				OwnerName = _userService.GetUser(section.Creator).FullName,
				Revision = section.Revision,
				Version = section.Version
			};
			model.Content = new ContentSectionVM () {
				Content = section.Content
			};
			model.Options = new OptionsSectionVM () {
				Clause = "0",
				Clauses = clauseList,
				Jurisdiction = "0",
				Territory = "0",
				Jurisdictions = localeList,
				Territories = localeList
			};

			return View (model);
		}

		[HttpGet]
		public ActionResult EditSection(string id)
		{
			string[] locales = new string[] {
				"Worldwide",
				"Worldwide ex USA/Canada",
				"New Zealand",
				"New Zealand, UK and Europe",
				"NZ/Australia",
				"NZ, Australia and Asia/Pacific region excluding USA or Canadian territories",
				"NZ, Australia and Pacific Islands excluding USA and Canada and their territories",
				"NZ, Pacific Islands excluding USA and Canada and their territories",
				"NZ, Pacific Islands including American Samoa",
				"Let me add a custom Territory"
			};
			IEnumerable<SelectListItem> localeList = locales.Select((r,index) => new SelectListItem{Text = r, Value = index.ToString()});
			string[] clauses = new string[] {
				"Entire Policy Clause",
				"Optional Inclusion Clause",
				"Optional Exclusion Clause",
				"Individual Clause"
			};
			IEnumerable<SelectListItem> clauseList = clauses.Select((r,index) => new SelectListItem{Text = r, Value = index.ToString()});

			Guid termId = new Guid (id);
			PolicyTermSection section = _termBuilderService.GetTerms ().FirstOrDefault (t => t.Id == termId);

			PolicySectionVM model = new PolicySectionVM ();
			model.Description = new DescriptionSectionVM () {
				Creator = section.Creator,
				CreatorName = _userService.GetUser(section.Creator).FullName,
				Description = section.Description,
				Name = section.Name,
				Owner = section.Owner,
				OwnerName = _userService.GetUser(section.Creator).FullName,
				Revision = section.Revision + 1,
				Version = section.Version
			};
			model.Content = new ContentSectionVM () {
				Content = section.Content
			};
			model.Options = new OptionsSectionVM () {
				Clause = "0",
				Clauses = clauseList,
				Jurisdiction = "0",
				Territory = "0",
				Jurisdictions = localeList,
				Territories = localeList
			};

			return View ("SectionBuilder", model);
		}

		[HttpGet]
		public bool DeprecateSection(string id)
		{
			Guid termId = new Guid (id);
			_termBuilderService.Deprecate (CurrentUser, termId);
			return true;
		}
    }
}
