using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class ClientAgreementBITerm : EntityBase, IAggregateRoot
    {
        protected ClientAgreementBITerm() : base(null) { }

        public ClientAgreementBITerm(User createdBy, Building building, ClientAgreementTerm clientAgreementTerm, int termLimit, decimal excess, decimal premium, decimal brokerageRate, decimal brokerage)
            : base(createdBy)
        {
            if (building == null)
                throw new ArgumentNullException(nameof(building));
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
            if (string.IsNullOrWhiteSpace(termLimit.ToString()))
                throw new ArgumentNullException(nameof(termLimit));
            if (string.IsNullOrWhiteSpace(excess.ToString()))
                throw new ArgumentNullException(nameof(excess));
            if (string.IsNullOrWhiteSpace(premium.ToString()))
                throw new ArgumentNullException(nameof(premium));
            if (string.IsNullOrWhiteSpace(brokerageRate.ToString()))
                throw new ArgumentNullException(nameof(brokerageRate));
            if (string.IsNullOrWhiteSpace(brokerage.ToString()))
                throw new ArgumentNullException(nameof(brokerage));

            Building = building;
            ClientAgreementTerm = clientAgreementTerm;
            TermLimit = termLimit;
            Excess = excess;
            Premium = premium;
            BrokerageRate = brokerageRate;
            Brokerage = brokerage;
        }

        public virtual int TermLimit
        {
            get;
            set;
        }

        public virtual decimal Excess
        {
            get;
            set;
        }

        public virtual decimal Premium
        {
            get;
            set;
        }

        public virtual string Reference
        {
            get;
            set;
        }

        public virtual int VesionNumber
        {
            get;
            set;
        }

        public virtual bool Bound
        {
            get;
            set;
        }

        public virtual decimal BrokerageRate
        {
            get;
            set;
        }

        public virtual decimal Brokerage
        {
            get;
            set;
        }

        public virtual decimal ReferralLoading
        {
            get;
            set;
        }

        public virtual decimal ReferralLoadingAmount
        {
            get;
            set;
        }

        public virtual Product Product
        {
            get;
            protected set;
        }

        public virtual ClientAgreementTerm ClientAgreementTerm
        {
            get;
            protected set;
        }

        public virtual Building Building
        {
            get;
            protected set;
        }

        public virtual int TermLimitPre
        {
            get;
            set;
        }

        public virtual decimal ExcessPre
        {
            get;
            set;
        }

        public virtual decimal PremiumPre
        {
            get;
            set;
        }

        public virtual int TermLimitDiffer
        {
            get;
            set;
        }

        public virtual decimal ExcessDiffer
        {
            get;
            set;
        }

        public virtual decimal PremiumDiffer
        {
            get;
            set;
        }

        public virtual string TermCategory
        {
            get;
            set;
        }

        public virtual decimal EQC
        {
            get;
            set;
        }

        public virtual decimal EQCPre
        {
            get;
            set;
        }

        public virtual decimal EQCDiffer
        {
            get;
            set;
        }

        public virtual decimal Rate
        {
            get;
            set;
        }

        public virtual decimal AdjustedRate
        {
            get;
            set;
        }

        public virtual string SubCover
        {
            get;
            set;
        }

        public virtual bool Deleted
        {
            get;
            set;
        }

        public virtual int IndemnityPeriod
        {
            get;
            set;
        }

        public virtual decimal BrokerageDiffer
        {
            get;
            set;
        }

        public virtual decimal BrokeragePre
        {
            get;
            set;
        }
    }
}
