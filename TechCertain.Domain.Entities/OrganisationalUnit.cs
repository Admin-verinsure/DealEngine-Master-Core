using System;
using System.Collections;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class OrganisationalUnit : EntityBase, IAggregateRoot
    {
		protected OrganisationalUnit () : base (null) {}

        public OrganisationalUnit(User createdBy, string name) 
			: base (createdBy)
		{
            Name = name;
            Locations = new List<Location>();
        }

        public virtual string Name { get; set; }

		public virtual Organisation Company { get; set; }

		public virtual IEnumerable<BranchCode> BranchCodes { get; set; }

		public virtual IEnumerable<User> Users { get; set; }

        public virtual IList<Location> Locations { get; set; }

        public virtual string EserviceProducerCode { get; set; }

        public virtual string EbixDepartmentCode { get; set; }

        public virtual string HPFBranchCode { get; set; }

    }
}