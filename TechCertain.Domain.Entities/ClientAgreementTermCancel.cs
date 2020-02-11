using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class ClientAgreementTermCancel : EntityBase, IAggregateRoot
    {
        public ClientAgreementTermCancel() : base (null) { }

        public ClientAgreementTermCancel(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType)
			: base (createdBy)
        {
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
            if (clientAgreement == null)
                throw new ArgumentNullException(nameof(clientAgreement));

            TermLimitCan = termLimit;
            ExcessCan = excess;
            PremiumCan = premium;
            FSLCan = fSL;
            BrokerageRateCan = brokerageRate;
            BrokerageCan = brokerage;
            ClientAgreementCan = clientAgreement;
            SubTermTypeCan = subTermType;

            BoatTermsCan = new List<ClientAgreementBVTermCancel>();
        }

        public virtual ClientAgreementTerm exClientAgreementTerm
        {
            get;
            set;
        }

        public virtual int TermLimitCan
        {
            get;
            set;
        }

        public virtual int AggregateLimitCan
        {
            get;
            protected set;
        }

        public virtual decimal ExcessCan
        {
            get;
            set;
        }

        public virtual int HigherExcessCan
        {
            get;
            protected set;
        }

        public virtual decimal PremiumCan
        {
            get;
            set;
        }

        public virtual string ReferenceCan
        {
            get;
            protected set;
        }

        public virtual bool DefaultTermCan
        {
            get;
            protected set;
        }

        public virtual int OrderNumberCan
        {
            get;
            protected set;
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

        public virtual decimal NDBrokerageRateCan
        {
            get;
            protected set;
        }

        public virtual decimal NDBrokerageCan
        {
            get;
            protected set;
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

        public virtual decimal NDCan
        {
            get;
            protected set;
        }

        public virtual decimal FSLCan
        {
            get;
            set;
        }

        public virtual decimal EQCCan
        {
            get;
            protected set;
        }

        public virtual Product ProductCan
        {
            get;
            protected set;
        }

        public virtual ClientAgreement ClientAgreementCan
        {
            get;
            protected set;
        }

        public virtual string SubTermTypeCan
        {
            get;
            protected set;
        }

        public virtual int TermLimitPreCan
        {
            get;
            set;
        }

        public virtual int AggregateLimitPreCan
        {
            get;
            protected set;
        }

        public virtual decimal ExcessPreCan
        {
            get;
            protected set;
        }

        public virtual int HigherExcessPreCan
        {
            get;
            protected set;
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

        public virtual int AggregateLimitDifferCan
        {
            get;
            protected set;
        }

        public virtual decimal ExcessDifferCan
        {
            get;
            protected set;
        }

        public virtual int HigherExcessDifferCan
        {
            get;
            protected set;
        }

        public virtual decimal PremiumDifferCan
        {
            get;
            set;
        }

		public virtual IList<ClientAgreementMVTermCancel> MotorTermsCan
        {
			get;
			set;
		}

        public virtual IList<ClientAgreementBVTermCancel> BoatTermsCan
        {
            get;
            set;
        }

        public virtual decimal BurnerPremiumCan
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

		public virtual decimal NDPreCan
        {
			get;
			protected set;
		}

		public virtual decimal FSLPreCan
        {
			get;
			set;
		}

		public virtual decimal EQCPreCan
        {
			get;
			protected set;
		}

		public virtual decimal NDDifferCan
        {
			get;
			protected set;
		}

		public virtual decimal FSLDifferCan
        {
			get;
			set;
		}

		public virtual decimal EQCDifferCan
        {
			get;
			protected set;
		}

        public virtual string MergeCodeCan
        {
            get;
            protected set;
        }

        public virtual string RiskCodeCan
        {
            get;
            protected set;
        }

        public virtual string SubCoverStringCan { get; set; }

        public virtual string AuthorisationNotesCan { get; set; }

    }
}
