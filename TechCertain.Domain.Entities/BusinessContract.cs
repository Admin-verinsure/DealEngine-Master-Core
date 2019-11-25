using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class BusinessContract : EntityBase, IAggregateRoot
    {
        protected BusinessContract() : base(null) { }

        public BusinessContract(User createdBy)
            : base(createdBy)
        {
            
        }
        public virtual string ContractTitle { get; set; }
        public virtual string Year { get; set; }
        public virtual string ConstructionValue { get; set; }
        public virtual string Fees { get; set; }
        public virtual string ContractType { get; set; }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }
        public virtual BusinessContract OriginalBusinessContract
        {
            get;
            protected set;
        }
        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual BusinessContract CloneForNewSheet(ClientInformationSheet newSheet)
        {
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone business contract for original information");

            BusinessContract newBusinessContract = new BusinessContract(newSheet.CreatedBy);
            newBusinessContract.Removed = Removed;
            newBusinessContract.Year = Year;
            newBusinessContract.ContractType = ContractType;
            newBusinessContract.Fees = Fees;
            newBusinessContract.ContractTitle = ContractTitle;
            newBusinessContract.ConstructionValue = ConstructionValue;
            newBusinessContract.OriginalBusinessContract = this;
            return newBusinessContract;
        }
    }
}
