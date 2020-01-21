using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class EGlobalSubmission : EntityBase, IAggregateRoot
    {
        protected EGlobalSubmission() : base(null) { }

        public EGlobalSubmission(User createdBy)
            : base(createdBy)
        {
            EGlobalSubmissionTerms = new List<EGlobalSubmissionTerm>();
            EGlobalSubmissionSubagentTerms = new List<EGlobalSubmissionSubagentTerm>();
        }

        public virtual ClientProgramme EGlobalSubmissionClientProgramme { get; set; }
        public virtual Package EGlobalSubmissionPackage { get; set; }
        public virtual string SubmissionRequestXML { get; set; }
        public virtual EGlobalResponse EGlobalResponse { get; set; }
        public virtual Guid TransactionReferenceID { get; set; }
        public virtual string SubmissionDesc { get; set; }
        public virtual IList<EGlobalSubmissionTerm> EGlobalSubmissionTerms { get; set; }
        public virtual IList<EGlobalSubmissionSubagentTerm> EGlobalSubmissionSubagentTerms { get; set; }
    }
}
