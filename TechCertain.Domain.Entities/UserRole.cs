using NHibernate.AspNetCore.Identity;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class UserRole : EntityBase, IAggregateRoot
    {
        protected UserRole() : base(null) { }
        public UserRole(User createdBy) : base(createdBy)
        {
        }

        public virtual IdentityRole IdentityRole { get; set; }
        public virtual Organisation Organisation { get; set; }

    }
}

