using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;


namespace DealEngine.Domain.Entities
{
    public class ClaimNotification : EntityBase, IAggregateRoot
    {
        protected ClaimNotification() : base(null) { }

        public ClaimNotification(User createdBy)
            : base(createdBy)
        {
            ClaimProducts = new List<Product>();
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }

        public virtual Organisation Organisation
        {
            get;
            set;
        }

        public virtual string ClaimTitle
        {
            get;
            set;
        }

        public virtual string ClaimDescription
        {
            get;
            set;
        }

        public virtual DateTime ClaimDateOfLoss
        {
            get;
            set;
        }

        public virtual string ClaimInsuredName
        {
            get;
            set;
        }

        public virtual DateTime ClaimNotifiedDate
        {
            get;
            set;
        }

        public virtual string Claimant
        {
            get;
            set;
        }

        public virtual decimal ClaimEstimateInsuredLiability
        {
            get;
            set;
        }

        public virtual decimal ClaimPaid
        {
            get;
            set;
        }

        public virtual decimal ClaimReserve
        {
            get;
            set;
        }

        public virtual string ClaimNotes
        {
            get;
            set;
        }

        public virtual string ClaimReference
        {
            get;
            set;
        }

        public virtual string ClaimPolicyReference
        {
            get;
            set;
        }

        public virtual string ClaimBrokerReference
        {
            get;
            set;
        }

        public virtual string ClaimInsurerReference
        {
            get;
            set;
        }

        public virtual string ClaimInsurerName
        {
            get;
            set;
        }

        public virtual string ClaimStatus
        {
            get;
            set;
        }

        public virtual string ClaimMembershipNumber
        {
            get;
            set;
        }

        public virtual IList<Product> ClaimProducts
        {
            get;
            set;
        }
        //public List<SelectListItem> InterestedPartyList { get; set; }
        public virtual string SelectedClaimProducts { get; set; }
        public virtual string SelectedResponsiblePrincipal { get; set; }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual ClaimNotification OriginalClaim
        {
            get;
            set;
        }

        public virtual ClaimNotification CloneForNewSheet(ClientInformationSheet newSheet)
        {
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone claim for original information");

            ClaimNotification newClaim = new ClaimNotification(newSheet.CreatedBy);
            newClaim.ClaimTitle = ClaimTitle;
            newClaim.ClaimDescription = ClaimDescription;
            newClaim.ClaimInsuredName = ClaimInsuredName;
            newClaim.Claimant = Claimant;
            newClaim.ClaimEstimateInsuredLiability = ClaimEstimateInsuredLiability;
            newClaim.ClaimPaid = ClaimPaid;
            newClaim.ClaimReserve = ClaimReserve;
            newClaim.ClaimNotes = ClaimNotes;
            newClaim.ClaimReference = ClaimReference;
            newClaim.ClaimPolicyReference = ClaimPolicyReference;
            newClaim.ClaimBrokerReference = ClaimBrokerReference;
            newClaim.ClaimInsurerReference = ClaimInsurerReference;
            newClaim.ClaimInsurerName = ClaimInsurerName;
            newClaim.ClaimStatus = ClaimStatus;
            if (ClaimNotifiedDate > DateTime.MinValue)
                newClaim.ClaimNotifiedDate = ClaimNotifiedDate;
            if (ClaimDateOfLoss > DateTime.MinValue)
                newClaim.ClaimDateOfLoss = ClaimDateOfLoss;

            newClaim.OriginalClaim = this;
            return newClaim;
        }

    }
}
