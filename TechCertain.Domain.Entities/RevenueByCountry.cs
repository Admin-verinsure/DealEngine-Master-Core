using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class RevenueByCountry : EntityBase, IAggregateRoot
	{
		protected RevenueByCountry () : this (null) { }

		public RevenueByCountry (User createdBy)
			: base (createdBy)
		{
		}

		public virtual string Country { get; set; }

		public virtual decimal DeclaredRevenue { get; set; }

		public virtual string Currency { get; set; }
	}
}

