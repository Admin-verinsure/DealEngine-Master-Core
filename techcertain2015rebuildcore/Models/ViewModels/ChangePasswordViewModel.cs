using System;
using System.ComponentModel.DataAnnotations;

namespace techcertain2015rebuildcore.Models.ViewModels
{
	public class ChangePasswordViewModel : BaseViewModel
	{
		[Required (ErrorMessage = "Please enter your current password")]
		[Display (Name = "Confirm password")]
		public string CurrentPassword { get; set; }

		[Required (ErrorMessage = "Please enter your new password")]
		[Display (Name = "Enter new password")]
		public string NewPassword { get; set; }

		[Required (ErrorMessage = "Please confirm your new password")]
		[Display (Name = "Confirm new password")]
		public string ConfirmNewPassword { get; set; }
	}
}

