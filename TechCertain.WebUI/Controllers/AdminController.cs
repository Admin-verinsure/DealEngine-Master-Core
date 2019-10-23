using AutoMapper;
using Elmah;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;

namespace TechCertain.WebUI.Controllers
{
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

		public AdminController (IUserService userRepository, IPrivateServerService privateServerService, IFileService fileService,
			IOrganisationService organisationService, IUnitOfWork unitOfWork, IInformationTemplateService informationTemplateService,
            IClientInformationService clientInformationService, IProgrammeService programeService, IVehicleService vehicleService, IMapper mapper, IPaymentGatewayService paymentGatewayService,
            IMerchantService merchantService, ISystemEmailService systemEmailService, IReferenceService referenceService)
			: base (userRepository)
		{		
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

            var privateServers = _privateServerService.GetAllPrivateServers().Result;
            var paymentGateways = _paymentGatewayService.GetAllPaymentGateways().Result;
            var merchants = _merchantService.GetAllMerchants().Result;
            return View (new AdminViewModel() {
				PrivateServers = _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>>(privateServers),
                PaymentGateways = _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways),
                Merchants = _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants)
            });

        }
        
        [HttpGet]
        public async Task<IActionResult> PrivateServerList()
        {
			var privateServers = _privateServerService.GetAllPrivateServers().Result;

			return PartialView ("_PrivateServerList", _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>> (privateServers));
			//return Json(privateServers, JsonRequestBehavior.AllowGet) ;
        }

		[HttpPost]
        public async Task<IActionResult> AddPrivateServer(PrivateServerViewModel privateServer)
        {
			var privateServers = _privateServerService.GetAllPrivateServers().Result;
				
            try
            {
				// check to see if we are updating a private server
				if (privateServers.Any(ps => ps.ServerAddress == privateServer.ServerAddress))
					await _privateServerService.RemoveServer(CurrentUser, privateServer.ServerAddress).ConfigureAwait(false);  

				await _privateServerService.AddNewServer(CurrentUser, privateServer.ServerName, privateServer.ServerAddress).ConfigureAwait(false);
				// reload servers
				privateServers = _privateServerService.GetAllPrivateServers().Result;
				return PartialView ("_PrivateServerList", _mapper.Map<IList<PrivateServer>, IList<PrivateServerViewModel>> (privateServers));
            }
			catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpPost]
		public async Task<IActionResult> DeletePrivateServer(string id)
		{
            await _privateServerService.RemoveServer(CurrentUser, id).ConfigureAwait(false);
			return await PrivateServerList().ConfigureAwait(false);
		}

        [HttpGet]
        public async Task<IActionResult> PaymentGatewayList()
        {
            var paymentGateways = _paymentGatewayService.GetAllPaymentGateways().Result;

            return PartialView("_PaymentGatewayList", _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways));
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentGateway(PaymentGatewayViewModel paymentGateway)
        {
            var paymentGateways = _paymentGatewayService.GetAllPaymentGateways().Result;

            try
            {
                // check to see if we are updating a payment gateway
                if (paymentGateways.Any(pgws => pgws.PaymentGatewayWebServiceURL == paymentGateway.PaymentGatewayWebServiceURL))
                    await _paymentGatewayService.RemovePaymentGateway(CurrentUser, paymentGateway.PaymentGatewayWebServiceURL).ConfigureAwait(false);

                await _paymentGatewayService.AddNewPaymentGateway(CurrentUser, paymentGateway.PaymentGatewayName, paymentGateway.PaymentGatewayWebServiceURL, paymentGateway.PaymentGatewayResponsePageURL,
                    paymentGateway.PaymentGatewayType).ConfigureAwait(false);
                // reload payment gateways
                paymentGateways = _paymentGatewayService.GetAllPaymentGateways().Result;
                return PartialView("_PaymentGatewayList", _mapper.Map<IList<PaymentGateway>, IList<PaymentGatewayViewModel>>(paymentGateways));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentGateway(string id)
        {
            _paymentGatewayService.RemovePaymentGateway(CurrentUser, id).ConfigureAwait(false);            
            return await PaymentGatewayList().ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<IActionResult> MerchantList()
        {
            var merchants = _merchantService.GetAllMerchants().Result;
            MerchantViewModel merchantModel = new MerchantViewModel();
            var allPaymentGateways = new List<PaymentGatewayViewModel>();
            foreach (PaymentGateway pg in _paymentGatewayService.GetAllPaymentGateways().Result)
            {
                allPaymentGateways.Add(PaymentGatewayViewModel.FromEntity(pg));
            }
            merchantModel.AllPaymentGateways = allPaymentGateways;

            return PartialView("_MerchantList", _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants));
        }

        [HttpPost]
        public async Task<IActionResult> AddMerchant(MerchantViewModel merchant)
        {
            var merchants = _merchantService.GetAllMerchants().Result;

            try
            {
                if (merchants.Any(ms => ms.MerchantKey == merchant.MerchantKey))
                    await _merchantService.RemoveMerchant(CurrentUser, merchant.MerchantKey).ConfigureAwait(false);

                await _merchantService.AddNewMerchant(CurrentUser, merchant.MerchantUserName, merchant.MerchantPassword, merchant.MerchantKey, 
                    merchant.MerchantReference).ConfigureAwait(false);
                // reload merchants
                merchants = _merchantService.GetAllMerchants().Result;
                return PartialView("_MerchantList", _mapper.Map<IList<Merchant>, IList<MerchantViewModel>>(merchants));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMerchant(string id)
        {
            await _merchantService.RemoveMerchant(CurrentUser, id).ConfigureAwait(false);                
            return await MerchantList().ConfigureAwait(false);
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadDataFiles(HttpPostedFileWrapper uploadedUserData, HttpPostedFileWrapper uploadedLocationData, HttpPostedFileWrapper uploadedOrgUISData, HttpPostedFileWrapper uploadedVehicleData, HttpPostedFileWrapper uploadedIPOrganisationData)
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
        //                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
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
        //                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
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
        //                            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
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
        //                                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
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
        //                                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
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
		public async Task<IActionResult> UnlockUser (string username)
		{
            throw new Exception("method needs to be implemented in identity");
			//_userService.RemoveGlobalBan (_userService.GetUser (username), CurrentUser);
			return Redirect ("/Admin/Index");
		}

        [HttpGet]
        public async Task<IActionResult> SysEmailTemplate(String systemEmailType, String internalNotes)
        {
            SystemEmail systemEmailTemplate = _systemEmailService.GetSystemEmailByType(systemEmailType).Result;
             
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
        public async Task<IActionResult> SysEmailTemplate(SystemEmailTemplateViewModel model)
        {
            SystemEmail systemEmailTemplate = _systemEmailService.GetSystemEmailByType(model.SystemEmailType).Result;

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
                    systemEmailTemplate.LastModifiedBy = CurrentUser;
                    systemEmailTemplate.LastModifiedOn = DateTime.UtcNow;

                    await uow.Commit().ConfigureAwait(false);
                }
            }
            else
            {
               await _systemEmailService.AddNewSystemEmail(CurrentUser, systememailtemplatename, model.InternalNotes, model.Subject, model.Body, model.SystemEmailType).ConfigureAwait(false);
            }
           
            return Redirect("~/Admin/Index");

        }


        [HttpGet]
        public async Task<IActionResult> SysExchange()
        {
            return View("SysExchange");
        }

        [HttpPost]
        public async Task<IActionResult> SysExchange(OrganisationViewModel model)
        {
            Organisation org = _organisationService.GetOrganisationByEmail(model.Email).Result;            
            User user = _userService.GetUserByEmail(model.Email).Result;

            return Redirect("~/Admin/Index");

        }

    }
}
