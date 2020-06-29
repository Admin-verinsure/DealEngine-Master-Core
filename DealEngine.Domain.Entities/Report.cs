using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;


namespace DealEngine.Domain.Entities
{
    public class Report : EntityBase
    {
        //private Organisation _primaryOrganisation;
        public Report() : this(null) { }

        public Report(User createdBy) : base(createdBy)
        {
        }
      //  [DontShowMe]
        public virtual string MemberName { get; set; }
        public virtual string ReferenceID { get; set; }
      // [DontShowMe]
        public virtual string IndividualName { get; set; }
       // [DontShowMe]
        public virtual string CompanyName { get; set; }

      //  [DontShowMe]
        public virtual string selectedlimit { get; set; }
        //public virtual string Limit { get; set; }
        public virtual string Premium { get; set; }
        public virtual string selectedwaerdlimit { get; set; }
        //public virtual string Limit { get; set; }
        public virtual string Premwerium { get; set; }
        public virtual string Inceptiondate { get; set; }

    }

    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    //public class DontShowMe : Attribute
    //{
    //    private object p;
    //    public DontShowMe() : this(null) { }

    //    public DontShowMe(object p)
    //    {
    //        this.p = p;
    //    }
    //}
}



