using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.AppInitialize
{
    public class ProductConfigure : IAppConfigure
    {
        IUnitOfWork _uowFactory;
        IMapperSession<Product> _productRepository;
        IMapperSession<RiskCategory> _riskCategoryRepository;
        IMapperSession<Organisation> _organisationRepository;

        public ProductConfigure(IUnitOfWork uowFactory, IMapperSession<Product> productRepository, IMapperSession<RiskCategory> riskCategoryRepository, IMapperSession<Organisation> organisationRepository)
        {
            _uowFactory = uowFactory;
            _productRepository = productRepository;
            _riskCategoryRepository = riskCategoryRepository;
            _organisationRepository = organisationRepository;
        }

        public void Configure()
        {
            // Set up risk categories followed by products
            SetupRiskCategories();

            SetupProducts();
        }

        void SetupRiskCategories()
        {
            var riskCategories = new RiskCategory[] {
                new RiskCategory(null, "People", ""),
                new RiskCategory(null, "Business", ""),
                new RiskCategory(null, "Associations", ""),
                new RiskCategory(null, "Professions", ""),
                new RiskCategory(null, "Goods", ""),
                new RiskCategory(null, "Agriculture", ""),
                new RiskCategory(null, "Transport", ""),
                new RiskCategory(null, "Marine", "")
            };

            var existingCategoryNames = _riskCategoryRepository.FindAll().Select(rc => rc.Name).ToArray();
            using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork())
            {
                foreach (var riskCategory in riskCategories)
                {
                    if (!existingCategoryNames.Contains(riskCategory.Name))
                        _riskCategoryRepository.Add(riskCategory);
                    //	Console.WriteLine ("Saving RiskCategory: " + riskCategory.Name);
                    //else
                    //	Console.WriteLine ("Skip saving RiskCategory: " + riskCategory.Name);
                }
            }
        }

        void SetupProducts()
        {
            var defaultOrganisation = _organisationRepository.FindAll().FirstOrDefault(o => o.Name == "TechCertain Ltd.");

            var baseProducts = new Product[] {
                CreateCyberProduct (defaultOrganisation),
                CreateAssociationsProduct (defaultOrganisation),
                CreateBroadformProduct (defaultOrganisation),
                CreateBusinessInterruptionProduct (defaultOrganisation),
                CreateConsequentialLossProduct (defaultOrganisation),
                CreateDirectorsAndOfficersProduct (defaultOrganisation),
                CreateEmployersGeneralProduct (defaultOrganisation),
                CreateEmployersPracticeProduct (defaultOrganisation),
                CreateFidelityProduct (defaultOrganisation),
                CreateInternetUseLiabilitiesProduct (defaultOrganisation),
                CreateGeneralOrPublicLiabilitiesProduct (defaultOrganisation),
                CreateInsolventInsurerLegalCostsProduct (defaultOrganisation),
                CreateInvestmentDefenseAndSettlementCostsProduct (defaultOrganisation),
                CreateLegalProsecutionDefenseProduct (defaultOrganisation),
                CreateManagementLiabilityProduct (defaultOrganisation),
                CreateMaterialDamageProduct (defaultOrganisation),
                CreateMotorVehicleAndPlantProduct (defaultOrganisation),
                CreateMultimediaProduct (defaultOrganisation),
                CreateProfessionalIndemnityProduct (defaultOrganisation),
                CreateStatutoryLiabilitiesProduct (defaultOrganisation),
                CreateTrusteesIndemnityProduct (defaultOrganisation),
                CreateScaffoldingProduct (defaultOrganisation),
                CreateMarqueeHireProduct (defaultOrganisation),
                CreateAirsidePlantAndEquipmentHireDryProduct (defaultOrganisation),
                CreateAirsidePlantAndEquipmentHireWetProduct (defaultOrganisation),
                CreateLossOfMoneyProduct (defaultOrganisation)
            };

            using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork())
            {
                string[] productNames = _productRepository.FindAll().Where(p => p.IsBaseProduct).Select(p => p.Name).ToArray();
                foreach (Product defaultProduct in baseProducts)
                {
                    if (!productNames.Contains(defaultProduct.Name))
                        _productRepository.Add(defaultProduct);
                    //	Console.WriteLine ("Saving product: " + defaultProduct.Name);
                    //else
                    //	Console.WriteLine ("Skip saving product: " + defaultProduct.Name);
                }
            }
        }

        Product CreateCyberProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Airside Plant and Equipment Hire - Dry", "Hire and configuration professional advice relating to providing Airside Plant and Equipment hire and services - Dry means without operating labout");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", true),
                GetRiskCover("Professions", true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateAssociationsProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Associations", "Associations - Liabilities Covers");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateBroadformProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Broadform Product & Public", "Broadform Product - liability to the public for product and services");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateBusinessInterruptionProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Business Interruption", "Business Interruption  - costs of restoring business operations");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, true, false),
                GetRiskCover("Associations", false, true, false),
                GetRiskCover("Professions", false, true, false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false, true, false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateConsequentialLossProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Consequential Losses", "Consequential Losses - liability to others for negligence and similar failures of duties to others");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false, false, true),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false, false, true),
                GetRiskCover("Agriculture", false, false, true),
                GetRiskCover("Transport", false, false, true),
                GetRiskCover("Marine", false, false, true)
            };
            return product;
        }

        Product CreateDirectorsAndOfficersProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Directors & Officers", "Cover for Directors & Officers for liability arising from being an officeholder");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateEmployersGeneralProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Employers - General", "Cover for employers for liability arising from being an employer excluding employment practices");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateEmployersPracticeProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Employers Practices", "Cover for Employers liability arising from employers failing to meet employment practices mandated by law");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateFidelityProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Fidelity", "Cover for loss arising from loss of money, securities, or inventory resulting from crime such as employee dishonesty, fraud and other criminal acts");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateInternetUseLiabilitiesProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Internet Use Liabilities", "Internet Use Liabilities covers people and businesses for the liabilities to others for wrong use while on the internet such as defamation, IP infringments, incorrect marketing or misleading or transmitting viruses ");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false, false, true),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateGeneralOrPublicLiabilitiesProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "General or Public Liability", "Liability to others excluding product (only in Broadform or Product covers) or services liability (only available in Professional Indemnity or Malpractices covers)");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false, false, true),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateInsolventInsurerLegalCostsProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Insolvent Insurer Legal Costs and Expenses", "Covers for loss of benefit of covers when an insurer recommended for another party, such as client, becomes insolvent - this is usually an extension cover for Professional Liability");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateInvestmentDefenseAndSettlementCostsProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Investment Defence and Settlement Costs", "Liability to others from advice on investing and the legal costs - usually and extension cover to Professional Liability");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateLegalProsecutionDefenseProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Legal Prosecution Defence", "Cover for the costs of defending a prosecution - often an extension to Statutory Liability cover");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false, false, true),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false, false, true),
                GetRiskCover("Agriculture", false, false, true),
                GetRiskCover("Transport", false, false, true),
                GetRiskCover("Marine", false, false, true)
            };
            return product;
        }

        Product CreateManagementLiabilityProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Management Liability", "a broadform set of covers for liabilities that may arise for managers and a buisness in their practices as employers statutory responsibilities and management duties to others");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateMaterialDamageProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Material Damage", "Cover for loss of a physical asset such as a building, stock and/or plant and equipment.  Do not include motor vehicles. May include liability covers and business interruption costs covers");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", true),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateMotorVehicleAndPlantProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Motor Vehicles and Mobile Plant", "Covers for loss and liability arising from ownership and operation of registered and non-registered vehicles.  Can be extended with special covers for hiring out, baliment etc. May cover loss of use costs");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", true),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateMultimediaProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Multimedia", "Over related to production, broadcasting and distribution of content audio or video content.");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateProfessionalIndemnityProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Professional Indemnity", "Cover for loss of others arising from errors or ommissions when providing professional services.");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateStatutoryLiabilitiesProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Statutory Liabilities", "Civil liability arising from breach of a statutory duty under legislation such as the Health and Safety Act");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false, false, true),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false, false, true),
                GetRiskCover("Agriculture", false, false, true),
                GetRiskCover("Transport", false, false, true),
                GetRiskCover("Marine", false, false, true)
            };
            return product;
        }

        Product CreateTrusteesIndemnityProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Trustees Indemnity", "Cover for errors and omissions that give rise to liability while acting as a Trustee");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false, false, true),
                GetRiskCover("Business", false, false, true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateScaffoldingProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Scaffolding", "Hire, erection, design and configuration professional advice relating to providing scaffolding hire and services");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", true, false, true),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateMarqueeHireProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Marquee Hire", "Hire, erection, design and configuration professional advice relating to providing marquee hire sales and services");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false, false, true),
                GetRiskCover("Goods", true, false, true),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateAirsidePlantAndEquipmentHireDryProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Airside Plant and Equipment Hire - Dry", "Hire and configuration professional advice relating to providing Airside Plant and Equipment hire and services - Dry means without operating labout");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateAirsidePlantAndEquipmentHireWetProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Airside Plant and Equipment hire and services - Wet", "Hire and configuration professional advice relating to providing Airside Plant and Equipment hire and services - Wet means privinding operating labour as part of the hire");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", false),
                GetRiskCover("Business", true),
                GetRiskCover("Associations", false),
                GetRiskCover("Professions", false),
                GetRiskCover("Goods", false),
                GetRiskCover("Agriculture", false),
                GetRiskCover("Transport", false),
                GetRiskCover("Marine", false)
            };
            return product;
        }

        Product CreateLossOfMoneyProduct(Organisation organisation)
        {
            Product product = CreateProduct(organisation, "Loss of money", "Covers theft and other losses relating to money that is not covered by Fidelity Insurance");
            product.RiskCategoriesCovered = new List<RiskCover> {
                GetRiskCover("People", true, false, false),
                GetRiskCover("Business", false, false, false),
                GetRiskCover("Associations", false, false, false),
                GetRiskCover("Professions", false, false, false),
                GetRiskCover("Goods", false, false, false),
                GetRiskCover("Agriculture", false, false, false),
                GetRiskCover("Transport", false, false, false),
                GetRiskCover("Marine", false, false, false)
            };
            return product;
        }

        RiskCover GetRiskCover(string categoryName, bool coverAll)
        {
            return GetRiskCover(categoryName, coverAll, coverAll, coverAll);
        }

        RiskCover GetRiskCover(string categoryName, bool loss, bool interuption, bool thirdParty)
        {
            var riskCover = new RiskCover(null)
            {
                BaseRisk = _riskCategoryRepository.FindAll().FirstOrDefault(rc => rc.Name == categoryName),
                CoverAll = loss && interuption && thirdParty,
                Loss = loss,
                Interuption = interuption,
                ThirdParty = thirdParty
            };
            return riskCover;
        }

        Product CreateProduct(Organisation owner, string name, string description)
        {
            var product = new Product(null, owner.Id, name)
            {
                Description = description,
                Active = false,
                IsBaseProduct = true,
                Public = false,
                Published = false,
                UnderwritingEnabled = false
            };
            return product;
        }
    }
}

