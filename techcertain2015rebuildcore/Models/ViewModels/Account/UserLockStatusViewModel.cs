using System;
namespace techcertain2019core.Models.ViewModels.Account
{
	public class UserLockStatusViewModel : BaseViewModel
	{
		public Guid Id { get; set; }

		public string Status { get; set; }

		public UserLockStatusViewModel ()
		{
		}
	}
}

