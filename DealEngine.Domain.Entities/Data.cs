using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DealEngine.Domain.Entities
{
    public class Data : EntityBase, IAggregateRoot
    {
        public Data() : base(null) { }

        public Data(User createdBy)
            : base(createdBy)
        {
        }
        public virtual string BindType { get; set; }
        public virtual DateTime AgreementDate { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string FileName { get; set; }
        public virtual string FullPath { get; set; }
       
        // Lists of objects        
        public virtual string TotalSumInsured { get; set; }
        public virtual string Excess { get; set; }
        public virtual string TotalPremium { get; set; }
        public virtual string Coy { get; set; }
        public virtual string FENZ { get; set; }
        public virtual string GST { get; set; }
        public virtual string Brokerage { get; set; }

        // Coastguard Attributes
        public virtual IList<DataBoat> Boats { get; set; }


    }
}

