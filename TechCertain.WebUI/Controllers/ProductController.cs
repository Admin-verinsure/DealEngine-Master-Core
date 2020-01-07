using ElmahCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Product;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;

namespace TechCertain.WebUI.Controllers
{
	//[Authorize]
	public class ProductController : BaseController
	{		
		IInformationTemplateService _informationService;
        IUnitOfWork _unitOfWork;
		IMapperSession<Product> _productRepository;
        ITerritoryService _territoryService;
        IMapperSession<RevenueByActivity> _revenueByActivityRepository;
        IMapperSession<RiskCategory> _riskRepository;
		IMapperSession<RiskCover> _riskCoverRepository;
		IMapperSession<Organisation> _organisationRepository;
		IMapperSession<Document> _documentRepository;
        IProgrammeService _programmeService;

        public ProductController(IUserService userRepository, IInformationTemplateService informationService, IMapperSession<RevenueByActivity> revenueByActivityRepository,
                                 IUnitOfWork unitOfWork, IMapperSession<Product> productRepository, ITerritoryService territoryService, IMapperSession<RiskCategory> riskRepository,
		                         IMapperSession<RiskCover> riskCoverRepository, IMapperSession<Organisation> organisationRepository,
								 IMapperSession<Document> documentRepository, IProgrammeService programmeService)
			: base (userRepository)
		{
            _revenueByActivityRepository = revenueByActivityRepository;
            _informationService = informationService;
			_unitOfWork = unitOfWork;
			_productRepository = productRepository;
            _territoryService = territoryService;
            _riskRepository = riskRepository;
			_riskCoverRepository = riskCoverRepository;
			_organisationRepository = organisationRepository;
			_documentRepository = documentRepository;
            _programmeService = programmeService;
		}

		[HttpGet]
        public async Task<IActionResult> MyProducts()
        {
			try {
                var user = await CurrentUser();
				var products = _productRepository.FindAll().Where (p => p.OwnerCompany == user.PrimaryOrganisation.Id);
				BaseListViewModel<ProductInfoViewModel> models = new BaseListViewModel<ProductInfoViewModel> ();
				foreach (Product p in products) {
                    var company = await _organisationRepository.GetByIdAsync(p.CreatorCompany);
                    ProductInfoViewModel model = new ProductInfoViewModel {
						DateCreated = LocalizeTime (p.DateCreated.GetValueOrDefault()),
						Id = p.Id,
						Name = p.Name,
						OwnerCompany = company.Name,
						SelectedLanguages = p.Languages
					};
					models.Add (model);
				}
				return View ("AllProducts", models);
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}

			return View ("AllProducts");
        }

		[HttpGet]
		public async Task<IActionResult> AllProducts ()
		{
			try {
				var products = _productRepository.FindAll().Where(p => p.Public);
				BaseListViewModel<ProductInfoViewModel> models = new BaseListViewModel<ProductInfoViewModel> ();
				foreach (Product p in products) {
                    var creatorCompany = await _organisationRepository.GetByIdAsync(p.CreatorCompany);
                    ProductInfoViewModel model = new ProductInfoViewModel {
						DateCreated = LocalizeTime (p.DateCreated.GetValueOrDefault ()),
						Id = p.Id,
						Name = p.Name,
						OwnerCompany = creatorCompany.Name,
						SelectedLanguages = p.Languages
					};
					models.Add (model);
				}
				return View (models);
			}
			catch (Exception ex) {
				Console.WriteLine (ex);
			}

			return View ();
		}

		// Proposal Element,
		// Premium Element,
                   		// Policy Element
		[HttpGet]
		public async Task<IActionResult> CreateProduct ()
		{
			return View ();
		}

		// Can not create a product without different insurance elements existing
		// Can only map and not add new elements
		[HttpGet]
		public async Task<IActionResult> CreateNew()
		{
            var user = await CurrentUser();
			ProductViewModel model = new ProductViewModel ();
			model.Description = new ProductDescriptionVM {
				CreatorOrganisation = user.PrimaryOrganisation.Id,
				OwnerOrganisation = user.PrimaryOrganisation.Id,
				// TODO - load this from db
				Languages = new List<SelectListItem> {
					new SelectListItem { Text = "English (NZ)", Value = "nz" },
					new SelectListItem { Text = "English (US)", Value = "uk" },
					new SelectListItem { Text = "English (UK)", Value = "us" },
					new SelectListItem { Text = "German", Value = "de" },
					new SelectListItem { Text = "French", Value = "fr" },
					new SelectListItem { Text = "Chinese", Value = "cn" }
				},
				BaseProducts = new List<SelectListItem> { new SelectListItem { Text = "Select Base Product", Value = "" } }
			};

			//if (System.Web.Security.Roles.IsUserInRole ("superuser"))
				model.Description.BaseProducts.Add (new SelectListItem { Text = "Set as base product", Value = Guid.Empty.ToString () });

			foreach (Product product in _productRepository.FindAll().Where (p => p.IsBaseProduct)) {
				model.Description.BaseProducts.Add (new SelectListItem { Text = product.Name, Value = product.Id.ToString () });
			}

			foreach (RiskCategory risk in _riskRepository.FindAll())
				model.Risks.Add (new RiskEntityViewModel { Insured = risk.Name, Id = risk.Id, CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false });

			// set product settings
			foreach (Document doc in _documentRepository.FindAll().Where (d => d.OwnerOrganisation == user.PrimaryOrganisation))
				model.Settings.Documents.Add (new SelectListItem { Text = doc.Name, Value = doc.Id.ToString () });

			model.Settings.InformationSheets = new List<SelectListItem> ();
			model.Settings.InformationSheets.Add (new SelectListItem { Text = "Select Information Sheet", Value = "" });
			var templates = await _informationService.GetAllTemplates();
			foreach (var template in templates)
				model.Settings.InformationSheets.Add (
					new SelectListItem {
						Text = template.Name,
						Value = template.Id.ToString ()
					}
				);

			model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = "Select Product Owner", Value = "" });
			model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = user.PrimaryOrganisation.Name, Value = user.PrimaryOrganisation.Id.ToString () });
			// loop over all non personal organisations and add them, excluding our own since its already added
			foreach (Organisation org in _organisationRepository.FindAll().Where (org => org.OrganisationType.Name != "personal").OrderBy (o => o.Name))
				if (org.Id.ToString () != model.Settings.PossibleOwnerOrganisations [1].Value)
					model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = org.Name, Value = org.Id.ToString () });

			var programmes = new List<Programme>();
            var programmeList = await _programmeService.GetAllProgrammes();
			//foreach (Programme programme in _programmeRepository.FindAll().Where(p => CurrentUser().Organisations.Contains(p.Owner)))
			foreach (Programme programme in programmeList)
				model.Settings.InsuranceProgrammes.Add (
					new SelectListItem {
						Text = programme.Name,
						Value = programme.Id.ToString ()
					}
				);

			model.Parties = new ProductPartiesVM {
				Brokers = new List<SelectListItem>(),
				Insurers = new List<SelectListItem>()
			};


			return View (model);
		}


        // Can not create a product without different insurance elements existing
        // Can only map and not add new elements
        [HttpGet]
        public async Task<IActionResult> CreateTerritory()
        {
            TerritoryViewModel model = new TerritoryViewModel();
            var user = await CurrentUser();
            model.Builder = new TerritoryBuilderVM
            {
                Location = "",
                Zoneorder = 0,
                Ispublic = false,
                // TODO - load this from db
                BaseExclIncl = new List<SelectListItem> {
                    new SelectListItem { Text = "Inclusion", Value = "Incl" },
                    new SelectListItem { Text = "Exclusion", Value = "Excl" },
                   
                }
            };

            List<SelectListItem> proglist = new List<SelectListItem>();
            var progList = await _programmeService.GetProgrammesByOwner(user.PrimaryOrganisation.Id);
            foreach (Programme programme in progList)
            {
                 proglist.Add(new SelectListItem
                 {
                        Selected = false,
                        Text = programme.Name,
                        Value = programme.Id.ToString(),
                 });
                
            }
         
            model.TerritoryAttach = new TerritoryAttachVM
            {
                BaseProgList = proglist
            };

            return View("TerritoryBuilder",model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTerritoryForProgramme(string Location, string IncluExclu, int ZoneNo, bool IsPublic, string ProgrammeId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Form has not been completed");
                throw new Exception("Form has not been completed");
            }

            try
            {
                var user = await CurrentUser();
                var programme = await _programmeService.GetProgrammeById(Guid.Parse(ProgrammeId));

                TerritoryTemplate territoryTemplate = new TerritoryTemplate(user, Location)
                {
                    Ispublic = IsPublic,
                    Zoneorder = ZoneNo,
                    ExclorIncl = IncluExclu,                    
                };

                var territoryNZ = await _territoryService.GetTerritoryTemplateByName("NZ");
                await _territoryService.AddTerritoryTemplate(territoryTemplate);
                await _programmeService.AttachProgrammeToTerritory(programme, territoryTemplate);

                return Redirect("~/Product/MyProducts");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }


        [HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNew(ProductViewModel model)
		{
			if (!ModelState.IsValid) {
				ModelState.AddModelError ("", "Form has not been completed");
				throw new Exception ("Form has not been completed");
			}

			try {
                var user = await CurrentUser();
				Product baseProduct = null;
				Guid baseProductId = Guid.Empty;
				if (Guid.TryParse (model.Description.SelectedBaseProduct, out baseProductId))
					baseProduct = await _productRepository.GetByIdAsync(baseProductId);

				Guid ownerCompanyGuid = Guid.Empty;
				if (!Guid.TryParse (model.Settings.SelectedOwnerOrganisation, out ownerCompanyGuid))
					throw new Exception ("Invalid owner organisation id: " + model.Settings.SelectedOwnerOrganisation);

				Product product = new Product (user, model.Description.CreatorOrganisation, model.Description.Name) {
					Description = model.Description.Description,
					OwnerCompany = ownerCompanyGuid,
					Languages = new List<string> (model.Description.SelectedLanguages),
					OriginalProductId = baseProductId,
					Public = model.Description.Public,
					IsBaseProduct = (baseProductId == Guid.Empty)
				};

				foreach (RiskEntityViewModel risk in model.Risks) {
					RiskCover cover = new RiskCover (user) {
						BaseRisk = await _riskRepository.GetByIdAsync(risk.Id),
						CoverAll = risk.CoverAll,
						Interuption = risk.CoverInterruption,
						Loss = risk.CoverLoss,
						ThirdParty = risk.CoverThirdParty
					};
					if (risk.CoverAll)
						cover.SelectAll ();
					product.RiskCategoriesCovered.Add (cover);
				}

				product.Documents = new List<Document> ();
				if (model.Settings != null && model.Settings.SelectedDocuments != null) {
					foreach (string sid in model.Settings.SelectedDocuments) {
						Guid id = Guid.Empty;
						if (Guid.TryParse (sid, out id))
							product.Documents.Add (await _documentRepository.GetByIdAsync(id));
					}
				}

				if (model.Description.SelectedBaseProduct != Guid.Empty.ToString ()) {
					// temp, remove once the question builder is intergrated into the product builder
					Guid informationTemplateId = Guid.Empty;
					if (Guid.TryParse (model.Settings.SelectedInformationSheet, out informationTemplateId)) {
						var sheet = await _informationService.GetTemplate (informationTemplateId);
						if (sheet == null)
							throw new Exception ("No UIS Template found for id " + informationTemplateId);
                        await _informationService.AddProductTo (sheet.Id, product);
					}
				}

				if (!string.IsNullOrEmpty (model.Settings.SelectedInsuranceProgramme)) {
                    Guid programmeId = Guid.Empty;
                    if (Guid.TryParse(model.Settings.SelectedInsuranceProgramme, out programmeId))
                    {
                        Programme programme = await _programmeService.GetProgrammeById(programmeId);
                        programme.Products.Add(product);
                    }
                }

                if (baseProduct != null)
                    baseProduct.ChildProducts.Add(product);

                await _productRepository.AddAsync(product);


                return Redirect ("~/Product/MyProducts");
				//return Content (string.Format("Your product [{0}] has been successfully created.", model.Description.Name));
			}
			catch (Exception ex) {
				ElmahExtensions.RiseError(ex);
				Response.StatusCode = 500;
				return Content (ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> ViewProduct (Guid id)
		{
			ProductViewModel model = new ProductViewModel ();
			Product product = await _productRepository.GetByIdAsync(id);
			if (product != null) {
				model.Description = new ProductDescriptionVM {
					DateCreated = LocalizeTime (product.DateCreated.GetValueOrDefault ()),
					Description = product.Description,
					Name = product.Name,
					SelectedLanguages = product.Languages.ToArray (),
					Public = product.Public
				};
				model.Risks = new ProductRisksVM ();
				foreach (RiskCover risk in product.RiskCategoriesCovered)
					model.Risks.Add (new RiskEntityViewModel { Insured = risk.BaseRisk.Name, CoverAll = risk.CoverAll, CoverLoss = risk.Loss, 
							CoverInterruption = risk.Interuption, CoverThirdParty = risk.ThirdParty });
			}
			return View (model);
		}

		[HttpGet]
		public async Task<IActionResult> CloneProduct (Guid id)
		{
            var user = await CurrentUser();
			ProductViewModel model = new ProductViewModel ();
			Product originalProduct = await _productRepository.GetByIdAsync(id);
			if (originalProduct != null) {
				model.Description = new ProductDescriptionVM {
					OriginalProductId = originalProduct.Id,
					CreatorOrganisation = user.PrimaryOrganisation.Id,
					OwnerOrganisation = user.PrimaryOrganisation.Id,
					Name = originalProduct.Name,
					Description = originalProduct.Description,
					SelectedLanguages = originalProduct.Languages.ToArray(),
					// TODO - load this from db
					Languages = new List<SelectListItem> {
						new SelectListItem { Text = "English (NZ)", Value = "nz" },
						new SelectListItem { Text = "English (US)", Value = "uk" },
						new SelectListItem { Text = "English (UK)", Value = "us" },
						new SelectListItem { Text = "German", Value = "de" },
						new SelectListItem { Text = "French", Value = "fr" },
						new SelectListItem { Text = "Chinese", Value = "cn" }
					},
					BaseProducts = new List<SelectListItem> { new SelectListItem { Text = "Select Base Product", Value = "" } },
					SelectedBaseProduct = id.ToString (),
					IsBaseProduct = originalProduct.IsBaseProduct
				};
                
				foreach (Product product in _productRepository.FindAll().Where (p => p.IsBaseProduct)) {
					model.Description.BaseProducts.Add (new SelectListItem { Text = product.Name, Value = product.Id.ToString () });
				}

				model.Risks = new ProductRisksVM ();
				foreach (RiskCategory risk in _riskRepository.FindAll()) {
					RiskCover productRisk = originalProduct.RiskCategoriesCovered.FirstOrDefault (r => r.BaseRisk == risk);
					if (productRisk == null)
						productRisk = new RiskCover (user) { CoverAll = false, Loss = false, Interuption = false, ThirdParty = false };
					model.Risks.Add (
						new RiskEntityViewModel {
							Insured = risk.Name,
							Id = risk.Id,
							CoverAll = productRisk.CoverAll,
							CoverLoss = productRisk.Loss,
							CoverInterruption = productRisk.Interuption,
							CoverThirdParty = productRisk.ThirdParty
						});
				}
				//model.InformationSheet = new ProductInformationSheetVM ();
				//var templates = _informationService.GetAllTemplates ();
				//foreach (var template in templates)
				//	((IList<SelectListItem>)model.InformationSheet.InformationSheets).Add (
				//		new SelectListItem {
				//			Text = template.Name,
				//			Value = template.Id.ToString ()
				//		}
				//	);

				model.Settings = new ProductSettingsVM ();
				model.Settings.Documents = new List<SelectListItem> ();
				foreach (Document doc in _documentRepository.FindAll().Where (d => d.OwnerOrganisation == user.PrimaryOrganisation))
					model.Settings.Documents.Add (new SelectListItem { Text = doc.Name, Value = doc.Id.ToString () });

				model.Settings.InformationSheets = new List<SelectListItem> ();
				var templates = await _informationService.GetAllTemplates();
				foreach (var template in templates)
					model.Settings.InformationSheets.Add (
						new SelectListItem {
							Text = template.Name,
							Value = template.Id.ToString ()
						}
					);

				model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = user.PrimaryOrganisation.Name, Value = user.PrimaryOrganisation.Id.ToString () });
				// loop over all non personal organisations and add them, excluding our own since its already added
				foreach (Organisation org in _organisationRepository.FindAll().Where (org => org.OrganisationType.Name != "personal").OrderByDescending (o => o.Name))
					if (org.Id.ToString () != model.Settings.PossibleOwnerOrganisations [0].Value)
						model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = org.Name, Value = org.Id.ToString () });

				model.Parties = new ProductPartiesVM {
					Brokers = new List<SelectListItem> (),
					Insurers = new List<SelectListItem> ()
				};
			}
			return View ("CreateNew", model);
		}

		[HttpPost]
		public async Task<IActionResult> CloneProduct (ProductViewModel model)
		{
			return await CreateNew(model);
		}

		[HttpGet]
		public async Task<IActionResult> FindProducts ()
		{
			ProductRisksVM model = new ProductRisksVM ();
			foreach (RiskCategory risk in _riskRepository.FindAll())
				model.Add (new RiskEntityViewModel { Insured = risk.Name, Id = risk.Id, CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false });

			return View (model);
		}

		[HttpPost]
		public async Task<IActionResult> FindProducts (ProductRisksVM model)
		{
			BaseListViewModel<ProductInfoViewModel> models = new BaseListViewModel<ProductInfoViewModel> ();
			var riskCovers = _riskCoverRepository.FindAll();
			foreach (var m in model) {
				var covers = riskCovers.Where(rc => rc.BaseRisk.Id == m.Id &&
				                              		rc.CoverAll == m.CoverAll &&
				                              		rc.Interuption == m.CoverInterruption &&
				                              		rc.Loss == m.CoverLoss &&
				                              		rc.ThirdParty == m.CoverThirdParty);
				Console.WriteLine (covers.Count ());
				foreach (var r in covers) {
					Console.WriteLine (r.ToString ());                   
                    Product p = r.Product;
                    var creatorCompany = await _organisationRepository.GetByIdAsync(p.CreatorCompany);
                    ProductInfoViewModel vm = new ProductInfoViewModel {
						DateCreated = LocalizeTime (p.DateCreated.GetValueOrDefault ()),
						Id = p.Id,
						Name = p.Name,
						OwnerCompany = creatorCompany.Name,
						SelectedLanguages = p.Languages
					};
					models.Add (vm);
				}
			}

			return View ("AllProducts", models);
		}
	}
}
