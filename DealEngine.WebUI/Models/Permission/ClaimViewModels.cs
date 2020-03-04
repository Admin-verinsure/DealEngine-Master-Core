using System;
using System.ComponentModel.DataAnnotations;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Permission
{
	public class ClaimViewModels
	{
		public Guid Id { get; set; }
		public string ClaimName { get; set; }
		public string Description { get; set; }
		public bool IsSystemRole { get; set; }

		public ClaimViewModels() { }
		public ClaimViewModels(AuthClaims claim)
		{
			Id = claim.Id;
            ClaimName = claim.Name;
			Description = claim.Description;
			IsSystemRole = claim.IsSystemRole;
		}
	}

	//public class EditRoleViewModel
	//{
	//	public Guid Id { get; set; }
	//	public string OriginalRoleName { get; set; }
	//	public string RoleName { get; set; }
	//	public string Description { get; set; }

	//	public EditRoleViewModel () { }
	//	public EditRoleViewModel (ApplicationRole role)
	//	{
	//		Id = role.Id;
	//		OriginalRoleName = role.Name;
	//		RoleName = role.Name;
	//		Description = role.Description;
	//	}
	//}
}

