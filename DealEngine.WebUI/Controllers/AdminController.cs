using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.WebUI.Models;
using DealEngine.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using IdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;
using IdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;
using Microsoft.AspNetCore.Identity;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class AdminController : BaseController
	{		
		IPrivateServerService _privateServerService;
        IPaymentGatewayService _paymentGatewayService;
        IMerchantService _merchantService;
        IFileService _fileService;
		IOrganisationService _organisationService;		
		IUnitOfWork _unitOfWork;
		IInformationTemplateService _informationTemplateService;
        IClientInformationService _clientInformationService;
		IProgrammeService _programmeService;
		IVehicleService _vehicleService;
        ISystemEmailService _systemEmailService;
        IReferenceService _referenceService;
        IMapper _mapper;
        ILogger<AdminController> _logger;
        IApplicationLoggingService _applicationLoggingService;
        IImportService _importService;
        SignInManager<IdentityUser> _signInManager;
        UserManager<IdentityUser> _userManager;

        public AdminController (
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IImportService importService,
            IApplicationLoggingService applicationLoggingService,
            ILogger<AdminController> logger,
            IUserService userRepository, 
            IPrivateServerService privateServerService, 
            IFileService fileService,
			IOrganisationService organisationService, 
            IUnitOfWork unitOfWork, 
            IInformationTemplateService informationTemplateService,
            IClientInformationService clientInformationService, 
            IProgrammeService programeService, 
            IVehicleService vehicleService, 
            IMapper mapper, 
            IPaymentGatewayService paymentGatewayService,
            IMerchantService merchantService, 
            ISystemEmailService systemEmailService, 
            IReferenceService referenceService)
			: base (userRepository)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _importService = importService;
            _applicationLoggingService = applicationLoggingService;
            _logger = logger;
			_privateServerService = privateServerService;
			_fileService = fileService;
			_organisationService = organisationService;
			_unitOfWork = unitOfWork;
			_informationTemplateService = informationTemplateService;
			_clientInformationService = clientInformationService;
			_programmeService = programeService;
			_vehicleService = vehicleService;
			_mapper = mapper;
            _paymentGatewayService = paymentGatewayService;
            _merchantService = merchantService;
            _systemEmailService = systemEmailService;
            _referenceService = referenceService;
        }

		[HttpGet]
		public async Task<IActionResult> Index ()
		{
            AdminViewModel model = new AdminViewModel();
            var user = await CurrentUser();
            try
            {         
                var privateServers = await _privateServerService.GetAllPrivateServers();
                var paymentGateways = await _paymentGatewayService.GetAllPaymentGateways();
                var merchants = await _merchantService.GetAllMerchants();
                var users = _userManager.Users.ToList();

                model.PrivateServers = _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>>(privateServers);
                model.PaymentGateways = _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways);
                model.Merchants = _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants);
                model.Users = users;
                return View(model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }
        
        [HttpGet]
        public async Task<IActionResult> AONImportUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportAOEServiceIndividuals(user);

                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CEASImportUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportCEASServiceIndividuals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CEASUpdateUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportCEASServiceUpdateUsers(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PMINZImportUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportPMINZServiceIndividuals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> AAAImportUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportNZFSGServiceIndividuals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AAAImportPrincipals()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportNZFSGServicePrincipals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> NZFSGImportUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportNZFSGServiceIndividuals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> NZFSGImportPrincipals()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportNZFSGServicePrincipals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CEASImportClaims()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportCEASServiceClaims(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CEASImportContracts()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportCEASServiceContract(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PMINZImportContracts()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportPMINZServiceContract(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AONImportPrincipals()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportAOEServicePrincipals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CEASImportPrincipals()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportCEASServicePrincipals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PMINZImportPrincipals()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportPMINZServicePrincipals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PMINZImportPreRenewData()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportPMINZServicePreRenewData(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DANZImportUsers()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportDANZServiceIndividuals(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DANZImportPersonnel()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportDANZServicePersonnel(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DANZImportClaims()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportDANZServiceClaims(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DANZImportPreRenewData()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportDANZServicePreRenewData(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AONImportContracts()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportAOEServiceContract(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AONImportClaims()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                await _importService.ImportAOEServiceClaims(user);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrivateServerList()
        {
			var privateServers = await _privateServerService.GetAllPrivateServers();

			return PartialView ("_PrivateServerList", _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>> (privateServers));
			//return Json(privateServers, JsonRequestBehavior.AllowGet) ;
        }

		[HttpPost]
        public async Task<IActionResult> AddPrivateServer(PrivateServerViewModel privateServer)
        {
			var privateServers = await _privateServerService.GetAllPrivateServers();
            User user = null;
            try
            {
                user = await CurrentUser();
                // check to see if we are updating a private server
                if (privateServers.Any(ps => ps.ServerAddress == privateServer.ServerAddress))
					await _privateServerService.RemoveServer(user, privateServer.ServerAddress);  

				await _privateServerService.AddNewServer(user, privateServer.ServerName, privateServer.ServerAddress);
				// reload servers
				privateServers = await _privateServerService.GetAllPrivateServers();
				return PartialView ("_PrivateServerList", _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>> (privateServers));
            }
			catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);                
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpPost]
		public async Task<IActionResult> DeletePrivateServer(string id)
		{            
            await _privateServerService.RemoveServer(await CurrentUser(), id);
			return await PrivateServerList();
		}

        [HttpGet]
        public async Task<IActionResult> PaymentGatewayList()
        {
            var paymentGateways = await _paymentGatewayService.GetAllPaymentGateways();

            return PartialView("_PaymentGatewayList", _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways));
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentGateway(PaymentGatewayViewModel paymentGateway)
        {
            var paymentGateways = await _paymentGatewayService.GetAllPaymentGateways();
            var user = await CurrentUser();
            try
            {
                // check to see if we are updating a payment gateway
                if (paymentGateways.Any(pgws => pgws.PaymentGatewayWebServiceURL == paymentGateway.PaymentGatewayWebServiceURL))
                    await _paymentGatewayService.RemovePaymentGateway(user, paymentGateway.PaymentGatewayWebServiceURL);

                await _paymentGatewayService.AddNewPaymentGateway(user, paymentGateway.PaymentGatewayName, paymentGateway.PaymentGatewayWebServiceURL, paymentGateway.PaymentGatewayResponsePageURL,
                    paymentGateway.PaymentGatewayType);
                // reload payment gateways
                paymentGateways = await _paymentGatewayService.GetAllPaymentGateways();
                return PartialView("_PaymentGatewayList", _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways));
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentGateway(string id)
        {
            await _paymentGatewayService.RemovePaymentGateway(await CurrentUser(), id);            
            return await PaymentGatewayList();
        }

        [HttpGet]
        public async Task<IActionResult> MerchantList()
        {
            
            MerchantViewModel merchantModel = new MerchantViewModel();
            var allPaymentGateways = new List<PaymentGatewayViewModel>();
            var user = await CurrentUser();
            try
            {
                var merchants = await _merchantService.GetAllMerchants();
                var dbPaymentGateways = await _paymentGatewayService.GetAllPaymentGateways();
                foreach (PaymentGateway pg in dbPaymentGateways)
                {
                    allPaymentGateways.Add(PaymentGatewayViewModel.FromEntity(pg));
                }
                merchantModel.AllPaymentGateways = allPaymentGateways;

                return PartialView("_MerchantList", _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants));
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddMerchant(MerchantViewModel merchant)
        {
            var merchants = await _merchantService.GetAllMerchants();
            var user = await CurrentUser();
            try
            {
                if (merchants.Any(ms => ms.MerchantKey == merchant.MerchantKey))
                    await _merchantService.RemoveMerchant(user, merchant.MerchantKey);

                await _merchantService.AddNewMerchant(user, merchant.MerchantUserName, merchant.MerchantPassword, merchant.MerchantKey, 
                    merchant.MerchantReference);
                // reload merchants
                merchants = await _merchantService.GetAllMerchants();
                return PartialView("_MerchantList", _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants));
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMerchant(string id)
        {
            await _merchantService.RemoveMerchant(await CurrentUser(), id);                
            return await MerchantList();
        }        

        [HttpPost]
		public async Task<IActionResult> UnlockUser(string UserId)
		{            
            var user = await _userService.GetUserById(Guid.Parse(UserId));
            user.Unlock();
            await _userService.Update(user);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ManageRsaUsers()
        {
            AdminViewModel model = new AdminViewModel();
            var user = await CurrentUser();
            try
            {
                var lockedUsers = await _userService.GetLockedUsers();

                if (lockedUsers.Count != 0)
                {
                    model.LockedUsers = new List<SelectListItem>();
                    foreach (var lockedUser in lockedUsers)
                    {
                        model.LockedUsers.Add(new SelectListItem
                        {
                            Value = lockedUser.Id.ToString(),
                            Text = lockedUser.LastName + " username: " + lockedUser.UserName
                        });
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SysEmailTemplate(String systemEmailType, String internalNotes)
        {
            
             
            SystemEmailTemplateViewModel model = new SystemEmailTemplateViewModel();
            var user = await CurrentUser();
            try
            {
                SystemEmail systemEmailTemplate = await _systemEmailService.GetSystemEmailByType(systemEmailType);
                model.InternalNotes = internalNotes;
                model.SystemEmailType = systemEmailType;

                if (systemEmailTemplate != null)
                {
                    model.SystemEmailName = systemEmailTemplate.SystemEmailName;
                    model.Subject = systemEmailTemplate.Subject;
                    model.Body = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);

                }
                else
                {
                    model.SystemEmailName = "";
                    model.Subject = "";
                    model.Body = "";
                }

                ViewBag.Title = "Add/Edit System Email Template";

                return View("SysEmailTemplate", model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SysEmailTemplate(SystemEmailTemplateViewModel model)
        {
            var user = await CurrentUser();
            try
            {
                SystemEmail systemEmailTemplate = await _systemEmailService.GetSystemEmailByType(model.SystemEmailType);
                string systememailtemplatename = null;

                switch (model.SystemEmailType)
                {
                    case "LoginEmail":
                        {
                            systememailtemplatename = "Login Instruction Email";
                            break;
                        }
                    case "UISIssueNotificationEmail":
                        {
                            systememailtemplatename = "Information Sheet Issue Notification Email";
                            break;
                        }
                    case "InvoiceSuccessConfig":
                        {
                            systememailtemplatename = "Invoice Success Configuration Notification Email";
                            break;
                        }
                    case "InvoiceFailConfig":
                        {
                            systememailtemplatename = "Invoice Fail Configuration Notification Email";
                            break;
                        }
                    case "PaymentSuccessConfig":
                        {
                            systememailtemplatename = "Payment Success Configuration Notification Email";
                            break;
                        }
                    case "PaymentFailConfig":
                        {
                            systememailtemplatename = "Payment Fail Configuration Notification Email";
                            break;
                        }
                    case "UISSubmissionConfirmationEmail":
                        {
                            systememailtemplatename = "Information Sheet Submission Confirmation Email";
                            break;
                        }
                    case "UISSubmissionNotificationEmail":
                        {
                            systememailtemplatename = "Information Sheet Submission Notification Email";
                            break;
                        }
                    case "AgreementReferralNotificationEmail":
                        {
                            systememailtemplatename = "Agreement Referral Notification Email";
                            break;
                        }
                    case "AgreementIssueNotificationEmail":
                        {
                            systememailtemplatename = "Agreement Issue Notification Email";
                            break;
                        }
                    case "AgreementBoundNotificationEmail":
                        {
                            systememailtemplatename = "Agreement Bound Notification Email";
                            break;
                        }
                    case "OtherMarinaTCNotifyEmail":
                        {
                            systememailtemplatename = "Create Other Marina Notification Email";
                            break;
                        }
                    default:
                        {
                            throw new Exception(string.Format("Invalid System Email Template Type for ", model.SystemEmailType));
                        }
                }

                if (systemEmailTemplate != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        systemEmailTemplate.Subject = model.Subject;
                        systemEmailTemplate.Body = model.Body;
                        systemEmailTemplate.LastModifiedBy = await CurrentUser();
                        systemEmailTemplate.LastModifiedOn = DateTime.UtcNow;

                        await uow.Commit();
                    }
                }
                else
                {
                    await _systemEmailService.AddNewSystemEmail(await CurrentUser(), systememailtemplatename, model.InternalNotes, model.Subject, model.Body, model.SystemEmailType);
                }

                return Redirect("~/Admin/Index");
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            

        }


        [HttpGet]
        public async Task<IActionResult> SysExchange()
        {
            return View("SysExchange");
        }

        [HttpPost]
        public async Task<IActionResult> SysExchange(OrganisationViewModel model)
        {
            throw new Exception("new organisation method");
            //Organisation org = await _organisationService.GetOrganisationByEmail(model.Email);            
            //User user = await  _userService.GetUserByEmail(model.Email);

            //return Redirect("~/Admin/Index");

        }

        [HttpPost]
        public async Task<IActionResult> ImpersonateUser(IFormCollection form)
        {
            await _signInManager.SignOutAsync();
            var deUser = await _userManager.FindByNameAsync(form["username"].ToString());
            await _signInManager.SignInAsync(deUser, true);

            return Redirect("~/Home/Index");

        }

        [HttpGet]
        public async Task<IActionResult> AONOrganisationRefactor()
        {
            var programmeId = Guid.Parse("48ce028d-1fcb-4f3b-881b-9fd769b87643");
            //await _organisationService.RefactorOrganisations(programmeId);

            return Redirect("~/Home/Index");

        }
    }
}
