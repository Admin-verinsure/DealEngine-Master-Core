using System;
using System.Threading;
using System.Xml.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Areas.Identity.Data;
using System.Linq;

namespace TechCertain.WebUI.Controllers
{
    public class BaseController : Controller
    {
        protected IUserService _userService;
        protected DealEngineDBContext _dealEngineDBContext;

        protected string _localTimeZone = "New Zealand Standard Time"; //Pacific/Auckland
        protected CultureInfo _localCulture = CultureInfo.CreateSpecificCulture ("en-NZ");

        public BaseController(IUserService userService, DealEngineDBContext dealEngineDBContext)
        {
            _dealEngineDBContext = dealEngineDBContext;
            _userService = userService;
        }

        public User CurrentUser
        {
            get
            {
                //UserManager<User>                
                var user = _dealEngineDBContext.Users.FirstOrDefault().UserName;
                if (string.IsNullOrWhiteSpace (user))
                    return null;
				return _userService.GetUser(user); 
            }
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

		//public string Title {
		//	get {
		//		return ViewBag.Title;
		//	}
		//	set {
		//		ViewBag.Title = value;
		//	}
		//}

//        public Account CurrentUserAccount
//        {
//            get
//            {
//                return _accountService.GetAccount(HttpContext.User.Identity.Name);
//            }
//        }

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
                //return WebConfigurationManager.AppSettings ["IntermediatePassword"]; }
            }
        }

        //throw new Exception("This method will need to be re-written");
        //protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        //{
        //    string cultureName = null;
        //    throw new Exception("This method will need to be re-written");
        //    //// Attempt to read the culture cookie from Request
        //    //HttpCookie cultureCookie = Request.Cookies["_culture"];
        //    //if (cultureCookie != null)
        //    //    cultureName = cultureCookie.Value;
        //    //else
        //    //    cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
        //    //        Request.UserLanguages[0] : // obtain it from HTTP header AcceptLanguages
        //    //        null;

        //    //// Validate culture name
        //    //cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

        //    //// Modify current thread's cultures            
        //    //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
        //    //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

        //    //return base.BeginExecuteCore(callback, state);
        //}

        //         throw new Exception("This method will need to be re-written");
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Title = "Proposalonline";
            User account = CurrentUser;
            if (account != null)
            {
                //ViewBag.Name = account.FullName;
                ViewBag.Account = account;
                //ViewBag.Organisations = account.Organisations;

                //if (CurrentUserAccount.Organisations.FirstOrDefault() != null)
                //    ViewBag.CurrentOrganisation = CurrentUserAccount.Organisations.FirstOrDefault().Name;
            }

            base.OnActionExecuting(filterContext);
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

		protected ActionResult Xml (XDocument document)
		{
            throw new Exception("This method will need to be re-written");
            //return new XmlActionResult(document);
		}

		protected string LocalizeTime (DateTime dateTime)
		{
			return LocalizeTime (dateTime, "G");
		}

		protected string LocalizeTime (DateTime dateTime, string format)
		{
            throw new Exception("This method will need to be re-written");
            //return dateTime.ToTimeZoneTime (UserTimeZone).ToString ("G", UserCulture);
		}

        protected string LocalizeTimeDate(DateTime dateTime, string format)
        {
            throw new Exception("This method will need to be re-written");
            //return dateTime.ToTimeZoneTime(UserTimeZone).ToString("d", UserCulture);
        }
    }
}
