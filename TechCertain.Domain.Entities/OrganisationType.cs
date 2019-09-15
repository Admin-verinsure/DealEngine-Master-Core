using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class OrganisationType : EntityBase, IAggregateRoot
    {
        public virtual string Name { get; protected set; }

        protected OrganisationType() : base (null) { }

        public OrganisationType(User createdBy, string name)
			: base (createdBy)
        {
            Name = name;
        }

	}
}