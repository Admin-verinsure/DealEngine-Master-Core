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
        public virtual AdditionalInformation AdditionalInformation { get; set; }
    }

    public class AdditionalInformation : EntityBase, IAggregateRoot
    {
        protected AdditionalInformation() : this(null) { }

        public AdditionalInformation(User createdBy)
            : base(createdBy)
        {
        }

        public virtual InspectionReportsSupplementaryQuestionnaire InspectionReportsSupplementaryQuestionnaire { get; set; }



    }

    public class InspectionReportsSupplementaryQuestionnaire : EntityBase, IAggregateRoot
    {
        protected InspectionReportsSupplementaryQuestionnaire() : this(null) { }

        public InspectionReportsSupplementaryQuestionnaire(User createdBy)
            : base(createdBy)
        {
        }

        public virtual string QuestionOneAnswer { get; set; }
        public virtual bool QuestionTwoAnswer { get; set; }

    }
}

