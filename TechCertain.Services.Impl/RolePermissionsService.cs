using System;
using System.Configuration;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class RolePermissionsService : IRolePermissionsService
	{
		IMapperSession<ApplicationGroup> _groupRespoitory;
		IMapperSession<ApplicationRole> _roleRepository;

		IUserService _userService;
		IMapperSession<User> _userRepository;

		IUnitOfWork _uowFactory;

		public RolePermissionsService (IUserService userService, IMapperSession<User> userRepository, IMapperSession<ApplicationGroup> groupRepository, IMapperSession<ApplicationRole> roleRepository, IUnitOfWork uowFactory)
		{
			_groupRespoitory = groupRepository;
			_roleRepository = roleRepository;

			_userService = userService;
			_userRepository = userRepository;

			_uowFactory = uowFactory;

			//SetUpDefaultObjects ();
		}

		public void AddRolesToGroup (Guid [] roleIds, Guid groupId)
		{
			AddRolesToGroups (roleIds, new Guid [] { groupId });
		}

		public void AddRolesToGroups (Guid [] roleIds, Guid [] groupIds)
		{
			var groups = _groupRespoitory.FindAll ().Where (g => groupIds.Contains (g.Id));
			var roles = _roleRepository.FindAll ().Where (r => roleIds.Contains (r.Id));

			foreach (var appGroup in groups) {
				foreach (var appRole in roles) {
					if (!appGroup.Roles.Contains (appRole))
						appGroup.Roles.Add (appRole);
				}
				UpdateGroup (appGroup);
			}
		}

		public void AddRoleToGroup (Guid roleId, Guid groupId)
		{
			AddRolesToGroups (new Guid [] { roleId }, new Guid [] { groupId });
		}

		public void AddRoleToGroups (Guid roleId, Guid [] groupIds)
		{
			AddRolesToGroups (new Guid [] { roleId }, groupIds);
		}

		public void AddUsersToGroup (Guid [] userIds, Guid groupId)
		{
			AddUsersToGroups (userIds, new Guid [] { groupId });
		}

		public void AddUsersToGroups (Guid [] userIds, Guid [] groupIds)
		{
			var groups = _groupRespoitory.FindAll ().Where (g => groupIds.Contains (g.Id)).ToArray();
			var users = _userRepository.FindAll ().Where (u => userIds.Contains (u.Id)).ToArray();

			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				foreach (var user in users) {
					foreach (var group in groups)
						if (!user.Groups.Contains(group))
							user.Groups.Add (group);
					_userRepository.Add (user);
				}
				uow.Commit ();
			};
		}

		public void AddUserToGroup (Guid userId, Guid groupId)
		{
			AddUsersToGroups (new Guid [] { userId }, new Guid [] { groupId });
		}

		public void AddUserToGroups (Guid userId, Guid [] groupIds)
		{
			AddUsersToGroups (new Guid [] { userId }, groupIds);
		}

		public ApplicationGroup CreateGroup (string groupName)
		{
			if (_groupRespoitory.FindAll().FirstOrDefault (g => g.Name == groupName) != null)
				return null;
			ApplicationGroup group;
			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				group = new ApplicationGroup (null, groupName);
				_groupRespoitory.Add (group);
				uow.Commit ();
			};
			return group;
		}

		public ApplicationRole CreateRole (string roleName)
		{
			if (_roleRepository.FindAll().FirstOrDefault (r => r.Name == roleName) != null)
				return null;
			ApplicationRole appRole;
			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				appRole = new ApplicationRole (null, roleName, "");
				_roleRepository.Add (appRole);
				uow.Commit ();
			};
			return appRole;
		}

		public bool DeleteGroup (Guid groupId, User deletingUser)
		{
			ApplicationGroup group = GetGroup(groupId);
			if (group == null)
				return false;
			
			group.Delete (deletingUser);
			return UpdateGroup (group);
		}

		public bool DeleteRole (Guid roleId, User deletingUser)
		{
			ApplicationRole appRole = GetRole(roleId);
			if (appRole == null)
				return false;
			if (appRole.IsSystemRole)
				return false;

			appRole.Delete (deletingUser);
			return UpdateRole (appRole);
		}

		public bool DoesUserHaveRole (string username, string roleName)
		{
			User user = _userService.GetUser (username);
			if (user == null)
				return false;
			return user.GetRoles ().Any (r => r.Name == roleName);
		}

		public ApplicationGroup [] GetAllGroups ()
		{
			return GetAllGroups (false);
		}

		public ApplicationGroup [] GetAllGroups (bool includeDeleted)
		{
			IQueryable<ApplicationGroup> groups = _groupRespoitory.FindAll ();
			if (!includeDeleted)
				groups = groups.Where (g => !g.DateDeleted.HasValue);

			return groups.ToArray ();
		}

		public ApplicationRole [] GetAllRoles ()
		{
			return GetAllRoles (false);
		}

		public ApplicationRole [] GetAllRoles (bool includeDeleted)
		{
			IQueryable<ApplicationRole> roles = _roleRepository.FindAll ();
			if (!includeDeleted)
				roles = roles.Where (g => !g.DateDeleted.HasValue);

			return roles.ToArray ();
		}

		public ApplicationGroup GetGroup (Guid groupId)
		{
			return _groupRespoitory.GetById (groupId);
		}

		public ApplicationGroup GetGroup (string groupname)
		{
			return GetAllGroups ().FirstOrDefault (g => g.Name == groupname);
		}

		public ApplicationRole GetRole (Guid roleId)
		{
			return _roleRepository.GetById (roleId);
		}

		public ApplicationRole GetRole (string rolename)
		{
			return GetAllRoles ().FirstOrDefault (r => r.Name == rolename);
		}

		public string [] GetRolesForUser (string username)
		{
			if (string.IsNullOrWhiteSpace (username))
				return new string [0];
			
			return _userService.GetUser (username).GetRoles ().Select (r => r.Name).ToArray ();
		}

		public string [] GetUsersInGroup (string groupName)
		{
			// horrible
			return _groupRespoitory.FindAll ().Where (g => g.Name == groupName).SelectMany (u => u.Users).Select(u => u.UserName).ToArray ();
		}

		public string [] GetUsersWithRole (string roleName)
		{
			// also horrible
			//return _groupRespoitory.FindAll ().Where (u => u.Roles.Select(r => r.Name).Contains (roleName)).Select (u => u.UserName).ToArray ();
			// even more horrible
			return _roleRepository.FindAll ().Where (r => r.Name == roleName).SelectMany (r => r.Groups).SelectMany(g => g.Users).Select (u => u.UserName).ToArray ();
			// is there even a clean way to do this?

		}

		public bool GroupExists (Guid groupId)
		{
			return _groupRespoitory.FindAll ().FirstOrDefault (g => g.Id == groupId) != null;
		}

		public bool IsUserInGroup (string username, string groupName)
		{
			return _userService.GetUser (username).Groups.FirstOrDefault (g => g.Name == groupName) != null;
		}

		public void RemoveRoleFromGroup (Guid roleId, Guid groupId)
		{
			RemoveRolesFromGroups (new Guid [] { roleId }, new Guid [] { groupId });
		}

		public void RemoveRoleFromGroups (Guid roleId, Guid [] groupIds)
		{
			RemoveRolesFromGroups (new Guid [] { roleId }, groupIds);
		}

		public void RemoveRolesFromGroup (Guid [] roleIds, Guid groupId)
		{
			RemoveRolesFromGroups (roleIds, new Guid [] { groupId });
		}

		public void RemoveRolesFromGroups (Guid [] roleIds, Guid [] groupIds)
		{
			foreach (Guid groupId in groupIds) {
				ApplicationGroup appGroup = GetGroup (groupId);
				if (appGroup != null) {
					var rolesToRemove = appGroup.Roles.Where (r => roleIds.Contains (r.Id)).ToArray ();
					foreach (var appRole in rolesToRemove) {
						appGroup.Roles.Remove (appRole);
						appRole.Groups.Remove (appGroup);
						UpdateRole (appRole);
					}
					UpdateGroup (appGroup);
				}
			}


			//foreach (Guid roleId in roleIds) {
			//	ApplicationRole appRole = _roleRepository.FindAll ().FirstOrDefault (r => r.Id == roleId);
			//	if (appRole != null) {
			//		var groupsToRemove = appRole.Groups.Where (g => groupIds.Contains (g.Id)).ToArray();
			//		foreach (var appGroup in groupsToRemove)
			//			appRole.Groups.Remove (appGroup);
			//		UpdateRole (appRole);
			//	}
			//}
		}

		public void RemoveUserFromGroup (Guid userId, Guid groupId)
		{
			RemoveUsersFromGroups (new Guid [] { userId }, new Guid [] { groupId });
		}

		public void RemoveUserFromGroups (Guid userId, Guid [] groupIds)
		{
			RemoveUsersFromGroups (new Guid [] { userId }, groupIds);
		}

		public void RemoveUsersFromGroup (Guid [] userIds, Guid groupId)
		{
			RemoveUsersFromGroups (userIds, new Guid [] { groupId });
		}

		public void RemoveUsersFromGroups (Guid [] userIds, Guid [] groupIds)
		{
			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				foreach (Guid userId in userIds) {
					User user = _userService.GetUser (userId);
					if (user != null) {
						var groupsToRemove = user.Groups.Where (g => groupIds.Contains (g.Id)).ToArray ();
						foreach (var appGroup in groupsToRemove) {
							user.Groups.Remove (appGroup);
						}
					}
					_userRepository.Add (user);
				}
				uow.Commit ();
			}
		}

		public bool RoleExists (Guid roleId)
		{
			throw new NotImplementedException ();
		}

		public void SetDefaultPermissions (string username)
		{
			if (string.IsNullOrWhiteSpace (username))
				throw new ArgumentNullException (nameof (username));

			User user = _userService.GetUser (username);

			string [] superusers = ConfigurationManager.AppSettings ["SuperUsers"].Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (superusers.Contains (username)) {
				if (!IsUserInGroup (username, "SystemAdmin")) {
					AddUserToGroup (user.Id, GetGroup("SystemAdmin").Id);
				}
			}
			if (!IsUserInGroup (username, "Client"))
				AddUserToGroup (user.Id, GetGroup("Client").Id);
		}

		public bool UpdateGroup (ApplicationGroup appGroup)
		{
			bool result = false;
			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				_groupRespoitory.Add (appGroup);
				uow.Commit ();
			}
			return result;
		}

		public bool UpdateRole (ApplicationRole appRole)
		{
			bool result = false;
			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				_roleRepository.Add (appRole);
				uow.Commit ();
			}
			return result;
		}

		public void SetUpDefaultObjects ()
		{
			ApplicationRole [] defaultSystemRoles = {
				new ApplicationRole(null, "Admin", "Global Access", true),
				new ApplicationRole(null, "CanEditUser", "Add, modify and delete Users", true),
				new ApplicationRole(null, "CanEditGroup", "Add, modify and delete Groups", true),
				new ApplicationRole(null, "CanEditRole", "Add, modify and delete Roles", true),
				new ApplicationRole(null, "CanEditProduct", "Add, modify and delete Products", true),
				new ApplicationRole(null, "CanEditProgramme", "Add, modify and delete Programmes", true),
				new ApplicationRole(null, "CanViewAllInformation", "Can view all available Information Sheets", true),
			};
			ApplicationGroup [] defaultSystemGroups = {
				new ApplicationGroup(null, "SystemAdmin", true),
				new ApplicationGroup(null, "Client", true),
			};

			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				string [] groupNames = GetAllGroups ().Select (g => g.Name).ToArray ();
				foreach (var systemGroup in defaultSystemGroups)
					if (!groupNames.Contains (systemGroup.Name))
						_groupRespoitory.Add (new ApplicationGroup (null, systemGroup.Name, true));

				string [] roleNames = GetAllRoles ().Select (r => r.Name).ToArray ();
				foreach (var systemRole in defaultSystemRoles)
					if (!roleNames.Contains (systemRole.Name))
						_roleRepository.Add (new ApplicationRole (null, systemRole.Name, systemRole.Description, true));

				var adminGroup = _groupRespoitory.FindAll ().FirstOrDefault (g => g.Name == "SystemAdmin");
				foreach (var appRole in _roleRepository.FindAll ())
					if (!adminGroup.Roles.Contains (appRole))
						adminGroup.Roles.Add (appRole);

				uow.Commit ();
			}
		}
	}
}

