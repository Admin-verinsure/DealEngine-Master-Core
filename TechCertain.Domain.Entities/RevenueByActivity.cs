using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
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
        public virtual decimal TotalRevenue { get; set; }
        public virtual AdditionalActivityInformation AdditionalActivityInformation { get; set; }
    }

    public class AdditionalActivityInformation : EntityBase, IAggregateRoot
    {
        public virtual string InspectionReportTextId { get; set; }
        public virtual string InspectionReportBoolId { get; set; }
        public virtual string ValuationTextId { get; set; }
        public virtual string ValuationTextId2 { get; set; }
        public virtual string SchoolsDesignWorkBoolId { get; set; }
        public virtual string SchoolsDesignWorkBoolId2 { get; set; }
        public virtual string SchoolsDesignWorkBoolId3 { get; set; }
        public virtual string SchoolsDesignWorkBoolId4 { get; set; }
        public virtual string OtherActivitiesTextId { get; set; }
        public virtual string CanterburyEarthquakeRebuildWorkId { get; set; }
        public virtual string ValuationBoolId { get; set; }

        protected AdditionalActivityInformation() : this(null) { }

        public AdditionalActivityInformation(User createdBy)
            : base(createdBy)
        {
        }



    }
    
}

