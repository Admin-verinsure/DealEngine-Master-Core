using System;
using System.Collections;
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
		}

		public virtual IList<RevenueByCountry> RevenueByCountry { get; set; }

		public virtual string Activity { get; set; }
	}
}

