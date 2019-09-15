using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class StatusUpdate : EntityBase, IAggregateRoot
	{
		public virtual string Title { get; set; }

		public virtual string Content { get; set; }

		protected StatusUpdate () : base (null) { }

		public StatusUpdate (User createdBy)
			: base(createdBy)
		{
			
		}
	}
}

