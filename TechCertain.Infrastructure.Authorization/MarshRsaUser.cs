using System;
using System.Collections.Generic;

namespace TechCertain.Infrastructure.Authorization
{
	public class MarshRsaUser
	{
		public string Email { get; set; }

		public string DevicePrint { get; set; }

		public string DeviceTokenCookie { get; set; }

		public string ClientGenCookie { get; set; }

		public string IpAddress { get; set; }

		public string OrgName { get; set; }

		public string Otp { get; set; }

		public string SessionId { get; set; }

		public string TransactionId { get; set; }

		public string UserAgent { get; set; }

		public string HttpReferer { get; set; }

		public List<string> Groups { get; set; }

		public string Username { get; set; }

		public string CurrentSessionId { get; set; }

		public string CurrentTransactionId { get; set; }

		public MarshRsaUser (string email)
		{
			if (string.IsNullOrWhiteSpace (email))
				throw new ArgumentNullException (nameof (email));
			Email = email;
		}

		public bool ValidUser ()
		{
			return 
				(!string.IsNullOrWhiteSpace (Email) && 
				 !string.IsNullOrWhiteSpace (IpAddress) && 
				 !string.IsNullOrWhiteSpace (OrgName) && 
				 Groups != null && 
				 Groups.Count < 0);
		}
	}
}

