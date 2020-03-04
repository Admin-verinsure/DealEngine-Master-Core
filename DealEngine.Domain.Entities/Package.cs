using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Package : EntityBase, IAggregateRoot
    {
        public virtual string PackageName { get; set; }

        public virtual string PackageInvoiceType { get; set; }

        public virtual Organisation Owner { get; protected set; }

        public virtual string RiskCode { get; set; }

        public virtual string Branch { get; set; }

        public virtual string DescriptionNew { get; set; }

        public virtual string DescriptionRenew { get; set; }

        public virtual string DescriptionCancel { get; set; }

        public virtual string StatementNew { get; set; }

        public virtual string StatementRenew { get; set; }

        public virtual string StatementCancel { get; set; }

        public virtual string ContractCode { get; set; }

        public virtual string FTPFolder { get; set; }

        public virtual string DescriptionLapse { get; set; }

        public virtual string StatementChange { get; set; }

        public virtual string DescriptionChange { get; set; }

        public virtual IList<Programme> PackageProgrammes { get; protected set; }

        public virtual IList<PackageProduct> PackageProducts { get; protected set; }
        public virtual bool HasInvoicePayment { get; set; }
        public virtual bool HasCCPayment { get; set; }
        public virtual bool HasPremiumPayment { get; set; }

        protected Package() : this(null) { }

        public Package(User createdBy) : base(createdBy)
        {
            PackageProgrammes = new List<Programme>();
            PackageProducts = new List<PackageProduct>();
        }


    }
}

