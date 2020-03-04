using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class MilestoneTemplate : EntityBase, IAggregateRoot
    {
        public MilestoneTemplate() : base(null) { }

        public MilestoneTemplate(User createdBy)
            : base(createdBy)
        {
        }

        public virtual IList<ProgrammeProcess> ProgrammeProcesses { get; set; }        
        public virtual IList<Activity> Activities { get; set; }        
    }

    public class Activity : EntityBase, IAggregateRoot
    {
        public Activity() : base(null) { }
        public Activity(string name) : base(null) { }

        public Activity(User createdBy)
            : base(createdBy)
        {
        }

        public virtual string Name { get; set; }
        public virtual Milestone Milestone { get; set; }
    }

    public class ProgrammeProcess : EntityBase, IAggregateRoot
    {
        public ProgrammeProcess() : base(null) { }
        public ProgrammeProcess(string name) : base(null) { }

        public ProgrammeProcess(User createdBy)
            : base(createdBy)
        {
        }

        public virtual string Name { get; set; }
        public virtual Milestone Milestone { get; set; }

    }
}