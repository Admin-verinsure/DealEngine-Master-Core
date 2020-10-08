using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;
using Newtonsoft.Json;

namespace DealEngine.Domain.Entities
{
    public class ClientAgreementBVTerm : EntityBase, IAggregateRoot
    {
        protected ClientAgreementBVTerm() : base(null) { }

        public ClientAgreementBVTerm(User createdBy, ClientAgreementTerm clientAgreementTerm, Boat boat, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage)
            : this(createdBy, boat.BoatName, boat.YearOfManufacture, boat.BoatMake, boat.BoatModel, termLimit, excess, premium, fSL, brokerageRate, brokerage, clientAgreementTerm, boat)
        {

        }

        public ClientAgreementBVTerm(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTerm clientAgreementTerm, Boat boat)
            : base(createdBy)
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
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
            if (boat == null)
                throw new ArgumentNullException(nameof(boat));

            BoatName = boatName;
            YearOfManufacture = yearOfManufacture;
            BoatMake = boatMake;
            BoatModel = boatModel;
            TermLimit = termLimit;
            Excess = excess;
            Premium = premium;
            FSL = fSL;
            BrokerageRate = brokerageRate;
            Brokerage = brokerage;
            ClientAgreementTerm = clientAgreementTerm;
            Boat = boat;
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

        public virtual decimal FSL
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
            set;
        }
        public virtual Boat Boat
        {
            get;
            protected set;
        }

        public virtual string BoatName
        {
            get;
            set;
        }


        public virtual string BoatMake
        {
            get;
            set;
        }

        public virtual string BoatModel
        {
            get;
            set;
        }

        public virtual int YearOfManufacture
        {
            get;
            set;
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

        public virtual decimal FSLPre
        {
            get;
            set;
        }

        public virtual decimal FSLDiffer
        {
            get;
            set;
        }

        public virtual string TermCategory
        {
            get;
            set;
        }

        public virtual decimal AnnualPremium
        {
            get;
            set;
        }

        public virtual decimal AnnualFSL
        {
            get;
            set;
        }

        public virtual decimal AnnualBrokerage
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
