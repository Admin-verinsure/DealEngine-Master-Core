using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class ClientAgreementBVTermCancel : EntityBase, IAggregateRoot
    {

        protected ClientAgreementBVTermCancel() : this(null) { }

        protected ClientAgreementBVTermCancel(User createdBy)
            : base(createdBy)
        {

        }

        public ClientAgreementBVTermCancel(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTermCancel clientAgreementTermCancel, Boat boat)
            : this(createdBy)
        {
            if (string.IsNullOrWhiteSpace(boatName.ToString()))
                throw new ArgumentNullException(nameof(boatName));
            if (string.IsNullOrWhiteSpace(yearOfManufacture.ToString()))
                throw new ArgumentNullException(nameof(yearOfManufacture));
            if (string.IsNullOrWhiteSpace(termLimit.ToString()))
                throw new ArgumentNullException(nameof(termLimit));
            if (string.IsNullOrWhiteSpace(excess.ToString()))
                throw new ArgumentNullException(nameof(excess));
            if (string.IsNullOrWhiteSpace(premium.ToString()))
                throw new ArgumentNullException(nameof(premium));
            if (string.IsNullOrWhiteSpace(fSL.ToString()))
                throw new ArgumentNullException(nameof(fSL));
            if (string.IsNullOrWhiteSpace(brokerageRate.ToString()))
                throw new ArgumentNullException(nameof(brokerageRate));
            if (string.IsNullOrWhiteSpace(brokerage.ToString()))
                throw new ArgumentNullException(nameof(brokerage));
            if (clientAgreementTermCancel == null)
                throw new ArgumentNullException(nameof(clientAgreementTermCancel));
            if (boat == null)
                throw new ArgumentNullException(nameof(boat));

            BoatNameCan = boatName;
            YearOfManufactureCan = yearOfManufacture;
            BoatMakeCan = boatMake;
            BoatModelCan = boatModel;
            TermLimitCan = termLimit;
            ExcessCan = excess;
            PremiumCan = premium;
            FSLCan = fSL;
            BrokerageRateCan = brokerageRate;
            BrokerageCan = brokerage;
            ClientAgreementTermCan = clientAgreementTermCancel;
            BoatCan = boat;

        }

        public virtual ClientAgreementBVTerm exClientAgreementBVTerm
        {
            get;
            set;
        }

        public virtual int TermLimitCan
        {
            get;
            set;
        }

        public virtual decimal ExcessCan
        {
            get;
            set;
        }

        public virtual decimal PremiumCan
        {
            get;
            set;
        }

        public virtual string ReferenceCan
        {
            get;
            set;
        }

        public virtual int VesionNumberCan
        {
            get;
            set;
        }

        public virtual bool BoundCan
        {
            get;
            set;
        }

        public virtual decimal BrokerageRateCan
        {
            get;
            set;
        }

        public virtual decimal BrokerageCan
        {
            get;
            set;
        }

        public virtual decimal ReferralLoadingCan
        {
            get;
            set;
        }

        public virtual decimal ReferralLoadingAmountCan
        {
            get;
            set;
        }

        public virtual decimal FSLCan
        {
            get;
            set;
        }

        public virtual Product ProductCan
        {
            get;
            protected set;
        }

        public virtual ClientAgreementTermCancel ClientAgreementTermCan
        {
            get;
            set;
        }

        public virtual Boat BoatCan
        {
            get;
            protected set;
        }

        public virtual string BoatNameCan
        {
            get;
            set;
        }


        public virtual string BoatMakeCan
        {
            get;
            set;
        }

        public virtual string BoatModelCan
        {
            get;
            set;
        }

        public virtual int YearOfManufactureCan
        {
            get;
            set;
        }

        public virtual int TermLimitPreCan
        {
            get;
            set;
        }

        public virtual decimal ExcessPreCan
        {
            get;
            set;
        }

        public virtual decimal PremiumPreCan
        {
            get;
            set;
        }

        public virtual int TermLimitDifferCan
        {
            get;
            set;
        }

        public virtual decimal ExcessDifferCan
        {
            get;
            set;
        }

        public virtual decimal PremiumDifferCan
        {
            get;
            set;
        }

        public virtual decimal FSLPreCan
        {
            get;
            set;
        }

        public virtual decimal FSLDifferCan
        {
            get;
            set;
        }

        public virtual string TermCategoryCan
        {
            get;
            set;
        }

        public virtual decimal AnnualPremiumCan
        {
            get;
            set;
        }

        public virtual decimal AnnualFSLCan
        {
            get;
            set;
        }

        public virtual decimal AnnualBrokerageCan
        {
            get;
            set;
        }
    }
}
