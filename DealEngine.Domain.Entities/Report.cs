using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;


namespace DealEngine.Domain.Entities
{
    public class PIReport : EntityBase
    {
        public PIReport() : this(null) { }
        public PIReport(User createdBy) : base(createdBy)
        {
        }
        public virtual string ReferenceID { get; set; }
        public virtual string IndividualName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Inceptiondate { get; set; }
        public virtual string selectedlimit { get; set; }
        public virtual string Premium { get; set; }

    }



    public class EDReport : EntityBase
    {
        public EDReport() : this(null) { }

        public EDReport(User createdBy) : base(createdBy)
        {
        }
        public virtual string ReferenceID { get; set; }
        public virtual string IndividualName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Inceptiondate { get; set; }
        public virtual string selectedlimit { get; set; }
        public virtual string Premium { get; set; }

    }
}



