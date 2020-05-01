using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{

    public class InformationViewModel : BaseViewModel
    {
        public InformationViewModel() { }
        public InformationViewModel(ClientInformationSheet ClientInformationSheet)
        {
            ELViewModel = new ELViewModel(); //Employment Liability Insurance
            EPLViewModel = new EPLViewModel(); //Employers Practices Insurance
            CLIViewModel = new CLIViewModel(); //Cyber Liability Insurance
            PIViewModel = new PIViewModel(); //Professional Indemnity
            DAOLIViewModel = new DAOLIViewModel(); //Directors officers liability
            GLViewModel = new GLViewModel(); //General liability 
            SLViewModel = new SLViewModel(); //Statutory Liability
            ClaimsHistoryViewModel = new ClaimsHistoryViewModel();
            RevenueDataViewModel = new RevenueDataViewModel(ClientInformationSheet.Programme.BaseProgramme);
            LocationViewModel = new LocationViewModel(ClientInformationSheet);
            ProjectViewModel = new ProjectViewModel(ClientInformationSheet);
        }
        public Domain.Entities.Programme Programme;
        public string CompanyName { get; set; }
        public Guid AnswerSheetId { get; set; }
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string SectionView { get; set; }
        public List<InformationSection> Section { get; set; }
        public List<string> ListProductName { get; set; }
        public IEnumerable<InformationSectionViewModel> Sections { get; set; }

        // TODO - find a better way to pass these in
        public bool HasVehicles { get; set; }
        public IEnumerable<VehicleViewModel> AllVehicles { get; set; }
        public IEnumerable<VehicleViewModel> RegisteredVehicles { get; set; }
        public IEnumerable<VehicleViewModel> UnregisteredVehicles { get; set; }
        public IEnumerable<OrganisationalUnitViewModel> OrganisationalUnits { get; set; }
        public LocationViewModel LocationViewModel { get; set; }
        public IEnumerable<OrganisationViewModel> InterestedParties { get; set; }
        public OrganisationalUnitVM OrganisationalUnitsVM { get; set; }
        public IEnumerable<SelectListItem> ClaimProducts { get; set; }
        public IEnumerable<SelectListItem> AvailableOrganisations { get; set; }
        public OrganisationDetailsVM OrganisationDetails { get; set; }
        public UserDetailsVM UserDetails { get; set; }
        public IEnumerable<BuildingViewModel> Buildings { get; set; }
        public IEnumerable<ClientInformationAnswer> ClientInformationAnswers { get; set; }
        public IEnumerable<WaterLocationViewModel> WaterLocations { get; set; }
        public IEnumerable<BoatViewModel> Boats { get; set; }
        public List<SelectListItem> BoatUseslist { get; set; }
        public IEnumerable<OrganisationViewModel> MarinaLocations { get; set; }
        public List<SelectListItem> InterestedPartyList { get; set; }
        public List<SelectListItem> SkipperList { get; set; }
        public IEnumerable<BoatUseViewModel> BoatUse { get; set; }
        public IEnumerable<ClaimViewModel> Claims { get; set; }
        public IEnumerable<OrganisationViewModel> Operators { get; set; }
        public string Advisory { get; set; }
        public IEnumerable<BusinessContractViewModel> BusinessContracts { get; set; }
        public RevenueDataViewModel RevenueDataViewModel { get; set; }
        public SharedRoleViewModel SharedRoleViewModel { get; set; }
        public ClaimsHistoryViewModel ClaimsHistoryViewModel { get; set; }
        public EPLViewModel EPLViewModel { get; set; }
        public ELViewModel ELViewModel { get; set; }
        public CLIViewModel CLIViewModel { get; set; }
        public PIViewModel PIViewModel { get; set; }
        public DAOLIViewModel DAOLIViewModel { get; set; }
        public GLViewModel GLViewModel { get; set; }
        public SLViewModel SLViewModel { get; set; }
        public ProjectViewModel ProjectViewModel { get;set;}
        public IList<string> Wizardsteps { get; set; }
        public ClientInformationSheet ClientInformationSheet { get; internal set; }
        public ClientProgramme ClientProgramme { get; internal set; }
        public ClientAgreement ClientAgreement { get; internal set; }
    }

    public class InformationSectionViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<InformationItemViewModel> Items { get; set; }

        public string CustomView { get; set; }

        public int Position { get; set; }
    }
    public class InformationItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ControlType { get; set; }
        public string Class { get; set; }

        public string Label { get; set; }

        public string Icon { get; set; }

        public int Width { get; set; }

        public ItemType Type { get; set; }

        public string DefaultText { get; set; }

        public IEnumerable<SelectListItem> Options { get; set; }

        public IList<InformationItem> ConditionalList { get; set; }

        public string Value { get; set; }
        public Rule Rule { get; set; }

        public IEnumerable<LabelPersentageViewModel> LabelPercentageValue { get; set; }

        public InformationItemViewModel()
        {
            Options = new List<SelectListItem>();
        }

    }
    public enum ItemType
    {
        LABEL,
        TEXTBOX,
        TEXTAREA,
        DROPDOWNLIST,
        PERCENTAGEBREAKDOWN,
        MULTISELECT,
        JSBUTTON,
        SUBMITBUTTON,
        MOTORVEHICLELIST,
        STATICVEHICLEPLANTLIST,
        SECTIONBREAK
    }
    public class OrganisationalUnitVM : List<OrganisationalUnitViewModel>
    {
        public IList<SelectListItem> OrganisationalUnits { get; set; }

        public string[] SelectedOrganisationalUnits { get; set; }
    }
    public class UserDetailsVM
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string PostalAddress { get; set; }

        public string StreetAddress { get; set; }
    }
    public class OrganisationDetailsVM
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }

        public Organisation ToEntity(User creatingUser)
        {
            Organisation org = new Organisation(creatingUser, Name);
            UpdateEntity(org);
            return org;
        }

        public Organisation UpdateEntity(Organisation org)
        {
            org.ChangeOrganisationName(org.Name);
            org.Phone = Phone;
            org.Domain = Website;
            org.Email = Email;
            return org;
        }

        public static OrganisationDetailsVM FromEntity(Organisation org)
        {
            OrganisationDetailsVM model = new OrganisationDetailsVM
            {
                Name = org.Name,
                Phone = org.Phone,
                Website = org.Domain,
                Email = org.Email
            };
            return model;
        }
    }
    public class RevenueDataViewModel
    {
        public RevenueDataViewModel() { }
        public RevenueDataViewModel(Domain.Entities.Programme programme)
        {
            Territories = GetTerritories(programme);
            Activities = GetActivities(programme);
            AdditionalActivityViewModel = new AdditionalActivityViewModel();
        }
        private IList<BusinessActivity> GetActivities(Domain.Entities.Programme programme)
        {
            Activities = new List<BusinessActivity>();
            foreach (var template in programme.BusinessActivityTemplates)
            {
                Activities.Add(new BusinessActivity(null)
                {
                    Description = template.Description,
                    AnzsciCode = template.AnzsciCode,
                    Selected = false,
                    Percentage = 0
                });
            }
            return Activities;
        }
        private IList<Territory> GetTerritories(Domain.Entities.Programme programme)
        {
            Territories = new List<Territory>();
            foreach (var template in programme.TerritoryTemplates)
            {
                Territories.Add(new Territory(null)
                {
                    TemplateId = template.Id,
                    Location = template.Location,
                    Percentage = 0,
                    Selected = false
                });
            }
            return Territories;
        }
        public IList<Territory> Territories { get; set; }
        public IList<BusinessActivity> Activities { get; set; }
        public decimal NextFinancialYearTotal { get; set; }
        public decimal CurrentYearTotal { get; set; }
        public decimal LastFinancialYearTotal { get; set; }
        public AdditionalActivityViewModel AdditionalActivityViewModel { get; set; }
    }
    public class AdditionalActivityViewModel
    {
        public AdditionalActivityViewModel(AdditionalActivityInformation additionalActivityInformation = null)
        {
            SetOptions();
        }

        public void SetOptions()
        {
            HasInspectionReportOptions = GetSelectListOptions();
            HasDisclaimerReportsOptions = GetSelectListOptions();
            HasObservationServicesOptions = GetSelectListOptions();
            HasRecommendedCladdingOptions = GetSelectListOptions();
            HasStateSchoolOptions = GetSelectListOptions();
            HasIssuedCertificatesOptions = GetSelectListOptions();
        }

        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        public IList<SelectListItem> HasInspectionReportOptions { get; set; }
        public IList<SelectListItem> HasDisclaimerReportsOptions { get; set; }
        public IList<SelectListItem> HasObservationServicesOptions { get; set; }
        public IList<SelectListItem> HasRecommendedCladdingOptions { get; set; }
        public IList<SelectListItem> HasStateSchoolOptions { get; set; }
        public IList<SelectListItem> HasIssuedCertificatesOptions { get; set; }
        public string QualificationDetails { get; set; }
        public string ValuationDetails { get; set; }
        public string OtherDetails { get; set; }
        public string RebuildDetails { get; set; }
        public string InspectionReportDetails { get; set; }
        public string OtherProjectManagementDetails { get; set; }
        public string NonProjectManagementDetails { get; set; }
        public decimal ConstructionCommercialDetails { get; set; }
        public decimal ConstructionDwellingDetails { get; set; }
        public decimal ConstructionIndustrialDetails { get; set; }
        public decimal ConstructionInfrastructureDetails { get; set; }
        public decimal ConstructionSchoolDetails { get; set; }
        public string ConstructionEngineerDetails { get; set; }
        
    }
    public class SharedRoleViewModel
    {
        public SharedRoleViewModel()
        {
            SharedRoles = new List<SelectListItem>();
            SharedDataRoles = new List<SharedDataRole>();
        }
        public string OtherProfessionId { get; set; }
        public IList<SelectListItem> SharedRoles { get; set; }
        public IList<SharedDataRole> SharedDataRoles { get; set; }
    }
    public class BusinessActivityViewModel
    {
        public int Classification { get; set; }
        public string AnzsciCode { get; set; }
        public string Description { get; set; }
    }
    public class ClaimsHistoryViewModel
    {
        public ClaimsHistoryViewModel()
        {
            HasDamageLossOptions = GetSelectListOptions();
            HasWithdrawnOptions = GetSelectListOptions();
            HasRefusedOptions = GetSelectListOptions();
            HasStatutoryOffenceOptions = GetSelectListOptions();
            HasLiquidationOptions = GetSelectListOptions();
        }

        public string DamageLossDetails { get; set; }
        public string WithdrawnDetails { get; set; }
        public string RefusedDetails { get; set; }
        public string StatutoryOffenceDetails { get; set; }
        public string LiquidationDetails { get; set; }
        public IList<SelectListItem> HasDamageLossOptions { get; set; }
        public IList<SelectListItem> HasWithdrawnOptions { get; set; }
        public IList<SelectListItem> HasRefusedOptions { get; set; }
        public IList<SelectListItem> HasStatutoryOffenceOptions { get; set; }
        public IList<SelectListItem> HasLiquidationOptions { get; set; }
        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
    }
    public class SLViewModel
    {
        public SLViewModel()
        {
            HasSLOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
        }

        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        public IList<SelectListItem> HasSLOptions { get; set; }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }
    }
    public class ELViewModel
    {
        public ELViewModel()
        {
            HasELOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
        }

        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        public IList<SelectListItem> HasELOptions { get; set; }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }

    }
    public class EPLViewModel
    {
        public EPLViewModel()
        {
            HasEPLOptions = GetSelectListOptions();
            CoveredOptions = GetSelectListOptions();
            LegalAdvisorOptions = GetSelectListOptions();
            CasualBasisOptions = GetSelectListOptions();
            DefinedOptions = GetSelectListOptions();
            ManualOptions = GetSelectListOptions();
            PostingNoticesOptions = GetSelectListOptions();
            StaffRedundancyOptions = GetSelectListOptions();
            HasEPLIOptions = GetSelectListOptions();
            IsInsuredClaimOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
        }
        public int TotalEmployees { get; set; }
        public string InsuredClaimDetails { get; set; }
        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        public IList<SelectListItem> HasEPLOptions { get; set; }
        public IList<SelectListItem> HasEPLIOptions { get; set; }
        public IList<SelectListItem> CoveredOptions { get; set; }
        public IList<SelectListItem> LegalAdvisorOptions { get; set; }
        public IList<SelectListItem> CasualBasisOptions { get; set; }
        public IList<SelectListItem> DefinedOptions { get; set; }
        public IList<SelectListItem> ManualOptions { get; set; }
        public IList<SelectListItem> PostingNoticesOptions { get; set; }
        public IList<SelectListItem> StaffRedundancyOptions { get; set; }
        public IList<SelectListItem> IsInsuredClaimOptions { get; set; }
        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }

    }
    public class CLIViewModel
    {
        public CLIViewModel()
        {
            HasCLIOptions = GetSelectListOptions();
            HasSecurityOptions = GetSelectListOptions();
            HasAccessControlOptions = GetSelectListOptions();
            HasProhibitAccessOptions = GetSelectListOptions();
            HasBackupOptions = GetSelectListOptions();
            HasDomiciledOperationOptions = GetSelectListOptions();
            HasActivityOptions = GetSelectListOptions();
            HasConfidencialOptions = GetSelectListOptions();
            HasBreachesOptions = GetSelectListOptions();
            HasKnowledgeOptions = GetSelectListOptions();
            HasOptionalCLEOptions = GetSelectListOptions();
            HasProceduresOptions = GetSelectListOptions();
            HasApprovedVendorsOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
            HasLocationOptions = GetSelectListOptions();
        }
        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        public IList<SelectListItem> HasCLIOptions { get; set; }
        public IList<SelectListItem> HasSecurityOptions { get; set; }
        public IList<SelectListItem> HasAccessControlOptions { get; set; }
        public IList<SelectListItem> HasProhibitAccessOptions { get; set; }
        public IList<SelectListItem> HasBackupOptions { get; set; }
        public IList<SelectListItem> HasDomiciledOperationOptions { get; set; }
        public IList<SelectListItem> HasActivityOptions { get; set; }
        public IList<SelectListItem> HasConfidencialOptions { get; set; }
        public IList<SelectListItem> HasBreachesOptions { get; set; }
        public IList<SelectListItem> HasKnowledgeOptions { get; set; }
        public IList<SelectListItem> HasOptionalCLEOptions { get; set; }
        public IList<SelectListItem> HasProceduresOptions { get; set; }
        public IList<SelectListItem> HasApprovedVendorsOptions { get; set; }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        
        public IList<SelectListItem> HasLocationOptions { get; set; }

        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }        
    }
    public class PIViewModel
    {
        public PIViewModel()
        {
            ContractingServicesOptions = GetContractingServicesOptions();
            HasStandardTermsOptions = GetSometimesSelectListOptions();
            HasNegotiateOptions = GetSometimesSelectListOptions();
            HasNoAgreementOptions = GetSometimesSelectListOptions();
            HasOwnPIOptions = GetSometimesSelectListOptions();
            HasBoundContractOptions = GetSometimesSelectListOptions();
            HasEngagementLetterOptions = GetSometimesSelectListOptions();
            HasRecordedOptions = GetSometimesSelectListOptions();
            HasDiaryRecordOptions = GetSometimesSelectListOptions();
            HasComplaintOptions = GetSometimesSelectListOptions();
            HasEngageOptions = GetSelectListOptions(); 
            HasDisciplinaryOptions = GetSelectListOptions();
            HasClaimsAgainstOptions = GetSelectListOptions();
            HasResponsibleOptions = GetSelectListOptions();
            HasClaimsAgainstOptions2 = GetSelectListOptions();
            HasRefundOptions = GetSelectListOptions();
            HasSuedOptions = GetSelectListOptions();
            HasDisputeOptions = GetSelectListOptions();
            HasDisputeOptions2 = GetSelectListOptions();
            HasPenaltyOptions = GetSelectListOptions();
            HasManagedProjectOptions = GetSelectListOptions();
            HasIncludedDesignOptions = GetSelectListOptions();
            HasEngineerOptions = GetSelectListOptions();
            HasAluminium = GetSelectListOptions();
            HasPracticeClaimOptions = GetSelectListOptions();
            HasThirdPartyOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
            HasDANZOptions = GetSelectListOptions();
            HasSalesRelateOptions = GetSelectListOptions();
            HasSubstantialChangeOptions = GetSelectListOptions();
            IsFormInPracticeOptions = GetSelectListOptions();
            HasStandardContractFormOptions = GetSelectListOptions();
            HasAnyOtherFormOptions = GetSelectListOptions();
            HasPersonnelDismissedOptions = GetSelectListOptions();
            HasReferencesObtainedOptions = GetSelectListOptions();
            HasLeakyBuildingCoverOptions = GetSelectListOptions();
            HasRiskManagementOptions = GetRiskManagementOptions();
            HasRetainedDocumentOptions = GetAlternativeSelectListOptions();
            HasComplaintAlternativeOptions = GetAlternativeSelectListOptions();
            HasCircumstanceAriseOptions = GetAlternativeSelectListOptions();
        }
        private IList<SelectListItem> GetContractingServicesOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "Network security", Value = "1"
                },
                new SelectListItem
                {
                    Text = "On-line stock trading", Value = "2"
                },
                new SelectListItem
                {
                    Text = "Funds management / investment and financial advisingy", Value = "3"
                },
                new SelectListItem
                {
                    Text = "Manufacturing control processes", Value = "4"
                },
                new SelectListItem
                {
                    Text = "Oil & gas", Value = "5"
                },
                new SelectListItem
                {
                    Text = "Mining", Value = "6"
                },
                new SelectListItem
                {
                    Text = "Medical", Value = "7"
                },
                new SelectListItem
                {
                    Text = "Defence", Value = "8"
                },
                new SelectListItem
                {
                    Text = "None of the above", Value = "10"
                },
            };
        }
        private IList<SelectListItem> GetRiskManagementOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "Always obtain clients instructions in writing ", Value = "1"
                },
                new SelectListItem
                {
                    Text = "If applicable, check rates notice to ensure the client owns the land", Value = "2"
                },
                new SelectListItem
                {
                    Text = "Set out a budget and actively review it with your client as may be required ", Value = "3"
                },
                new SelectListItem
                {
                    Text = "Review contract conditions to ensure requirements are within your professional indemnity insurance conditions or policy limits", Value = "4"
                },
                new SelectListItem
                {
                    Text = "Have your client sign off each page of the contract documents", Value = "5"
                },
                new SelectListItem
                {
                    Text = "None of the above", Value = "10"
                },
            };
        }
        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        private IList<SelectListItem> GetSometimesSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { 
                    Text = "No", Value = "2" 
                },
                new SelectListItem
                {
                    Text = "Sometimes", Value = "3"
                }
            };
        }
        private IList<SelectListItem> GetAlternativeSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes for all of the above", Value = "1"
                },
                new SelectListItem
                {
                    Text = "No", Value = "2"
                },
                new SelectListItem
                {
                    Text = "Don't know", Value = "3"
                }
            };
        }
        public IList<SelectListItem> ContractingServicesOptions { get; set; }
        public IList<SelectListItem> HasStandardTermsOptions { get; set; }
        public IList<SelectListItem> HasNegotiateOptions { get; set; }
        public IList<SelectListItem> HasNoAgreementOptions { get; set; }
        public IList<SelectListItem> HasOwnPIOptions { get; set; }
        public IList<SelectListItem> HasBoundContractOptions { get; set; }
        public IList<SelectListItem> HasEngagementLetterOptions { get; set; }
        public IList<SelectListItem> HasRecordedOptions { get; set; }
        public IList<SelectListItem> HasDiaryRecordOptions { get; set; }
        public IList<SelectListItem> HasComplaintOptions { get; set; }
        public IList<SelectListItem> HasEngageOptions { get; set; }
        public IList<SelectListItem> HasDisciplinaryOptions { get; set; }
        public IList<SelectListItem> HasClaimsAgainstOptions { get; set; }
        public IList<SelectListItem> HasClaimsAgainstOptions2 { get; set; }
        public IList<SelectListItem> HasResponsibleOptions { get; set; }
        public IList<SelectListItem> HasRefundOptions { get; set; }
        public IList<SelectListItem> HasSuedOptions { get; set; }
        public IList<SelectListItem> HasDisputeOptions { get; set; }
        public IList<SelectListItem> HasDisputeOptions2 { get; set; }        
        public IList<SelectListItem> HasPenaltyOptions { get; set; }
        public IList<SelectListItem> HasManagedProjectOptions { get; set; }
        public IList<SelectListItem> HasIncludedDesignOptions { get; set; }
        public IList<SelectListItem> HasEngineerOptions { get; set; }
        public IList<SelectListItem> HasAluminium { get; set; }
        public IList<SelectListItem> HasPracticeClaimOptions { get; set; }
        public IList<SelectListItem> HasThirdPartyOptions { get; set; }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        public IList<SelectListItem> HasDANZOptions { get; set; }
        public IList<SelectListItem> HasSalesRelateOptions { get; set; }
        public IList<SelectListItem> HasSubstantialChangeOptions { get; set; }
        public IList<SelectListItem> HasStandardContractFormOptions { get; set; }
        public IList<SelectListItem> IsFormInPracticeOptions { get; set; }
        public IList<SelectListItem> HasAnyOtherFormOptions { get; set; }
        public IList<SelectListItem> HasPersonnelDismissedOptions { get; set; }
        public IList<SelectListItem> HasReferencesObtainedOptions { get; set; }
        public IList<SelectListItem> HasLeakyBuildingCoverOptions { get; set; }
        public IList<SelectListItem> HasRiskManagementOptions { get; set; }
        public IList<SelectListItem> HasRetainedDocumentOptions { get; set; }
        public IList<SelectListItem> HasComplaintAlternativeOptions { get; set; }
        public IList<SelectListItem> HasCircumstanceAriseOptions { get; set; }


        public string EngageDetails { get; set; }
        public string DisciplinaryDetails { get; set; }
        public string ClaimDetails { get; set; }
        public string ClaimDetails2 { get; set; }
        public string ResponsibleDetails { get; set; }
        public string RefundDetails { get; set; }
        public string SuedDetails { get; set; }
        public string DisputeDetails { get; set; }
        public string PenaltyDetails { get; set; }
        public string ManagedProjectDetails { get; set; }
        public string IncludedDesignDetails { get; set; }
        public string EngineerDetails { get; set; }
        public string ContractingServicesDetails { get; set; }
        public int CoverAmount { get; set; }
        public int PercentFees { get; set; }
        public string PercentDetails { get; set; }
        public string PersonnelDismisedDetails { get; set; }
        public string FormInPracticeDetails { get; set; }
        public string UseInCircumstancesDetails { get; set; }
        
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }
        public string SubstantialChangeDetails { get; set; }
        
    }
    public class DAOLIViewModel
    {
        public DAOLIViewModel()
        {
            HasDAOLIOptions = GetSelectListOptions();
            HasClaimOptions = GetSelectListOptions();
            HasCircumstanceOptions = GetSelectListOptions();
            HasInvestigationOptions = GetSelectListOptions();
            HasDeclinedOptions = GetSelectListOptions();
            HasReceivershipOptions = GetSelectListOptions();
            HasCriminalOptions = GetSelectListOptions();
            HasProcecutionOptions = GetSelectListOptions();
            HasObligationOptions = GetSelectListOptions();
            HasDebtsOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
            FormDate = DateTime.Now;
        }

        public IList<SelectListItem> HasDAOLIOptions { get; set; }
        public IList<SelectListItem> HasClaimOptions { get; set; }
        public IList<SelectListItem> HasCircumstanceOptions { get; set; }
        public IList<SelectListItem> HasInvestigationOptions { get; set; }
        public IList<SelectListItem> HasDeclinedOptions { get; set; }
        public IList<SelectListItem> HasReceivershipOptions { get; set; }
        public IList<SelectListItem> HasCriminalOptions { get; set; }
        public IList<SelectListItem> HasProcecutionOptions { get; set; }
        public IList<SelectListItem> HasObligationOptions { get; set; }
        public IList<SelectListItem> HasDebtsOptions { get; set; }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        

        public int ShareholderTotal { get; set; }
        public int AssetTotal { get; set; }
        public int DebtTotal { get; set; }
        public DateTime FormDate { get; set; }
        public string CompanyNameDetails { get; set; }
        public string CircumstanceDetails { get; set; }
        public string InvestigationDetails { get; set; }
        public string DeclinedDetails { get; set; }
        public string ReceivershipDetails { get; set; }
        public string CriminalDetails { get; set; }
        public string ProcecutionDetails { get; set; }
        public string ObligationDetails { get; set; }
        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }


        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }

    }
    public class GLViewModel
    {
        public GLViewModel()
        {
            HasGLOptions = GetSelectListOptions();
            HasHigherGLOptions = GetSelectListOptions();
            HasExistingPolicyOptions = GetSelectListOptions();
        }
        private IList<SelectListItem> GetSelectListOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "-- Select --", Value = "0"
                },
                new SelectListItem
                {
                    Text = "Yes", Value = "1"
                },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }
        public IList<SelectListItem> HasGLOptions { get; set; }
        public IList<SelectListItem> HasHigherGLOptions { get; set; }
        public IList<SelectListItem> HasExistingPolicyOptions { get; set; }
        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }
    }

    public class ProjectViewModel
    {
        public ProjectViewModel(ClientInformationSheet clientInformationSheet)
        {
            BusinessContracts = GetBusinessContracts(clientInformationSheet);
            ResponsibilityOptions = GetResponsibilityOptions();
        }

        private IList<BusinessContract> GetBusinessContracts(ClientInformationSheet clientInformationSheet)
        {
            BusinessContracts = new List<BusinessContract>();
            foreach (var businessContract in clientInformationSheet.BusinessContracts)
            {
                BusinessContracts.Add(businessContract);
            }
            return BusinessContracts;
        }

        private IList<SelectListItem> GetResponsibilityOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "Proj. Director", Value = "1"
                },
                new SelectListItem
                {
                    Text = "Proj. Manager", Value = "2"
                },
                new SelectListItem
                { 
                    Text = "Proj. Coordinator/Administrator", Value = "3" 
                },
                new SelectListItem
                {
                    Text = "Proj. Engineers", Value = "3"
                }
            };
        }

        public IList<BusinessContract> BusinessContracts { get; set; }
        public IList<SelectListItem> ResponsibilityOptions { get; set; }
        public string ProjectDescription { get; set; }
        public string Fees { get; set; }
        public string ConstructionValue { get; set; }
        public string ProjectDuration { get; set; }
        
    }
}
