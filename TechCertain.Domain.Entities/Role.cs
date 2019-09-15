using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Role : EntityBase, IAggregateRoot
    {
        public Role() : base(null) { }
        public virtual Programme Programme { get; set; }
        public virtual string Title { get; set; }
        public virtual int Count { get; set; }
    }
}

