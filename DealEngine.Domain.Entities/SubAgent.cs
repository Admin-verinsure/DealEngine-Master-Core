using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class SubAgent : EntityBase, IAggregateRoot
    {
        public virtual string SubCode { get; set; }

        public virtual string SubAbbrName { get; set; }

        public virtual int SubGSTRegistered { get; set; }

        public virtual string SubBasisOfCalcr { get; set; }

        public virtual decimal SubPercentCommr { get; set; }

        public virtual string SubBasisOfCalcn { get; set; }

        public virtual decimal SubPercentCommn { get; set; }

        protected SubAgent() : this(null) { }

        public SubAgent(User createdBy) : base(createdBy)
        {
        }


    }
}

