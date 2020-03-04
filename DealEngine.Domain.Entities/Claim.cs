using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Claim : EntityBase, IAggregateRoot
    {
        public Claim(User createdBy) : base(createdBy)
        {
        }

        protected Claim() : base(null) { }
        public Claim(string type, string value) : base(null)
        {
            Type = type;
            Value = value;
        }

        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
    }
}
