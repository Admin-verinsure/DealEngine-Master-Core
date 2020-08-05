﻿using System;
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
        public virtual Advisory Advisory { get; set; }
        public virtual UserTask UserTask { get; set; }
        //public virtual EmailTemplate EmailTemplate { get; set; }
    }

    public class ProgrammeProcess : EntityBase, IAggregateRoot
    {
        public ProgrammeProcess() : base(null) { }
        public ProgrammeProcess(string name) : base(null) { }

        public ProgrammeProcess(User createdBy, string programmeProcess)
            : base(createdBy)
        {
            Name = programmeProcess;
            Activities = new List<Activity>();
        }

        public virtual string Name { get; set; }
        public virtual IList<Activity> Activities { get; set; }
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
        public virtual string Description { get; set; }
        public virtual string Details { get; set; }
        public virtual DateTime DueDate { get; set; }
        public virtual bool Completed { get; set; }
        public virtual User CompletedBy { get; set; }
        public virtual DateTime CompletedOn { get; set; }
        public virtual bool IsActive { get; set; }

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