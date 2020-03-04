using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class BusinessActivityTemplate : EntityBase, IAggregateRoot
    {
        public virtual string AnzsciCode { get; set; }
        public virtual string Description { get; set; }
        public virtual int Classification {get; set; }
        public virtual bool Required { get; set; }
        public virtual decimal Pecentage { get; set; }
        public virtual Programme Programme { get; set; }

        protected BusinessActivityTemplate() : this (null) { }

		public BusinessActivityTemplate(User createdBy)
			: base (createdBy)
		{
		}
	}
}

