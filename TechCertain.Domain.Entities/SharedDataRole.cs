using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class SharedDataRoleTemplate : EntityBase, IAggregateRoot
    {
        public SharedDataRoleTemplate() : base(null) { }
        public virtual bool IsPublic { get; set; }
        public virtual string Name { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Programme Programme { get; set; }
    }
}

