using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class ClientAgreementSubTerm : EntityBase, IAggregateRoot
	{
        protected ClientAgreementSubTerm() : base(null) { }

        public ClientAgreementSubTerm(User createdBy, ClientAgreementTerm clientAgreementTerm, int termLimit, decimal excess, decimal premium)
            : this(createdBy, termLimit, excess, premium,clientAgreementTerm)
        {

        }

        public ClientAgreementSubTerm(User createdBy, int termLimit, decimal excess, decimal premium,  ClientAgreementTerm clientAgreementTerm)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(termLimit.ToString()))
                throw new ArgumentNullException(nameof(termLimit));
            if (string.IsNullOrWhiteSpace(excess.ToString()))
                throw new ArgumentNullException(nameof(excess));
            if (string.IsNullOrWhiteSpace(premium.ToString()))
                throw new ArgumentNullException(nameof(premium));
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
         
            TermLimit = termLimit;
            Excess = excess;
            Premium = premium;
            ClientAgreementTerm = clientAgreementTerm;
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

        public virtual ClientAgreementTerm ClientAgreementTerm
        {
            get;
            protected set;
        }

    }
}
