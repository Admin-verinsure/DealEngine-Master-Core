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
		
	}

	// Used to display a single role group with a checkbox, within a list structure:
	public class SelectGroupEditorViewModel
	{
		public SelectGroupEditorViewModel () { }		

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

		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public List<RoleViewModel> Roles { get; set; }
	}
}

