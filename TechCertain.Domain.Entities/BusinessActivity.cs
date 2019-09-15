using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class BusinessActivity : EntityBase, IAggregateRoot
    {
        public virtual string AnzsciCode { get; set; }
        public virtual Programme Programme { get; set; }
        public virtual string Description { get; set; }
        public virtual int Classification {get; set; }
        public virtual bool Required { get; set; }

		protected BusinessActivity () : this (null) { }

		public BusinessActivity (User createdBy)
			: base (createdBy)
		{
		}
	}
}

