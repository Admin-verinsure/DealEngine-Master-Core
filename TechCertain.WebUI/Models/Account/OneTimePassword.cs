using System;

namespace TechCertain.WebUI.Models.Account
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

	public class RsaOneTimePasswordModel 
	{
		public string DevicePrint { get; set; }

		public string SessionId { get; set; }

		public string TransactionId { get; set; }

		public string UserName { get; set; }

		public string OtpCode { get; set; }

        public string DeviceTokenCookie { get; set; }
    }
}

