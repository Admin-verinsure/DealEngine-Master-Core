using System;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.WebUI.Controllers
{
	public interface IPermissionsService
	{
		/*
		 * bool RoleExists(string name);
		 * IdentityResult CreateRole(string name, string description = "");
		 * void DeleteRole(string roleId);
		 * IEnumerable<ApplicationRole> GetAllRoles();
		 * 
		 * IdentityResult CreateUser(User user, string password);
		 * IdentityResult AddUserToRole(Guid userId, string roleName);
		 * IEnumerable<ApplicationGroup> GetAllGroups();
		 * void AddUserToGroup(Guid userId, Guid groupId);
		 * 
		 * void ClearUserRoles(Guid userId);
		 * void ClearUserGroups(Guid userId);
		 * 
		 * void RemoveFromRole(Guid userId, string roleName);
		 * 
		 * bool GroupNameExists(string groupName);
		 * ApplicationGroup CreateGroup(string groupName);
		 * void DeleteGroup(Guid groupId);
		 * 
		 * void AddRoleToGroup(Guid groupId, string roleName);
		 * 
		 * void ClearGroupRoles(Guid groupId);
		 * */
	}

	public class PermissionsService : IPermissionsService
	{
		IMapperSession<ApplicationGroup> _groupRepository;
		IMapperSession<ApplicationRole> _roleRepository;

		public PermissionsService (IMapperSession<ApplicationGroup> groupRepository, IMapperSession<ApplicationRole> roleRepository)
		{
			_groupRepository = groupRepository;
			_roleRepository = roleRepository;
		}
	}
}

