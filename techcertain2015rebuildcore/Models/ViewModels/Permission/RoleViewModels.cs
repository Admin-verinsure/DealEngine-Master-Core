using System;
using System.ComponentModel.DataAnnotations;
using TechCertain.Domain.Entities;

namespace techcertain2019core.Models.ViewModels.Permission
{
	public class RoleViewModel
	{
		public Guid Id { get; set; }
		public string RoleName { get; set; }
		public string Description { get; set; }
		public bool IsSystemRole { get; set; }

		public RoleViewModel () { }
		public RoleViewModel (ApplicationRole role)
		{
			Id = role.Id;
			RoleName = role.Name;
			Description = role.Description;
			IsSystemRole = role.IsSystemRole;
		}
	}

	public class EditRoleViewModel
	{
		public Guid Id { get; set; }
		public string OriginalRoleName { get; set; }
		public string RoleName { get; set; }
		public string Description { get; set; }

		public EditRoleViewModel () { }
		public EditRoleViewModel (ApplicationRole role)
		{
			Id = role.Id;
			OriginalRoleName = role.Name;
			RoleName = role.Name;
			Description = role.Description;
		}
	}
}

