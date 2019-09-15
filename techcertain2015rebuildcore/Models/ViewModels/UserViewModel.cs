using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace techcertain2015rebuildcore.Models.ViewModels
{
	public class UserViewModel : BaseViewModel
	{
		public Guid ID { get; set; }

		[Required (ErrorMessage = "Incorrect Username")]
		[Display (Name = "Username: ")]
		public string UserName { get; set; }

		[Required (ErrorMessage = "Incorrect Email")]
		[Display (Name = "Email: ")]
		public string Email { get; set; }

		[Required (ErrorMessage = "Incorrect First Name")]
		[Display (Name = "First Name: ")]
		public string FirstName { get; set; }

		[Required (ErrorMessage = "Incorrect Last Name")]
		[Display (Name = "Last Name: ")]
		public string LastName { get; set; }

		[Display (Name = "Phone: ")]
		public string Phone { get; set; }

		[Display (Name = "Address: ")]
		public string Address { get; set; }

		[Display (Name = "Password: ")]
		public string Password { get; set; }

		[Display (Name = "Full Name: ")]
		public string FullName { get; set; }

		public Nullable<System.DateTime> ActivatedDate { get; set; }

		[Display (Name = "Secret Question: ")]
		public string SecretQuestion { get; set; }

		[Display (Name = "Secret Answer: ")]
		public string SecretAnswer { get; set; }

		[Display (Name = "Activated: ")]
		public bool Activated {
			get {
				return ActivatedDate.HasValue;
			}
		}

		[Display (Name = "Personal Email: ")]
		public string PersonalEmail { get; set; }

		public Guid[] Instances { get; set; }

		public bool OrganisationContactUser { get; set; }
	}
}

