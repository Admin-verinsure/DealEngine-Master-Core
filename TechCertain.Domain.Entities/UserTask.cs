using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class UserTask : EntityBase, IAggregateRoot
    {
        public virtual Organisation For { get; protected set; }

        public virtual UserTask SuccessorTask { get; set; }

        public virtual string ClientName { get; set; }

        public virtual string Description { get; set; }

        public virtual string Details { get; set; }

        public virtual string TaskUrl { get; set; }

        public virtual int Priority { get; set; }

        public virtual DateTime DueDate { get; protected set; }

        public virtual bool Completed { get; protected set; }

        public virtual User CompletedBy { get; protected set; }

        public virtual DateTime CompletedOn { get; protected set; }

        public virtual IList<Organisation> InterestedOrganisations { get; protected set; }
        public virtual bool IsActive { get; set; }

        protected UserTask() : base(null) { }

        public UserTask(User createdBy, Organisation createdFor, string clientName, DateTime dueDate)
            : base(createdBy)
        {
            For = createdFor;
            ClientName = clientName;
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

