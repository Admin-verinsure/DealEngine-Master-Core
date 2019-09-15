using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
using System;


namespace DealEngine.Infrastructure.Identity
{
    public class CookieAuthenticationManager : IAuthenticationManager
	{
		public string CurrentUser {
			get {
                //return ClaimsPrincipal.Current.Identity.Name;

                return "";// HttpContext.Current.User.Identity.Name;
            }
		}

		//protected HttpResponse Response {
  //          get { return HttpContext.Current.Response; }
		//}

		public void SetToken (string tokenId, string tokenValue, DateTime expiry)
		{
			SetCookie (tokenId, tokenValue, expiry, "");
		}

		public void SignIn (string username, bool remember)
		{
			//HttpContext.Current.Session.Abandon ();
			//FormsAuthentication.SetAuthCookie (username, remember);
			//SetCookie ("ASP.NET_SessionId", "", DateTime.MinValue);
		}

		public void SignOut ()
		{
			//FormsAuthentication.SignOut ();
			//SetCookie (FormsAuthentication.FormsCookieName, "", DateTime.MinValue, FormsAuthentication.CookieDomain);
			SetCookie ("ASP.NET_SessionId", "", DateTime.MinValue);
		}

		void SetCookie (string cookieName, string value, DateTime expiry)
		{
			SetCookie (cookieName, value, expiry, "");
		}

		void SetCookie (string cookieName, string value, DateTime expiry, string domain)
		{
		 //   HttpCookie cookie = new HttpCookie (cookieName, value);
			//if (!string.IsNullOrWhiteSpace (domain))
			//	cookie.Domain = domain;
			//cookie.Expires = expiry;
			//Response.SetCookie (cookie);
		}
	}
}

