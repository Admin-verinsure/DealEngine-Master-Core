using AutoMapper;
using Elmah;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using TechCertain.WebUI.Areas.Identity.Data;
using TechCertain.WebUI.Models;

namespace TechCertain.WebUI.Controllers
{
    public class AdminController : BaseController
	{
		ILogger _logger;

		IPrivateServerService _privateServerService;
        IPaymentGatewayService _paymentGatewayService;
        IMerchantService _merchantService;
        IFileService _fileService;
		IOrganisationService _organisationService;
		IOrganisationRepository _organisationRepository;
		IUnitOfWorkFactory _unitOfWorkFactory;
		IInformationTemplateService _informationTemplateService;
        ICilentInformationService _clientInformationService;
		IProgrammeService _programmeService;
		IVehicleService _vehicleService;
        ISystemEmailService _systemEmailService;
        IReferenceService _referenceService;

        IMapper _mapper;

		public AdminController (IUserService userRepository, DealEngineDBContext dealEngineDBContext, ILogger logger, IPrivateServerService privateServerService, IFileService fileService,
			IOrganisationRepository organisationRepository, IOrganisationService organisationService, IUnitOfWorkFactory unitOfWorkFactory, IInformationTemplateService informationTemplateService,
            ICilentInformationService clientInformationService, IProgrammeService programeService, IVehicleService vehicleService, IMapper mapper, IPaymentGatewayService paymentGatewayService,
            IMerchantService merchantService, ISystemEmailService systemEmailService, IReferenceService referenceService)
			: base (userRepository, dealEngineDBContext)
		{
			_logger = logger;
			_privateServerService = privateServerService;
			_fileService = fileService;
			_organisationService = organisationService;
			_organisationRepository = organisationRepository;
			_unitOfWorkFactory = unitOfWorkFactory;
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
		public ActionResult Index ()
		{
			Console.WriteLine ("Debug: " + _logger.IsDebugEnabled);
			Console.WriteLine ("Error: " + _logger.IsFatalEnabled);
			Console.WriteLine ("Fatal: " + _logger.IsFatalEnabled);
			Console.WriteLine ("Info: " + _logger.IsInfoEnabled);
			Console.WriteLine ("Trace: " + _logger.IsTraceEnabled);
			Console.WriteLine ("Warn: " + _logger.IsWarnEnabled);

            var privateServers = _privateServerService.GetAllPrivateServers ().ToList();
            var paymentGateways = _paymentGatewayService.GetAllPaymentGateways().ToList();
            var merchants = _merchantService.GetAllMerchants().ToList();
            return View (new AdminViewModel() {
				PrivateServers = _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>>(privateServers),
                PaymentGateways = _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways),
                Merchants = _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants)
            });

        }
        
        [HttpGet]
        public ActionResult PrivateServerList()
        {
			var privateServers = _privateServerService.GetAllPrivateServers().ToList();

			return PartialView ("_PrivateServerList", _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>> (privateServers));
			//return Json(privateServers, JsonRequestBehavior.AllowGet) ;
        }

		[HttpPost]
        public ActionResult AddPrivateServer(PrivateServerViewModel privateServer)
        {
			var privateServers = _privateServerService.GetAllPrivateServers ().ToList();
				
            try
            {
				// check to see if we are updating a private server
				if (privateServers.Any(ps => ps.ServerAddress == privateServer.ServerAddress))
					_privateServerService.RemoveServer(CurrentUser, privateServer.ServerAddress);  

				_privateServerService.AddNewServer(CurrentUser, privateServer.ServerName, privateServer.ServerAddress);
				// reload servers
				privateServers = _privateServerService.GetAllPrivateServers ().ToList();
				return PartialView ("_PrivateServerList", _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>> (privateServers));
            }
			catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpPost]
		public ActionResult DeletePrivateServer(string id)
		{
			if (_privateServerService.RemoveServer (CurrentUser, id))
				return PrivateServerList ();
			return PrivateServerList ();
		}

        [HttpGet]
        public ActionResult PaymentGatewayList()
        {
            var paymentGateways = _paymentGatewayService.GetAllPaymentGateways().ToList();

            return PartialView("_PaymentGatewayList", _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways));
        }

        [HttpPost]
        public ActionResult AddPaymentGateway(PaymentGatewayViewModel paymentGateway)
        {
            var paymentGateways = _paymentGatewayService.GetAllPaymentGateways().ToList();

            try
            {
                // check to see if we are updating a payment gateway
                if (paymentGateways.Any(pgws => pgws.PaymentGatewayWebServiceURL == paymentGateway.PaymentGatewayWebServiceURL))
                    _paymentGatewayService.RemovePaymentGateway(CurrentUser, paymentGateway.PaymentGatewayWebServiceURL);

                _paymentGatewayService.AddNewPaymentGateway(CurrentUser, paymentGateway.PaymentGatewayName, paymentGateway.PaymentGatewayWebServiceURL, paymentGateway.PaymentGatewayResponsePageURL,
                    paymentGateway.PaymentGatewayType);
                // reload payment gateways
                paymentGateways = _paymentGatewayService.GetAllPaymentGateways().ToList();
                return PartialView("_PaymentGatewayList", _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public ActionResult DeletePaymentGateway(string id)
        {
            if (_paymentGatewayService.RemovePaymentGateway(CurrentUser, id))
                return PaymentGatewayList();
            return PaymentGatewayList();
        }

        [HttpGet]
        public ActionResult MerchantList()
        {
            var merchants = _merchantService.GetAllMerchants().ToList();
            MerchantViewModel merchantModel = new MerchantViewModel();
            var allPaymentGateways = new List<PaymentGatewayViewModel>();
            foreach (PaymentGateway pg in _paymentGatewayService.GetAllPaymentGateways())
            {
                allPaymentGateways.Add(PaymentGatewayViewModel.FromEntity(pg));
            }
            merchantModel.AllPaymentGateways = allPaymentGateways;

            return PartialView("_MerchantList", _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants));
        }

        [HttpPost]
        public ActionResult AddMerchant(MerchantViewModel merchant)
        {
            var merchants = _merchantService.GetAllMerchants().ToList();

            try
            {
                if (merchants.Any(ms => ms.MerchantKey == merchant.MerchantKey))
                    _merchantService.RemoveMerchant(CurrentUser, merchant.MerchantKey);

                _merchantService.AddNewMerchant(CurrentUser, merchant.MerchantUserName, merchant.MerchantPassword, merchant.MerchantKey, 
                    merchant.MerchantReference);
                // reload merchants
                merchants = _merchantService.GetAllMerchants().ToList();
                return PartialView("_MerchantList", _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public ActionResult DeleteMerchant(string id)
        {
            if (_merchantService.RemoveMerchant(CurrentUser, id))
                return MerchantList();
            return MerchantList();
        }

        //[HttpPost]
        //[AuthorizeRole("Admin")]
        //public ActionResult UploadDataFiles(HttpPostedFileWrapper uploadedUserData, HttpPostedFileWrapper uploadedLocationData, HttpPostedFileWrapper uploadedOrgUISData, HttpPostedFileWrapper uploadedVehicleData, HttpPostedFileWrapper uploadedIPOrganisationData)
        //{
        //    byte[] buffer;
        //    // Parse uploaded organisation and user data here
        //    if (uploadedUserData != null)
        //    {
        //        buffer = new byte[uploadedUserData.ContentLength];
        //        uploadedUserData.InputStream.Read(buffer, 0, buffer.Length);
        //        string lines = _fileService.FromBytes(buffer);
        //        using (System.IO.StringReader reader = new System.IO.StringReader(lines))
        //        {
        //            string line = string.Empty;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                string[] parts = line.Split(',');
        //                // TODO - parse user data here
        //                string personalOrganisationName = "Default user organisation for " + parts[0] + " " + parts[1];
        //                Organisation organisation = null;

        //                organisation = _organisationService.GetOrganisationByName(parts[2]);
        //                if (organisation == null)
        //                {
        //                    organisation = new Organisation(CurrentUser, Guid.NewGuid(), parts[2]);
        //                    organisation.Phone = parts[3];
        //                    _organisationService.CreateNewOrganisation(organisation);
        //                    Console.WriteLine("Created Organisation " + organisation.Name);
        //                }
        //                //else
        //                //	Console.WriteLine ("Loaded Organisation " + organisation.Name);

        //                User user = null;
        //                User user2 = null;

        //                try
        //                {
        //                    user = _userService.GetUserByEmail(parts[5]);
        //                    //Console.WriteLine ("Loaded User " + user.FullName + " by email");
        //                }
        //                catch (Exception)
        //                {
        //                    string username = null;
        //                    if (parts[6] == "")
        //                    {
        //                        username = parts[0] + "_" + parts[1];
        //                    }
        //                    else
        //                    {
        //                        username = parts[6];
        //                    }

        //                    try
        //                    {
        //                        user2 = _userService.GetUser(username);
        //                        //Console.WriteLine ("Loaded User " + user.FullName + " by username");
        //                    }
        //                    catch (Exception)
        //                    {
        //                        // create personal organisation
        //                        //var personalOrganisation = new Organisation (CurrentUser, Guid.NewGuid (), personalOrganisationName, new OrganisationType (CurrentUser, "personal"));
        //                        //_organisationService.CreateNewOrganisation (personalOrganisation);
        //                        // create user object
        //                        user = new User(CurrentUser, Guid.NewGuid(), username);
        //                        user.FirstName = parts[0];
        //                        user.LastName = parts[1];
        //                        user.FullName = parts[0] + " " + parts[1];
        //                        user.Email = parts[5];
        //                        user.Phone = parts[3];
        //                        user.Password = "";
        //                        //user.Organisations.Add (personalOrganisation);
        //                        // save the new user
        //                        // creates a new user in the system along with a default organisation
        //                        _userService.Create(user);
        //                        //Console.WriteLine ("Created User " + user.FullName);
        //                    }
        //                    if (user2 != null && user != user2)
        //                    {
        //                        Exception ex = new Exception(string.Format("User with email {0} doesn't match user with username {1}", user.Email, user.UserName));
        //                        _logger.Error(ex);
        //                        throw ex;
        //                    }
        //                }
        //                finally
        //                {
        //                    if (!user.Organisations.Contains(organisation))
        //                        user.Organisations.Add(organisation);

        //                    _userService.Update(user);
        //                }
        //            }
        //        }
        //    }

        //    // parse uploaded organisational unit and location data here
        //    // TODO

        //    if (uploadedLocationData != null)
        //    {
        //        buffer = new byte[uploadedLocationData.ContentLength];
        //        uploadedLocationData.InputStream.Read(buffer, 0, buffer.Length);
        //        string lines = _fileService.FromBytes(buffer);
        //        using (System.IO.StringReader reader = new System.IO.StringReader(lines))
        //        {
        //            string line = string.Empty;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                string[] parts = line.Split(',');

        //                Organisation organisation = null;
        //                organisation = _organisationService.GetOrganisationByName(parts[0]);
        //                if (organisation != null)
        //                {
        //                    using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork())
        //                    {
        //                        OrganisationalUnit ou = new OrganisationalUnit(CurrentUser, parts[2]);
        //                        organisation.OrganisationalUnits.Add(ou);

        //                        Location location = new Location(CurrentUser)
        //                        {
        //                            Street = parts[1],
        //                            CommonName = parts[2],
        //                            Country = "New Zealand"
        //                        };

        //                        location.OrganisationalUnits = new List<OrganisationalUnit>();
        //                        location.OrganisationalUnits.Add(ou);

        //                        ou.Locations = new List<Location>();
        //                        ou.Locations.Add(location);

        //                        uow.Commit();
        //                    }

        //                }

        //            }
        //        }
        //    }


        //    // parse uploaded organisation data here for creating UIS
        //    // TODO

        //    if (uploadedOrgUISData != null)
        //    {
        //        buffer = new byte[uploadedOrgUISData.ContentLength];
        //        uploadedOrgUISData.InputStream.Read(buffer, 0, buffer.Length);
        //        string lines = _fileService.FromBytes(buffer);
        //        using (System.IO.StringReader reader = new System.IO.StringReader(lines))
        //        {
        //            string line = string.Empty;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                string[] parts = line.Split(',');

        //                //var template = _informationTemplateService.GetAllTemplates ().FirstOrDefault (t => t.Name == parts [1]);

        //                var programme = _programmeService.GetAllProgrammes().FirstOrDefault(p => p.Name == parts[1]);

        //                Organisation organisation = null;
        //                organisation = _organisationService.GetOrganisationByName(parts[0]);
        //                if (organisation != null)
        //                {
        //                    using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork())
        //                    {
        //                        var clientProgramme = _programmeService.CreateClientProgrammeFor(programme.Id, CurrentUser, organisation);
        //                        var reference = _referenceService.GetLatestReferenceId();
        //                        var sheet = _clientInformationService.IssueInformationFor(CurrentUser, organisation, clientProgramme, reference);
        //                        _referenceService.CreateClientInformationReference(sheet);

        //                        uow.Commit();
        //                    }

        //                }
        //            }
        //        }
        //    }

        //    // parse uploaded vehicle data here
        //    // TODO

        //    if (uploadedVehicleData != null)
        //    {
        //        buffer = new byte[uploadedVehicleData.ContentLength];
        //        uploadedVehicleData.InputStream.Read(buffer, 0, buffer.Length);
        //        string lines = _fileService.FromBytes(buffer);
        //        using (System.IO.StringReader reader = new System.IO.StringReader(lines))
        //        {
        //            string line = string.Empty;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                string[] parts = line.Split(',');

        //                Organisation organisation = null;
        //                organisation = _organisationService.GetOrganisationByName(parts[0]);
        //                if (organisation != null)
        //                {
        //                    ClientInformationSheet sheet = _clientInformationService.GetAllInformationFor(organisation).FirstOrDefault();

        //                    if (sheet != null)
        //                    {
        //                        if (parts[3] == "")
        //                        {
        //                            using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork())
        //                            {
        //                                Vehicle v = new Vehicle(CurrentUser, "", "", "")
        //                                {
        //                                    GroupSumInsured = Convert.ToInt32(parts[2]),
        //                                    FleetNumber = parts[1],
        //                                    Notes = parts[4],
        //                                    Validated = false
        //                                };

        //                                sheet.Vehicles.Add(v);

        //                                uow.Commit();
        //                            }
        //                        }
        //                        else
        //                        {

        //                            try
        //                            {
        //                                Vehicle vehicle = _vehicleService.GetValidatedVehicle(parts[3]);
        //                                if (vehicle != null)
        //                                {
        //                                    using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork())
        //                                    {
        //                                        Vehicle regv = new Vehicle(CurrentUser, vehicle.Registration, vehicle.Make, vehicle.Model)
        //                                        {
        //                                            GroupSumInsured = Convert.ToInt32(parts[2]),
        //                                            FleetNumber = parts[1],
        //                                            Notes = parts[4],
        //                                            Validated = vehicle.Validated,
        //                                            VIN = vehicle.VIN,
        //                                            ChassisNumber = vehicle.ChassisNumber,
        //                                            EngineNumber = vehicle.EngineNumber,
        //                                            Year = vehicle.Year,
        //                                            GrossVehicleMass = vehicle.GrossVehicleMass
        //                                        };

        //                                        sheet.Vehicles.Add(regv);

        //                                        uow.Commit();
        //                                    }
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork())
        //                                {
        //                                    Vehicle v = new Vehicle(CurrentUser, "", "", "")
        //                                    {
        //                                        GroupSumInsured = Convert.ToInt32(parts[2]),
        //                                        FleetNumber = parts[1],
        //                                        Notes = parts[4],
        //                                        Validated = false,
        //                                        SerialNumber = parts[3]
        //                                    };

        //                                    sheet.Vehicles.Add(v);

        //                                    uow.Commit();
        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Parse uploaded interest party organisation data here
        //    if (uploadedIPOrganisationData != null)
        //    {
        //        buffer = new byte[uploadedIPOrganisationData.ContentLength];
        //        uploadedIPOrganisationData.InputStream.Read(buffer, 0, buffer.Length);
        //        string lines = _fileService.FromBytes(buffer);
        //        using (System.IO.StringReader reader = new System.IO.StringReader(lines))
        //        {
        //            string line = string.Empty;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                string[] parts = line.Split(',');

        //                Organisation organisation = null;
        //                string organisationtypename = parts[1];

        //                organisation = _organisationService.GetOrganisationByName(parts[0]);
        //                if (organisation == null)
        //                {
        //                    organisation = new Organisation(CurrentUser, Guid.NewGuid(), parts[0], new OrganisationType(CurrentUser, organisationtypename));
        //                    _organisationService.CreateNewOrganisation(organisation);
        //                    //Console.WriteLine("Created Organisation " + organisation.Name);
        //                }

        //            }
        //        }
        //    }

        //    return Redirect("/Admin/Index");
        //}

        [HttpPost]
		public ActionResult UnlockUser (string username)
		{
			_userService.RemoveGlobalBan (_userService.GetUser (username), CurrentUser);
			return Redirect ("/Admin/Index");
		}

        [HttpGet]
        public ActionResult SysEmailTemplate(String systemEmailType, String internalNotes)
        {
            SystemEmail systemEmailTemplate = _systemEmailService.GetAllSystemEmails().FirstOrDefault(se => se.SystemEmailType == systemEmailType);
             
            SystemEmailTemplateViewModel model = new SystemEmailTemplateViewModel();

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

        [HttpPost]
        public ActionResult SysEmailTemplate(SystemEmailTemplateViewModel model)
        {
            SystemEmail systemEmailTemplate = _systemEmailService.GetAllSystemEmails().FirstOrDefault(se => se.SystemEmailType == model.SystemEmailType);

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
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    systemEmailTemplate.Subject = model.Subject;
                    systemEmailTemplate.Body = model.Body;
                    systemEmailTemplate.LastModifiedBy = CurrentUser;
                    systemEmailTemplate.LastModifiedOn = DateTime.UtcNow;

                    uow.Commit();
                }
            }
            else
            {
               _systemEmailService.AddNewSystemEmail(CurrentUser, systememailtemplatename, model.InternalNotes, model.Subject, model.Body, model.SystemEmailType);
            }

            _logger.Info("System email template " + systememailtemplatename + " updated");

            return Redirect("~/Admin/Index");

        }


        [HttpGet]
        public ActionResult SysExchange()
        {
            return View("SysExchange");
        }

        [HttpPost]
        public ActionResult SysExchange(OrganisationViewModel model)
        {
            Organisation org = _organisationService.GetOrganisationByEmail(model.Email);

            if (org == null)
            {
                _logger.Info("Create Org");
            }

            User user = _userService.GetUserByEmail(model.Email);

            if (org == null)
            {
                _logger.Info("Create User");
            }


            return Redirect("~/Admin/Index");

        }

    }
}
