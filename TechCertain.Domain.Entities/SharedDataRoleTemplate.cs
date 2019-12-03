using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class SharedDataRole : EntityBase, IAggregateRoot
    {
        public SharedDataRole() : base(null) { }
        public virtual Programme Programme { get; set; }
        public virtual string Name { get; set; }
        public virtual int Count { get; set; }
    }
}

