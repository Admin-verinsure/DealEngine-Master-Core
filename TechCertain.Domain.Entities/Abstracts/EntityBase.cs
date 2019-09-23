using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;

namespace TechCertain.Domain.Entities.Abstracts
{
    public abstract partial class EntityBase 
    {
        public virtual Guid Id { get; protected set; }

        [DisplayName("Date Created")]
		public virtual DateTime? DateCreated { get; protected set; }

        [DisplayName("Date Deleted")]
		public virtual DateTime? DateDeleted { get; set; }

        [DisplayName("Last Modified On")]
        public virtual DateTime? LastModifiedOn { get; set; }

		// TODO - http://stackoverflow.com/questions/12940954/where-to-put-created-date-and-created-by-in-ddd
		// TODO - http://stackoverflow.com/questions/13040380/how-to-keep-track-of-the-last-user-that-made-changes-to-an-object-in-ddd

		[DisplayName("Created By")]
        public virtual User CreatedBy { get; protected set; }

        [DisplayName("Deleted By")]
        public virtual User DeletedBy { get; protected set; }

        [DisplayName("Last Modified By")]
        public virtual User LastModifiedBy { get; set; }        

        public EntityBase(User createdBy)
        {
            //this.Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
            CreatedBy = createdBy;
        }
        
        public virtual void Delete(User deletedBy, DateTime? dateDeleted = null)
        {
			if (deletedBy == null)
				throw new ArgumentNullException (nameof(deletedBy));

            if (!dateDeleted.HasValue)
                DateDeleted = DateTime.UtcNow;
            else
                DateDeleted = dateDeleted;
			DeletedBy = deletedBy;
        }        

        public virtual bool Equals(EntityBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntityBase)obj);
        }

        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ Id.GetHashCode();
            return hash;
        }
    }
}
