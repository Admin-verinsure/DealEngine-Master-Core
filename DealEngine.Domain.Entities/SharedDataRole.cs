using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class SharedDataRole : EntityBase, IAggregateRoot
    {
        protected SharedDataRole() : this(null) { }
        public SharedDataRole(User createdBy) : base(createdBy) { }
        public virtual Programme Programme { get; set; }
        public virtual string Name { get; set; }
        public virtual int Count { get; set; }
        public virtual AdditionalRoleInformation AdditionalRoleInformation { get; set; }
    }

    public class AdditionalRoleInformation : EntityBase, IAggregateRoot
    {
        protected AdditionalRoleInformation() : this(null) { }
        public AdditionalRoleInformation(User createdBy) : base(createdBy) { }
        public virtual string OtherProfessionId { get; set; }
    }
}

