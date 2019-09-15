using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Claim : EntityBase, IAggregateRoot
    {
        protected Claim() : base(null) { }

        public Claim(User createdBy)
            : base(createdBy)
        {

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

        public virtual IList<Product> ClaimProducts
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual Claim OriginalClaim
        {
            get;
            set;
        }

        public virtual Claim CloneForNewSheet(ClientInformationSheet newSheet)
        {
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone claim for original information");

            Claim newClaim = new Claim(newSheet.CreatedBy);
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
