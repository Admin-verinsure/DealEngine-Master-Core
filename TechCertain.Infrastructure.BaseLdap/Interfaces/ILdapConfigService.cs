using System;

namespace TechCertain.Infrastructure.BaseLdap.Interfaces
{
	public interface ILdapConfigService
	{
		string ServerAddress { get; }

		int ServerPort { get; }

		string[] SearchAttributes { get; }

		string BaseDN { get; }

		string AdminBindDN { get; }

		string AdminBindPassword { get; }

		string BasePoliciesDN { get; }

		string BaseUserDN { get; }

		string BaseOrganisationDN { get; }

		string BaseBranchDN { get; }

		string BaseDepartmentDN { get; }

		string BaseRoleDN { get; }

		string GetUserDN(string userName);

		string GetOrganisationDN (Guid organisationID);

		string GetDepartmentDN (Guid departmentID);

		string GetBranchDN (Guid branchId);

		string GetRoleDN (string roleName);

		string GetBranchesDNByOrganisation (Guid organisationID);

		string GetDepartmentsDNByOrganisation (Guid organisationID);

		string GetUsersByIdSearch (Guid userID);

		string GetUsersByEmailSearch (string email);

		string GetUsersByOrganisationSearch (Guid organisationID);

		string GetUsersByBranchSearch (Guid branchID);

		string GetUsersByDepartmentSearch (Guid department);
	}
}