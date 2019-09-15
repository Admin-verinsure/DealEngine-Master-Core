using System;
namespace techcertain2015rebuildcore.Models.ViewModels.Account
{
	public class OneTimePasswordModel
	{
		public Guid UserId { get; set; }

		public string UserName { get; set; }

		public string OtpCode { get; set; }

		public OneTimePasswordModel (Guid userId)
		{
			UserId = userId;
		}

		public OneTimePasswordModel (string userName)
		{
			UserName = userName;
		}
	}

	public class RsaOneTimePasswordModel : OneTimePasswordModel
	{
		public string DevicePrint { get; set; }

		public string SessionId { get; set; }

		public string TransactionId { get; set; }

		public RsaOneTimePasswordModel (Guid userId)
			: base (userId)
		{

		}

		public RsaOneTimePasswordModel (string userName)
			: base (userName)
		{

		}
	}
}

