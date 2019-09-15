using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class PolicyTermSection : EntityBase, IAggregateRoot
	{
		public virtual string Name
		{
			get;
			set;
		}

		public virtual string Description
		{
			get;
			set;
		}

		public virtual string Version
		{
			get;
			set;
		}

		public virtual int Revision
		{
			get;
			set;
		}

		public virtual Guid Owner
		{
			get;
			set;
		}

		public virtual Guid Creator
		{
			get;
			set;
		}

		public virtual string Content
		{
			get;
			set;
		}

		public virtual Guid Territory
		{
			get;
			set;
		}

		public virtual Guid Jurisdiction
		{
			get;
			set;
		}

		public virtual Guid Clause
		{
			get;
			set;
		}

		protected PolicyTermSection () : base (null) { }

		public PolicyTermSection (User createdBy) : base (createdBy) { }
	}
}

