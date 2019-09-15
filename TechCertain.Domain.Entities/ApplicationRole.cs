using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class ApplicationRole : EntityBase,  IAggregateRoot
	{
		public virtual string Name { get; set; }

		public virtual string Description { get; set; }

		public virtual ICollection<ApplicationGroup> Groups { get; set; }

		public virtual bool IsSystemRole { get; protected set; }

		protected ApplicationRole () : base (null) { }

		public ApplicationRole (User createdBy, string roleName, string description)
			: this (createdBy, roleName, description, false)
		{ }

		public ApplicationRole (User createdBy, string roleName, string description, bool isSystemRole)
			: base (createdBy)
		{
			if (string.IsNullOrWhiteSpace (roleName))
				throw new ArgumentNullException (nameof (roleName));
			Name = roleName;
			Description = description;
			Groups = new List<ApplicationGroup> ();
			IsSystemRole = isSystemRole;
		}
	}
}

