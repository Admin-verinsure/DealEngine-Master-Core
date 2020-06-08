using DealEngine.Domain.Entities.Abstracts;
using System;

namespace DealEngine.Domain.Entities
{
    public class AuditHistory : EntityBase, IAggregateRoot
    {
        public AuditHistory() : base(null) { }

        public AuditHistory(User createdBy)
            : base(createdBy)
        {
        }

        public virtual ClientInformationSheet PreviousSheet { get; set; }
        public virtual ClientInformationSheet NextSheet { get; set; }
        //public virtual DateTime DateDeleted { get; set; }
        //public virtual DateTime DateAdded { get; set; }
    }
}

