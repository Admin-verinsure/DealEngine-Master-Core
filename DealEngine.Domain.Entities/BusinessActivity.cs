using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class BusinessActivity : EntityBase, IAggregateRoot
    {
        public virtual string AnzsciCode { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Percentage { get; set; }
        public virtual bool Selected { get; set; }

        protected BusinessActivity () : this (null) { }

		public BusinessActivity (User createdBy)
			: base (createdBy)
		{
        }
	}
}

