using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class Territory : EntityBase, IAggregateRoot
    {

        public virtual decimal Pecentage { get; set; }
        public virtual IList<RevenueByActivity> RevenueByActivities { get; set; }
        public virtual string Location { get; set; }
        public virtual Guid TerritoryTemplateId { get; set; }

        protected Territory() : base (null) { }

		public Territory (User createdBy)
			: base (createdBy)
		{
            RevenueByActivities = new List<RevenueByActivity>();
		}
	}
}

