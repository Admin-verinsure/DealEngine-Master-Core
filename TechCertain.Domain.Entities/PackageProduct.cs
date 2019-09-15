using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class PackageProduct : EntityBase, IAggregateRoot
    {
        public virtual Package PackageProductPackage { get; set; }

        public virtual Product PackageProductProduct { get; set; }

        public virtual string PackageProductRiskCode { get; set; }

        public virtual string PackageProductSubCover { get; set; }

        public virtual Product PackageProductMergeProduct { get; set; }

        public virtual bool PackageProductAlwaysInclude { get; set; }

        public virtual IList<PackageProductInsurer> PackageProductInsurers { get; protected set; }

        public virtual SubAgent PackageProductDefaultSubAgent { get; set; }

        protected PackageProduct() : this(null) { }

        public PackageProduct(User createdBy) : base(createdBy)
        {

            PackageProductInsurers = new List<PackageProductInsurer>();
        }


    }
}


