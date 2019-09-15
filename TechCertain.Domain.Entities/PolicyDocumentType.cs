using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class PolicyDocumentType : ValueObject
	{
		protected PolicyDocumentType ()
		{
		}

		public PolicyDocumentType (string type)
		{
			this.Type = type;
		}

		public virtual string Type { get; private set; }
	}
}

