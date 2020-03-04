using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class PolicyDocumentTemplate : EntityBase, IAggregateRoot
	{
		IList<PolicyTermSection> _sections;

		public virtual string Name
		{
			get;
			protected set;
		}

		public virtual string Description
		{
			get;
			protected set;
		}

		public virtual string Version
		{
			get;
			protected set;
		}

		public virtual Guid Owner
		{
			get;
			protected set;
		}

		public virtual Guid Creator
		{
			get;
			protected set;
		}

		public virtual IEnumerable<PolicyTermSection> Sections
		{
			get { return _sections; }
			protected set { _sections = new List<PolicyTermSection>(value); }
		}

		public virtual bool IsPrivate
		{
			get;
			set;
		}

		protected PolicyDocumentTemplate() : base (null) { }

		public PolicyDocumentTemplate (User createdBy, string name, string description, string version, Guid creator, Guid owner, bool isPrivate)
			: base (createdBy)
		{
			_sections = new List<PolicyTermSection> ();

			Name = name;
			Description = description;
			Version = version;
			Creator = creator;
			Owner = owner;
			IsPrivate = isPrivate;
		}

		public virtual void ChangeName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name), "Name can not be null, empty or white space.");

			Name = name;
		}

		public virtual void AddSection(PolicyTermSection section)
		{
			_sections.Add(section);
		}
	}
}

