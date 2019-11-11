using System;
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
        public virtual UserTask UserTask { get; set; }
        public virtual Advisory Advisory { get; set; }
        public virtual bool HasTriggered { get; set; }
        public virtual ProgrammeProcess ProgrammeProcess { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual Programme Programme { get; set; }
        public virtual string Method { get; set; }
        public virtual SystemEmail SystemEmailTemplate { get; set; }
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

    public class UserTask : EntityBase, IAggregateRoot
    {
        public virtual Organisation For { get; protected set; }

        public virtual UserTask SuccessorTask { get; set; }

        public virtual string Description { get; set; }

        public virtual string Details { get; set; }

        public virtual int Priority { get; set; }

        public virtual DateTime DueDate { get; set; }

        public virtual bool Completed { get; protected set; }

        public virtual User CompletedBy { get; protected set; }

        public virtual DateTime CompletedOn { get; protected set; }

        public virtual IList<Organisation> InterestedOrganisations { get; protected set; }
        public virtual bool IsActive { get; set; }
        public virtual Milestone Milestone { get; set; }

        protected UserTask() : base(null) { }

        public UserTask(User createdBy, Organisation createdFor, DateTime dueDate)
            : base(createdBy)
        {
            For = createdFor;
            DueDate = dueDate;
            InterestedOrganisations = new List<Organisation>();
            InterestedOrganisations.Add(createdFor);
        }

        public virtual void Complete(User completedBy)
        {
            Completed = true;
            CompletedBy = completedBy;
            CompletedOn = DateTime.UtcNow;
        }
    }
}

