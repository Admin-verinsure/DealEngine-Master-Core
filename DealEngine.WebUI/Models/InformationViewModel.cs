using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class InformationViewModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public Guid AnswerSheetId { get; set; }
        public Boolean IsChange { get; set; }
        public Guid MilestoneId { get; set; }
        public string MilestoneStatus { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string AgreementStatus { get; set; }
        public string Status { get; set; }
        public string SheetStatus { get; set; }
        public string SectionView { get; set; }
        //public string UserRole { get; set; }
        public IEnumerable<String> UserRole { get; set; }
        public IEnumerable<InformationItem> SectionItems { get; set; }
        public List<InformationSection> Section { get; set; }
        public List<string> ListProductName { get; set; }

        public IEnumerable<InformationSectionViewModel> Sections { get; set; }

        // TODO - find a better way to pass these in
        public bool HasVehicles { get; set; }

        public IEnumerable<VehicleViewModel> AllVehicles { get; set; }

        public IEnumerable<VehicleViewModel> RegisteredVehicles { get; set; }

        public IEnumerable<VehicleViewModel> UnregisteredVehicles { get; set; }

        public virtual IList<OrganisationalUnit> OrganisationalUnit { get; set; }

        public IEnumerable<OrganisationalUnitViewModel> OrganisationalUnits { get; set; }

        public IEnumerable<LocationViewModel> Locations { get; set; }

        public IEnumerable<OrganisationViewModel> InterestedParties { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationalUnitVM OrganisationalUnitsVM { get; set; }

        public IEnumerable<ProductItem> AvailableProducts { get; set; }

        public OrganisationDetailsVM OrganisationDetails { get; set; }

        public UserDetailsVM UserDetails { get; set; }

        public IEnumerable<BuildingViewModel> Buildings { get; set; }

        public IEnumerable<ClientInformationAnswer> ClientInformationAnswers { get; set; }


        public IEnumerable<WaterLocationViewModel> WaterLocations { get; set; }

        public IEnumerable<BoatViewModel> Boats { get; set; }

        public List<SelectListItem> BoatUseslist { get; set; }

        public IEnumerable<OrganisationViewModel> MarinaLocations { get; set; }

        public string[] SelectedBoatUse { get; set; }

        public List<SelectListItem> InterestedPartyList { get; set; }

        public List<SelectListItem> SkipperList { get; set; }

        public string[] SelectedInterestedParty { get; set; }

        public IEnumerable<BoatUseViewModel> BoatUse { get; set; }

        public IEnumerable<ClaimViewModel> Claims { get; set; }

        public IEnumerable<OrganisationViewModel> Operators { get; set; }
        public string Advisory { get; set; }
        //public IEnumerable<SectionBuilderViewModel> SectionBuilder { get; set; }
        public Guid ClientProgrammeID { get; set; }
        public IEnumerable<BusinessContractViewModel> BusinessContracts { get; set; }
        public RevenueByActivityViewModel RevenueByActivityViewModel { get; set; }
        public SharedRoleViewModel SharedRoleViewModel { get; set; }
        public IList<string> Wizardsteps { get; set; }
    }

    public class InformationSectionViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<InformationItemViewModel> Items { get; set; }

        public string CustomView { get; set; }

        public int Position { get; set; }
    }

    public class SectionBuilderViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<InformationItem> Items { get; set; }

        public string CustomView { get; set; }

        public int Position { get; set; }
    }

    public class MilestoneAdvisoryVM
    {
        public Guid Id { get; set; }
        public bool HasTriggered { get; set; }
        public string Method { get; set; }
        public Advisory Advisory { get; set; }
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

        public string Value { get; set; }

        public IEnumerable<LabelPersentageViewModel> LabelPercentageValue { get; set; }

        public virtual InformationItemConditionalViewModel Conditional { get; set; }

        public InformationItemViewModel()
        {
            Options = new List<SelectListItem>();
        }

    }

    public class InformationItemConditionalViewModel
    {
        public virtual string TriggerValue { get; set; }

        public virtual int VisibilityOnTrigger { get; set; }

        public virtual IEnumerable<InformationItemViewModel> Targets { get; set; }
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

    public class RevenueByActivityViewModel
    {
        public bool IsTradingOutsideNZ { get; set; }
        public IList<SelectListItem> Territories { get; set; }
        public IList<SelectListItem> Activities { get; set; }
        public decimal NextFincialYear { get; set; }
        public decimal CurrentYear { get; set; }
        public decimal LastFinancialYear { get; set; }
        public RevenueByActivity RevenueData { get; set; }
        public AdditionalActivityViewModel AdditionalInformation { get; set; }
    }

    public class AdditionalActivityViewModel
    {
        public AdditionalActivityViewModel()
        {
            InspectionReportBoolId = new List<SelectListItem>()
            {
                new SelectListItem
                { Text = "Select Option", Value = "" },
                new SelectListItem
                { Text = "Yes", Value = "1" },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
            ValuationBoolId = new List<SelectListItem>()
            {
                new SelectListItem
                { Text = "Select Option", Value = "" },
                new SelectListItem
                { Text = "Yes", Value = "1" },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
            SchoolsDesignWorkBoolId = new List<SelectListItem>()
            {
                new SelectListItem
                { Text = "Select Option", Value = "" },
                new SelectListItem
                { Text = "Yes", Value = "1" },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
            SchoolsDesignWorkBoolId2 = new List<SelectListItem>()
            {
                new SelectListItem
                { Text = "Select Option", Value = "" },
                new SelectListItem
                { Text = "Yes", Value = "1" },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
            SchoolsDesignWorkBoolId3 = new List<SelectListItem>()
            {
                new SelectListItem
                { Text = "Select Option", Value = "" },
                new SelectListItem
                { Text = "Yes", Value = "1" },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
            SchoolsDesignWorkBoolId4 = new List<SelectListItem>()
            {
                new SelectListItem
                { Text = "Select Option", Value = "" },
                new SelectListItem
                { Text = "Yes", Value = "1" },
                new SelectListItem
                { Text = "No", Value = "2" }
            };
        }        
        public virtual IList<SelectListItem> InspectionReportBoolId { get; set; }
        public virtual IList<SelectListItem> ValuationBoolId { get; set; }
        public virtual IList<SelectListItem> SchoolsDesignWorkBoolId { get; set; }
        public virtual IList<SelectListItem> SchoolsDesignWorkBoolId2 { get; set; }
        public virtual IList<SelectListItem> SchoolsDesignWorkBoolId3 { get; set; }
        public virtual IList<SelectListItem> SchoolsDesignWorkBoolId4 { get; set; }
        public virtual string ValuationTextId { get; set; }
        public virtual string ValuationTextId2 { get; set; }
        public virtual string OtherActivitiesTextId { get; set; }
        public virtual string CanterburyEarthquakeRebuildWorkId { get; set; }
        public virtual string InspectionReportTextId { get; set; }
        public virtual string OtherProjectManagementTextId { get; set; }
        public virtual string NonProjectManagementTextId { get; set; }
        public decimal ConstructionCommercial { get; set; }
        public decimal ConstructionDwellings { get; set; }
        public decimal ConstructionIndustrial { get; set; }
        public decimal ConstructionInfrastructure { get; set; }
        public decimal ConstructionSchool { get; set; }
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
}