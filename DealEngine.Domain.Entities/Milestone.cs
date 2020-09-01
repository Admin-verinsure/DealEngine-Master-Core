using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Milestone : EntityBase, IAggregateRoot
    {
        protected Milestone() : base(null) { }

        public Milestone(User createdBy)
            : base(createdBy) {
        }

        public virtual Programme Programme { get; set; }       
    }

    public class Advisory : EntityBase, IAggregateRoot
    {
        public Advisory(string description) : base(null)
        {
            Description = description;
        }
        public Advisory() : base(null) { }
        public virtual Milestone Milestone { get; set; }
        public virtual string Description { get; set; }
        public virtual Activity Activity { get; set; }
    }

    public class UserTask : EntityBase, IAggregateRoot
    {
        public virtual Organisation For { get; protected set; }
        public virtual Milestone Milestone { get; set; }
        public virtual string Description { get; set; }
        public virtual string Details { get; set; }
        public virtual DateTime DueDate { get; set; }
        public virtual bool Completed { get; set; }
        public virtual User CompletedBy { get; set; }
        public virtual DateTime CompletedOn { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual Activity Activity { get; set; }

        protected UserTask() : base(null) { }

        public UserTask(User createdBy, Organisation createdFor)
            : base(createdBy)
        {
            For = createdFor;
            DateTime today = DateTime.Now;
            DueDate = today.AddDays(7);
            IsActive = true;
        }

        public virtual void Complete(User completedBy)
        {
            Completed = true;
            CompletedBy = completedBy;
            CompletedOn = DateTime.UtcNow;
            IsActive = false;
        }
    }
}

