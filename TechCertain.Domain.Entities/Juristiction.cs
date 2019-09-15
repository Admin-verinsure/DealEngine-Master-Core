using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class Jurisdiction : EntityBase
	{
		public virtual string Location
		{
			get;
			protected set;
		}

		protected Jurisdiction() : base (null) { }

		public Jurisdiction (User createdBy, string location)
			: base (createdBy)
		{
			Location = location;
		}
	}
}

