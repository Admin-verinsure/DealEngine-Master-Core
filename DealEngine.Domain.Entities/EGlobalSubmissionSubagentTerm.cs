using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class EGlobalSubmissionSubagentTerm : EntityBase, IAggregateRoot
    {
        protected EGlobalSubmissionSubagentTerm() : base(null) { }

        public EGlobalSubmissionSubagentTerm(User createdBy)
            : base(createdBy)
        {

        }

        public virtual EGlobalSubmission ESSubagentTEGlobalSubmission { get; set; }
        public virtual EGlobalSubmissionTerm ESSubagentTEGlobalSubmissionTerm { get; set; }
        public virtual string ESSubagentTSubCode { get; set; }
        public virtual int ESSubagentTGSTRegistered { get; set; }
        public virtual string ESSubagentTSubBasisOfCalc { get; set; }
        public virtual decimal ESSubagentTSubPercentComm { get; set; }
        public virtual decimal ESSubagentTSubAmount { get; set; }


    }
}
