using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class RevenueByActivity : EntityBase, IAggregateRoot
	{
		protected RevenueByActivity () : this (null) {}

		public RevenueByActivity (User createdBy)
			: base (createdBy)
		{
            Territories = new List<Territory>();
            Activities = new List<BusinessActivity>();
		}

		public virtual IList<Territory> Territories { get; set; }
		public virtual IList<BusinessActivity> Activities { get; set; }
        public virtual decimal TotalRevenue { get; set; }
        public virtual string OtherInfomation { get; set; }
    }
}

