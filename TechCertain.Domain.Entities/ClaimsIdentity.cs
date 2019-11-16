using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class ClaimsIdentity : EntityBase, IAggregateRoot
    {
        public ClaimsIdentity(User createdBy) : base(createdBy)
        {
        }
        protected ClaimsIdentity() : base(null) { }
        public virtual ClaimsIdentity Actor { get; set; }
        public virtual string AuthenticationType { get; }
        public virtual string Name { get; }
        public virtual string Label { get; set; }
        public virtual bool IsAuthenticated { get; }
        public virtual IEnumerable<Claim> Claims { get; }
        public virtual string RoleClaimType { get; }
        public virtual string NameClaimType { get; }
    }
}
