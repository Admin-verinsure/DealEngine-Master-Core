using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DealEngine.Domain.Entities
{
    public class DataBoat : EntityBase, IAggregateRoot
    {
        public DataBoat() : base(null) { }

        public DataBoat(User createdBy)
            : base(createdBy)
        {
        }
        public virtual string Year { get; set; }
        public virtual string Make { get; set; }
        public virtual string Model { get; set; }
        public virtual string Type { get; set; }
        public virtual string Construction { get; set; }
        public virtual string Location { get; set; }
        public virtual string SumInsured { get; set; }
        public virtual string Hull { get; set; }
        // Now handled in DataVehicle
        //public virtual string Trailer { get; set; }
        public virtual string RacingRisk { get; set; }
        public virtual string BoatExcess { get; set; }
        public virtual string BoatLimit { get; set; }

    }
}

