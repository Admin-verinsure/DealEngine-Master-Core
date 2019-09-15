using System;
using TechCertain.Domain.Entities;
using techcertain2019core.Models.ViewModels.Permission;

namespace techcertain2019core.Models.ViewModels.Account
{
	public class ManageUserViewModel : BaseViewModel
	{
		public Guid Id { get; set; }

		public string Username { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public SelectUserGroupsViewModel UserGroups { get; set; }

		public UserAccountStatusViewModel AccountStatus { get; set; }

		public ManageUserViewModel () { }

		public ManageUserViewModel (User user)
		{
			Id = user.Id;
			Username = user.UserName;
			FirstName = user.FirstName;
			LastName = user.LastName;
			Email = user.Email;
			AccountStatus = new UserAccountStatusViewModel (user);
		}
	}

	public class UserAccountStatusViewModel : BaseViewModel
	{
		public bool IsLocked { get; set; }

		public string LockedMessage { get; set; }

		public string LastPasswordResetIssued { get; set; }

		public string PasswordResetExpiryDate { get; set; }

		public string PasswordResetStatus { get; set; }

		public UserAccountStatusViewModel (User user)
		{
			IsLocked = user.Locked;
			if (IsLocked)
				LockedMessage = "Locked on " + user.LockTime.GetValueOrDefault ().ToString ("f");
			else
				LockedMessage = "Not currently locked";
		}
	}
}

