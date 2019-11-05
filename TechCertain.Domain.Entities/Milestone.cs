using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Milestone : EntityBase, IAggregateRoot
    {
        protected Milestone() : base(null) { }

        public Milestone(User createdBy)
            : base(createdBy) {
        }
        public virtual UserTask Task { get; set; }
        public virtual IList<SystemEmail> EmailTemplates { get; set; }
        public virtual Advisory Advisory { get; set; }
        public virtual bool HasTriggered { get; set; }
        public virtual ProgrammeProcess ProgrammeProcess { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual Programme Programme { get; set; }
        public virtual string Method { get; set; }
    }

    public class Advisory : EntityBase, IAggregateRoot
    {
        public Advisory(string description) : base(null)
        {
            Description = description;
        }
        public Advisory() : base(null) { }
        public virtual string Description { get; set; }
    }
}

