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

        public string StartDate { get; set; }

		public string EndDate { get; set; }

        public string CurrencySymbol { get; set; }

		public string AdministrationFee { get; set; }

        public IEnumerable<InsuranceInclusion> Inclusions { get; set; }

        public IEnumerable<InsuranceExclusion> Exclusions { get; set; }

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

		public Guid ClientProgrammeId { get; set; }

        public bool HasBoats { get; set; }

        public IEnumerable<BoatViewModel> Boats { get; set; }
        public List<EditTermsViewModel> BVTerms { get; internal set; }
        public List<EditTermsViewModel> MVTerms { get; internal set; }
        public List<ClientAgreementReferral> Referrals { get; set; }
        //public IEnumerable<ClientAgreementBVTerm> BVTerms { get; set; }
        //public IEnumerable<ClientAgreementMVTerm>MVTerms { get; set; }
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
    }

    public class InsuranceInclusion
    {
        public string RiskName { get; set; }

        public string Inclusion { get; set; }
   
    }

    public class InsuranceExclusion
    {
        public string RiskName { get; set; }

        public string Exclusion { get; set; }
    }

    /// <summary> 
    /// Customer, 
    /// Broker, 
    /// Broker Agency, 
    /// Captive Insurer, 
    /// Insurer Agency, 
    /// Insurer, 
    /// Co-Insurer, 
    /// Excess Layer Insurer, 
    /// Re-Insurer, 
    /// Catastrophe Insurer, 
    /// Bond Insurer
    /// </summary>
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

    }

}