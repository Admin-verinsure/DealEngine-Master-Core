using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class ApplicationGroup : EntityBase, IAggregateRoot
	{
		public virtual string Name { get; set; }

		public virtual ICollection<ApplicationRole> Roles { get; set; }

		public virtual ICollection<User> Users { get; set; }

		public virtual bool IsSystemGroup { get; protected set; }

		protected ApplicationGroup () : base (null) { }

		public ApplicationGroup (User createdBy, string groupName)
			: this (createdBy, groupName, false)
		{ }

		public ApplicationGroup (User createdBy, string groupName, bool isSystemGroup)
			: base (createdBy)
		{
			if (string.IsNullOrWhiteSpace (groupName))
				throw new ArgumentNullException (nameof (groupName));
			Name = groupName;
			Roles = new List<ApplicationRole> ();
			Users = new List<User> ();
			IsSystemGroup = isSystemGroup;
		}
	}
}

