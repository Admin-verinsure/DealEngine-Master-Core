using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class TerritoryTemplate : EntityBase, IAggregateRoot
    {
		public virtual string Location { get; set; }

        public virtual string[] SelectedInclorExcl { get; set; }

        public virtual string ExclorIncl { get; set; }

        public virtual bool Ispublic { get; set; }

        public virtual int Zoneorder { get; set; }
                
        public virtual Programme Programme { get; set; }

        protected TerritoryTemplate() : base (null) { }

		public TerritoryTemplate(User createdBy, string location)
			: base (createdBy)
		{
            Location = location;
		}
	}
}

