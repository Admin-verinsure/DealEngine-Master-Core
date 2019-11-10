using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class SystemEmail : EntityBase, IAggregateRoot
    {
        protected SystemEmail() : base(null) { }

        public SystemEmail(User createdBy, string systemEmailName, string internalNotes, string subject, string body, string systemEmailType)
            : base(createdBy)
        {
            SystemEmailName = systemEmailName;
            InternalNotes = internalNotes;
            Subject = subject;
            Body = body;
            SystemEmailType = systemEmailType;
        }

        public virtual string SystemEmailName
        {
            get;
            set;
        }

        public virtual string InternalNotes
        {
            get;
            set;
        }

        public virtual string Subject
        {
            get;
            set;
        }

        public virtual string Body
        {
            get;
            set;
        }

        public virtual string SystemEmailType
        {
            get;
            set;
        }
    }
}
