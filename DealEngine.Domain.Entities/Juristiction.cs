using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
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

