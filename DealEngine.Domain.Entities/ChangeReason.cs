using System;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Domain.Entities
{
    public class ChangeReason : EntityBase
    {

        public ChangeReason() : base (null) { }
        public ChangeReason(User createdBy) : base(createdBy) { }     
        public ChangeReason(User createdBy, IFormCollection formCollection)
            : base(createdBy)
        {
            if(formCollection != null)
            {
                PopulateEntity(formCollection);
            }
        }
        public ChangeReason(User createdBy, ChangeReason changeReason)
            : base(createdBy)
        {
            //ChangeReason change = new ChangeReason(createdBy);
            ChangeType = changeReason.ChangeType;
            Reason = changeReason.Reason;
            Description = changeReason.Description;
            EffectiveDate = changeReason.EffectiveDate;
        }

        //public virtual Guid DealId { get; set; }

        public virtual string ChangeType { get; set; }

        public virtual string Reason { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime EffectiveDate { get; set; }

    }
}
