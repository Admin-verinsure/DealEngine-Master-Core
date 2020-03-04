using System;

namespace DealEngine.Infrastructure.BaseLdap.Entities
{
	public class LdapBranch : BaseLdapEntity
	{
		public Guid OrganisationID { get; set; }

		public string Name { get; set; }

		public LdapLocation BranchLocation { get; set; }

		public LdapBranch (Guid organisationBranchID, Guid parentOrganisationID, string branchName)
			: base(organisationBranchID)
		{
			OrganisationID = parentOrganisationID;
			Name = branchName;
		}
	}
}

