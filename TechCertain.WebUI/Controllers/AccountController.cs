#region Using

using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Exceptions;
using System.Collections.Generic;
using TechCertain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using Elmah;
using TechCertain.WebUI.Models.Account;
using TechCertain.WebUI.Models.Permission;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;



#endregion

namespace TechCertain.WebUI.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        IEmailService _emailService;
		IFileService _fileService;
        SignInManager<DealEngineUser> _signInManager;
        UserManager<DealEngineUser> _userManager;

        DealEngineDBContext _context;
        IProgrammeService _programmeService;
        ICilentInformationService _clientInformationService;
        IOrganisationService _organisationService;
        IOrganisationalUnitService _organisationalUnitService;
        
        public AccountController(
            SignInManager<DealEngineUser> signInManager,
            UserManager<DealEngineUser> userManager,
            IUserService userRepository,
            DealEngineDBContext dealEngineDBContext,
			IEmailService emailService, IFileService fileService, IProgrammeService programeService, ICilentInformationService clientInformationService, 
            IOrganisationService organisationService, IOrganisationalUnitService organisationalUnitService) : base (userRepository, dealEngineDBContext)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userRepository;
            _emailService = emailService;
			_fileService = fileService;
            _context = dealEngineDBContext;
            _programmeService = programeService;
            _clientInformationService = clientInformationService;
            _organisationService = organisationService;
            _organisationalUnitService = organisationalUnitService;
        }

		// GET: /account/forgotpassword
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			//if (Request.IsAuthenticated)
				return RedirectToLocal();
			
			// TODO - need to somehow call ResetPassword and return its view so we don't have to duplicate it here.
			// We do not want to use any existing identity information
			//EnsureLoggedOut();

			//return View ("ResetPassword");
		}

		// GET: /account/resetpassword
		[HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword()
		{
			//if (Request.IsAuthenticated)
				return RedirectToLocal();
			
            // We do not want to use any existing identity information
            //EnsureLoggedOut();

            //return View();
        }

		// POST: /account/resetpassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(AccountResetPasswordModel viewModel)
		{
			string errorMessage = @"We have sent you an email to the email address we have recorded in the system, that email address is different from the one you supplied. 
				Please check the other email addresses you may have used. If you cannot locate our email, 
				please email support@techcertain.com with your contact details, we can re-establish your account with your broker.";
			try
			{
				if (!string.IsNullOrWhiteSpace (viewModel.Email))
				{
                    //System Email Testing
                    /*var testuser = _userService.GetUserByEmail("mcgtestuser2@techcertain.com");
                    var programme = _programmeService.GetAllProgrammes().FirstOrDefault(p => p.Name == "Demo Coastguard Programme");
                    var organisation = _organisationService.GetOrganisationByEmail("mcgtestuser2@techcertain.com");
                    var sheet = _clientInformationService.GetInformation(new Guid("bc3c9972-1733-41a1-8786-fa22229c66f8"));
                    _emailService.SendSystemSuccessInvoiceConfigEmailUISIssueNotify(testuser, programme, sheet, organisation);*/

                    //SingleUseToken token = _authenticationService.GenerateSingleUseToken (viewModel.Email);
                    //User user = _userService.GetUser (token.UserID);
                    // change the users password to an intermediate
                    //Membership.GetUser (user.UserName).ChangePassword ("", IntermediateChangePassword);
                    // get local domain
                    //string domain = HttpContext.Request.Url.GetLeftPart (UriPartial.Authority);
                    //_emailService.SendPasswordResetEmail (viewModel.Email, token.Id, domain);

                    ViewBag.EmailSent = true;
                }
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException exception) {
                ErrorSignal.FromCurrentContext ().Raise (exception);
               

                ModelState.AddModelError ("FailureMessage", errorMessage);
                return View(viewModel);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex) {               

                ModelState.AddModelError ("FailureMessage", "Oops, Email services are currently unavailable. The techinical support staff have also been notified, and your password reset email will be sent once services have been restored.");
				return View (viewModel);
			}
			catch (Exception ex)
			{
				ErrorSignal.FromCurrentContext().Raise(ex);
				Exception exception = ex;
				while (exception.InnerException != null) exception = exception.InnerException;

				_emailService.ContactSupport (_emailService.DefaultSender, exception.GetType().Name + ": " + exception.Message, "");

				ModelState.AddModelError("FailureMessage", errorMessage);
				if (exception is MultipleUsersFoundException)
					ModelState.AddModelError ("FailureMessage", "We were unable to generate a password reset email for you.");
				return View(viewModel);
			}

			return View ();
		}

		// GET: /account/changepassword
		//[HttpGet]
		//[AllowAnonymous]
		//public ActionResult ChangePassword(Guid id)
		//{
		//	if (id != Guid.Empty && _authenticationService.GetToken (id) != null) {
		//		if (_authenticationService.ValidSingleUseToken (id))
		//			return View ();
		//		// invalid token? display an error
		//		return Redirect ("~/Error/InvalidPasswordReset");
		//	}
		//	// if we get here - either invalid guid or token doesn't exist - 404
		//	return PageNotFound ();
		//}

		// POST: /account/changepassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(Guid id, AccountChangePasswordModel viewModel)
		{
			try
			{
                //if (BrowserHelper.IsSafari(Request))
                //{
                //    Console.WriteLine("Safari Detected");
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine("Safari Detected");
                //    sb.AppendLine("Recording token to check for Safari conversion bug");
                //    sb.AppendLine("Token: " + id);
                //    _logger.Info(sb.ToString());
                //}

                if (id == Guid.Empty)
					// if we get here - either invalid guid or invalid token - 404
					return PageNotFound ();

				if (viewModel.Password != viewModel.PasswordConfirm) {
					ModelState.AddModelError ("passwordConfirm", "Passwords do not match");
					return View ();
				}
				//SingleUseToken st = _authenticationService.GetToken (id);
				//User user = _userService.GetUser (st.UserID);
				//if (user == null)
				//	// in theory, we should never get here. Reason being is that a reset request should not be created without a valid user
				//	throw new Exception (string.Format ("Could not find user with ID {0}", st.UserID));

				//string username = user.UserName;
				// change the users password as them using the intermediate password
				//if (!string.IsNullOrWhiteSpace (username) && Membership.GetUser (username).ChangePassword (IntermediateChangePassword, viewModel.Password)) {
				//	_logger.Info ("Password changed for [" + username + "]");
				//	_authenticationService.UseSingleUseToken (st.Id);
				//	//return Redirect ("~/Account/PasswordChanged");
				//	return RedirectToAction ("PasswordChanged", "Account");
				//}
				else {
					ModelState.AddModelError ("passwordConfirm", "The password change has failed. Is your new password complex enough?");
					return View ();
				}
			}
			catch (AuthenticationException ex) {
				ModelState.AddModelError ("passwordConfirm", "Your chosen password does not meet the requirements of our password policy. Please refer to the policy above to assist with creating an appropriate password.");
				
			}
			catch (Exception ex) {
				ModelState.AddModelError ("passwordConfirm", "There was an error while trying to change your password.");
				
			}

			return View ();
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult PasswordChanged ()
		{
			return View ();
		}

		[HttpGet]
		public ActionResult ImpersonateUser (string id)
		{
			User user = _userService.GetUser (id);

			//if (_permissionsService.DoesUserHaveRole(id, "Admin")) {
			//	_logger.Info (" Attempt by [" + CurrentUser.UserName + "] to impersonate admin user [" + user.UserName + "]");
			//}
			//else {
			//	//Session.Abandon ();
			//	//FormsAuthentication.SetAuthCookie (user.UserName, true);
			//	SetCookie ("ASP.NET_SessionId", "", DateTime.MinValue);

			//	_logger.Info ("[" + CurrentUser.UserName + "] is impersonating [" + user.UserName + "]");
			//}

			return Redirect ("~/Home/Index");
		}

		// GET: /account/login
		[HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            // We do not want to use any existing identity information
            //EnsureLoggedOut();            

            // Store the originating URL so we can attach it to a form field
            var viewModel = new AccountLoginModel { ReturnUrl = returnUrl };

            string nameExtension = "";// ConfigurationRoot["LoginPageExtension"];

			return View("Login"+nameExtension, viewModel);

			//return View(viewModel);
        }

        // POST: /account/login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
			}

			if (User.Identity.IsAuthenticated)
				return RedirectToLocal();

			string username = "";
            try
            {
				username = viewModel.Username.Trim();
				string password = viewModel.Password.Trim();

                var user1 = new DealEngineUser { UserName = username };
                
                //await _signInManager.SignInAsync(user1, viewModel.RememberMe);
                var createUser = await _userManager.CreateAsync(user1, password);
                if(createUser.Succeeded)
                {
                    await _signInManager.SignInAsync(user1, viewModel.RememberMe);
                }

                return RedirectToLocal(viewModel.ReturnUrl);
 
            }
			catch (UserImportException ex)
			{
				ErrorSignal.FromCurrentContext().Raise(ex);
				_emailService.ContactSupport (_emailService.DefaultSender, "TechCertain 2015 - User Import Error", ex.Message);
				ModelState.AddModelError(string.Empty, "We have encountered an error importing your account. Proposalonline has been notified, and will be in touch shortly to resolve this error.");
				return View(viewModel);
			}
			catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            ModelState.AddModelError(string.Empty, "We are unable to access your account with the username or password provided. You may have entered an incorrect password, or your account may be locked due to an extended period of inactivity. Please try entering your username or password again, or email support@techcertain.com.");

            return View(viewModel);
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult LoginMarsh (RsaAccountLoginModel viewModel)
		{
			string username = viewModel.Username.Trim ();
			//if (ModelState.IsValid) {

			//	_signInManager.SignIn (username, viewModel.Password, viewModel.RememberMe);

			//	if (Membership.ValidateUser (username, viewModel.Password)) {

			//		User user = _userService.GetUser(username);
			//		Console.WriteLine ("Creating RSA User");
			//		MarshRsaAuthProvider rsaAuth = new MarshRsaAuthProvider (_logger);
			//		MarshRsaUser rsaUser = rsaAuth.GetRsaUser (user.Email);
                   
   //                 rsaUser.IpAddress = Request.UserHostAddress;
			//		rsaUser.DevicePrint = viewModel.DevicePrint;
   //                 //for testing purposes
   //                 rsaUser.Email = user.Email;
   //                 //rsaUser.Username = rsaAuth.GetHashedId (username + "@dealengine.com");
   //                 rsaUser.Username = user.Email; //try as Marsh RSA team advised
   //                 rsaUser.HttpReferer = Request.UrlReferrer.ToString ();
			//		//rsaUser.OrgName = System.Web.Configuration.WebConfigurationManager.AppSettings ["RsaOrg"];
			//		rsaUser.OrgName = "Marsh_Model";

			//		Console.WriteLine ("Analzying RSA User");
			//			RsaStatus rsaStatus = rsaAuth.Analyze (rsaUser, true);
			//		if (rsaStatus == RsaStatus.Allow) {

			//			Console.WriteLine ("RSA User allowed, signing in...");
			//			Session.Abandon ();
			//			FormsAuthentication.SetAuthCookie (username, true);
			//			SetCookie ("ASP.NET_SessionId", "", DateTime.MinValue);

			//			_logger.Info ("RSA Authentication succeeded for [" + username + "]");

			//			return RedirectToLocal (viewModel.ReturnUrl);
			//		}
			//		if (rsaStatus == RsaStatus.RequiresOtp) {
			//			Console.WriteLine ("RSA User requires otp. Making request");
			//			string otp = rsaAuth.GetOneTimePassword (rsaUser);

			//			Console.WriteLine ("One Time Password: " + otp);
			//			_emailService.ContactSupport (_emailService.DefaultSender, "Marsh RSA OTP", otp);

			//			Console.WriteLine ("Sent otp. Redirecting to Otp page");
			//			// sent otp to user
			//			return View ("OneTimePasswordMarsh", new RsaOneTimePasswordModel (username) {
			//				DevicePrint = rsaUser.DevicePrint,
			//				SessionId = rsaUser.SessionId,
			//				TransactionId = rsaUser.TransactionId
			//			});
			//		}
			//	}
			//}

			//// If we got this far, something failed, redisplay form			
			ModelState.AddModelError ("", "The user name or password provided is incorrect.");
			return View (viewModel);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult OneTimePasswordMarsh (RsaOneTimePasswordModel viewModel)
		{
            throw new Exception("Method needs to be re-written");
			//if (ModelState.IsValid) {
			//	Console.WriteLine ("Creating RSA User for otp entry");
			//	MarshRsaAuthProvider rsaAuth = new MarshRsaAuthProvider (_logger);
			//	string username = viewModel.UserName;
			//	MarshRsaUser rsaUser = rsaAuth.GetRsaUser (username);
			//	//rsaUser.IpAddress = Request.UserHostAddress;
			//	//rsaUser.DevicePrint = viewModel.DevicePrint;
			//	//rsaUser.Username = rsaAuth.GetHashedId (username + "@dealengine.com");
			//	//rsaUser.HttpReferer = Request.UrlReferrer.ToString ();
			//	//rsaUser.Otp = viewModel.OtpCode;

			//	Console.WriteLine ("Authenticating RSA User");
			//	bool isAuthenticated = rsaAuth.Authenticate (rsaUser);
			//	if (isAuthenticated) {
			//		Console.WriteLine ("RSA User authenticated, creating mfa cookie");
			//		SetCookie ("MfaClientGenCookie", rsaUser.DevicePrint, DateTime.MinValue);

			//		return RedirectToLocal ("");
			//	}
			//}

			return View ();
		}

		// GET: /account/error
		[HttpGet]
        [AllowAnonymous]
        public ActionResult Error()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

		// GET: /account/register
		[HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
		{
			//if (Request.IsAuthenticated)
				return RedirectToLocal();
			
            // We do not want to use any existing identity information
            //EnsureLoggedOut();

            return View(new AccountRegistrationModel());
        }

        // POST: /account/register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountRegistrationModel model)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
				return View(model);

			return RedirectToLocal();

//			// Disable for now
//			var user = new TechCertain.Domain.Entities.User (Guid.NewGuid(), model.Username);
//			user.Email = model.Email;
//			user.FirstName = model.FirstName;
//			user.LastName = model.LastName;
//			user.FullName = user.FirstName + " " + user.LastName;
//			user.Password = model.Password;
//
//			if (_userRepository.Create (user))
//				_logger.Error ("Unable to create user for " + model.Username);
//
//			if (Membership.ValidateUser(user.UserName, user.Password))
//				FormsAuthentication.SetAuthCookie(user.UserName, true);

            // Prepare the identity with the provided information
            //var user = new IdentityUser
            //{
            //    UserName = viewModel.Username ?? viewModel.Email,
            //    Email = viewModel.Email
            //};

            // Try to create a user with the given identity
            //try
            //{
            //    var result = await _manager.CreateAsync(user, viewModel.Password);

            //    // If the user could not be created
            //    if (!result.Succeeded) {
            //        // Add all errors to the page so they can be used to display what went wrong
            //        AddErrors(result);

            //        return View(viewModel);
            //    }

            //    // If the user was able to be created we can sign it in immediately
            //    // Note: Consider using the email verification proces
            //    await SignInAsync(user, false);

            //}
            //catch (DbEntityValidationException ex)
            //{
            //    // Add all errors to the page so they can be used to display what went wrong
            //    AddErrors(ex);

            //    return View(viewModel);
            //
        }

        // GET: /account/coastguardreg
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CoastguardReg()
        {
            //if (Request.IsAuthenticated)
                //return RedirectToLocal();

            // We do not want to use any existing identity information
            //EnsureLoggedOut();

            return View(new AccountRegistrationModel());
        }

        // POST: /account/coastguardreg
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CoastguardReg(AccountRegistrationModel model)
        {
            // Ensure we have a valid viewModel to work with
            //if (!ModelState.IsValid)
                return View(model);

            //return RedirectToLocal();


        }

        // GET: /account/coastguardreg
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CoastguardForm()
        {
            //if (Request.IsAuthenticated)
                //return RedirectToLocal();

            // We do not want to use any existing identity information
            //EnsureLoggedOut();

            return View(new AccountRegistrationModel());
        }

        // POST: /account/coastguardreg
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CoastguardForm(AccountRegistrationModel model)
        {
            // Ensure we have a valid viewModel to work with
            //if (!ModelState.IsValid)
                return View(model);

            //return RedirectToLocal();


        }

        // POST: /account/Logout
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
			

            //FormsAuthentication.SignOut();
            //Session.Abandon();

            //_dealEngineSignInManager.SignOutAsync();

			//HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

			//DateTime expiredDate = DateTime.UtcNow.AddDays (-1);

			//// clear authentication cookie
			//SetCookie(FormsAuthentication.FormsCookieName, "", expiredDate, FormsAuthentication.CookieDomain);

			//// clear session cookie
			//SetCookie("ASP.NET_SessionId", "", expiredDate);

            return RedirectToLocal();
        }

        ActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            //            if (!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
            //                return Redirect(returnUrl);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
				&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
			{
				return Redirect(returnUrl);
			}

            // If we cannot verify if the url is local to our host we redirect to a default location
            return Redirect("~/Home/Index");
        }

        //private void AddErrors(DbEntityValidationException exc)
        //{
        //    foreach (var error in exc.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors.Select(validationError => validationError.ErrorMessage)))
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    // Add all errors that were returned to the page error collection
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action

            throw new Exception("this method needs to be re-written in core");

            //if (Request.IsAuthenticated)
            //    Logout();

            //if (!Request.IsAuthenticated)
            //{
            //    if (Response.Cookies[FormsAuthentication.FormsCookieName] != null)
            //        Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.UtcNow.AddDays(-1);

            //    if (Response.Cookies["ASP.NET_SessionId"] != null)
            //        Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.UtcNow.AddDays(-1);
            //}
        }

        async Task SignInAsync(string username, bool isPersistent)
        {
            // Clear any lingering authencation data
            throw new Exception("this method needs to be re-written in core");
            //FormsAuthentication.SignOut();

            //Account account = _authenticationService.LoginUser(username);



            //Write the authentication cookie
            //FormsAuthentication.SetAuthCookie(username, isPersistent);

            //string userData = account.UserName;

            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
            //    1,
            //    username,
            //    DateTime.UtcNow,
            //    DateTime.UtcNow.AddMinutes(2285),
            //    false,
            //    userData,
            //    FormsAuthentication.FormsCookiePath);

            //string encTicket = FormsAuthentication.Encrypt(ticket);

            //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
            //        encTicket);

            //Response.Cookies.Add(cookie);
        }

		void SetCookie(string cookieName, string value, DateTime expiry)
		{
			SetCookie (cookieName, value, expiry, "");
		}

		void SetCookie(string cookieName, string value, DateTime expiry, string domain)
		{
            //throw new Exception("this method needs to be re-written in core");
            //HttpCookie authCookie = new HttpCookie(cookieName, value);
            //if (!string.IsNullOrWhiteSpace(domain))
            //    authCookie.Domain = domain;
            //authCookie.Expires = expiry;
            //// overridden in Global.Application_EndRequest
            ////			authCookie.HttpOnly = true;
            ////			authCookie.Secure = true;
            //Response.SetCookie(authCookie);
        }

		// GET: /account/lock
		[HttpPost]
		public ActionResult Lock(UserLockStatusViewModel model)
        {
			User user = _userService.GetUser (model.Id);
			if (model.Status == "lock" && !user.Locked) {
				_userService.IssueLocalBan (user, CurrentUser);
			}
			else if (model.Status == "unlock" && user.Locked) {
				_userService.RemoveLocalban (user, CurrentUser);
			}
            return View();
        }

		[HttpGet]
		public ActionResult Profile(string id)
		{
			var user = string.IsNullOrWhiteSpace (id) ? CurrentUser : _userService.GetUser (id);
			if (user == null)
				return PageNotFound ();

			ProfileViewModel model = new ProfileViewModel();
			model.FirstName = user.FirstName;
			model.LastName = user.LastName;
			model.Email = user.Email;
			model.Phone = user.Phone;
            model.CurrentUser = CurrentUser;
            model.DefaultOU = user.DefaultOU;
            model.EmployeeNumber = user.EmployeeNumber;
            model.JobTitle = user.JobTitle;
            model.SalesPersonUserName = user.SalesPersonUserName;

            if (user.PrimaryOrganisation != null)
				model.PrimaryOrganisationName = user.PrimaryOrganisation.Name;
			//if (user.Organisations.Count() > 0 && user.Organisations.ElementAt(0).OrganisationType.Name != "personal")
			//	model.PrimaryOrganisationName = user.Organisations.ElementAt(0).Name;
			model.Description = user.Description;
			if (user.ProfilePicture != null)
				model.ProfilePicture = user.ProfilePicture.Name;    // TODO - remap this

			model.ViewingSelf = string.IsNullOrEmpty(id) || (CurrentUser.UserName == id);

			return View(model);
		}

		[HttpGet]
		public ActionResult ProfileEditor()
		{
			var user = CurrentUser;
			if (user == null)
				return PageNotFound ();
			
			ProfileViewModel model = new ProfileViewModel();
            var organisationalUnits = new List<OrganisationalUnitViewModel>();

            model.FirstName = CurrentUser.FirstName;
			model.LastName = CurrentUser.LastName;
			model.Email = CurrentUser.Email;
			model.Phone = CurrentUser.Phone;
            model.JobTitle = CurrentUser.JobTitle;            
            model.CurrentUser = CurrentUser;
            //model.DefaultOU = user.DefaultOU;
            model.EmployeeNumber = user.EmployeeNumber;
            model.JobTitle = user.JobTitle;
            model.SalesPersonUserName = user.SalesPersonUserName;

            var OrgUnitList = _organisationalUnitService.GetAllOrganisationalUnits();

            foreach (OrganisationalUnit ou in OrgUnitList)
            {
                organisationalUnits.Add(new OrganisationalUnitViewModel
                {
                    OrganisationalUnitId = ou.Id,
                    Name = ou.Name
                });
                //model.OrganisationalUnitsVM.OrganisationalUnits.Add(new SelectListItem { Text = ou.Name, Value = ou.Id.ToString() });
            }

            //if (CurrentUser.Organisations.Count() > 0 && CurrentUser.Organisations.ElementAt(0).OrganisationType.Name != "personal")
            //	model.PrimaryOrganisationName = CurrentUser.Organisations.ElementAt(0).Name;
            if (CurrentUser.PrimaryOrganisation != null)
                model.PrimaryOrganisationName = user.PrimaryOrganisation.Name;
            model.Description = CurrentUser.Description;
			if (user.ProfilePicture != null)
				model.ProfilePicture = user.ProfilePicture.Name;    // TODO - remap this

			model.ViewingSelf = true;
            model.OrganisationalUnitsVM = organisationalUnits;

            return View(model);
//			return View("Profile", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ProfileEditor(ProfileViewModel model)
		{

			var user = CurrentUser;
			if (user == null)
				return PageNotFound ();
            Guid defaultOU = Guid.Empty;

            throw new Exception("this method needs to be re-written in core");
            // if we've added a new avatar, upload it. Otherwise don't change
   //         if (Request.Files.Count > 0)
			//{
			//	//Console.WriteLine ("{0} files uploaded", Request.Files.Count);
			//	var file = Request.Files [0];
			//	if (file != null && file.ContentLength > 0)
			//	{
			//		byte[] buffer = new byte[file.ContentLength];
			//		file.InputStream.Read(buffer, 0, buffer.Length);
			//		if (_fileService.IsImageFile (buffer, file.ContentType, file.FileName)) {
			//			//_fileService.UploadFile (buffer, file.ContentType, file.FileName);
			//			Image img = new Image (user, file.FileName, file.ContentType);
			//			img.Contents = buffer;
			//			_fileService.UploadFile (img);
			//			model.ProfilePicture = file.FileName;
			//			user.ProfilePicture = img;
			//		}
			//		else
			//			ModelState.AddModelError ("", "Unable to upload profile picture - invalid image file");
			//	}
			//}

   //         if(Request.Form["branch"] != null)
   //         {
   //             defaultOU = Guid.Parse(Request.Form["branch"]);
   //             OrganisationalUnit DefaultOU = _organisationalUnitService.GetOrganisationalUnit(defaultOU);
   //             user.DefaultOU = DefaultOU;
   //         }

            user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.Phone = model.Phone;
			user.Description = model.Description;
            user.EmployeeNumber = model.EmployeeNumber;
            user.JobTitle = model.JobTitle;
            user.SalesPersonUserName = model.SalesPersonUserName;

            _userService.Update (user);

            return Redirect("~/Account/ProfileEditor");
		}

		[HttpGet]
		public ActionResult ChangeOwnPassword()
		{
			return PartialView ("_ChangeOwnPassword");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ChangeOwnPassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
				return PartialView ("_ChangeOwnPassword", model);

			if (model.NewPassword != model.ConfirmNewPassword) {
				ModelState.AddModelError ("ConfirmNewPassword", "Passwords do not match");
				return PartialView ("_ChangeOwnPassword", model);
			}
			if (model.CurrentPassword == model.NewPassword) {
				ModelState.AddModelError ("NewPassword", "New password needs to be different from the current password");
				return PartialView ("_ChangeOwnPassword", model);
			}

			try
			{
                throw new Exception("this method needs to be re-written in core");
                //if (Membership.GetUser(CurrentUser.UserName).ChangePassword(model.CurrentPassword, model.NewPassword))
                //{
                //    string content = "<div class=\"alert alert-success\">Your password has been successfully changed.</div>";
                //    content += "<button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>";
                //    return Content(content);
                //}
            }
			catch (Exception ex) {
				ModelState.AddModelError ("", ex.Message);
			}
			return PartialView ("_ChangeOwnPassword", model);
		}

		[HttpGet]
		public ActionResult ListAllUsers ()
		{
			BaseListViewModel<UserViewModel> userList = new BaseListViewModel<UserViewModel> ();

			foreach (var user in _userService.GetAllUsers()) {
				userList.Add (new UserViewModel {
					ID = user.Id,
					UserName = user.UserName,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email
				});
			}
			return View (userList);
		}

		[HttpGet]
		public PartialViewResult UserPermissions (Guid Id)
		{
			User user = _userService.GetUser (Id);
			if (user == null)
				return null;

            UserPermissionsViewModel permissions = new UserPermissionsViewModel (user);

			return PartialView ("_UserPermissions", permissions);
		}

		[HttpGet]
		public ActionResult ManageUser (Guid Id)
		{
            var user = _userService.GetUser(Id);
            var accountModel = new ManageUserViewModel(user);
            //accountModel.UserGroups = new SelectUserGroupsViewModel(user, _permissionsService.GetAllGroups());

            SingleUseToken passwordToken = null;// _authenticationService.GetTokensFor(Id).OrderByDescending(t => t.DateCreated.GetValueOrDefault()).FirstOrDefault();
            if (passwordToken != null)
            {
                accountModel.AccountStatus.LastPasswordResetIssued = passwordToken.DateCreated.GetValueOrDefault().ToString("f");
                accountModel.AccountStatus.PasswordResetExpiryDate = passwordToken.DateCreated.GetValueOrDefault().AddHours(passwordToken.Duration).ToString("f");
                if (passwordToken.Used)
                    accountModel.AccountStatus.PasswordResetStatus = "Used";
                else if (passwordToken.DateCreated.GetValueOrDefault().AddHours(passwordToken.Duration) < DateTime.UtcNow)
                    accountModel.AccountStatus.PasswordResetStatus = "Expired";
                else
                    accountModel.AccountStatus.PasswordResetStatus = "Active";
            }

            //accountModel.UserGroups = new SelectUserGroupsViewModel(user, _permissionsService.GetAllGroups());
            return View(accountModel);
        }
    }
}
