using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class AgreementTemplate : EntityBase, IAggregateRoot
	{
		public virtual Guid OriginalTemplateId { get; set; }

		public virtual Guid CreatorCompany { get; set; }

		public virtual Guid OwnerCompany { get; set; }

		public virtual string Name { get; set; }

		public virtual string Description { get; set; }

		public virtual InformationTemplate InformationTemplate { get; set; }

		public virtual IList<Product> Products { get; set; }

		// Save Terms here

		// Save Documents here

		// Save Interested Parties here

		//public virtual IList<KeyValuePair<string, Organisation>> Parties { get; protected set; }

		protected AgreementTemplate () : base (null) { }

		public AgreementTemplate (User createdBy) : base (createdBy) { }
	}
}

