using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class ClientAgreementMVTermCancel : EntityBase, IAggregateRoot
	{
        protected ClientAgreementMVTermCancel() : base(null) { }

        public ClientAgreementMVTermCancel(User createdBy, Vehicle vehicle, ClientAgreementTerm clientAgreementTerm, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, decimal burnerpremium)
            : this(createdBy, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, termLimit, excess, premium, fSL, brokerageRate, brokerage, vehicleCategory, vehicle.FleetNumber, clientAgreementTerm, vehicle, burnerpremium)
        {

        }

        public ClientAgreementMVTermCancel(User createdBy, string registration, string year, string make, string model, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, string fleetNumber, ClientAgreementTerm clientAgreementTerm, Vehicle vehicle, decimal burnerpremium)
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

            RegistrationCan = registration;
            YearCan = year;
            MakeCan = make;
            ModelCan = model;
            TermLimitCan = termLimit;
            ExcessCan = excess;
            PremiumCan = premium;
            FSLCan = fSL;
            BrokerageRateCan = brokerageRate;
            BrokerageCan = brokerage;
            VehicleCategoryCan = vehicleCategory;
            FleetNumberCan = fleetNumber;
            ClientAgreementTermCan = clientAgreementTerm;
            VehicleCan = vehicle;
            BurnerPremiumCan = burnerpremium;
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

        public virtual ClientAgreementTerm ClientAgreementTermCan
        {
            get;
            protected set;
        }

        public virtual Vehicle VehicleCan
        {
            get;
            protected set;
        }

        public virtual string MakeCan
        {
            get;
            set;
        }

        public virtual string ModelCan
        {
            get;
            set;
        }

        public virtual string RegistrationCan
        {
            get;
            set;
        }

        public virtual string YearCan
        {
            get;
            set;
        }

        public virtual string VehicleCategoryCan
        {
            get;
            set;
        }

        public virtual string FleetNumberCan
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

        public virtual decimal BurnerPremiumCan
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

        public virtual decimal BurnerPremiumPreCan
        {
            get;
            set;
        }

        public virtual decimal BurnerPremiumDifferCan
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
