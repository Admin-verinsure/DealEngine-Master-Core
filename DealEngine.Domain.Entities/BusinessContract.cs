﻿using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
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
        public virtual string MembershipNumber { get; set; }
        public virtual Boolean ProjectDirector { get; set; }
        public virtual Boolean ProjectManager { get; set; }
        public virtual Boolean ProjectCoordinator { get; set; }
        public virtual Boolean ProjectEngineer { get; set; }
        public virtual string Country { get; set; }
        public virtual ClientInformationSheet ClientInformationSheet { get; set; }
        public virtual BusinessContract OriginalBusinessContract { get; protected set; }
        public virtual bool Removed { get; set; }
        public virtual string ProjectDescription { get; set; }
        public virtual string MajorResponsibilities { get; set; }
        public virtual string ProjectDuration { get; set; }
        
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
            newBusinessContract.ProjectDescription = ProjectDescription;
            newBusinessContract.MajorResponsibilities = MajorResponsibilities;
            newBusinessContract.ProjectDuration = ProjectDuration;
            return newBusinessContract;
        }
    }
}
