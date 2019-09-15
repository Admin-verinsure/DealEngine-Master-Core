using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class RiskCategory : EntityBase, IAggregateRoot
	{
		//public RiskCategory[] ParentRisks { get; }

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

