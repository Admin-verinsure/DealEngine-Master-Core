using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class MilestoneTemplate : EntityBase, IAggregateRoot
    {
        public MilestoneTemplate() : base(null) { }

        public MilestoneTemplate(User createdBy)
            : base(createdBy)
        {
        }

        public virtual string Activity { get; set; }
        public virtual Guid ClientProgramme { get; set; }
        public virtual IList<string> Templates { get; set; }

    }
}