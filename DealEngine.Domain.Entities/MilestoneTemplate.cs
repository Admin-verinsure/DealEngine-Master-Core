using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Domain.Entities
{
    public class MilestoneTemplate : EntityBase, IAggregateRoot
    {
        public MilestoneTemplate() : base(null) { }

        public MilestoneTemplate(User createdBy)
            : base(createdBy)
        {
        }     
    }

    public class Activity : EntityBase, IAggregateRoot
    {
        public Activity() : base(null) { }

        public Activity(User createdBy, string activity, IFormCollection collection, UserTask userTask)
            : base(createdBy)
        {
            Name = activity;
            if(collection != null)
            {
                Advisory = new Advisory(createdBy, collection);
                UserTask = new UserTask(createdBy, Name, collection);
                //email
            }
            if(userTask != null)
            {
                UserTask = userTask;
            }
        }

        public virtual string Name { get; set; }
        public virtual Advisory Advisory { get; set; }
        public virtual UserTask UserTask { get; set; }
    }

    public class ProgrammeProcess : EntityBase, IAggregateRoot
    {
        public ProgrammeProcess() : base(null) { }
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
        protected Advisory() : base(null) { }
        public Advisory(User createdBy, IFormCollection collection) 
            : base(createdBy) 
        {
            PopulateEntity(collection);
        }

        public virtual string Description { get; set; }
    }

    public class UserTask : EntityBase, IAggregateRoot
    {

        public virtual string Name { get; set; }
        public virtual string Body { get; set; }
        public virtual string URL { get; set; }
        public virtual int Count { get; set; }
        public virtual DateTime DueDate { get; set; }
        public virtual bool Completed { get; set; }
        public virtual User CompletedBy { get; set; }
        public virtual DateTime CompletedOn { get; set; }
        public virtual bool IsActive { get; set; }

        protected UserTask() : base(null) {
            IsActive = true;
            DateTime today = DateTime.Now;
            DueDate = today.AddDays(7);
        }

        public UserTask(User createdBy, string name, IFormCollection collection) : base(createdBy)
        {
            Name = name;
            if(collection != null)
            {
                PopulateEntity(collection);
            }            
            DueDate = DateTime.Now.AddDays(7);
            Completed = false;
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