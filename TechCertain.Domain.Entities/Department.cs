using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
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

