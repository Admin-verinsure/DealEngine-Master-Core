using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DealEngine.Domain.Entities
{
    public class DataVehicle : EntityBase, IAggregateRoot
    {
        public DataVehicle() : base(null) { }

        public DataVehicle(User createdBy)
            : base(createdBy)
        {
        }
        public virtual string Registration { get; set; }
        public virtual string Year { get; set; }
        public virtual string Make { get; set; }
        public virtual string Model { get; set; }
        public virtual string GroupSumInsured { get; set; }
        public virtual string VehicleEffectiveDate { get; set; }
        public virtual string TrailerExcess { get; set; }
        public virtual string TrailerLimit { get; set; }
    }
}

