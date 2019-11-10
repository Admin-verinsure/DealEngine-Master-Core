using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class AuditLog : EntityBase, IAggregateRoot
    {
        protected AuditLog() : base(null) { }

        public AuditLog(User createdBy, ClientInformationSheet auditLogClientInformationSheet, ClientAgreement auditLogClientAgreement, string auditLogDetail)
            : base(createdBy)
        {
            AuditLogClientInformationSheet = auditLogClientInformationSheet;
            AuditLogClientAgreement = auditLogClientAgreement;
            AuditLogDetail = auditLogDetail;
        }

        public virtual ClientInformationSheet AuditLogClientInformationSheet
        {
            get;
            set;
        }

        public virtual ClientAgreement AuditLogClientAgreement
        {
            get;
            set;
        }

        public virtual string AuditLogDetail
        {
            get;
            set;
        }
    }
}

