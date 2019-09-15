using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class PackageProductInsurer : EntityBase, IAggregateRoot
    {
        public virtual PackageProduct PPInsurerPackageProduct { get; set; }

        public virtual Organisation PPInsurerOrganisation { get; set; }

        public virtual decimal PPInsurerProportionShare { get; set; }

        public virtual string PPInsurerCode { get; set; }

        public virtual string PPInsurerBranch { get; set; }

        public virtual int PPInsurerLead { get; set; }

        protected PackageProductInsurer() : this(null) { }

        public PackageProductInsurer(User createdBy) : base(createdBy)
        {
        }


    }
}