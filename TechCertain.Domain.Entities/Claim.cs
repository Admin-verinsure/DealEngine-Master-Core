using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
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
        public virtual ClaimsIdentity Subject { get; set; }
        public virtual IDictionary<string, string> Properties { get; set; }
        public virtual string OriginalIssuer { get; set; }
        public virtual string Issuer { get; set; }
        public virtual string ValueType { get; set; }
        public virtual string Value { get; set; }
    }
}
