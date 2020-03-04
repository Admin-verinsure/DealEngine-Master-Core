﻿using NHibernate.AspNetCore.Identity;
using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class UserRole : EntityBase, IAggregateRoot
    {
        protected UserRole() : base(null) { }
        public UserRole(User createdBy)
            : base(createdBy)
        {
        }        

        public virtual string IdentityRoleName { get; set; }
        public virtual Organisation Organisation { get; set; }

    }
}

