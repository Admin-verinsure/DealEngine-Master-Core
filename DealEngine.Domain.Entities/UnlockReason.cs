using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;


namespace DealEngine.Domain.Entities
{
    public class UnlockReason : EntityBase
    {

        public UnlockReason() : base (null) { }

        public UnlockReason(User createdBy)
        : base(createdBy)
        {
        }
        public virtual Guid DealId { get; set; }
        public virtual string Reason { get; set; }

    }
}
