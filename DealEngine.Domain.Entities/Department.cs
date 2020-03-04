using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class Department : EntityBase
	{
		protected Department () : base (null) { }

		public Department (User createdBy) : base (createdBy) { }

		public virtual string Name { get; set; }

		public virtual Organisation Company { get; set; }

		public virtual IEnumerable<User> Users { get; set; }		
	}
}

