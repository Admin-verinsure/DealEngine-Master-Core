using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class Territory : EntityBase, IAggregateRoot
    {
        public virtual decimal Percentage { get; set; }
        public virtual string Location { get; set; }
        public virtual Guid TemplateId { get; set; }
        public virtual bool Selected { get; set; }

        protected Territory() : base (null) { }

        public Territory(User createdBy)
            : base(createdBy) { }
	}
}

