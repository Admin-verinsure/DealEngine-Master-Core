using System;
using System.ComponentModel.DataAnnotations;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Permission
{
	public class RoleViewModel
	{
		public Guid Id { get; set; }
		public string RoleName { get; set; }
		public string Description { get; set; }
		public bool IsSystemRole { get; set; }

		public RoleViewModel () { }
	}

	public class EditRoleViewModel
	{
		public Guid Id { get; set; }
		public string OriginalRoleName { get; set; }
		public string RoleName { get; set; }
		public string Description { get; set; }

		public EditRoleViewModel () { }
	}
}

