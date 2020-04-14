using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class Territory : EntityBase, IAggregateRoot
    {
        public virtual decimal Pecentage { get; set; }
        public virtual string Location { get; set; }
        protected Territory() : base (null) { }

        public Territory(User createdBy)
            : base(createdBy) { }
	}
}

