using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class EGlobalSubmission : EntityBase, IAggregateRoot
    {
        protected EGlobalSubmission() : base(null) { }

        public EGlobalSubmission(User createdBy)
            : base(createdBy)
        {

        }

        public virtual ClientProgramme EGlobalSubmissionClientProgramme { get; set; }

        public virtual Package EGlobalSubmissionPackage { get; set; }

        public virtual string SubmissionRequestXML { get; set; }
        public virtual EGlobalResponse EGlobalResponse { get; set; }
        public virtual Guid TransactionReferenceID { get; set; }
    }
}
