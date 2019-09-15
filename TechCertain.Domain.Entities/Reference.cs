using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Reference : EntityBase, IAggregateRoot
    {
        public Reference() : base(null) { }
        public virtual Guid ClientInformationSheetId { get; set; }
        public virtual string ReferenceId { get; set; }
        public virtual Guid ClientAgreementId { get; set; }
    }
}

