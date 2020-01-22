using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class EGlobalSubmissionTerm : EntityBase, IAggregateRoot
    {
        protected EGlobalSubmissionTerm() : base(null) { }

        public EGlobalSubmissionTerm(User createdBy)
            : base(createdBy)
        {

        }

        public virtual EGlobalSubmission ESTEGlobalSubmission { get; set; }
        public virtual Package ESTEGlobalSubmissionPackage { get; set; }
        public virtual Product ESTProduct { get; set; }
        public virtual decimal ESTPremium { get; set; }
        public virtual decimal ESTNDPremium { get; set; }
        public virtual decimal ESTBrokerage { get; set; }
        public virtual decimal ESTBrokerageRate { get; set; }
        public virtual decimal ESTNDBrokerage { get; set; }
        public virtual decimal ESTNDBrokerageRate { get; set; }
        public virtual decimal ESTEQC { get; set; }
        public virtual decimal ESTFSL { get; set; }


    }
}
