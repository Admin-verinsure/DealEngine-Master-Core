﻿using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class Territory : EntityBase, IAggregateRoot
    {
		public virtual string Location
		{
			get;
			set;
		}

        public virtual string[] SelectedInclorExcl { get; set; }

        //public RiskEntityViewModel [] Risks { get; set; }
        public virtual string ExclorIncl { get; set; }

        public virtual Boolean Ispublic { get; set; }

        public virtual int Zoneorder { get; set; }

        public virtual IList<Organisation> Organisation
        {
            get;
            set;
        }
        
        public virtual decimal Pecentage { get; set; }
        public virtual Programme Programme { get; set; }

        protected Territory() : base (null) { }

		public Territory (User createdBy, string location)
			: base (createdBy)
		{
            Location = location;
		}
	}
}

