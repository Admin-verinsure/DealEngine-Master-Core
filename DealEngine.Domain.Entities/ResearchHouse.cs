using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class ResearchHouse : EntityBase, IAggregateRoot
    {
        protected ResearchHouse() : base(null) { }

        public ResearchHouse(User createdBy)
            : base(createdBy)
        {
            
        }
        public virtual string ConstructionValue { get; set; }
        //public virtual string Fees { get; set; }
        //public virtual string ContractType { get; set; }
        //public virtual string MembershipNumber { get; set; }
     
        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }
     
        public virtual bool Removed
        {
            get;
            set;
        }
        public virtual string ResearchServices { get; set; }
        public virtual string ResearchName { get; set; }
     
    }
}
