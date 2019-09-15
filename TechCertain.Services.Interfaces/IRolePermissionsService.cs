using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IRolePermissionsService
	{
		void AddRolesToGroup (Guid [] roleIds, Guid groupId);
		void AddRolesToGroups (Guid [] roleIds, Guid [] groupIds);
		void AddRoleToGroup (Guid roleId, Guid groupId);
		void AddRoleToGroups (Guid roleId, Guid [] groupIds);

		void AddUsersToGroup (Guid [] userIds, Guid groupId);
		void AddUsersToGroups (Guid [] userIds, Guid [] groupIds);
		void AddUserToGroup (Guid userId, Guid groupId);
		void AddUserToGroups (Guid userId, Guid [] groupIds);

		ApplicationGroup CreateGroup (string groupName);
		ApplicationRole CreateRole (string roleName);
		bool DeleteGroup (Guid groupId, User deletingUser);
		bool DeleteRole (Guid roleId, User deletingUser);
		bool UpdateGroup (ApplicationGroup appGroup);
		bool UpdateRole (ApplicationRole appRole);

		bool GroupExists (Guid groupId);
		bool RoleExists (Guid roleId);

		string [] GetRolesForUser (string username);

		string [] GetUsersInGroup (string groupName);
		string [] GetUsersWithRole (string roleName);

		ApplicationGroup [] GetAllGroups ();
		ApplicationGroup [] GetAllGroups (bool includeDeleted);
		ApplicationRole [] GetAllRoles ();
		ApplicationRole [] GetAllRoles (bool includeDeleted);

		ApplicationGroup GetGroup (Guid groupId);
		ApplicationGroup GetGroup (string groupname);
		ApplicationRole GetRole (Guid roleId);
		ApplicationRole GetRole (string rolename);

		bool IsUserInGroup (string username, string groupname);
		bool DoesUserHaveRole (string username, string rolename);

		void RemoveRoleFromGroup (Guid roleId, Guid groupId);
		void RemoveRoleFromGroups (Guid roleId, Guid [] groupIds);
		void RemoveRolesFromGroup (Guid [] roleIds, Guid groupId);
		void RemoveRolesFromGroups (Guid [] roleIds, Guid [] groupIds);

		void RemoveUserFromGroup (Guid userId, Guid groupId);
		void RemoveUserFromGroups (Guid userId, Guid [] groupIds);
		void RemoveUsersFromGroup (Guid [] userIds, Guid groupId);
		void RemoveUsersFromGroups (Guid [] userIds, Guid [] groupIds);

		void SetDefaultPermissions (string username);
		void SetUpDefaultObjects ();
	}
}

