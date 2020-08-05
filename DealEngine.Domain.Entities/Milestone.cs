using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Milestone : EntityBase, IAggregateRoot
    {
        protected Milestone() : base(null) { }

        public Milestone(User createdBy, Programme programme)
            : base(createdBy) {
            Programme = programme;
            ProgrammeProcesses = new List<ProgrammeProcess>();
        }

        public virtual bool HasTriggered { get; set; }
        public virtual Programme Programme { get; set; }
        public virtual IList<ProgrammeProcess> ProgrammeProcesses { get; set; }
    }    
}

