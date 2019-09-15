using System;
using System.ComponentModel.DataAnnotations;

namespace techcertain2015rebuildcore.Models.ViewModels
{
	public class AccountImportViewModel : BaseViewModel
	{
		[Required (ErrorMessage = "Incorrect Username")]
		[Display (Name = "Username")]
		public string Username { get; set; }

		[Required (ErrorMessage = "Incorrect First name")]
		[Display (Name = "First name")]
		public string FirstName { get; set; }

		[Required (ErrorMessage = "Incorrect Last name")]
		[Display (Name = "Last name")]
		public string LastName { get; set; }

		[Required (ErrorMessage = "Incorrect Email")]
		[Display (Name = "Email")]
		public string Email { get; set; }
	}
}

