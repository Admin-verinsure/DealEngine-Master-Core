using System;
using System.Collections.Generic;

namespace DealEngine.Infrastructure.BaseLdap.Entities
{
	public class LdapOrganisation : BaseLdapEntity
	{
		public string Name { get; set; }

		public string Role { get; set; }

		public string Description { get; set; }

		public string Phone { get; set; }

		public string Domain { get; set; }
        
		public LdapLocation Location { get; set; }

		public List<LdapDepartment> Departments { get; set; }

		public List<LdapBranch> Branches { get; set; }

		public LdapOrganisation (Guid organisationID)
			: base(organisationID)
		{
			
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

