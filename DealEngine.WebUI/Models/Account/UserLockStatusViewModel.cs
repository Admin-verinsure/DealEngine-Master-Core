using System;
namespace DealEngine.WebUI.Models.Account
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

