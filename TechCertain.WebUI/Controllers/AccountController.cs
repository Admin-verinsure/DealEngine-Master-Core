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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using TechCertain.Infrastructure.Ldap.Interfaces;
using IAuthenticationService = TechCertain.Services.Interfaces.IAuthenticationService;
using Microsoft.Extensions.Logging;
using DealEngine.Infrastructure.AuthorizationRSA;
using System.Linq;
using TechCertain.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Identity;
using IdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;
using IdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;

#endregion

namespace TechCertain.WebUI.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        IAuthenticationService _authenticationService;

        IEmailService _emailService;
		IFileService _fileService;
        SignInManager<IdentityUser> _signInManager;
        UserManager<IdentityUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        ILdapService _ldapService;
        IProgrammeService _programmeService;
        IClientInformationService _clientInformationService;
        IOrganisationService _organisationService;
        IOrganisationalUnitService _organisationalUnitService;
        ILogger<AccountController> _logger;
        IMapperSession<User> _userRepository;
        IHttpClientService _httpClientService;
        IAppSettingService _appSettingService;

        public AccountController(
            IAuthenticationService authenticationService,
			SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger,
            IMapperSession<User> userRepository,
            IHttpClientService httpClientService,
            ILdapService ldapService,
            IUserService userService,
			IEmailService emailService, IFileService fileService, IProgrammeService programeService, IClientInformationService clientInformationService, 
            IOrganisationService organisationService, IOrganisationalUnitService organisationalUnitService, IAppSettingService appSettingService) : base (userService)
		{
            _authenticationService = authenticationService;
            _ldapService = ldapService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
			_httpClientService = httpClientService;
			_logger = logger;
			_userService = userService;
            _emailService = emailService;
			_fileService = fileService;
            _programmeService = programeService;
            _clientInformationService = clientInformationService;
            _organisationService = organisationService;
            _organisationalUnitService = organisationalUnitService;
            _appSettingService = appSettingService;
        }

		// GET: /account/forgotpassword
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword()
		{
			//if (Request.IsAuthenticated)
				return await RedirectToLocal();
			
			// TODO - need to somehow call ResetPassword and return its view so we don't have to duplicate it here.
			// We do not want to use any existing identity information
			//EnsureLoggedOut();

			//return View ("ResetPassword");
		}

		// GET: /account/resetpassword
		[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword()
		{
            if (User.Identity.IsAuthenticated)
                return await RedirectToLocal();

            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

		// POST: /account/resetpassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(AccountResetPasswordModel viewModel)
		{
			string errorMessage = @"We have sent you an email to the email address we have recorded in the system, that email address is different from the one you supplied. 
				Please check the other email addresses you may have used. If you cannot locate our email, 
				please email support@techcertain.com with your contact details, we can re-establish your account with your broker.";
			try
			{
				if (!string.IsNullOrWhiteSpace (viewModel.Email))
				{
                    //System Email Testing
                    //var testuser = _userService.GetUserByEmail("mcgtestuser2@techcertain.com");
                    //var programme = _programmeService.GetAllProgrammes().FirstOrDefault(p => p.Name == "Demo Coastguard Programme");
                    //var organisation = _organisationService.GetOrganisationByEmail("mcgtestuser2@techcertain.com");
                    //var sheet = _clientInformationService.GetInformation(new Guid("bc3c9972-1733-41a1-8786-fa22229c66f8"));
                    //_emailService.SendSystemEmailLogin("support@techcertain.com");

                    SingleUseToken token = _authenticationService.GenerateSingleUseToken(viewModel.Email);
                    User user = await _userService.GetUser(token.UserID);
                    if (user != null)
                    {
                        //change the users password to an intermediate
                        _ldapService.ChangePassword(user.UserName, "", _appSettingService.IntermediatePassword);

                        var deUser = await _userManager.FindByNameAsync(user.UserName);
                        if (deUser != null)
                        {
                            var removePasswordResult = await _userManager.RemovePasswordAsync(deUser);
                            var addPasswordResult = await _userManager.AddPasswordAsync(deUser, _appSettingService.IntermediatePassword);
                            if (addPasswordResult.Succeeded)
                            {
                                
                            }
                        }

                        //get local domain
                        string domain = "https://" + _appSettingService.domainQueryString; //HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
                        await _emailService.SendPasswordResetEmail(viewModel.Email, token.Id, domain);

                    }
                    
                }
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException exception) {
                ErrorSignal.FromCurrentContext ().Raise (exception);
               

                ModelState.AddModelError ("FailureMessage", errorMessage);
                return View(viewModel);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex) {               

                ModelState.AddModelError ("FailureMessage", "Oops, Email services are currently unavailable. The technical support staff have also been notified, and your password reset email will be sent once services have been restored.");
				return View (viewModel);
			}
			catch (Exception ex)
			{
				Exception exception = ex;
				while (exception.InnerException != null) exception = exception.InnerException;

				await _emailService.ContactSupport (_emailService.DefaultSender, exception.GetType().Name + ": " + exception.Message, "");

				ModelState.AddModelError("FailureMessage", errorMessage);
				if (exception is MultipleUsersFoundException)
					ModelState.AddModelError ("FailureMessage", "We were unable to generate a password reset email for you.");
				return View(viewModel);
			}

			return View ();
		}

        // GET: /account/changepassword
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            if (id != Guid.Empty && _authenticationService.GetToken(id) != null)
            {
                if (_authenticationService.ValidSingleUseToken(id))
                    return View();
                // invalid token? display an error
                return Redirect("~/Error/InvalidPasswordReset");
            }
            // if we get here - either invalid guid or token doesn't exist - 404
            return PageNotFound();
        }

        // POST: /account/changepassword
        [HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(Guid id, AccountChangePasswordModel viewModel)
		{
			try
			{

                if (id == Guid.Empty)
					// if we get here - either invalid guid or invalid token - 404
					return PageNotFound ();

				if (viewModel.Password != viewModel.PasswordConfirm) {
					ModelState.AddModelError ("passwordConfirm", "Passwords do not match");
					return View ();
				}
                SingleUseToken st = _authenticationService.GetToken(id);
                User user = await _userService.GetUser(st.UserID);
                if (user == null)
                    // in theory, we should never get here. Reason being is that a reset request should not be created without a valid user
                    throw new Exception(string.Format("Could not find user with ID {0}", st.UserID));


                if (user != null)
                {
                    string username = user.UserName;

                    //change the users password to an intermediate
                    if (_ldapService.ChangePassword(user.UserName, _appSettingService.IntermediatePassword, viewModel.Password))
                    {
                        var deUser = await _userManager.FindByNameAsync(user.UserName);
                        if (deUser != null)
                        {
                            var removePasswordResult = await _userManager.RemovePasswordAsync(deUser);
                            var addPasswordResult = await _userManager.AddPasswordAsync(deUser, viewModel.Password);
                            if (addPasswordResult.Succeeded)
                            {
                                _authenticationService.UseSingleUseToken(st.Id);
                                return RedirectToAction("PasswordChanged", "Account");
                            }
                        } else
                        {
                            _authenticationService.UseSingleUseToken(st.Id);
                            return RedirectToAction("PasswordChanged", "Account");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("passwordConfirm", "The password change has failed. Is your new password complex enough?");
                        return View();
                    }

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
		public async Task<IActionResult> PasswordChanged ()
		{
			return View ();
		}

		// GET: /account/login
		[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var viewModel = new AccountLoginModel
            {
                ReturnUrl = returnUrl,
                DomainString = _appSettingService.domainQueryString,
            };

            string nameExtension = "";// ConfigurationRoot["LoginPageExtension"];

			return View("Login"+nameExtension, viewModel);
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
				return await RedirectToLocal();

            try
            {
                var userName = viewModel.Username.Trim();
				string password = viewModel.Password.Trim();
                var user = _userRepository.FindAll().FirstOrDefault(u => u.UserName == userName);
                int resultCode = -1;
                string resultMessage = "";

                // Step 1 validate in  LDap 
                _ldapService.Validate(userName, password, out resultCode, out resultMessage);
                if (resultCode == 0)
                {
                    IdentityUser deUser = await _userManager.FindByNameAsync(userName);
                    if (deUser == null)
                    {
                        deUser = new IdentityUser
                        {
                            Email = user.Email,
                            UserName = userName
                        };
                        await _userManager.CreateAsync(deUser, password);
                        var hasRole = await _roleManager.RoleExistsAsync("Client");
                        if (hasRole)
                        {
                            await _userManager.AddToRoleAsync(deUser, "Client");
                        }
                    }

                    var identityResult = await _signInManager.PasswordSignInAsync(deUser, password, viewModel.RememberMe, lockoutOnFailure: false);
                    if (identityResult.Succeeded)
                    {
                        if(!user.PrimaryOrganisation.IsBroker && !user.PrimaryOrganisation.IsInsurer && !user.PrimaryOrganisation.IsTC)
                        {
                            var hasRole = await _roleManager.RoleExistsAsync("Client");
                            if (hasRole)
                            {
                                await _userManager.AddToRoleAsync(deUser, "Client");
                            }
                        }
                    }
                    
                    if (_appSettingService.RequireRSA)
                    {
                        var result = await LoginMarsh(user, viewModel.DevicePrint);
                    }
                    
                    return LocalRedirect("~/Home/Index");
                }

                ModelState.AddModelError(string.Empty, "We are unable to access your account with the username or password provided. You may have entered an incorrect password, or your account may be locked due to an extended period of inactivity. Please try entering your username or password again, or email support@techcertain.com.");
                return View(viewModel);

            }
			catch (UserImportException ex)
			{
				ErrorSignal.FromCurrentContext().Raise(ex);
				await _emailService.ContactSupport (_emailService.DefaultSender, "TechCertain 2019 - User Import Error", ex.Message);
				ModelState.AddModelError(string.Empty, "We have encountered an error importing your account. Proposalonline has been notified, and will be in touch shortly to resolve this error.");
				return View(viewModel);
			}
			catch(Exception ex)
            {
                throw new Exception(ex.Message + " "+ ex.StackTrace);
            }
        }

		public async Task<IdentityResult> LoginMarsh (User user, string devicePrint)
		{
            MarshRsaAuthProvider rsaAuth = new MarshRsaAuthProvider(_logger, _httpClientService);
            MarshRsaUser rsaUser = rsaAuth.GetRsaUser(user.Email);

            rsaUser.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            rsaUser.DevicePrint = devicePrint;
            //for testing purposes
            rsaUser.Email = user.Email;
            //rsaUser.Username = rsaAuth.GetHashedId (username + "@dealengine.com");
            rsaUser.Username = user.Email; //try as Marsh RSA team advised
            rsaUser.HttpReferer = Url.ToString();
            //rsaUser.OrgName = System.Web.Configuration.WebConfigurationManager.AppSettings ["RsaOrg"];
            rsaUser.OrgName = "Marsh_Model";

            try
            {
                _httpClientService.GetEglobalStatus();
                Console.WriteLine("Analzying RSA User");
                RsaStatus rsaStatus = await rsaAuth.Analyze(rsaUser, true);                
                if (rsaStatus == RsaStatus.Allow)
                {
                    Console.WriteLine("RSA User allowed, signing in...");
                    _logger.LogInformation("RSA Authentication succeeded for [" + user.UserName + "]");
                }
                if (rsaStatus == RsaStatus.RequiresOtp)
                {
                    Console.WriteLine("RSA User requires otp. Making request");
                    string otp = rsaAuth.GetOneTimePassword(rsaUser);

                    Console.WriteLine("One Time Password: " + otp);
                    _emailService.ContactSupport(_emailService.DefaultSender, "Marsh RSA OTP", otp);

                    Console.WriteLine("Sent otp. Redirecting to Otp page");
                    // sent otp to user                
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return IdentityResult.Success;
        }

        [HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OneTimePasswordMarsh (RsaOneTimePasswordModel viewModel)
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
        public async Task<IActionResult> Error()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

		// GET: /account/register
		[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
		{
            if (User.Identity.IsAuthenticated)
                return await RedirectToLocal();

            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View(new AccountRegistrationModel());
        }

        // POST: /account/register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountRegistrationModel model)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
				return View(model);

			return await RedirectToLocal();

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
        public async Task<IActionResult> CoastguardReg()
        {
            if (User.Identity.IsAuthenticated)
                return await RedirectToLocal();

            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View(new AccountRegistrationModel());
        }

        // POST: /account/coastguardreg
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoastguardReg(AccountRegistrationModel model)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View(model);

            return await RedirectToLocal();


        }

        // GET: /account/coastguardreg
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CoastguardForm()
        {
            if (User.Identity.IsAuthenticated)
                return await RedirectToLocal();

            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

        // POST: /account/coastguardreg
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoastguardForm(AccountRegistrationModel model)
        {
            // Ensure we have a valid viewModel to work with
            //if (!ModelState.IsValid)
                return View(model);

            //return RedirectToLocal();
        }

        // POST: /account/Logout
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await  HttpContext.SignOutAsync();
            HttpContext.Response.Cookies.Delete(".AspNet.Consent");

            return await RedirectToLocal();
        }

        async Task<IActionResult> RedirectToLocal(string returnUrl = "")
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

        void EnsureLoggedOut()
        {
            if (User.Identity.IsAuthenticated)
                Logout();
        }

		[HttpGet]
		public async Task<IActionResult> Profile(string id)
		{
            var currentUser = await CurrentUser();
            var user = string.IsNullOrWhiteSpace (id) ? currentUser : await _userService.GetUser(id);
			if (user == null)
				return PageNotFound ();

			ProfileViewModel model = new ProfileViewModel();
			model.FirstName = user.FirstName;
			model.LastName = user.LastName;
			model.Email = user.Email;
			model.Phone = user.Phone;
            model.CurrentUser = currentUser;
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

			model.ViewingSelf = string.IsNullOrEmpty(id) || (currentUser.UserName == id);

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> ProfileEditor()
		{
			var user = await CurrentUser();
			if (user == null)
				return PageNotFound ();
			
			ProfileViewModel model = new ProfileViewModel();
            var organisationalUnits = new List<OrganisationalUnitViewModel>();

            model.FirstName = user.FirstName;
			model.LastName = user.LastName;
			model.Email = user.Email;
			model.Phone = user.Phone;
            model.JobTitle = user.JobTitle;            
            model.CurrentUser = user;
            //model.DefaultOU = user.DefaultOU;
            model.EmployeeNumber = user.EmployeeNumber;
            model.JobTitle = user.JobTitle;
            model.SalesPersonUserName = user.SalesPersonUserName;

            var OrgUnitList = await _organisationalUnitService.GetAllOrganisationalUnitsByOrg(user.PrimaryOrganisation);
            OrgUnitList.GroupBy(o => o.Name);
            foreach (OrganisationalUnit ou in OrgUnitList)
            {
                    organisationalUnits.Add(new OrganisationalUnitViewModel
                    {
                        OrganisationalUnitId = ou.Id,
                        Name = ou.Name
                    });
            }

            if (user.PrimaryOrganisation != null)
                model.PrimaryOrganisationName = user.PrimaryOrganisation.Name;
            model.Description = user.Description;
			if (user.ProfilePicture != null)
				model.ProfilePicture = user.ProfilePicture.Name;    // TODO - remap this

			model.ViewingSelf = true;
            model.OrganisationalUnitsVM = organisationalUnits;

            return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ProfileEditor(ProfileViewModel model)
		{

			var user = await CurrentUser();
			if (user == null)
				return PageNotFound ();
            Guid defaultOU = Guid.Empty;

            //if we've added a new avatar, upload it. Otherwise don't change
            if (Request.Form.Files.Count > 0)
            {
                //Console.WriteLine ("{0} files uploaded", Request.Files.Count);
                var file = Request.Form.Files[0];
                if (file != null && file.Length > 0)
                {
                    byte[] buffer = new byte[file.Length];
                    file.OpenReadStream().Read(buffer, 0, buffer.Length);
                    if (_fileService.IsImageFile(buffer, file.ContentType, file.FileName))
                    {
                        //_fileService.UploadFile (buffer, file.ContentType, file.FileName);
                        Image img = new Image(user, file.FileName, file.ContentType);
                        img.Contents = buffer;
                        await _fileService.UploadFile(img);
                        model.ProfilePicture = file.FileName;
                        user.ProfilePicture = img;
                    }
                    else
                        ModelState.AddModelError("", "Unable to upload profile picture - invalid image file");
                }
            }

            Microsoft.Extensions.Primitives.StringValues branchId;
            Request.Form.TryGetValue("branch", out branchId);
            if (branchId.Count == 1)
            {
                defaultOU = Guid.Parse(branchId);
                OrganisationalUnit DefaultOU = await _organisationalUnitService.GetOrganisationalUnit(defaultOU);
                user.DefaultOU = DefaultOU;
            }

            user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Email = model.Email;
			user.Phone = model.Phone;
			user.Description = model.Description;
            user.EmployeeNumber = model.EmployeeNumber;
            user.JobTitle = model.JobTitle;
            user.SalesPersonUserName = model.SalesPersonUserName;

            _userService.Update(user);

            return Redirect("~/Account/ProfileEditor");
		}

		[HttpGet]
		public async Task<IActionResult> ChangeOwnPassword()
		{
			return PartialView ("_ChangeOwnPassword");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeOwnPassword(ChangePasswordViewModel model)
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
            }
			catch (Exception ex) {
				ModelState.AddModelError ("", ex.Message);
			}
			return PartialView ("_ChangeOwnPassword", model);
		}

		[HttpGet]
		public async Task<IActionResult> ListAllUsers ()
		{
			BaseListViewModel<UserViewModel> userList = new BaseListViewModel<UserViewModel> ();
            var allUsers = await _userService.GetAllUsers();
            foreach (var user in allUsers) {
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
		public async Task<PartialViewResult> UserPermissions (Guid Id)
		{
            throw new Exception("Method needs to be rewritten");			
		}

		[HttpGet]
		public async Task<IActionResult> ManageUser (Guid Id)
		{
            var user = await _userService.GetUser(Id);
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
