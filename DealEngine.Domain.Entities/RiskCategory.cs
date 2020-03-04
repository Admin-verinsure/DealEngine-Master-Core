using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class RiskCategory : EntityBase, IAggregateRoot
	{
		public virtual string Name { get; set; }

		public virtual string DescriptiveName { get; set; }

		protected RiskCategory () : base (null) { }

		public RiskCategory(User createdBy, string name, string descriptiveName)
			: base (createdBy)
		{
			Name = name;
			DescriptiveName = descriptiveName;
		}
	}
}

