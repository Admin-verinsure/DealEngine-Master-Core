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
        public virtual string Services { get; set; }
        public virtual string Name { get; set; }
        public virtual ResearchHouse CloneForNewSheet(ClientInformationSheet newSheet)
        {
            ResearchHouse newResearchHouse = new ResearchHouse(newSheet.CreatedBy);
            newResearchHouse.ClientInformationSheet = newSheet;
            newResearchHouse.ConstructionValue = ConstructionValue;
            newResearchHouse.DateCreated = DateTime.Now;
            newResearchHouse.Name = Name;
            newResearchHouse.Services = Services;
            return newResearchHouse;
        }
    }


}
