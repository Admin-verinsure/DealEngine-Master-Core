using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using ElmahCore;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.WebUI.Models.ControlModels;
using DealEngine.WebUI.Models.Policy;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DealEngine.WebUI.Controllers
{
    public class PolicyController : BaseController
    {
		IDocumentService _policyDocumentService;
		ITermBuilderService _termBuilderService;		
		IRiskCategoryService _riskCategoryService;
		IProductService _productService;
		IMapper _mapper;
		IApplicationLoggingService _applicationLoggingService;
		ILogger<PolicyController> _logger;

		public PolicyController(
			ILogger<PolicyController> logger,
			IApplicationLoggingService applicationLoggingService,
			IRiskCategoryService riskCategoryService,
			IProductService productService,
			IUserService userRepository,
			IDocumentService policyDocumentService,
			ITermBuilderService termBuilderService,						
			IMapper mapper
			)
			: base (userRepository)
		{
			_logger = logger;
			_applicationLoggingService = applicationLoggingService;
			_riskCategoryService = riskCategoryService;
			_productService = productService;
			_policyDocumentService = policyDocumentService;            
            _termBuilderService = termBuilderService;
			_mapper = mapper;

		}

		[HttpGet]
		public async Task<IActionResult> PolicyIndex()
        {
			PolicyDocumentListViewModel documents = new PolicyDocumentListViewModel ();
			documents.Documents = new List<PolicyDocumentViewModel> ();
			User user = null;

			try
			{
				user = await CurrentUser();
				var documentTemplateList = await _policyDocumentService.GetDocumentTemplates();
				foreach (Old_PolicyDocumentTemplate doc in documentTemplateList)
				{
					PolicyDocumentViewModel model = new PolicyDocumentViewModel();
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

					documents.Documents.Add(model);
				}

				return View(documents);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}

		}

		[Obsolete]
		[HttpGet]
		public async Task<IActionResult> DocumentBuilder()
		{
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
		public async Task<IActionResult> SubmitCompletedDocument(PolicyDocumentViewModel model)
		{
            var user = await CurrentUser();
			Old_PolicyDocumentTemplate document = new Old_PolicyDocumentTemplate (user, model.Title);
			document.ChangeCreator(user.FullName);
			document.ChangeOwner(user.FullName);
			document.ChangeDescription (model.Description);
			document.SetRevision(model.Revision);
			document.ChangeText(model.Text);			
			document.ChangeVersion(model.Version);
			document.ChangeTerritory (model.Territory);
			document.ChangeJurisdiction (model.Jurisdiction);
			document.SetCustomTerritory (model.CustomTerritory);
			document.SetCustomJurisdiction (model.CustomJurisdiction);

			await _policyDocumentService.SaveDocumentTemplate (document);

			// TODO enable below when the Mono framework gets updated
			//return RedirectToAction ("DocumentBuilder");
			return Redirect("~/Policy/DocumentBuilder");
		}

		[Obsolete]
		[HttpGet]
		public async Task<IActionResult> CreateDocuments()
		{
			return View();
		}

		[Obsolete]
		[HttpGet]
		public async Task<IActionResult> CreateDocumentSections()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Risks ()
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				var riskCategoryList = await _riskCategoryService.GetAllRiskCategories();
				IList<InsuranceRiskCategory> riskCategories = _mapper.Map<IList<InsuranceRiskCategory>>(riskCategoryList);
				IList<SelectListItem> selectList = new List<SelectListItem>();
				foreach (RiskCategory risk in riskCategoryList)
					selectList.Add(new SelectListItem { Text = risk.Name, Value = risk.Name });

				InsuranceRiskCategories model = new InsuranceRiskCategories
				{
					CategoriesList = riskCategories,
					CategoryItems = selectList
				};

				return View(model);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		public async Task<IActionResult> Risks (InsuranceRiskCategory category)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				var riskCategoryList = await _riskCategoryService.GetAllRiskCategories();
				if (riskCategoryList.FirstOrDefault(r => r.Name == category.Name) != null)
					return await Risks();

				RiskCategory risk = new RiskCategory(user, category.Name, category.Description);
				await _riskCategoryService.AddRiskCategory(risk);

				return await Risks();
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetDocuments(string _search, string nd, string rows, string page, string sidx, string sord)
		{
			User user = null;
			XDocument document = null;
			try 
			{
				// FIX - fields don't map nicely.
				List<PolicyTermSection> sections = await _termBuilderService.GetTerms(sidx, sord);
				int perPage = Convert.ToInt32(rows);

				JqGridViewModel model = new JqGridViewModel ();
				model.Page = Convert.ToInt32(page);
				model.TotalRecords = sections.Count;
				model.TotalPages = ((model.TotalRecords - 1) / perPage) + 1;

                foreach (PolicyTermSection doc in sections)
				{
                    var docCreator = await _userService.GetUserById(doc.Creator);
                    try
					{
						JqGridRow row = new JqGridRow(doc.Id);
						row.AddValue(doc.Name);
						row.AddValue(docCreator.FullName);
						row.AddValue(docCreator.FullName);
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
				return this.Xml(document);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}						
		}

		[HttpGet]
		public async Task<IActionResult> ViewDocumentTemplate(string id)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				Guid documentId = new Guid(id);
				Old_PolicyDocumentTemplate document = await _policyDocumentService.GetDocumentTemplate(documentId);
				PolicyDocumentViewModel model = _mapper.Map<PolicyDocumentViewModel>(document);

				return View(model);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpGet]
		public async Task<IActionResult> EditDocument(string id)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				Guid documentId = new Guid(id);
				Old_PolicyDocumentTemplate document = await _policyDocumentService.GetDocumentTemplate(documentId);
				document.SetRevision(document.Revision + 1);
				PolicyDocumentViewModel model = _mapper.Map<PolicyDocumentViewModel>(document);

				return View("DocumentBuilder", model);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpPost]
		public async Task<bool> DeprecateDocument(string id)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				Guid documentId = new Guid(id);
				Old_PolicyDocumentTemplate document = await _policyDocumentService.GetDocumentTemplate(documentId);
				document.SetDeprecated(true);
				await _policyDocumentService.SaveDocumentTemplate(document);
				return true;
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return false;
			}
		}

		[HttpGet]
		public async Task<string> RenderDocument()
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				// TODO - just renders a pre selected document. Finish later when not pressed for time to finish the demo.

				// Get Proposal Information here
				List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
				// Demo stuff here
				mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", "TestClientName"));
				mergeFields.Add(new KeyValuePair<string, string>("[[InceptionDate]]", "1 April 2016"));
				mergeFields.Add(new KeyValuePair<string, string>("[[ExpiryDate]]", "1 April 2017"));
				mergeFields.Add(new KeyValuePair<string, string>("[[RetroactiveDate_MedMal]]", "Unlimited excluding claims and circumstances/Policy Inception for New Business"));
				mergeFields.Add(new KeyValuePair<string, string>("[[RetroactiveDate_SL]]", "Unlimited excluding claims and circumstances/Policy Inception for New Business"));
				mergeFields.Add(new KeyValuePair<string, string>("[[RetroactiveDate_PL]]", "N/A"));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundLimit_MedMal]]", "$500,000"));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundLimit_SL]]", "$500,000"));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundLimit_PL]]", "$1,000,000"));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundExcess_MedMal]]", "$2,000"));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundExcess_SL]]", "$500"));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundExcess_PL]]", "$500"));
				mergeFields.Add(new KeyValuePair<string, string>("[[NameOfInsured]]", "TestClientName"));
				mergeFields.Add(new KeyValuePair<string, string>("[[TableStart:Endorsements]]", ""));
				mergeFields.Add(new KeyValuePair<string, string>("[[Endorsement]]", ""));
				mergeFields.Add(new KeyValuePair<string, string>("[[EndorsementText]]", ""));
				mergeFields.Add(new KeyValuePair<string, string>("[[TableEnd:Endorsements]]", ""));
				mergeFields.Add(new KeyValuePair<string, string>("[[BrokerSignature]]", ""));
				mergeFields.Add(new KeyValuePair<string, string>("[[BoundOrQuoteDate]]", "18 Febuary 2016"));

				// gets the most recent revision of the specified document and produces a complete version based off the merge fields
				string document = await _policyDocumentService.RenderDocument("WAHAP Certificate", mergeFields);

				return document;
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return "";
			}
		}

		[HttpGet]
		public async Task<IActionResult> SectionBuilder()
		{
			User user = null;

			try
			{
				user = await CurrentUser();
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
				IEnumerable<SelectListItem> localeList = locales.Select((r, index) => new SelectListItem { Text = r, Value = index.ToString() });
				string[] clauses = new string[] {
				"Entire Policy Clause",
				"Optional Inclusion Clause",
				"Optional Exclusion Clause",
				"Individual Clause"
			};
				IEnumerable<SelectListItem> clauseList = clauses.Select((r, index) => new SelectListItem { Text = r, Value = index.ToString() });

				List<SelectListItem> productlist = new List<SelectListItem>();
				var productList = await _productService.GetAllProducts();
				foreach (Product product in productList.Where(p => p.Public == true || user.PrimaryOrganisation.Name == "TC" || p.OwnerCompany == user.PrimaryOrganisation.Id))
				{
					productlist.Add(new SelectListItem
					{
						Selected = false,
						Text = product.Name,
						Value = product.Id.ToString(),
					});

				}

				PolicySectionVM model = new PolicySectionVM()
				{
					Content = new ContentSectionVM()
					{
						//Content = "Insert some text here"
					},
					Description = new DescriptionSectionVM()
					{
						Creator = user.Id,
						//Description = "A sample description",
						Owner = user.Id,
						//Revision = 0,
						//Name = "Sample Policy Section (Term)",
						//Version = "1"
					},
					Draft = true,
					Options = new OptionsSectionVM()
					{
						//Clause = "2",
						Clauses = clauseList,
						CustomJurisdiction = "",
						CustomTerritory = "",
						//Jurisdiction = "3",
						Jurisdictions = localeList,
						Territories = localeList,
						Products = productlist
					}
				};

				return View(model);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> SubmitTermSection(PolicySectionVM model)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				await _termBuilderService.Create(
					user,
					model.Description.Name,
					model.Description.Description,
					model.Description.Version,
					model.Description.Revision,
					model.Content.Content,
					model.Description.Creator,
					Guid.Empty,     //model.Options.Territory,
					Guid.Empty);    //model.Options.Jurisdiction);

				return Ok();
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpGet]
		public async Task<IActionResult> ViewSectionTemplate(string id)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
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
				IEnumerable<SelectListItem> localeList = locales.Select((r, index) => new SelectListItem { Text = r, Value = index.ToString() });
				string[] clauses = new string[] {
				"Entire Policy Clause",
				"Optional Inclusion Clause",
				"Optional Exclusion Clause",
				"Individual Clause"
			};
				IEnumerable<SelectListItem> clauseList = clauses.Select((r, index) => new SelectListItem { Text = r, Value = index.ToString() });

				Guid termId = new Guid(id);
				PolicyTermSection section = await _termBuilderService.GetTerm(termId);
				var sectionCreator = await _userService.GetUserById(section.Creator);
				PolicySectionVM model = new PolicySectionVM();
				model.Description = new DescriptionSectionVM()
				{
					Creator = section.Creator,
					CreatorName = sectionCreator.FullName,
					Description = section.Description,
					Name = section.Name,
					Owner = section.Owner,
					OwnerName = sectionCreator.FullName,
					Revision = section.Revision,
					Version = section.Version
				};
				model.Content = new ContentSectionVM()
				{
					Content = section.Content
				};
				model.Options = new OptionsSectionVM()
				{
					Clause = "0",
					Clauses = clauseList,
					Jurisdiction = "0",
					Territory = "0",
					Jurisdictions = localeList,
					Territories = localeList
				};

				return View(model);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpGet]
		public async Task<IActionResult> EditSection(string id)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
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
				IEnumerable<SelectListItem> localeList = locales.Select((r, index) => new SelectListItem { Text = r, Value = index.ToString() });
				string[] clauses = new string[] {
				"Entire Policy Clause",
				"Optional Inclusion Clause",
				"Optional Exclusion Clause",
				"Individual Clause"
			};
				IEnumerable<SelectListItem> clauseList = clauses.Select((r, index) => new SelectListItem { Text = r, Value = index.ToString() });

				Guid termId = new Guid(id);
				PolicyTermSection section = await _termBuilderService.GetTerm(termId);

				var sectionCreator = await _userService.GetUserById(section.Creator);
				PolicySectionVM model = new PolicySectionVM();
				model.Description = new DescriptionSectionVM()
				{
					Creator = section.Creator,
					CreatorName = sectionCreator.FullName,
					Description = section.Description,
					Name = section.Name,
					Owner = section.Owner,
					OwnerName = sectionCreator.FullName,
					Revision = section.Revision + 1,
					Version = section.Version
				};
				model.Content = new ContentSectionVM()
				{
					Content = section.Content
				};
				model.Options = new OptionsSectionVM()
				{
					Clause = "0",
					Clauses = clauseList,
					Jurisdiction = "0",
					Territory = "0",
					Jurisdictions = localeList,
					Territories = localeList
				};

				return View("SectionBuilder", model);
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return RedirectToAction("Error500", "Error");
			}
		}

		[HttpGet]
		public async Task<bool> DeprecateSection(string id)
		{
			User user = null;

			try
			{
				user = await CurrentUser();
				Guid termId = new Guid(id);
				await _termBuilderService.Deprecate(user, termId);
				return true;
			}
			catch (Exception ex)
			{
				await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
				return false;
			}

		}
    }
}
