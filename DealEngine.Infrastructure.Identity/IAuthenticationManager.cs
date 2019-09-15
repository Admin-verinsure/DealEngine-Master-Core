using System;
using System.Web;

namespace DealEngine.Infrastructure.Identity
{
	public interface IAuthenticationManager
	{
		string CurrentUser { get; }
		void SetToken (string tokenId, string tokenValue, DateTime expiry);
		void SignIn (string username, bool remember);
		void SignOut ();
	}
}

