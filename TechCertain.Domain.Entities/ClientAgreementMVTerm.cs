using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class ClientAgreementMVTerm : EntityBase, IAggregateRoot
	{
        protected ClientAgreementMVTerm() : base(null) { }

        public ClientAgreementMVTerm(User createdBy, Vehicle vehicle, ClientAgreementTerm clientAgreementTerm, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, decimal burnerpremium)
            : this(createdBy, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, termLimit, excess, premium, fSL, brokerageRate, brokerage, vehicleCategory, vehicle.FleetNumber, clientAgreementTerm, vehicle, burnerpremium)
        {

        }

        public ClientAgreementMVTerm(User createdBy, string registration, string year, string make, string model, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, string fleetNumber, ClientAgreementTerm clientAgreementTerm, Vehicle vehicle, decimal burnerpremium)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(year))
                throw new ArgumentNullException(nameof(year));
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentNullException(nameof(make));
            //Carjam returns null
            //if (string.IsNullOrWhiteSpace(model))
            //    throw new ArgumentNullException(nameof(model));
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
            //if (string.IsNullOrWhiteSpace(vehicleCategory))
            //    throw new ArgumentNullException(nameof(vehicleCategory));
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            Registration = registration;
            Year = year;
            Make = make;
            Model = model;
            TermLimit = termLimit;
            Excess = excess;
            Premium = premium;
            FSL = fSL;
            BrokerageRate = brokerageRate;
            Brokerage = brokerage;
            VehicleCategory = vehicleCategory;
            FleetNumber = fleetNumber;
            ClientAgreementTerm = clientAgreementTerm;
            Vehicle = vehicle;
            BurnerPremium = burnerpremium;
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
            protected set;
        }

        public virtual Vehicle Vehicle
        {
            get;
            protected set;
        }

        public virtual string Make
        {
            get;
            set;
        }

        public virtual string Model
        {
            get;
            set;
        }

        public virtual string Registration
        {
            get;
            set;
        }

        public virtual string Year
        {
            get;
            set;
        }

        public virtual string VehicleCategory
        {
            get;
            set;
        }

        public virtual string FleetNumber
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

        public virtual decimal BurnerPremium
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

        public virtual decimal BurnerPremiumPre
        {
            get;
            set;
        }

        public virtual decimal BurnerPremiumDiffer
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
