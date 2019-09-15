using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class RiskCover : EntityBase, IAggregateRoot
	{
		protected RiskCover () : base (null) { }

		public RiskCover (User createdBy) : base (createdBy) { }

		public virtual RiskCategory BaseRisk { get; set; }

		public virtual Product Product { get; set; }

		public virtual bool CoverAll { get; set; }

		public virtual bool Loss { get; set; }

		public virtual bool Interuption { get; set; }

		public virtual bool ThirdParty { get; set; }

		public virtual void SelectAll ()
		{
			CoverAll = true;
			Loss = true;
			Interuption = true;
			ThirdParty = true;
		}
	}
}

