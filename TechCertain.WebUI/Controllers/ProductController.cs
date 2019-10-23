using Elmah;
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
        IMapperSession<Territory> _TerritoryRepository;
        IMapperSession<RiskCategory> _riskRepository;
		IMapperSession<RiskCover> _riskCoverRepository;
		IMapperSession<Organisation> _organisationRepository;
		IMapperSession<Document> _documentRepository;
		IMapperSession<Programme> _programmeRepository;

		public ProductController(IUserService userRepository, IInformationTemplateService informationService, 
		                         IUnitOfWork unitOfWork, IMapperSession<Product> productRepository, IMapperSession<Territory> territoryRepository, IMapperSession<RiskCategory> riskRepository,
		                         IMapperSession<RiskCover> riskCoverRepository, IMapperSession<Organisation> organisationRepository,
								 IMapperSession<Document> documentRepository, IMapperSession<Programme> programmeRepository)
			: base (userRepository)
		{			
			_informationService = informationService;
			_unitOfWork = unitOfWork;
			_productRepository = productRepository;
            _TerritoryRepository = territoryRepository;
            _riskRepository = riskRepository;
			_riskCoverRepository = riskCoverRepository;
			_organisationRepository = organisationRepository;
			_documentRepository = documentRepository;
			_programmeRepository = programmeRepository;
		}

		[HttpGet]
        public async Task<IActionResult> MyProducts()
        {
			try {
				var products = _productRepository.FindAll().Where (p => p.OwnerCompany == CurrentUser.PrimaryOrganisation.Id);
				BaseListViewModel<ProductInfoViewModel> models = new BaseListViewModel<ProductInfoViewModel> ();
				foreach (Product p in products) {
					ProductInfoViewModel model = new ProductInfoViewModel {
						DateCreated = LocalizeTime (p.DateCreated.GetValueOrDefault()),
						Id = p.Id,
						Name = p.Name,
						OwnerCompany = _organisationRepository.GetByIdAsync(p.CreatorCompany).Result.Name,
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
					ProductInfoViewModel model = new ProductInfoViewModel {
						DateCreated = LocalizeTime (p.DateCreated.GetValueOrDefault ()),
						Id = p.Id,
						Name = p.Name,
						OwnerCompany = _organisationRepository.GetByIdAsync(p.CreatorCompany).Result.Name,
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
			ProductViewModel model = new ProductViewModel ();
			model.Description = new ProductDescriptionVM {
				CreatorOrganisation = CurrentUser.PrimaryOrganisation.Id,
				OwnerOrganisation = CurrentUser.PrimaryOrganisation.Id,
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

			//model.Risks = new ProductRisksVM {
			//	new RiskEntityViewModel { Insured = "People", CoverAll = true, CoverLoss = true, CoverInterruption = false, CoverThirdParty = false },
			//	new RiskEntityViewModel { Insured = "Businesses", CoverAll = true, CoverLoss = false, CoverInterruption = true, CoverThirdParty = false },
			//	new RiskEntityViewModel { Insured = "Associations", CoverAll = false, CoverLoss = false, CoverInterruption = true, CoverThirdParty = true },
			//	new RiskEntityViewModel { Insured = "Professions", CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false },
			//	new RiskEntityViewModel { Insured = "Goods", CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false },
			//	new RiskEntityViewModel { Insured = "Agriculture", CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false },
			//	new RiskEntityViewModel { Insured = "Transport", CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false },
			//	new RiskEntityViewModel { Insured = "Marine", CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false }
			//};
			foreach (RiskCategory risk in _riskRepository.FindAll())
				model.Risks.Add (new RiskEntityViewModel { Insured = risk.Name, Id = risk.Id, CoverAll = false, CoverLoss = false, CoverInterruption = false, CoverThirdParty = false });

			//model.InformationSheet = new ProductInformationSheetVM ();
			//var templates = _informationService.GetAllTemplates ();
			//foreach (var template in templates)
			//	((IList<SelectListItem>)model.InformationSheet.InformationSheets).Add (
			//		new SelectListItem { 
			//			Text = template.Name,
			//			Value = template.Id.ToString()
			//		}
			//	);

			// set product settings
			foreach (Document doc in _documentRepository.FindAll().Where (d => d.OwnerOrganisation == CurrentUser.PrimaryOrganisation))
				model.Settings.Documents.Add (new SelectListItem { Text = doc.Name, Value = doc.Id.ToString () });

			model.Settings.InformationSheets = new List<SelectListItem> ();
			model.Settings.InformationSheets.Add (new SelectListItem { Text = "Select Information Sheet", Value = "" });
			var templates = _informationService.GetAllTemplates().Result;
			foreach (var template in templates)
				model.Settings.InformationSheets.Add (
					new SelectListItem {
						Text = template.Name,
						Value = template.Id.ToString ()
					}
				);

			model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = "Select Product Owner", Value = "" });
			model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = CurrentUser.PrimaryOrganisation.Name, Value = CurrentUser.PrimaryOrganisation.Id.ToString () });
			// loop over all non personal organisations and add them, excluding our own since its already added
			foreach (Organisation org in _organisationRepository.FindAll().Where (org => org.OrganisationType.Name != "personal").OrderBy (o => o.Name))
				if (org.Id.ToString () != model.Settings.PossibleOwnerOrganisations [1].Value)
					model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = org.Name, Value = org.Id.ToString () });

			var programmes = new List<Programme>();
			//foreach (Programme programme in _programmeRepository.FindAll().Where(p => CurrentUser.Organisations.Contains(p.Owner)))
			foreach (Programme programme in _programmeRepository.FindAll())
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
                //BaseProducts = new List<SelectListItem> { new SelectListItem { Text = "Select Base Product", Value = "" } }
            };

            List<SelectListItem> proglist = new List<SelectListItem>();
            foreach (Programme programme in _programmeRepository.FindAll().Where(p => p.IsPublic == true || p.Owner.Id == CurrentUser.PrimaryOrganisation.Id))
            {
                 proglist.Add(new SelectListItem
                 {
                        Selected = false,
                        Text = programme.Name,
                        Value = programme.Id.ToString(),
                 });
                
            }

            //foreach (var org in CurrentUser.Organisations)
            //{
            //    foreach (var prog in org.Programmes)
            //    {
            //        proglist.Add(new SelectListItem
            //        {
            //            Selected = false,
            //            Text = org.Name,
            //            Value = org.Id.ToString(),
            //        });
            //    }
                   
            //}
         
            model.TerritoryAttach = new TerritoryAttachVM
            {
                BaseProgList = proglist
            };

            return View("TerritoryBuilder",model);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTerritory(TerritoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Form has not been completed");
                throw new Exception("Form has not been completed");
            }

            try
            {
                Territory territory = new Territory(CurrentUser , model.Builder.Location);
                territory.Ispublic = model.Builder.Ispublic;
                territory.Zoneorder = model.Builder.Zoneorder;
                territory.ExclorIncl = model.Builder.SelectedInclorExcl[0];
                var organisations = new List<Organisation>();
                //for (var i = 0; i < model.TerritoryAttach.SelectedProgramme.Length; i++)
                //{
                //    organisations.Add(_organisationRepository.GetById(Guid.Parse(model.TerritoryAttach.SelectedProgramme[i])));

                //}
                //foreach (Organisation org in organisations)
                //{
                //    org.territory.Add(territory);
                //}
                //territory.Organisation = organisations;


                var programm = new List<Programme>();
                for (var i = 0; i < model.TerritoryAttach.SelectedProgramme.Length; i++)
                {
                    programm.Add(_programmeRepository.GetByIdAsync(Guid.Parse(model.TerritoryAttach.SelectedProgramme[i])).Result);

                }
                foreach (Programme prog in programm)
                {
                    prog.territory.Add(territory);
                }
                territory.Programmes = programm;
                _TerritoryRepository.AddAsync(territory);

                return Redirect("~/Product/MyProducts");
                //return Content (string.Format("Your product [{0}] has been successfully created.", model.Description.Name));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
				Product baseProduct = null;
				Guid baseProductId = Guid.Empty;
				if (Guid.TryParse (model.Description.SelectedBaseProduct, out baseProductId))
					baseProduct = _productRepository.GetByIdAsync(baseProductId).Result;

				Guid ownerCompanyGuid = Guid.Empty;
				if (!Guid.TryParse (model.Settings.SelectedOwnerOrganisation, out ownerCompanyGuid))
					throw new Exception ("Invalid owner organisation id: " + model.Settings.SelectedOwnerOrganisation);

				Product product = new Product (CurrentUser, model.Description.CreatorOrganisation, model.Description.Name) {
					Description = model.Description.Description,
					OwnerCompany = ownerCompanyGuid,
					Languages = new List<string> (model.Description.SelectedLanguages),
					OriginalProductId = baseProductId,
					Public = model.Description.Public,
					IsBaseProduct = (baseProductId == Guid.Empty)
				};

				foreach (RiskEntityViewModel risk in model.Risks) {
					RiskCover cover = new RiskCover (CurrentUser) {
						BaseRisk = _riskRepository.GetByIdAsync(risk.Id).Result,
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
							product.Documents.Add (_documentRepository.GetByIdAsync(id).Result);
					}
				}

				if (model.Description.SelectedBaseProduct != Guid.Empty.ToString ()) {
					// temp, remove once the question builder is intergrated into the product builder
					Guid informationTemplateId = Guid.Empty;
					if (Guid.TryParse (model.Settings.SelectedInformationSheet, out informationTemplateId)) {
						var sheet = _informationService.GetTemplate (informationTemplateId).Result;
						if (sheet == null)
							throw new Exception ("No UIS Template found for id " + informationTemplateId);
						_informationService.AddProductTo (sheet.Id, product);
					}
				}

				if (!string.IsNullOrEmpty (model.Settings.SelectedInsuranceProgramme)) {
                    Guid programmeId = Guid.Empty;
                    if (Guid.TryParse(model.Settings.SelectedInsuranceProgramme, out programmeId))
                    {
                        Programme programme = _programmeRepository.GetByIdAsync(programmeId).Result;
                        programme.Products.Add(product);
                    }
                }

                if (baseProduct != null)
                    baseProduct.ChildProducts.Add(product);

                _productRepository.AddAsync(product);


                return Redirect ("~/Product/MyProducts");
				//return Content (string.Format("Your product [{0}] has been successfully created.", model.Description.Name));
			}
			catch (Exception ex) {
				ErrorSignal.FromCurrentContext ().Raise (ex);
				Response.StatusCode = 500;
				return Content (ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> ViewProduct (Guid id)
		{
			ProductViewModel model = new ProductViewModel ();
			Product product = _productRepository.GetByIdAsync(id).Result;
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
			ProductViewModel model = new ProductViewModel ();
			Product originalProduct = _productRepository.GetByIdAsync(id).Result;
			if (originalProduct != null) {
				model.Description = new ProductDescriptionVM {
					OriginalProductId = originalProduct.Id,
					CreatorOrganisation = CurrentUser.PrimaryOrganisation.Id,
					OwnerOrganisation = CurrentUser.PrimaryOrganisation.Id,
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
						productRisk = new RiskCover (CurrentUser) { CoverAll = false, Loss = false, Interuption = false, ThirdParty = false };
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
				foreach (Document doc in _documentRepository.FindAll().Where (d => d.OwnerOrganisation == CurrentUser.PrimaryOrganisation))
					model.Settings.Documents.Add (new SelectListItem { Text = doc.Name, Value = doc.Id.ToString () });

				model.Settings.InformationSheets = new List<SelectListItem> ();
				var templates = _informationService.GetAllTemplates().Result;
				foreach (var template in templates)
					model.Settings.InformationSheets.Add (
						new SelectListItem {
							Text = template.Name,
							Value = template.Id.ToString ()
						}
					);

				model.Settings.PossibleOwnerOrganisations.Add (new SelectListItem { Text = CurrentUser.PrimaryOrganisation.Name, Value = CurrentUser.PrimaryOrganisation.Id.ToString () });
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
					ProductInfoViewModel vm = new ProductInfoViewModel {
						DateCreated = LocalizeTime (p.DateCreated.GetValueOrDefault ()),
						Id = p.Id,
						Name = p.Name,
						OwnerCompany = _organisationRepository.GetByIdAsync(p.CreatorCompany).Result.Name,
						SelectedLanguages = p.Languages
					};
					models.Add (vm);
				}
			}

			return View ("AllProducts", models);
		}
	}
}
