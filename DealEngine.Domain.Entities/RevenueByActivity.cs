using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class RevenueByActivity : EntityBase, IAggregateRoot
	{
		protected RevenueByActivity () : this (null) {}

		public RevenueByActivity (User createdBy)
			: base (createdBy)
		{
            Territories = new List<Territory>();
            Activities = new List<BusinessActivity>(); 

        }

		public virtual IList<Territory> Territories { get; set; }
		public virtual IList<BusinessActivity> Activities { get; set; }
        public virtual decimal CurrentYear { get; set; }
        public virtual decimal LastFinancialYear { get; set; }
        public virtual decimal NextFinancialYear { get; set; }
        public virtual AdditionalActivityInformation AdditionalActivityInformation { get; set; }
    }

    public class AdditionalActivityInformation : EntityBase, IAggregateRoot
    {
        public virtual string InspectionReportTextId { get; set; }
        public virtual int InspectionReportBoolId { get; set; }
        public virtual string ValuationTextId { get; set; }
        public virtual string ValuationTextId2 { get; set; }
        public virtual int SchoolsDesignWorkBoolId { get; set; }
        public virtual int SchoolsDesignWorkBoolId2 { get; set; }
        public virtual int SchoolsDesignWorkBoolId3 { get; set; }
        public virtual int SchoolsDesignWorkBoolId4 { get; set; }
        public virtual string OtherActivitiesTextId { get; set; }
        public virtual string CanterburyEarthquakeRebuildWorkId { get; set; }
        public virtual int ValuationBoolId { get; set; }
        public virtual string OtherProjectManagementTextId { get; set; }
        public virtual string NonProjectManagementTextId { get; set; }
        public virtual decimal ConstructionCommercial { get; set; }
        public virtual decimal ConstructionDwellings { get; set; }
        public virtual decimal ConstructionIndustrial { get; set; }
        public virtual decimal ConstructionInfrastructure { get; set; }
        public virtual decimal ConstructionSchool { get; set; }

        protected AdditionalActivityInformation() : this(null) { }

        public AdditionalActivityInformation(User createdBy)
            : base(createdBy)
        {
        }



    }
    
}

