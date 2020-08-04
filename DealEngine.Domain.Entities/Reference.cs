using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Reference : EntityBase, IAggregateRoot
    {
        public Reference(Guid clientInformationSheetId, string referenceId) 
            : base(null) {
            ClientInformationSheetId = clientInformationSheetId;
            ReferenceId = referenceId;
        }
        public Reference() : base(null) { }
        public virtual Guid ClientInformationSheetId { get; set; }
        public virtual string ReferenceId { get; set; }
        public virtual Guid ClientAgreementId { get; set; }
    }
}

