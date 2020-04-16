﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{

    public class InformationViewModel : BaseViewModel
    {
        public InformationViewModel() { }
        public InformationViewModel(Domain.Entities.Programme Programme)
        {
            PMINZEPLViewModel = new PMINZEPLViewModel();
            CLIViewModel = new CLIViewModel();
            PMINZPIViewModel = new PMINZPIViewModel();
            DAOLIViewModel = new DAOLIViewModel();
            ClaimsHistoryViewModel = new ClaimsHistoryViewModel();
            RevenueDataViewModel = new RevenueDataViewModel(Programme);
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
        public IEnumerable<LocationViewModel> Locations { get; set; }
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
        public PMINZEPLViewModel PMINZEPLViewModel { get; set; }
        public CLIViewModel CLIViewModel { get; set; }
        public PMINZPIViewModel PMINZPIViewModel { get; set; }
        public DAOLIViewModel DAOLIViewModel { get; set; }
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
                    Pecentage = 0
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
                    Id = template.Id,
                    Location = template.Location,
                    Pecentage = 0,
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

            //var des = this.GetType();
            //var src = info.GetType();
            //foreach (var item in src.GetType())
            //{
            //    var property = (des)des.GetMember
            //    if (item.Name.Replace("get_", string.Empty) == des.Name)
            //    {
            //        var test = item.GetType();
            //        var test2 = field.GetType();
            //        if (item.GetType() == field.GetType())
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }
            //}
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
    public class PMINZEPLViewModel
    {
        public PMINZEPLViewModel()
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
            HasRenewalOptions = GetSelectListOptions();
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
        public IList<SelectListItem> HasRenewalOptions { get; set; }
        public IList<SelectListItem> HasLocationOptions { get; set; }

        public int CoverAmount { get; set; }
        public string DateLapsed { get; set; }
        public string RetroactiveDate { get; set; }
        public string InsurerName { get; set; }        
    }
    public class PMINZPIViewModel
    {
        public PMINZPIViewModel()
        {
            ContractingServicesOptions = GetContractingServicesOptions();
            HasStandardTermsOptions = GetSelectListOptions();
            HasNegotiateOptions = GetSelectListOptions();
            HasNoAgreementOptions = GetSelectListOptions();
            HasOwnPIOptions = GetSelectListOptions();
            HasBoundContractOptions = GetSelectListOptions();
            HasEngagementLetterOptions = GetSelectListOptions();
            HasRecordedOptions = GetSelectListOptions();
            HasDiaryRecordOptions = GetSelectListOptions();
            HasComplaintOptions = GetSelectListOptions();
            HasEngageOptions = GetSelectListOptions();
            HasDisciplinaryOptions = GetSelectListOptions();
            HasClaimsAgainstOptions = GetSelectListOptions();
            HasResponsibleOptions = GetSelectListOptions();
            HasClaimsAgainstOptions2 = GetSelectListOptions();
            HasRefundOptions = GetSelectListOptions();
            HasSuedOptions = GetSelectListOptions();
            HasDisputeOptions = GetSelectListOptions();
            HasPenaltyOptions = GetSelectListOptions();
            HasManagedProjectOptions = GetSelectListOptions();
            HasIncludedDesignOptions = GetSelectListOptions();
            HasEngineerOptions = GetSelectListOptions();
        }
        private IList<SelectListItem> GetContractingServicesOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = " Network security", Value = "1"
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
        public IList<SelectListItem> HasPenaltyOptions { get; set; }
        public IList<SelectListItem> HasManagedProjectOptions { get; set; }
        public IList<SelectListItem> HasIncludedDesignOptions { get; set; }
        public IList<SelectListItem> HasEngineerOptions { get; set; }

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

        public int ShareholderTotal { get; set; }
        public int AssetTotal { get; set; }
        public int DebtTotal { get; set; }
        public DateTime FormDate { get; set; }
        public string CompanyNameDetails { get; set; }
        public string ClaimDetails { get; set; }
        public string CircumstanceDetails { get; set; }
        public string InvestigationDetails { get; set; }
        public string DeclinedDetails { get; set; }
        public string ReceivershipDetails { get; set; }
        public string CriminalDetails { get; set; }
        public string ProcecutionDetails { get; set; }
        public string ObligationDetails { get; set; }


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
}
