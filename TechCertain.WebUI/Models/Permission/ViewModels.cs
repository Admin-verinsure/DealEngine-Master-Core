using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Permission
{
	// Used to display a single role with a checkbox, within a list structure:
	public class SelectRoleEditorViewModel : BaseViewModel
	{
		public SelectRoleEditorViewModel () { }

		// Update this to accept an argument of type ApplicationRole:
		public SelectRoleEditorViewModel (ApplicationRole role)
		{
			RoleId = role.Id;
			RoleName = role.Name;

			// Assign the new Descrption property:
			Description = role.Description;
		}

		public bool Selected { get; set; }

		public Guid RoleId { get; set; }

		[Required]
		public string RoleName { get; set; }

		// Add the new Description property:
		public string Description { get; set; }
	}

	// Wrapper for SelectGroupEditorViewModel to select user group membership:
	public class SelectUserGroupsViewModel
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public List<SelectGroupEditorViewModel> Groups { get; set; }

		public SelectUserGroupsViewModel ()
		{
			Groups = new List<SelectGroupEditorViewModel> ();
		}

		public SelectUserGroupsViewModel (User user, IEnumerable<ApplicationGroup> groups)
			: this ()
		{
			UserId = user.Id;
			UserName = user.UserName;
			FirstName = user.FirstName;
			LastName = user.LastName;

			foreach (var group in groups)
				Groups.Add (new SelectGroupEditorViewModel (group));

			foreach (var group in user.Groups) {
				var groupModel = Groups.Find (g => g.GroupId == group.Id);
				if (groupModel != null)
					groupModel.Selected = true;
			}

			//var Db = new ApplicationDbContext ();

			//// Add all available groups to the public list:
			//var allGroups = Db.Groups;
			//foreach (var role in allGroups) {
			//	// An EditorViewModel will be used by Editor Template:
			//	var rvm = new SelectGroupEditorViewModel (role);
			//	this.Groups.Add (rvm);
			//}

			//// Set the Selected property to true where user is already a member:
			//foreach (var group in user.Groups) {
			//	var checkUserRole =
			//		this.Groups.Find (r => r.GroupName == group.Group.Name);
			//	checkUserRole.Selected = true;
			//}
		}
	}

	// Used to display a single role group with a checkbox, within a list structure:
	public class SelectGroupEditorViewModel
	{
		public SelectGroupEditorViewModel () { }
		public SelectGroupEditorViewModel (ApplicationGroup group)
		{
			GroupName = group.Name;
			GroupId = group.Id;
		}

		public bool Selected { get; set; }

		[Required]
		public Guid GroupId { get; set; }
		public string GroupName { get; set; }
	}

	public class SelectGroupRolesViewModel : BaseViewModel
	{
		public Guid GroupId { get; set; }
		public string GroupName { get; set; }
		public BaseListViewModel<SelectRoleEditorViewModel> Roles { get; set; }
		
		public SelectGroupRolesViewModel ()
		{
			Roles = new BaseListViewModel<SelectRoleEditorViewModel> ();
		}
	}

	public class UserPermissionsViewModel
	{
		public UserPermissionsViewModel ()
		{
			Roles = new List<RoleViewModel> ();
		}

		// Enable initialization with an instance of ApplicationUser:
		public UserPermissionsViewModel (User user)
			: this ()
		{
			UserName = user.UserName;
			FirstName = user.FirstName;
			LastName = user.LastName;
			foreach (var role in user.GetRoles()) {
				var appRole = role;
				var pvm = new RoleViewModel (appRole);
				Roles.Add (pvm);
			}
		}

		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public List<RoleViewModel> Roles { get; set; }
	}
}

