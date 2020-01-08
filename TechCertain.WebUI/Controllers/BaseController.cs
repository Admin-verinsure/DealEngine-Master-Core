using System;
using System.Xml.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechCertain.WebUI.Helpers;
using TechCertain.WebUI.Helpers.CustomActions;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;

namespace TechCertain.WebUI.Controllers
{
    public class BaseController : Controller
    {
        protected IUserService _userService;
        protected string _localTimeZone = "New Zealand Standard Time"; //Pacific/Auckland
        protected CultureInfo _localCulture = CultureInfo.CreateSpecificCulture ("en-NZ");
        
        public BaseController(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<User> CurrentUser()
        {
                var userName = "";
                try
                {
                    userName = User.Identity.Name;
                }
                catch (Exception ex)
                {
                    return null;
                }
                if (string.IsNullOrWhiteSpace(userName))
                    return null;

                return await _userService.GetUser(userName);            
        }

        /// <summary>
        /// Returns true if we are running on a Unix style OS (including Linux and OS X), and false if Windows.
        /// </summary>
        /// <value><c>true</c> if is linux; otherwise, <c>false</c>.</value>
        public static bool IsLinux {
			get {
				int p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}

		public string UserTimeZone
		{
			get { return IsLinux ? "NZ" : "New Zealand Standard Time"; } //Pacific/Auckland
		}

		public CultureInfo UserCulture
		{
			get { return CultureInfo.CreateSpecificCulture ("en-NZ"); }
		}

        public bool DemoEnvironment
        {
            get
            {
                return true;
            }// return WebConfigurationManager.AppSettings["DemoEnvironment"].ToLower() == "true"; }
        }

		public string IntermediateChangePassword
		{
            get
            {
                throw new Exception("This method will need to be re-written");
                //return _appSettingService.IntermediatePassword;
            }
        }

        protected ActionResult PageNotFound ()
		{
            return Redirect ("~/Error/Error404");
			// renable when mono fixes this
			//return RedirectToAction ("Error404", "Error");
		}

		protected ActionResult ServerError ()
		{
            return Redirect ("~/Error/Error500");
			// renable when mono fixes this
			//return RedirectToAction ("Error500", "Error");
		}

        protected ActionResult Xml(XDocument document)
        {            
            return new XmlActionResult(document);
        }

		protected string LocalizeTime (DateTime dateTime)
		{
			return LocalizeTime (dateTime, "G");
		}

		protected string LocalizeTime (DateTime dateTime, string format)
		{
            return dateTime.ToTimeZoneTime (UserTimeZone).ToString ("G", UserCulture);
        }


        protected string LocalizeTimeDate(DateTime dateTime, string format)
        {            
            return dateTime.ToTimeZoneTime(UserTimeZone).ToString("d", UserCulture);
        }
    }
}
