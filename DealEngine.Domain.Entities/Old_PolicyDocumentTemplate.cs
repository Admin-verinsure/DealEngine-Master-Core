using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
//	public enum PolicyDocumentType
//	{
//		Wording = 0,
//		Certificate = 1,
//		Schedule = 2
//	}

	public class Old_PolicyDocumentTemplate : EntityBase, IAggregateRoot
	{
		public virtual string Title { get; protected set; }

		public virtual string Creator { get; protected set; }

		public virtual string Owner { get; protected set; }

		public virtual string Description { get; protected set; }

		public virtual string DocumentType { get; protected set; }
		// TODO - currently not working with Automapper
		//public virtual PolicyDocumentType DocumentType { get; protected set; }

		public virtual string Version { get; protected set; }

		public virtual int Revision { get; protected set; }

		public virtual string Text { get; protected set; }

		public virtual int Territory { get; protected set; }

		public virtual int Jurisdiction { get; protected set; }

		public virtual string CustomTerritory { get; protected set; }

		public virtual string CustomJurisdiction { get; protected set; }

		public virtual bool Deprecated { get; protected set; }

		protected Old_PolicyDocumentTemplate() : base (null) { }

		public Old_PolicyDocumentTemplate (User createdBy, string title)
			: base (createdBy)
		{
			Title = title;
		}

		public virtual Old_PolicyDocumentTemplate ChangeTitle(string title)
		{
			Title = title;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate ChangeCreator(string creator)
		{
			Creator = creator;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate ChangeOwner(string owner)
		{
			Owner = owner;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate ChangeDescription(string description)
		{
			Description = description;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate ChangeVersion(string version)
		{
			Version = version;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate SetRevision(int revision)
		{
			Revision = revision;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate ChangeText(string text)
		{
			Text = text;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate ChangeTerritory(int territory)
		{
			Territory = territory;
			return this;
   		}

		public virtual Old_PolicyDocumentTemplate ChangeJurisdiction(int jurisdiction)
		{
			Jurisdiction = jurisdiction;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate SetCustomTerritory(string territory)
		{
			CustomTerritory = territory;
			return this;
		}

		public virtual Old_PolicyDocumentTemplate SetCustomJurisdiction(string jurisdiction)
		{
			CustomJurisdiction = jurisdiction;
			return this;
        }

		public virtual Old_PolicyDocumentTemplate SetDeprecated(bool deprecated)
		{
			Deprecated = deprecated;
			return this;
		}
	}
}

