using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechCertain.Domain.Entities;


namespace TechCertain.WebUI.Models.Agreement
{
    public class ViewAgreementViewModel : BaseViewModel
    {
        public IEnumerable<InsuranceRoleViewModel> InsuranceRoles { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? IssuedToCustomer { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public Boolean NextInfoSheet { get; set; }
        public Boolean IsChange { get; set; }
        public string StartDate { get; set; }
        public string Sheetstatus { get; set; }
        public string EndDate { get; set; }
        public string CurrencySymbol { get; set; }
		public string AdministrationFee { get; set; }
        public IEnumerable<InsuranceInclusion> Inclusions { get; set; }
        public IEnumerable<InsuranceExclusion> Exclusions { get; set; }
        public IEnumerable<MultiCoverOptions> MultiCoverOptions { get; set; }
        public IEnumerable<RiskPremiumsViewModel> RiskPremiums { get; set; }
		public Guid InformationSheetId { get; set; }
		public bool HasVehicles { get; set; }
		public IEnumerable<VehicleViewModel> Vehicles { get; set; }
		public Guid ClientAgreementId { get; set; }
		public string ClientNumber { get; set; }
		public string PolicyNumber { get; set; }
		public string BrokerageRate { get; set; }
		public IList<AgreementDocumentViewModel> Documents { get; set; }
		public bool EditEnabled { get; set; }
        public bool IsMultipleOption { get; set; }
        public Guid ClientProgrammeId { get; set; }
        public bool HasBoats { get; set; }
        public IEnumerable<BoatViewModel> Boats { get; set; }
        public List<EditTermsViewModel> BVTerms { get; internal set; }
        public List<EditTermsViewModel> MVTerms { get; internal set; }
        public List<EditTermsViewModel> PLTerms { get; internal set; }
        public List<EditTermsViewModel> EDTerms { get; internal set; }
        public List<EditTermsViewModel> PITerms { get; internal set; }
        public List<EditTermsViewModel> ELTerms { get; internal set; }
        public List<EditTermsViewModel> CLTerms { get; internal set; }
        public List<EditTermsViewModel> SLTerms { get; internal set; }
        public List<EditTermsViewModel> DOTerms { get; internal set; }
        public List<EditTermsCancelViewModel> BVTermsCan { get; internal set; }
        public List<EditTermsCancelViewModel> MVTermsCan { get; internal set; }
        public List<ClientAgreementReferral> Referrals { get; set; }
        public User CurrentUser { get; set; }
        public DateTime CancellEffectiveDate { get; set; }
        public string InformationSheetStatus { get; set; }
        public decimal ReferralAmount { get; set; }
        public decimal ReferralLoading { get; set; }
        public string AuthorisationNotes { get; set; }
        public bool EGlobalIsActive { get; set; }
        public string CancellNotes { get; set; }
        public string DeclineNotes { get; set; }
        public string Advisory { get; internal set; }
        public string Declaration { get; set; }
        public bool ProgrammeStopAgreement { get; set; }
        public string ProgrammeStopAgreementMessage { get; set; }
        public bool RequirePayment { get; set; }
        public string NoPaymentRequiredMessage { get; set; }
        public string CancelAgreementReason { get; set; }


    }

    public class InsuranceInclusion
    {
        public string RiskName { get; set; }

        public string Inclusion { get; set; }
   
    }

    public class MultiCoverOptions
    {
        public Guid ProductId { get; set; }
        public Guid TermId { get; set; }
        public string RiskName { get; set; }
        public string isSelected { get; set; }
        public string Inclusion { get; set; }
        public string Exclusion { get; set; }
        public string limit { get; set; }
        public string excess { get; set; }
        public string premium { get; set; }
        public string TotalPremium { get; set; }
    }

    public class InsuranceExclusion
    {
        public string RiskName { get; set; }

        public string Exclusion { get; set; }
    }

    public class InsuranceRoleViewModel
    {
        public string RoleName { get; set; }

        public string Name { get; set; }

        public string ManagedBy { get; set; }

        public string Email { get; set; }
    }

    public class RiskPremiumsViewModel
    {
        public string RiskName { get; set; }

        public string Premium { get; set; }

		public string FSL { get; set; }
           
        public string TotalPremium { get; set; }

        public string TotalPremiumIncFeeGST { get; set; }

        public string TotalPremiumIncFeeIncGST { get; set; }

    }

}