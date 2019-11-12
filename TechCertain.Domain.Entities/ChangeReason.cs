using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;


namespace TechCertain.Domain.Entities
{
    public class ChangeReason : EntityBase
    {

        public ChangeReason() : base (null) { }

        public ChangeReason(User createdBy)
        : base(createdBy)
        {
        }

        public virtual Guid DealId { get; set; }

        public virtual string ChangeType { get; set; }

        public virtual string Reason { get; set; }

        public virtual string ReasonDesc { get; set; }

        public virtual DateTime EffectiveDate { get; set; }

    }
}
