using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class BusinessActivity : EntityBase, IAggregateRoot
    {
        public virtual string AnzsciCode { get; set; }
        public virtual string Description { get; set; }
        public virtual int Classification {get; set; }
        public virtual decimal Pecentage { get; set; }
        public virtual IList<RevenueByActivity> RevenueByActivities { get; set; }
        public virtual Guid BusinessActivityTemplate { get; set; }

        protected BusinessActivity () : this (null) { }

		public BusinessActivity (User createdBy)
			: base (createdBy)
		{
            RevenueByActivities = new List<RevenueByActivity>();
        }
	}
}

