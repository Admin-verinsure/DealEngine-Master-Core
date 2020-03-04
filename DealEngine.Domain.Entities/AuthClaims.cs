using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class AuthClaims : EntityBase, IAggregateRoot 
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ICollection<AuthClaims> Claims { get; set; }

        public virtual bool IsSystemRole { get; protected set; }

        public AuthClaims() : base(null) { }

        public AuthClaims(User createdBy, string roleName, string description)
            : this(createdBy, roleName, description, false)
        { }

        public AuthClaims(User createdBy, string roleName, string description, bool isSystemRole)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));
            Name = roleName;
            Description = description;
            Claims = new List<AuthClaims>();
            IsSystemRole = isSystemRole;
        }
    }
}

