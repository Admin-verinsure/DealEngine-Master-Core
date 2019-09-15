using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Proposal : EntityBase, IAggregateRoot
    {
        protected Proposal() : base (null) { }

        public Proposal(User createdBy, string name)
			: base (createdBy)
        {
            Name = name;
        }

        public virtual string Name { get; set; }
    }
}