using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Xml.Linq;
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.ControlModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic;
using ServiceStack;
using System.Threading;
using System.Threading.Tasks;
using TechCertain.WebUI.Helpers;

namespace TechCertain.WebUI.Controllers
{

    public class ServicesController : BaseController
    {
        IClientInformationService _clientInformationService;
        IClientAgreementService _clientAgreementService;
        IMapperSession<Vehicle> _vehicleRepository;
        IMapperSession<OrganisationalUnit> _organisationalUnitRepository;
        IMapperSession<Location> _locationRepository;
        IMapperSession<WaterLocation> _waterLocationRepository;
        IMapperSession<Boat> _boatRepository;
        IMapperSession<BoatUse> _boatUseRepository;
        IMapperSession<User> _userRepository;
        IVehicleService _vehicleService;
        IOrganisationService _organisationService;
        IBoatUseService _boatUseService;
        IMapperSession<Building> _buildingRepository;
        IMapperSession<InsuranceAttribute> _InsuranceAttributesRepository;
        IMapperSession<BusinessInterruption> _businessInterruptionRepository;
        IMapperSession<MaterialDamage> _materialDamageRepository;
        IMapperSession<ClaimNotification> _claimRepository;
        IMapperSession<Product> _productRepository;
        IProgrammeService _programmeService;
        IOrganisationTypeService _organisationTypeService;
        IUnitOfWork _unitOfWork;
        IMapperSession<Organisation> _OrganisationRepository;
        IReferenceService _referenceService;
        IEmailService _emailService;
        IAppSettingService _appSettingService;
        IInsuranceAttributeService _insuranceAttributeService;
        IMapperSession<BusinessContract> _businessContractRepository;
        IMapper _mapper;


        public ServicesController(
            IUserService userService, 
            IClientAgreementService clientAgreementService, 
            IAppSettingService appSettingService, 
            IMapperSession<User> userRepository, 
            IClientInformationService clientInformationService, 
            IMapperSession<Vehicle> vehicleRepository, 
            IMapperSession<BoatUse> boatUseRepository,
            IMapperSession<OrganisationalUnit> organisationalUnitRepository, 
            IMapperSession<InsuranceAttribute> insuranceAttributesRepository, 
            IMapperSession<Location> locationRepository, 
            IMapperSession<WaterLocation> waterLocationRepository, 
            IMapperSession<Building> buildingRepository, 
            IMapperSession<BusinessInterruption> businessInterruptionRepository,
            IMapperSession<MaterialDamage> materialDamageRepository, 
            IMapperSession<ClaimNotification> claimRepository, 
            IMapperSession<Product> productRepository, 
            IVehicleService vehicleService, 
            IMapperSession<Boat> boatRepository,
            IOrganisationService organisationService, 
            IBoatUseService boatUseService, 
            IProgrammeService programeService, 
            IOrganisationTypeService organisationTypeService, 
            IMapperSession<BusinessContract> businessContractRepository,
            IMapperSession<Organisation> OrganisationRepository, 
            IEmailService emailService, 
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IInsuranceAttributeService insuranceAttributeService, 
            IReferenceService referenceService
            )

            : base(userService)
        {
            _clientAgreementService = clientAgreementService;
            _appSettingService = appSettingService;
            _userRepository = userRepository;
            _clientInformationService = clientInformationService;
            _vehicleRepository = vehicleRepository;
            _organisationalUnitRepository = organisationalUnitRepository;
            _InsuranceAttributesRepository = insuranceAttributesRepository;
            _vehicleService = vehicleService;
            _locationRepository = locationRepository;
            _waterLocationRepository = waterLocationRepository;
            _boatRepository = boatRepository;
            _boatUseRepository = boatUseRepository;
            _organisationService = organisationService;
            _boatUseService = boatUseService;
            _buildingRepository = buildingRepository;
            _businessInterruptionRepository = businessInterruptionRepository;
            _materialDamageRepository = materialDamageRepository;
            _claimRepository = claimRepository;
            _productRepository = productRepository;
            _programmeService = programeService;
            _organisationTypeService = organisationTypeService;
            _unitOfWork = unitOfWork;
            _OrganisationRepository = OrganisationRepository;
            _referenceService = referenceService;
            _emailService = emailService;
            _mapper = mapper;
            _insuranceAttributeService = insuranceAttributeService;
            _businessContractRepository = businessContractRepository;

        }

        #region Vehicle

        [HttpPost]
        public async Task<IActionResult> SearchVehicle(string registration)
        {
            // TODO - move to web.config or DB
            //string apiKey = "6C2FC149A76FF9F13152D0837A236645D242275E"; // TC Development key
            //string apiKey = "E2EE901D4322EA7F808C99AB7644BA8B1CB71249"; // ICIB Production Key

            VehicleViewModel model = new VehicleViewModel();
            model.Registration = registration;
            model.Make = "Not Found";
            model.VehicleModel = "Not Found";
            model.SumInsured = 5000;

            try
            {
                // Development
                //XmlServiceClient client = new XmlServiceClient ("http://test.carjam.co.nz/api/");
                ////CarJamResponse response = client.Get<CarJamResponse> ("car/?plate=" + registration + "&key=" + apiKey + "&translate=1");
                //CarJamResponse response = null;
                //var http = client.Get ("car/?plate=" + registration + "&key=" + apiKey + "&translate=1");
                //XDocument doc = XDocument.Load (http.GetResponseStream ());
                //if (doc != null) {
                //	XmlSerializer serial;
                //	if (doc.Root.Name == "message") {
                //		serial = new XmlSerializer (typeof (CarJamResponse));
                //		response = (CarJamResponse)serial.Deserialize (doc.Root.CreateReader ());
                //	}
                //	else if (doc.Root.Name == "error")
                //		throw new Exception (doc.Root.Element ("message").Value);
                //}

                // Fixed Test
                //XmlServiceClient client = new XmlServiceClient ("https://dev.carjam.co.nz/");
                //CarJamResponse response = client.Get<CarJamResponse> ("wp-content/uploads/2016/08/cbc192.xml");

                // Production
                //XmlServiceClient client = new XmlServiceClient("https://www.carjam.co.nz/api/");
                //CarJamResponse response = client.Get<CarJamResponse>("car/?plate=" + registration + "&key=" + apiKey + "&translate=1");

                //if (response != null) {
                //	model.Validated = true;
                //	model.Year = response.Details.VehicleDetails.Year;
                //	model.Make = response.Details.VehicleDetails.Make;
                //	model.VehicleModel = response.Details.VehicleDetails.Model;
                //	model.VIN = response.Details.VehicleDetails.VIN;
                //	model.ChassisNumber = response.Details.VehicleDetails.Chassis;
                //	model.EngineNumber = response.Details.VehicleDetails.EngineNo;
                //}

                Vehicle vehicle = _vehicleService.GetValidatedVehicle(registration);
                if (vehicle != null)
                {
                    model.Validated = vehicle.Validated;
                    model.Year = vehicle.Year;
                    model.Make = vehicle.Make;
                    model.VehicleModel = vehicle.Model;
                    model.VIN = vehicle.VIN;
                    model.ChassisNumber = vehicle.ChassisNumber;
                    model.EngineNumber = vehicle.EngineNumber;
                    model.GrossVehicleMass = vehicle.GrossVehicleMass.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Vehicle - No Client information for " + model.AnswerSheetId);

            // get existing vehicle (if any)
            Vehicle vehicle = _vehicleRepository.FindAll().FirstOrDefault(v => v.Id == model.VehicleId);
            // no vehicle, so create new
            if (vehicle == null)
                vehicle = model.ToEntity(user);
            model.UpdateEntity(vehicle);

            //	vehicle = new Vehicle (CurrentUser(), model.Registration, model.Make, model.VehicleModel);
            //// update properties
            //vehicle.Make = model.Make;
            //vehicle.Model = model.VehicleModel;
            //vehicle.Year = model.Year;
            //vehicle.GroupSumInsured = model.SumInsured;
            //vehicle.FleetNumber = model.FleetNumber;
            //vehicle.SerialNumber = model.SerialNumber;
            //vehicle.AreaOfOperation = model.AreaOfOperation;
            //vehicle.VehicleType = model.VehicleType;
            //vehicle.UseType = model.Use;
            //vehicle.SubUseType = model.SubUse;
            //vehicle.VIN = model.VIN;
            //vehicle.ChassisNumber = model.ChassisNumber;
            //vehicle.EngineNumber = model.EngineNumber;
            //vehicle.GrossVehicleMass = Convert.ToInt32 (model.GrossVehicleMass);
            //vehicle.SerialNumber = model.SerialNumber;
            //vehicle.Validated = model.Validated;
            //vehicle.Notes = model.Notes;
            if (model.VehicleLocation != Guid.Empty)
                vehicle.GarageLocation = await _locationRepository.GetByIdAsync(model.VehicleLocation);

            var allOrganisations = await _organisationService.GetAllOrganisations();
            if (model.InterestedParties != null)
                vehicle.InterestedParties = allOrganisations.Where(org => model.InterestedParties.Contains(org.Id)).ToList();

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Vehicles.Add(vehicle);
                await uow.Commit();
            }

            model.VehicleId = vehicle.Id;


            return Json(model);
            //return Json ("Success");
        }

        [HttpPost]
        public async Task<IActionResult> GetVehicle(Guid answerSheetId, Guid vehicleId)
        {
            VehicleViewModel model = new VehicleViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Vehicle vehicle = sheet.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle != null)
            {
                model = VehicleViewModel.FromEntity(vehicle);
                model.AnswerSheetId = answerSheetId;

                //model.Validated = vehicle.Validated;
                //model.VehicleId = vehicleId;
                //model.Registration = vehicle.Registration;
                //model.Year = vehicle.Year;
                //model.Make = vehicle.Make;
                //model.VehicleModel = vehicle.Model;
                //model.VIN = vehicle.VIN;
                //model.ChassisNumber = vehicle.ChassisNumber;
                //model.EngineNumber = vehicle.EngineNumber;
                //model.GrossVehicleMass = vehicle.GrossVehicleMass.ToString ();
                //model.SumInsured = vehicle.GroupSumInsured;
                //model.FleetNumber = vehicle.FleetNumber;
                //model.SerialNumber = vehicle.SerialNumber;
                //model.AreaOfOperation = vehicle.AreaOfOperation;
                //model.VehicleType = vehicle.VehicleType;
                //model.Use = vehicle.UseType;
                //model.SubUse = vehicle.SubUseType;
                //model.InterestedParties = vehicle.InterestedParties.Select(v => v.Id).ToArray();
                //model.Notes = vehicle.Notes;
                if (vehicle.GarageLocation != null)
                    model.VehicleLocation = vehicle.GarageLocation.Id;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicles(Guid informationId, bool validated, bool removed, bool transfered, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var vehicles = new List<Vehicle>();

            if (transfered)
            {
                vehicles = sheet.Vehicles.Where(v => v.Validated == validated && v.Removed == removed && v.DateDeleted == null && v.VehicleCeaseDate > DateTime.MinValue && v.VehicleCeaseReason == 4).ToList();
            }
            else
            {
                vehicles = sheet.Vehicles.Where(v => v.Validated == validated && v.Removed == removed && v.DateDeleted == null).ToList();
            }

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        vehicles = vehicles.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        vehicles = vehicles.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        vehicles = vehicles.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //vehicles = vehicles.OrderBy(sidx + " " + sord).ToList();
            vehicles = vehicles.ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = vehicles.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                Vehicle vehicle = vehicles[i];
                JqGridRow row = new JqGridRow(vehicle.Id);
                if (vehicle.Validated)
                {
                    row.AddValues(vehicle.Id, vehicle.Year, vehicle.Registration, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured.ToString("C", UserCulture), vehicle.Id);
                }
                else
                {
                    if (sheet.Programme.BaseProgramme.Products.First().Id == new Guid("e2eae6d8-d68e-4a40-b50a-f200f393777a")) // Marsh Coastguard
                    {
                        row.AddValues(vehicle.Id, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured.ToString("C", UserCulture), vehicle.Id);
                    }
                    else
                    {
                        row.AddValues(vehicle.Id, vehicle.Year, vehicle.FleetNumber, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured.ToString("C", UserCulture), vehicle.Id);
                    }

                }


                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }


        [HttpGet]
        public async Task<IActionResult> GetPrincipalPartners(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();


            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var organisations = new List<Organisation>();
            foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Where(ia => ia.InsuranceAttributeName == "Principal" || ia.InsuranceAttributeName == "Subsidiary"
                                                                                                || ia.InsuranceAttributeName == "PreviousConsultingBusiness" || ia.InsuranceAttributeName == "JointVenture"
                                                                                                || ia.InsuranceAttributeName == "Mergers"))
            {
                foreach (var org in IA.IAOrganisations)
                {
                    foreach (var organisation in sheet.Organisation.Where(o => o.Id == org.Id && o.Removed != true))
                    {
                        organisations.Add(organisation);
                    }

                }
            }
            //for (var i = 0; i < sheet.Organisation.Where(o => o.OrganisationType.Name == "Person - Individual").Count(); i++)
            //{
            //    organisations.Add(sheet.Organisation.ElementAtOrDefault(i));
            //}


            try
            {

                if (_search)
                {
                    switch (searchOper)
                    {
                        case "eq":
                            organisations = organisations.Where(searchField + " = \"" + searchString + "\"").ToList();
                            break;
                        case "bw":
                            organisations = organisations.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                            break;
                        case "cn":
                            organisations = organisations.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                            break;
                    }
                }
                //organisations = organisations.OrderBy(sidx + " " + sord).ToList();
                model.Page = page;
                model.TotalRecords = organisations.Count;
                model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;
                JqGridRow row1 = new JqGridRow(sheet.Owner.Id);
                row1.AddValues(sheet.Owner.Id, sheet.Owner.Name, "Owner");
                model.AddRow(row1);
                int offset = rows * (page - 1);
                for (int i = offset; i < offset + rows; i++)
                {
                    if (i == model.TotalRecords)
                        break;
                    Organisation organisation = organisations[i];
                    JqGridRow row = new JqGridRow(organisation.Id);

                    for (int x = 0; x < organisation.InsuranceAttributes.Count; x++)
                    {
                        row.AddValues(organisation.Id, organisation.Name, organisation.InsuranceAttributes[x].InsuranceAttributeName, organisation.Id);
                    }
                    model.AddRow(row);
                }


                //// convert model to XDocument for rendering.
                //document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                document = model.ToXml();
                return Xml(document);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeletedPartners(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                       string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();

            User userdb = _userRepository.FindAll().FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var organisations = new List<Organisation>();
           
                        foreach ( var org in sheet.Organisation.Where(o => o.Removed == true))
                        {
                            organisations.Add(org);
                        }
                  

           
           
            try
            {

                if (_search)
                {
                    switch (searchOper)
                    {
                        case "eq":
                            organisations = organisations.Where(searchField + " = \"" + searchString + "\"").ToList();
                            break;
                        case "bw":
                            organisations = organisations.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                            break;
                        case "cn":
                            organisations = organisations.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                            break;
                    }
                }
                //organisations = organisations.OrderBy(sidx + " " + sord).ToList();
                model.Page = page;
                model.TotalRecords = organisations.Count;
                model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;
                int offset = rows * (page - 1);
                for (int i = offset; i < offset + rows; i++)
                {
                    if (i == model.TotalRecords)
                        break;
                    Organisation organisation = organisations[i];
                    JqGridRow row = new JqGridRow(organisation.Id);

                    for (int x = 0; x < organisation.InsuranceAttributes.Count; x++)
                    {
                        row.AddValues(organisation.Id, organisation.Name, organisation.InsuranceAttributes[x].InsuranceAttributeName, organisation.Id);
                    }
                    model.AddRow(row);
                }


                //// convert model to XDocument for rendering.
                //document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                document = model.ToXml();
                return Xml(document);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetNamedParties(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();


            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var organisations = new List<Organisation>();
            for (var i = 0; i < sheet.Organisation.Count(); i++)
            {
                organisations.Add(sheet.Organisation.ElementAtOrDefault(i));
            }


            try
            {

                if (_search)
                {
                    switch (searchOper)
                    {
                        case "eq":
                            organisations = organisations.Where(searchField + " = \"" + searchString + "\"").ToList();
                            break;
                        case "bw":
                            organisations = organisations.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                            break;
                        case "cn":
                            organisations = organisations.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                            break;
                    }
                }
                //organisations = organisations.OrderBy(sidx + " " + sord).ToList();
                model.Page = page;
                model.TotalRecords = organisations.Count;
                model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;
                JqGridRow row1 = new JqGridRow(sheet.Owner.Id);
                row1.AddValues(sheet.Owner.Id, sheet.Owner.Name, "Owner", sheet.Owner.Id);
                model.AddRow(row1);
                int offset = rows * (page - 1);
                for (int i = offset; i < offset + rows; i++)
                {
                    if (i == model.TotalRecords)
                        break;
                    Organisation organisation = organisations[i];
                    JqGridRow row = new JqGridRow(organisation.Id);

                    for (int x = 0; x < organisation.InsuranceAttributes.Count; x++)
                    {
                        row.AddValues(organisation.Id, organisation.Name, organisation.InsuranceAttributes[x].InsuranceAttributeName, organisation.Id);
                    }
                    model.AddRow(row);
                }


                //// convert model to XDocument for rendering.
                //document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                document = model.ToXml();
                return Xml(document);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehiclePlates(string term)
        {
            var plateList = _vehicleRepository.FindAll().Select(v => v.Registration);
            var results = plateList.Where(n => n.ToLower().Contains(term.ToLower()));
            throw new Exception("Method needs to be re-written");
            //return new System.Web.Mvc.JsonResult()
            //{
            //    Data = results.ToArray(),
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};
        }

        [HttpPost]
        public async Task<IActionResult> SetVehicleRemovedStatus(Guid vehicleId, bool status)
        {
            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                vehicle.Removed = status;
                await uow.Commit();
            }
            //return new JsonResult(true);
            return new JsonResult(new { status = true, id = vehicleId });
        }

        [HttpPost]
        public async Task<IActionResult> SetVehicleCeasedStatus(Guid vehicleId, bool status, DateTime ceaseDate, int ceaseReason)
        {
            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                vehicle.VehicleCeaseDate = DateTime.Parse(LocalizeTime(ceaseDate, "d"));
                vehicle.VehicleCeaseReason = ceaseReason;
                await uow.Commit().ConfigureAwait(false);
            }

            return new JsonResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> SetVehicleTransferedStatus(Guid vehicleId, bool status)
        {
            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                vehicle.VehicleCeaseDate = DateTime.MinValue;
                vehicle.VehicleCeaseReason = '0';
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> RevalidateVehicle(Guid vehicleId)
        {
            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                throw new Exception("Vehicle is null");
            if (vehicle.Validated == false)
            {
                if (string.IsNullOrWhiteSpace(vehicle.Registration))
                    throw new Exception("Unable to revalidate a non registered & validated vehicle");
            }


            return null;
        }

        #endregion

        #region Organisational Units

        [HttpPost]
        public async Task<IActionResult> SearchOrganisationalUnit(Guid answerSheetId, string name)
        {
            OrganisationalUnitViewModel model = new OrganisationalUnitViewModel();
            model.Name = name;
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            OrganisationalUnit unit = sheet.Owner.OrganisationalUnits.FirstOrDefault(ou => ou.Name == name);
            if (unit != null)
            {
                model.Name = unit.Name;
                model.OrganisationId = sheet.Owner.Id;
                model.OrganisationalUnitId = unit.Id;
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganisationalUnit(OrganisationalUnitViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Organisational Unit - No Client information for " + model.AnswerSheetId);

            OrganisationalUnit ou = await _organisationalUnitRepository.GetByIdAsync(model.OrganisationalUnitId);
            if (ou == null)
                ou = new OrganisationalUnit(user, model.Name);
            ou.Name = model.Name;

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Owner.OrganisationalUnits.Add(ou);
                await uow.Commit();
            }
            model.OrganisationalUnitId = ou.Id;

            return Json(model);
            //return Json("Success");
        }

        [HttpPost]
        public async Task<IActionResult> GetOrganisationalUnit(Guid answerSheetId, Guid unitId)
        {
            OrganisationalUnitViewModel model = new OrganisationalUnitViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            OrganisationalUnit unit = sheet.Owner.OrganisationalUnits.FirstOrDefault(ou => ou.Id == unitId);
            if (unit != null)
            {
                model.Name = unit.Name;
                model.OrganisationId = sheet.Owner.Id;
                model.OrganisationalUnitId = unit.Id;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisationalUnits(Guid informationId, bool _search, string nd, int rows, int page, string sidx, string sord,
                                                     string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var organisationalUnits = sheet.Owner.OrganisationalUnits;


            if (_search)
            {

                switch (searchOper)
                {
                    case "eq":
                        organisationalUnits = organisationalUnits.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        organisationalUnits = organisationalUnits.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        organisationalUnits = organisationalUnits.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //organisationalUnits = organisationalUnits.OrderBy(sidx + " " + sord).ToList();


            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = 1;
            model.TotalRecords = organisationalUnits.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                OrganisationalUnit ou = organisationalUnits[i];
                JqGridRow row = new JqGridRow(ou.Id);
                row.AddValues(ou.Id, ou.Name);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);

        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisationalUnitName(string term)
        {
            var organisationalUnitNameList = _organisationalUnitRepository.FindAll().Select(ou => ou.Name);
            var results = organisationalUnitNameList.Where(n => n.ToLower().Contains(term.ToLower()));

            return new JsonResult(results.ToArray());
            //{
            //    Data = results.ToArray(),
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};
        }

        #endregion

        #region Locations

        [HttpPost]
        public async Task<IActionResult> SearchLocationStreet(Guid answerSheetId, string street)
        {
            LocationViewModel model = new LocationViewModel();
            model.Street = street;
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Location location = _locationRepository.FindAll().FirstOrDefault(l => l.Street == street);
            if (location != null)
            {
                model = LocationViewModel.FromEntity(location);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Location - No Client information for " + model.AnswerSheetId);

            Location location = _locationRepository.FindAll().FirstOrDefault(loc => loc.Id == model.LocationId);
            if (location == null)
                location = model.ToEntity(user);
            model.UpdateEntity(location);
            var OUList = new List<OrganisationalUnit>();

            if (sheet.Owner.OrganisationalUnits.Count() > 0)
                OUList.Add(sheet.Owner.OrganisationalUnits.ElementAtOrDefault(0));

            location.OrganisationalUnits = OUList;
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Locations.Add(location);
                await uow.Commit();
            }

            model.LocationId = location.Id;

            return Json(model);
            //return Json("Success");
        }

        [HttpPost]
        public async Task<IActionResult> GetLocation(Guid answerSheetId, Guid locationId)
        {
            LocationViewModel model = new LocationViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Location location = sheet.Locations.FirstOrDefault(loc => loc.Id == locationId);
            if (location != null)
            {
                model = LocationViewModel.FromEntity(location);
                model.AnswerSheetId = answerSheetId;
                //model.SelectedOrganisationalUnits = location.OrganisationalUnits.Select(ou => ou.Id).ToArray();
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var locations = sheet.Owner.OrganisationalUnits.SelectMany(ou => ou.Locations).Distinct().ToList();

            locations = sheet.Locations.Where(loc => loc.Removed == removed && loc.DateDeleted == null).ToList();

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        locations = locations.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        locations = locations.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        locations = locations.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //locations = locations.OrderBy(sidx + " " + sord).ToList();
            locations = locations.ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = 1;
            model.TotalRecords = locations.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                Location location = locations[i];
                JqGridRow row = new JqGridRow(location.Id);
                row.AddValues(location.Id, location.LocationType, location.CommonName, location.Street, location.Suburb, location.Postcode, location.City, location.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering
            document = model.ToXml();
            return Xml(document);

        }


        [HttpGet]
        public async Task<IActionResult> GetLocationss(Guid informationId, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var locations = sheet.Owner.OrganisationalUnits.SelectMany(ou => ou.Locations).Distinct().ToList();

            locations = sheet.Locations.Where(loc => loc.Removed == false && loc.DateDeleted == null).ToList();


            //throw new Exception("Method needs to be re-written");
            //locations = locations.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = 1;
            model.TotalRecords = locations.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                Location location = locations[i];
                JqGridRow row = new JqGridRow(location.Id);
                row.AddValues(location.Id, location.LocationType, location.CommonName, location.Street, location.Suburb, location.Postcode, location.City, location.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering
            document = model.ToXml();
            return Xml(document);

        }

        [HttpGet]
        public async Task<IActionResult> GetLocationStreet(string term)
        {
            var locationStreetList = _locationRepository.FindAll().Select(l => l.Street);
            var results = locationStreetList.Where(n => n.ToLower().Contains(term.ToLower()));
            return new JsonResult(results.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationList(Guid answerSheetId)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);

            List<LocationViewModel> models = new List<LocationViewModel>();
            foreach (var location in sheet.Locations)
                models.Add(LocationViewModel.FromEntity(location));

            return new JsonResult(models.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationsByCountry(Guid answerSheetId)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);

            List<Location> countries = sheet.Locations.GroupBy(loc => loc.Country)
                                          .Select(grp => grp.First())
                                          .ToList();

            List<LocationViewModel> models = new List<LocationViewModel>();
            foreach (var location in countries)
                models.Add(LocationViewModel.FromEntity(location));

            return new JsonResult(models.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> SetLocationRemovedStatus(Guid locationId, bool status)
        {
            Location location = await _locationRepository.GetByIdAsync(locationId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                location.Removed = status;
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> SetPrincipalRemovedStatus(Guid answersheetId, Guid principalId, bool status)
        {
            Organisation org = await _OrganisationRepository.GetByIdAsync(principalId);
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answersheetId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                if (org != null && answersheetId != null)
                {
                    org.Removed = status;
                }
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Buildings

        [HttpPost]
        public async Task<IActionResult> AddBuilding(BuildingViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Building - No Client information for " + model.AnswerSheetId);

            Building building = _buildingRepository.FindAll().FirstOrDefault(bui => bui.Id == model.BuildingId);
            if (building == null)
                building = model.ToEntity(user);
            model.UpdateEntity(building);

            if (model.BuildingLocation != null)
                building.Location = await _locationRepository.GetByIdAsync(model.BuildingLocation);

            if (model.InterestedParties != null)
            {
                var getAllOrganisations = await _organisationService.GetAllOrganisations();
                building.InterestedParties = getAllOrganisations.Where(org => model.InterestedParties.Contains(org.Id)).ToList();
            }

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Buildings.Add(building);
                await uow.Commit();
            }

            model.LocationStreet = building.Location.Street;
            model.BuildingId = building.Id;


            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetBuilding(Guid answerSheetId, Guid buildingId)
        {
            BuildingViewModel model = new BuildingViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Building building = sheet.Buildings.FirstOrDefault(b => b.Id == buildingId);
            if (building != null)
            {
                model = BuildingViewModel.FromEntity(building);
                model.AnswerSheetId = answerSheetId;

                if (building.Location != null)
                    model.BuildingLocation = building.Location.Id;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBuildings(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var buildings = new List<Building>();

            buildings = sheet.Buildings.Where(b => b.Removed == removed && b.DateDeleted == null).ToList();

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        buildings = buildings.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        buildings = buildings.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        buildings = buildings.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //buildings = buildings.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = buildings.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                Building building = buildings[i];
                JqGridRow row = new JqGridRow(building.Id);
                row.AddValues(building.Id, building.BuildingName, building.BuildingCategory, building.Location.Street, building.Location.Suburb, building.Location.City, building.Id, building.BuildingCategory);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public async Task<IActionResult> SetBuildingRemovedStatus(Guid buildingId, bool status)
        {
            Building building = await _buildingRepository.GetByIdAsync(buildingId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                building.Removed = status;
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region WaterLocations

        [HttpPost]
        public async Task<IActionResult> SearchWaterLocationName(Guid answerSheetId, string waterLocationName)
        {
            WaterLocationViewModel model = new WaterLocationViewModel();
            model.WaterLocationName = waterLocationName;
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            WaterLocation waterLocation = _waterLocationRepository.FindAll().FirstOrDefault(wl => wl.WaterLocationName == waterLocationName);
            if (waterLocation != null)
            {
                model = WaterLocationViewModel.FromEntity(waterLocation);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddWaterLocation(WaterLocationViewModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                var user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Water Location - No Client information for " + model.AnswerSheetId);

                WaterLocation waterLocation = _waterLocationRepository.FindAll().FirstOrDefault(wloc => wloc.Id == model.WaterLocationId);
                if (waterLocation == null)
                    waterLocation = model.ToEntity(user);
                model.UpdateEntity(waterLocation);

                if (model.WaterLocationLocation != Guid.Empty)
                    waterLocation.WaterLocationLocation = await _locationRepository.GetByIdAsync(model.WaterLocationLocation);
                if (model.WaterLocationMarinaLocation != null)
                {
                    waterLocation.WaterLocationMarinaLocation = await _OrganisationRepository.GetByIdAsync(model.WaterLocationMarinaLocation);

                }
                if (model.OrganisationalUnit != null)
                {
                    waterLocation.OrganisationalUnit = await _organisationalUnitRepository.GetByIdAsync(model.OrganisationalUnit);

                }
                // waterLocation.OrganisationalUnit = OrganisationalUnit;

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.WaterLocations.Add(waterLocation);
                    await uow.Commit();
                }

                model.WaterLocationId = waterLocation.Id;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetWaterLocation(Guid answerSheetId, Guid waterLocationId)
        {
            WaterLocationViewModel model = new WaterLocationViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            WaterLocation waterLocation = sheet.WaterLocations.FirstOrDefault(wloc => wloc.Id == waterLocationId);
            if (waterLocation != null)
            {
                model = WaterLocationViewModel.FromEntity(waterLocation);
                model.AnswerSheetId = answerSheetId;

                if (waterLocation.WaterLocationLocation != null)
                    model.WaterLocationLocation = waterLocation.WaterLocationLocation.Id;
                if (waterLocation.WaterLocationMarinaLocation != null)
                    model.WaterLocationMarinaLocation = waterLocation.WaterLocationMarinaLocation.Id;
                if (waterLocation.OrganisationalUnit != null)
                    model.OrganisationalUnit = waterLocation.OrganisationalUnit.Id;


            }

            Organisation organisation = null;

            organisation = await _organisationService.GetOrganisation(waterLocation.WaterLocationMarinaLocation.Id);
            var organisationalUnits = new List<OrganisationalUnitViewModel>();

            foreach (OrganisationalUnit ou in organisation.OrganisationalUnits)
            {
                organisationalUnits.Add(new OrganisationalUnitViewModel
                {
                    OrganisationalUnitId = ou.Id,
                    Name = ou.Name
                });
            }

            model.LOrganisationalUnits = organisationalUnits;

            var Locations = new List<LocationViewModel>();

            foreach (Location loc in waterLocation.OrganisationalUnit.Locations)
            {
                Locations.Add(new LocationViewModel
                {
                    LocationId = loc.Id,
                    Street = loc.Street
                });
            }

            model.lLocation = Locations;


            //OrganisationalUnit = waterLocation.OrganisationalUnit,


            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaterLocations(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var waterLocations = new List<WaterLocation>();

            waterLocations = sheet.WaterLocations.Where(wl => wl.Removed == removed && wl.DateDeleted == null).ToList();

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        waterLocations = waterLocations.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        waterLocations = waterLocations.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        waterLocations = waterLocations.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //waterLocations = waterLocations.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = waterLocations.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                WaterLocation waterLocation = waterLocations[i];
                JqGridRow row = new JqGridRow(waterLocation.Id);
                if (waterLocation.WaterLocationMarinaLocation != null)
                {
                    row.AddValues(waterLocation.Id, waterLocation.WaterLocationName, waterLocation.WaterLocationMarinaLocation.Name, waterLocation.WaterLocationMooringType, waterLocation.Id);
                }
                else
                {
                    row.AddValues(waterLocation.Id, waterLocation.WaterLocationName, " ", waterLocation.WaterLocationMooringType, waterLocation.Id);

                }
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);

        }

        [HttpGet]
        public async Task<IActionResult> GetWaterLocationName(string term)
        {
            var waterLocationNameList = _waterLocationRepository.FindAll().Select(wl => wl.WaterLocationName);
            var results = waterLocationNameList.Where(n => n.ToLower().Contains(term.ToLower()));
            return new JsonResult(results.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> GetWaterLocationList(Guid answerSheetId)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);

            List<WaterLocationViewModel> models = new List<WaterLocationViewModel>();
            foreach (var waterLocation in sheet.WaterLocations)
                models.Add(WaterLocationViewModel.FromEntity(waterLocation));

            return new JsonResult(models.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> SetWaterLocationRemovedStatus(Guid waterLocationId, bool status)
        {
            WaterLocation waterLocation = await _waterLocationRepository.GetByIdAsync(waterLocationId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                waterLocation.Removed = status;
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region BusinessInterruption

        [HttpPost]
        public async Task<IActionResult> AddBusinessInterruption(BusinessInterruptionViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save BusinessInterruption - No Client information for " + model.AnswerSheetId);

            BusinessInterruption businessInterruption = _businessInterruptionRepository.FindAll().FirstOrDefault(bi => bi.Id == model.BusinessInterruptionId);
            if (businessInterruption == null)
                businessInterruption = model.ToEntity(user);
            model.UpdateEntity(businessInterruption);

            if (model.BusinessInterruptionLocation != null)
                businessInterruption.Location = await _locationRepository.GetByIdAsync(model.BusinessInterruptionLocation);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.BusinessInterruptions.Add(businessInterruption);
                await uow.Commit();
            }

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetBusinessInterruption(Guid answerSheetId, Guid businessInterruptionId)
        {
            BusinessInterruptionViewModel model = new BusinessInterruptionViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            BusinessInterruption businessInterruption = sheet.BusinessInterruptions.FirstOrDefault(bi => bi.Id == businessInterruptionId);
            if (businessInterruption != null)
            {
                model = BusinessInterruptionViewModel.FromEntity(businessInterruption);
                model.AnswerSheetId = answerSheetId;

                if (businessInterruption.Location != null)
                    model.BusinessInterruptionLocation = businessInterruption.Location.Id;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessInterruptions(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var businessInterruptions = new List<BusinessInterruption>();

            businessInterruptions = sheet.BusinessInterruptions.Where(bi => bi.Removed == removed && bi.DateDeleted == null).ToList();

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        businessInterruptions = businessInterruptions.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        businessInterruptions = businessInterruptions.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        businessInterruptions = businessInterruptions.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //businessInterruptions = businessInterruptions.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = businessInterruptions.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                BusinessInterruption businessInterruption = businessInterruptions[i];
                JqGridRow row = new JqGridRow(businessInterruption.Id);
                row.AddValues(businessInterruption.IndemnityPeriod, businessInterruption.Location.CommonName, businessInterruption.Location.Street, businessInterruption.Location.Suburb, businessInterruption.Location.Postcode,
                    businessInterruption.Location.City, businessInterruption.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public async Task<IActionResult> SetBusinessInterruptionRemovedStatus(Guid businessInterruptionId, bool status)
        {
            BusinessInterruption businessInterruption = await _businessInterruptionRepository.GetByIdAsync(businessInterruptionId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                businessInterruption.Removed = status;
                await uow.Commit();
            }


            return new JsonResult(true);
        }

        #endregion

        #region MaterialDamage

        [HttpPost]
        public async Task<IActionResult> AddMaterialDamage(MaterialDamageViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save BusinessInterruption - No Client information for " + model.AnswerSheetId);

            MaterialDamage materialDamage = _materialDamageRepository.FindAll().FirstOrDefault(md => md.Id == model.MaterialDamageId);
            if (materialDamage == null)
                materialDamage = model.ToEntity(user);
            model.UpdateEntity(materialDamage);

            if (model.MaterialDamageLocation != null)
                materialDamage.Location = await _locationRepository.GetByIdAsync(model.MaterialDamageLocation);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.MaterialDamages.Add(materialDamage);
                await uow.Commit();
            }

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetMaterialDamage(Guid answerSheetId, Guid materialDamageId)
        {
            MaterialDamageViewModel model = new MaterialDamageViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            MaterialDamage materialDamage = sheet.MaterialDamages.FirstOrDefault(md => md.Id == materialDamageId);
            if (materialDamage != null)
            {
                model = MaterialDamageViewModel.FromEntity(materialDamage);
                model.AnswerSheetId = answerSheetId;

                if (materialDamage.Location != null)
                    model.MaterialDamageLocation = materialDamage.Location.Id;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialDamages(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var materialDamages = new List<MaterialDamage>();

            materialDamages = sheet.MaterialDamages.Where(md => md.Removed == removed && md.DateDeleted == null).ToList();

            if (_search)
            {

                switch (searchOper)
                {
                    case "eq":
                        materialDamages = materialDamages.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        materialDamages = materialDamages.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        materialDamages = materialDamages.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //materialDamages = materialDamages.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = materialDamages.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                MaterialDamage materialDamage = materialDamages[i];
                JqGridRow row = new JqGridRow(materialDamage.Id);
                row.AddValues(materialDamage.NonHirePlant, materialDamage.Location.CommonName, materialDamage.Location.Street, materialDamage.Location.Suburb, materialDamage.Location.Postcode,
                    materialDamage.Location.City, materialDamage.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public async Task<IActionResult> SetMaterialDamageRemovedStatus(Guid materialDamageId, bool status)
        {
            MaterialDamage materialDamage = await _materialDamageRepository.GetByIdAsync(materialDamageId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                materialDamage.Removed = status;
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Boat

        [HttpPost]
        public async Task<IActionResult> AddUsetoBoat(string[] Boatuse, Guid BoatId)
        {
            Boat boat = _boatRepository.FindAll().FirstOrDefault(b => b.Id == BoatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                List<string> boatuselist = new List<string>();

                boat.BoatUse = new List<BoatUse>();
                foreach (var useid in Boatuse)
                {
                    //ListBoatUse.Add(useid);
                    //ListBoatUse.Add(_boatUseService.GetBoatUse(Guid.Parse(useid)));
                    boat.BoatUse.Add(await _boatUseService.GetBoatUse(Guid.Parse(useid)));
                }
                await uow.Commit();
            }
            return Json(boat);

        }


        //[HttpPost]
        //public async Task<IActionResult> AddBoat(BoatViewModel model)
        //{
        //    if (model == null)
        //        throw new ArgumentNullException(nameof(model));

        //    ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
        //    if (sheet == null)
        //        throw new Exception("Unable to save Boat - No Client information for " + model.AnswerSheetId);

        //    // get existing boat (if any)
        //    Boat boat = _boatRepository.Repository.FindAll().FirstOrDefault(b => b.Id == model.BoatId);
        //    // no boat, so create new
        //    try
        //    {
        //        if (boat == null)
        //        boat = model.ToEntity(CurrentUser());
        //    model.UpdateEntity(boat);
        //    if (model.BoatLandLocation != Guid.Empty)
        //        boat.BoatLandLocation = _buildingRepository.GetById(model.BoatLandLocation);
        //    if (model.BoatWaterLocation != Guid.Empty)
        //        boat.BoatWaterLocation = _waterLocationRepository.GetById(model.BoatWaterLocation);
        //    if (model.InterestedParties != null)
        //        boat.InterestedParties = _organisationService.GetAllOrganisations().Where(org => model.InterestedParties.Contains(org.Id)).ToList();

        //    if (model.SelectedBoatUse != null)
        //    {
        //        try
        //        {
        //            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
        //            {
        //                List<string> boatuselist = new List<string>();

        //                boat.BoatUse = new List<BoatUse>();

        //                string strArray = model.SelectedBoatUse.Substring(0, model.SelectedBoatUse.Length - 1);
        //                string[] BoatUse = strArray.Split(',');

        //                model.BoatUse = new List<BoatUse>();

        //                foreach (var useid in BoatUse)
        //                {

        //                    model.BoatUse.Add(_boatUseService.GetBoatUse(Guid.Parse(useid)));

        //                }
        //                boat.BoatUse = model.BoatUse;
        //                uow.Commit();

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }

        //    if (model.BoatTrailer != Guid.Empty)
        //        boat.BoatTrailer = _vehicleRepository.GetById(model.BoatTrailer);
        //    if (model.BoatOperator != Guid.Empty)
        //        boat.BoatOperator = _operatorRepository.GetById(model.BoatOperator);

        //        using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
        //        {

        //            sheet.Boats.Add(boat);
        //            uow.Commit();
        //        }
        //    }catch(Exception rx)
        //    {
        //        Console.WriteLine(rx.Message);
        //    }
        //    return Json(model);
        //}

        [HttpPost]
        public async Task<IActionResult> AddBoat(BoatViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat - No Client information for " + model.AnswerSheetId);
            var user = await CurrentUser();
            // get existing boat (if any)
            Boat boat = _boatRepository.FindAll().FirstOrDefault(b => b.Id == model.BoatId);
            // no boat, so create new
            if (boat == null)
                boat = model.ToEntity(user);
            model.UpdateEntity(boat);
            if (model.BoatLandLocation != Guid.Empty)
                boat.BoatLandLocation = await _buildingRepository.GetByIdAsync(model.BoatLandLocation);
            //if (model.BoatWaterLocation != Guid.Empty)
            //    boat.BoatWaterLocation = _waterLocationRepository.GetById(model.BoatWaterLocation);
            //if (model.InterestedParties != null)
            //    boat.InterestedParties = _organisationService.GetAllOrganisations().Where(org => model.InterestedParties.Contains(org.Id)).ToList();
            if (model.BoatOperator != Guid.Empty)
                boat.BoatOperator = await _OrganisationRepository.GetByIdAsync(model.BoatOperator);
            boat.BoatWaterLocation = null;

            if (model.BoatWaterLocation != Guid.Empty)
                boat.BoatWaterLocation = await _OrganisationRepository.GetByIdAsync(model.BoatWaterLocation);

            if (model.OtherMarinaName != null)
            {
                boat.OtherMarinaName = model.OtherMarinaName;
                boat.OtherMarina = true;
            }
            else
            {
                boat.OtherMarina = false;

            }
            if (model.SelectedBoatUse != null || model.SelectedBoatUse != "null")
            {
                try
                {
                    List<string> boatuselist = new List<string>();

                    boat.BoatUse = new List<BoatUse>();

                    //string strArray = model.SelectedBoatUse.Substring(0, model.SelectedBoatUse.Length - 1);
                    string[] BoatUse = model.SelectedBoatUse.Split(',');

                    model.BoatUse = new List<BoatUse>();

                    foreach (var useid in BoatUse)
                    {
                        boat.BoatUse.Add(await _boatUseService.GetBoatUse(Guid.Parse(useid)));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (model.SelectedInterestedParty != null || model.SelectedInterestedParty != "null")
            {
                try
                {
                    List<string> interestedpartylist = new List<string>();

                    boat.InterestedParties = new List<Organisation>();

                    //string strArray = model.SelectedBoatUse.Substring(0, model.SelectedBoatUse.Length - 1);
                    string[] interestedParty = model.SelectedInterestedParty.Split(',');

                    model.InterestedParties = new List<Organisation>();

                    foreach (var useid in interestedParty)
                    {
                        boat.InterestedParties.Add(await _organisationService.GetOrganisation(Guid.Parse(useid)));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (model.BoatTrailer != Guid.Empty)
                boat.BoatTrailer = await _vehicleRepository.GetByIdAsync(model.BoatTrailer);
            //if (model.BoatOperator != Guid.Empty)
            //    boat.BoatOperator = _operatorRepository.GetById(model.BoatOperator);
            try
            {

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    //boat.BoatUse = model.BoatUse;

                    sheet.Boats.Add(boat);
                    await uow.Commit();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }


            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetOriginalVehicle(Guid answerSheetId, Guid vehicleId)
        {
            VehicleViewModel model = new VehicleViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Vehicle vehicle = sheet.Vehicles.FirstOrDefault(b => b.Id == vehicleId);
            if (vehicle != null)
            {
                model.AnswerSheetId = answerSheetId;
                if (vehicle.OriginalVehicle != null)
                    model.OriginalVehicleId = vehicle.OriginalVehicle.Id;
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetOriginalBoat(Guid answerSheetId, Guid boatId)
        {
            BoatViewModel model = new BoatViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Boat boat = sheet.Boats.FirstOrDefault(b => b.Id == boatId);
            if (boat != null)
            {
                model.AnswerSheetId = answerSheetId;
                if (boat.OriginalBoat != null)
                {
                    model.OriginalBoatId = boat.OriginalBoat.Id;
                }
                else
                {
                    model.OriginalBoatId = Guid.Parse("00000000-0000-0000-0000-000000000000");
                }
            }
            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> GetBoat(Guid answerSheetId, Guid boatId)
        {
            BoatViewModel model = new BoatViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Boat boat = sheet.Boats.FirstOrDefault(b => b.Id == boatId);
            if (boat != null)
            {
                model = BoatViewModel.FromEntity(boat);
                model.AnswerSheetId = answerSheetId;
                if (boat.BoatLandLocation != null)
                    model.BoatLandLocation = boat.BoatLandLocation.Id;
                if (boat.BoatWaterLocation != null)
                    model.BoatWaterLocation = boat.BoatWaterLocation.Id;
                if (boat.BoatTrailer != null)
                    if (boat.BoatTrailer != null)
                        model.BoatTrailer = boat.BoatTrailer.Id;

                if (boat.OtherMarinaName != null)
                    model.OtherMarinaName = boat.OtherMarinaName;
                if (boat.BoatUse != null)
                    //model.BoatUse = new List<BoatUse>();
                    model.BoatselectedVal = new List<String>();
                model.BoatselectedText = new List<Guid>();

                try
                {
                    foreach (var boatuse in boat.BoatUse)
                    {
                        //model.BoatUse.Add(_boatUseService.GetBoatUse(boatuse.Id));
                        model.BoatselectedVal.Add(boatuse.BoatUseCategory);
                        model.BoatselectedText.Add(boatuse.Id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            //if (boat.InterestedParties != null)
            //{
            //model.BoatUse = new List<BoatUse>();
            model.BoatpartyVal = new List<String>();
            model.BoatpartyText = new List<Guid>();

            try
            {
                foreach (var boatparty in boat.InterestedParties)
                {
                    //model.BoatUse.Add(_boatUseService.GetBoatUse(boatuse.Id));
                    model.BoatpartyVal.Add(boatparty.Name);
                    model.BoatpartyText.Add(boatparty.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //}
            return Json(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetBoats(Guid informationId, bool validated, bool removed, bool transfered, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;

            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var boats = new List<Boat>();

            if (transfered)
            {
                boats = sheet.Boats.Where(b => b.Removed == removed && b.DateDeleted == null && b.BoatCeaseDate > DateTime.MinValue && b.BoatCeaseReason == 4).ToList();
            }
            else
            {
                boats = sheet.Boats.Where(b => b.Removed == removed && b.DateDeleted == null).ToList();
            }

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        boats = boats.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        boats = boats.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        boats = boats.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }

            //boats = boats.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = boats.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                Boat boat = boats[i];
                JqGridRow row = new JqGridRow(boat.Id);
                //row.AddValue("");
                row.AddValues(boat.Id, boat.BoatName, boat.YearOfManufacture, boat.MaxSumInsured.ToString("C", UserCulture), boat.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatRemovedStatus(Guid boatId, bool status)
        {
            Boat boat = await _boatRepository.GetByIdAsync(boatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boat.Removed = status;
                await uow.Commit();
            }
            return new JsonResult(true);
        }



        [HttpPost]
        public async Task<IActionResult> UndoBoatRemovedStatus(BoatViewModel removedboat)
        {
            Boat boat = await _boatRepository.GetByIdAsync(removedboat.BoatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boat.Removed = removedboat.Removed;
                await uow.Commit();
            }
            return new JsonResult(true);

        }

        [HttpPost]
        public async Task<IActionResult> SetBoatCeasedStatus(Guid boatId, bool status, DateTime ceaseDate, int ceaseReason)
        {
            Boat boat = await _boatRepository.GetByIdAsync(boatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {

                boat.BoatCeaseDate = DateTime.Parse(LocalizeTime(ceaseDate, "d"));

                boat.BoatCeaseReason = ceaseReason;
                await uow.Commit().ConfigureAwait(false);
            }

            return new JsonResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatTransferedStatus(Guid boatId, bool status)
        {
            Boat boat = await _boatRepository.GetByIdAsync(boatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boat.BoatCeaseDate = DateTime.MinValue;
                boat.BoatCeaseReason = '0';
                await uow.Commit().ConfigureAwait(false);
            }

            return new JsonResult(true);
        }

        #endregion

        #region BoatUse

        [HttpPost]
        public async Task<IActionResult> AddBoatUse(BoatUseViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

            // get existing boat (if any)
            BoatUse boatUse = _boatUseRepository.FindAll().FirstOrDefault(bu => bu.Id == model.BoatUseId);
            // no boatUse, so create new

            if (boatUse == null)
                boatUse = model.ToEntity(user);
            model.UpdateEntity(boatUse);
            //if (model.BoatUseBoat != Guid.Empty)
            //    boatUse.BoatUseBoat = _boatRepository.GetById(model.BoatUseBoat);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.BoatUses.Add(boatUse);
                await uow.Commit().ConfigureAwait(false);
            }
            model.BoatUseId = boatUse.Id;


            var boatUses = new List<BoatUseViewModel>();
            foreach (BoatUse bu in sheet.BoatUses)
            {
                boatUses.Add(BoatUseViewModel.FromEntity(bu));
            }


            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddPrincipalDirectors(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var currentUser = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save - No Client information for " + model.AnswerSheetId);
            string orgTypeName = "";

            try
            {
                switch (model.OrganisationTypeName)
                {
                    case "Person - Individual":
                        {
                            orgTypeName = "Person - Individual";
                            break;
                        }
                    case "Corporate":
                        {
                            orgTypeName = "Corporation – Limited liability";
                            break;
                        }
                    case "Trust":
                        {
                            orgTypeName = "Trust";
                            break;
                        }
                    case "Partnership":
                        {
                            orgTypeName = "Partnership";
                            break;
                        }
                    default:
                        {
                            throw new Exception(string.Format("Invalid Organisation Type: ", orgTypeName));
                        }
                }

                InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(model.Type);
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = await _insuranceAttributeService.CreateNewInsuranceAttribute(currentUser, model.Type);
                }
                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName(orgTypeName);
                if (organisationType == null)
                {
                    organisationType = await _organisationTypeService.CreateNewOrganisationType(currentUser, orgTypeName);
                }

                Organisation organisation = null;
                User userdb = null;
                try
                {
                    if (orgTypeName == "Person - Individual")
                    {
                        userdb = await _userService.GetUserByEmail(model.Email);
                        if (userdb == null)
                        {
                            userdb = new User(currentUser, Guid.NewGuid(), model.FirstName);
                            userdb.FirstName = model.FirstName;
                            userdb.LastName = model.LastName;
                            userdb.FullName = model.FirstName + " " + model.LastName;
                            userdb.Email = model.Email;
                            await _userService.Create(userdb);
                        }


                    }
                    else
                    {
                        userdb = _userRepository.FindAll().FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);

                    }

                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);

                    if (orgTypeName == "Person - Individual")
                    {
                        userdb = new User(currentUser, Guid.NewGuid(), model.FirstName);
                        userdb.FirstName = model.FirstName;
                        userdb.LastName = model.LastName;
                        userdb.FullName = model.FirstName + " " + model.LastName;
                        userdb.Email = model.Email;
                        await _userService.Create(userdb);
                    }
                    else
                    {
                        userdb = _userRepository.FindAll().FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);

                    }

                }
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);

                var organisationName = "";
                    if (orgTypeName == "Person - Individual")
                    { 
                     organisationName = model.FirstName + " " + model.LastName;
                    }
                    else
                    {
                        organisationName = model.OrganisationName;
                    }
                    organisation = new Organisation(currentUser, Guid.NewGuid(), organisationName, organisationType, userdb.Email);
                    organisation.Qualifications = model.Qualifications;
                    organisation.IsNZIAmember = model.IsNZIAmember;
                    organisation.NZIAmembership = model.NZIAmembership;
                    organisation.IsADNZmember = model.IsADNZmember;
                    organisation.IsRetiredorDecieved = model.IsRetiredorDecieved;
                    organisation.IsLPBCategory3 = model.IsLPBCategory3;
                    organisation.YearofPractice = model.YearofPractice;
                    organisation.PrevPractice = model.prevPractice;
                    organisation.IsOtherdirectorship = model.IsOtherdirectorship;
                    organisation.Othercompanyname = model.Othercompanyname;
                    organisation.Activities = model.Activities;
                    organisation.Email = userdb.Email;
                    organisation.Type = model.Type;
                    if(model.DateofRetirement != null)
                    {
                    organisation.DateofRetirement = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofRetirement), "d"));
                    }
                   if (model.DateofDeceased != null)
                    {
                    organisation.DateofDeceased = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofDeceased), "d"));
                    }
                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    insuranceAttribute.IAOrganisations.Add(organisation);
                    await _organisationService.CreateNewOrganisation(organisation);
                
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    //User owneruser = _userRepository.FindAll().FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
                    //owneruser.Organisations.Add(organisation);
                    userdb.Organisations.Add(organisation);
                    sheet.Organisation.Add(organisation);
                    model.ID = organisation.Id;
                    //NewMethod(uow);
                    await uow.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrincipalDirectors(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var currentUser = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);
            string orgTypeName = "";

            try
            {
                if (model.OrganisationTypeName != null)
                {
                    switch (model.OrganisationTypeName)
                    {
                        case "Person - Individual":
                            {
                                orgTypeName = "Person - Individual";
                                break;
                            }
                        case "Corporate":
                            {
                                orgTypeName = "Corporation – Limited liability";
                                break;
                            }
                        case "Trust":
                            {
                                orgTypeName = "Trust";
                                break;
                            }
                        case "Partnership":
                            {
                                orgTypeName = "Partnership";
                                break;
                            }
                        default:
                            {
                                throw new Exception(string.Format("Invalid Organisation Type: ", orgTypeName));
                            }
                    }
                }
                InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(model.Type);
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = await _insuranceAttributeService.CreateNewInsuranceAttribute(currentUser, model.Type);
                }
                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName(orgTypeName);
                if (organisationType == null)
                {
                    organisationType = await _organisationTypeService.CreateNewOrganisationType(currentUser, orgTypeName);
                }

                User userdb = null;
                Organisation organisation = null;

                organisation = await _organisationService.GetOrganisation(model.ID);
                try
                {
                    if (orgTypeName == "Person - Individual")
                    {
                        userdb = await _userService.GetUserByEmail(organisation.Email);
                        if (userdb != null)
                        {
                            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                            {
                                userdb.FirstName = model.FirstName;
                                userdb.LastName = model.LastName;
                                userdb.FullName = model.FirstName + " " + model.LastName;
                                userdb.Email = model.Email;
                                await uow.Commit();
                            }
                        }

                    }
                   
                }
                catch (Exception ex)
                {

                    if (orgTypeName == "Person - Individual")
                    {
                        userdb = new User(currentUser, Guid.NewGuid(), model.FirstName);
                        userdb.FirstName = model.FirstName;
                        userdb.LastName = model.LastName;
                        userdb.FullName = model.FirstName + " " + model.LastName;
                        userdb.Email = model.Email;
                        await _userService.Create(userdb);
                    }
                    else
                    {
                        userdb = _userRepository.FindAll().FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);

                    }

                }

                var organisationName = "";
                if (orgTypeName == "Person - Individual")
                {
                    organisationName = model.FirstName + " " + model.LastName;
                }
                else
                {
                    organisationName = model.OrganisationName;
                }
               
                //{
                //    organisation = new Organisation(CurrentUser(), Guid.NewGuid(), model.OrganisationName);
                //    _organisationService.CreateNewOrganisation(organisation);
                //}
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    organisation.ChangeOrganisationName(organisationName);
                    organisation.Qualifications = model.Qualifications;
                    organisation.IsNZIAmember = model.IsNZIAmember;
                    organisation.NZIAmembership = model.NZIAmembership;
                    organisation.IsADNZmember = model.IsADNZmember;
                    organisation.IsLPBCategory3 = model.IsLPBCategory3;
                    organisation.YearofPractice = model.YearofPractice;
                    organisation.PrevPractice = model.prevPractice;
                    organisation.IsOtherdirectorship = model.IsOtherdirectorship;
                    organisation.Othercompanyname = model.Othercompanyname;
                    organisation.Activities = model.Activities;
                    organisation.Email = userdb.Email;
                    organisation.Type = model.Type;
                    if(model.DateofRetirement != null)
                    {
                        organisation.DateofRetirement = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofRetirement), "d"));
                    }
                    if(model.DateofDeceased != null)
                    {
                        organisation.DateofDeceased = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofDeceased), "d"));
                    }
                    await uow.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditPrincipalDirectorsOwner(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            Organisation org = await _OrganisationRepository.GetByIdAsync(sheet.Owner.Id);
            try
            {
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    org.Email = model.Email;
                    org.ChangeOrganisationName(model.OrganisationName);
                    await uow.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(model);


        }

        [HttpPost]
        public async Task<IActionResult> GetPrincipalPartners(Guid answerSheetId, Guid partyID)
        {
            OrganisationViewModel model = new OrganisationViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Organisation org = sheet.Organisation.FirstOrDefault(o => o.Id == partyID);
            try
            {
                if (org != null)
                {
                    User userdb = await _userService.GetUserByEmail(org.Email);

                    model.ID = partyID;
                    model.FirstName = userdb.FirstName;
                    model.LastName = userdb.LastName;
                    model.Email = org.Email;
                    model.Qualifications = org.Qualifications;
                    model.IsNZIAmember = org.IsNZIAmember;
                    model.NZIAmembership = org.NZIAmembership;
                    model.IsADNZmember = org.IsADNZmember;
                    model.IsLPBCategory3 = org.IsLPBCategory3;
                    model.YearofPractice = org.YearofPractice;
                    model.prevPractice = org.PrevPractice;
                    if (org.OrganisationType.Name == "Corporation – Limited liability")
                    {
                        model.OrganisationTypeName = "Corporate";
                    }
                    else
                    {
                        model.OrganisationTypeName = org.OrganisationType.Name;
                    }
                    model.IsOtherdirectorship = org.IsOtherdirectorship;
                    model.IsRetiredorDecieved = org.IsRetiredorDecieved;
                    model.Othercompanyname = org.Othercompanyname;
                    model.Type = org.Type;
                    model.DateofDeceased = (org.DateofDeceased > DateTime.MinValue) ? org.DateofDeceased.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "";
                    model.DateofRetirement = (org.DateofRetirement > DateTime.MinValue) ? org.DateofRetirement.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "";
                    //model.DateofDeceased = (org.DateofDeceased > DateTime.MinValue) ? org.DateofDeceased.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "";
                    //model.DateofDeceased = DateTime.Parse(LocalizeTime(org.DateofDeceased, "d"));
                    //model.DateofRetirement = DateTime.Parse(LocalizeTime(org.DateofRetirement, "d"));
                    model.OrganisationName = org.Name;
                    model.Activities = org.Activities;
                    model.AnswerSheetId = answerSheetId;
                }
                else
                {
                    if (partyID == sheet.Owner.Id)
                    {
                        model.ID = partyID;
                        model.OrganisationName = sheet.Owner.Name;
                        model.Type = "Owner";
                        model.Email = sheet.Owner.Email;
                        model.AnswerSheetId = answerSheetId;
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddNamedParty(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

            try
            {

                Organisation organisation = null;

                organisation = await _organisationService.GetOrganisation(model.ID);
                //{
                //    organisation = new Organisation(CurrentUser(), Guid.NewGuid(), model.OrganisationName);
                //    _organisationService.CreateNewOrganisation(organisation);
                //}
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    organisation.ChangeOrganisationName(model.OrganisationName);
                    organisation.Phone = model.OrganisationPhone;
                    organisation.Email = model.OrganisationEmail;
                    await uow.Commit();
                }

                //model.PartyUseId = organisation.Id;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetNamedParty(Guid answerSheetId, Guid partyID)
        {
            OrganisationViewModel model = new OrganisationViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            Organisation org = sheet.Organisation.FirstOrDefault(o => o.Id == partyID);
            if (org != null)
            {
                model.ID = partyID;
                model.OrganisationName = org.Name;
                model.OrganisationPhone = org.Phone;
                model.Email = org.Email;
                model.OperatorYearsOfExp = org.SkipperExp;
                model.AnswerSheetId = answerSheetId;
            }
            else
            {
                if (partyID == sheet.Owner.Id)
                {
                    model.ID = partyID;
                    model.OrganisationName = sheet.Owner.Name;
                    model.OrganisationPhone = sheet.Owner.Phone;
                    model.Email = sheet.Owner.Email;
                    model.AnswerSheetId = answerSheetId;
                }
            }
            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddMarina(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

            try
            {
                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName("Other Marina");
                var user = await CurrentUser();
                if (organisationType == null)
                {
                    organisationType = await _organisationTypeService.CreateNewOrganisationType(user, "Other Marina");
                }
                Organisation organisation = null;

                organisation = await _organisationService.GetOrganisationByEmail(model.OrganisationName);
                if (organisation == null)
                {
                    organisation = new Organisation(user, Guid.NewGuid(), model.OrganisationName, organisationType);
                    await _organisationService.CreateNewOrganisation(organisation);
                }

                model.ID = organisation.Id;
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.Organisation.Add(organisation);
                    //NewMethod(uow);
                    await uow.Commit();
                }

                //model.PartyUseId = organisation.Id;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetMooredType(Guid OrgID)
        {
            Organisation organisation = null;

            organisation = await _organisationService.GetOrganisation(OrgID);
            var organisationalUnits = new List<OrganisationalUnitViewModel>();
            List<SelectListItem> mooredtypes = new List<SelectListItem>();

            foreach (var mooredtype in organisation.Marinaorgmooredtype)
            {

                mooredtypes.Add(new SelectListItem
                {
                    Selected = false,
                    Value = mooredtype,
                    Text = mooredtype
                });

            }

            return Json(mooredtypes);
        }

        [HttpPost]
        public async Task<IActionResult> OUSelected(Guid OUselect)
        {
            OrganisationalUnit orgunit = null;

            orgunit = await _organisationalUnitRepository.GetByIdAsync(OUselect);
            var locations = new List<LocationViewModel>();

            //var Locations = new List<Location>();
            // ou.Locations.Add(location);
            foreach (Location ou in orgunit.Locations)
            {
                locations.Add(new LocationViewModel
                {
                    LocationId = ou.Id,
                    Street = ou.Street
                });
            }

            return Json(locations);
        }

        [HttpPost]
        public async Task<IActionResult> getUserEmail(String Useremail)
        {
            User user = null;
            var userName = "";

            try
            {
                user = await _userService.GetUserByEmail(Useremail);
                userName = user.FirstName;

            }
            catch (Exception ex)
            {
                if (user == null)
                    userName = "NotFound";

            }

            return Json(userName);
        }




        [HttpPost]
        public async Task<IActionResult> AddInterestedParty(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);
            try
            {
                var user = await CurrentUser();
                InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(model.InsuranceAttribute);
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = await _insuranceAttributeService.CreateNewInsuranceAttribute(user, model.InsuranceAttribute);
                }

                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName(model.OrganisationTypeName);
                if (organisationType == null)
                {
                    organisationType = await _organisationTypeService.CreateNewOrganisationType(user, model.OrganisationTypeName);
                }

                Organisation organisation = null;
                User userDb = null;
                //if (model.InsuranceAttribute.EqualsIgnoreCase("Financial"))
                //{
                organisation = await _organisationService.GetOrganisationByEmail(model.OrganisationEmail);
                if (organisation == null)
                {
                    organisation = new Organisation(user, Guid.NewGuid(), model.OrganisationName, organisationType);
                    organisation.Phone = model.OrganisationPhone;
                    organisation.Email = model.OrganisationEmail;
                    await _organisationService.CreateNewOrganisation(organisation);
                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    insuranceAttribute.IAOrganisations.Add(organisation);
                }
                //}

                if (model.InsuranceAttribute.EqualsIgnoreCase("Private") || model.InsuranceAttribute.EqualsIgnoreCase("CoOwner"))
                {
                    try
                    {
                        if (model.IsAdmin.EqualsIgnoreCase("Yes"))
                        {
                            user = await _userService.GetUserByEmail(user.Email);
                        }
                        else
                        {
                            user = await _userService.GetUserByEmail(model.Email);
                        }
                    }
                    catch (Exception ex)
                    {
                        user = new User(user, Guid.NewGuid(), model.FirstName);
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.FullName = model.FirstName + " " + model.LastName;
                        user.Email = model.Email;
                        user.Phone = model.Phone;
                        user.Password = "";
                        await _userService.Create(user);

                    }

                }

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    user.Organisations.Add(organisation);
                    sheet.Organisation.Add(organisation);
                    model.ID = organisation.Id;

                    await uow.Commit();
                }

                //model.PartyUseId = organisation.Id;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetBoatUse(Guid answerSheetId, Guid boatUseId)
        {
            BoatUseViewModel model = new BoatUseViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            BoatUse boatUse = sheet.BoatUses.FirstOrDefault(bu => bu.Id == boatUseId);
            if (boatUse != null)
            {
                model = BoatUseViewModel.FromEntity(boatUse);
                model.AnswerSheetId = answerSheetId;
                //if (boatUse.BoatUseBoat != null)
                //    model.BoatUseBoat = boatUse.BoatUseBoat.Id;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBoatUses(Guid informationId, bool removed, bool ceased, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var boatUses = new List<BoatUse>();

            if (ceased)
            {
                boatUses = sheet.BoatUses.Where(bu => bu.Removed == removed && bu.DateDeleted == null && bu.BoatUseCeaseDate > DateTime.MinValue).ToList();
            }
            else
            {
                boatUses = sheet.BoatUses.Where(bu => bu.Removed == removed && bu.DateDeleted == null && bu.BoatUseCeaseDate == DateTime.MinValue).ToList();
            }

            if (_search)
            {

                switch (searchOper)
                {
                    case "eq":
                        boatUses = boatUses.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        boatUses = boatUses.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        boatUses = boatUses.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //boatUses = boatUses.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = boatUses.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                BoatUse boatUse = boatUses[i];
                JqGridRow row = new JqGridRow(boatUse.Id);
                row.AddValues(boatUse.Id, boatUse.BoatUseCategory, boatUse.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatUseRemovedStatus(Guid boatUseId, bool status)
        {
            BoatUse boatUse = await _boatUseRepository.GetByIdAsync(boatUseId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boatUse.Removed = status;
                await uow.Commit();
            }
            return new JsonResult(true);
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatUseCeasedStatus(Guid boatUseId, bool status)
        {
            BoatUse boatUse = await _boatUseRepository.GetByIdAsync(boatUseId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boatUse.BoatUseCeaseDate = DateTime.MinValue;
                boatUse.BoatUseCeaseReason = '0';
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Claim

        [HttpPost]
        public async Task<IActionResult> AddClaim(ClaimViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();

            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Claim - No Client information for " + model.AnswerSheetId);

            ClaimNotification claimNotification = _claimRepository.FindAll().FirstOrDefault(c => c.Id == model.ClaimId);
            // no claim, so create new
            if (claimNotification == null)
                claimNotification = model.ToEntity(user);
            model.UpdateEntity(claimNotification);

            if (model.OrganisationId != Guid.Empty)
            {
                Organisation org = await _organisationService.GetOrganisation(model.OrganisationId);
                claimNotification.Organisation = org;
            }

            if (model.ClaimProducts != null)
                claimNotification.ClaimProducts = _productRepository.FindAll().Where(pro => model.ClaimProducts.Contains(pro.Id)).ToList();

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.ClaimNotifications.Add(claimNotification);
                await uow.Commit();
            }

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetClaim(Guid answerSheetId, Guid claimId)
        {
            ClaimViewModel model = new ClaimViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            ClaimNotification claim = sheet.ClaimNotifications.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                model = ClaimViewModel.FromEntity(claim);
                model.AnswerSheetId = answerSheetId;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetClaims(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var claims = new List<ClaimNotification>();

            claims = sheet.ClaimNotifications.Where(c => c.Removed == removed && c.DateDeleted == null).ToList();

            if (_search)
            {

                switch (searchOper)
                {
                    case "eq":
                        claims = claims.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        claims = claims.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        claims = claims.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            //claims = claims.OrderBy(sidx + " " + sord).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = page;
            model.TotalRecords = claims.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                ClaimNotification claim = claims[i];
                JqGridRow row = new JqGridRow(claim.Id);
                row.AddValues(claim.Id, claim.ClaimTitle, claim.ClaimDescription, claim.ClaimReference, claim.Claimant, claim.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public async Task<IActionResult> SetClaimRemovedStatus(Guid claimId, bool status)
        {
            ClaimNotification claim = await _claimRepository.GetByIdAsync(claimId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                claim.Removed = status;
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Operators

        [HttpPost]
        public async Task<IActionResult> AddOperator(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var currentUser = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);
            try
            {
                InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName("Skipper");
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = await _insuranceAttributeService.CreateNewInsuranceAttribute(currentUser, "Skipper");
                }
                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
                if (organisationType == null)
                {
                    organisationType = await _organisationTypeService.CreateNewOrganisationType(currentUser, "Person - Individual");
                }

                Organisation organisation = null;
                User userdb = null;
                try
                {
                    userdb = await _userService.GetUserByEmail(model.Email);
                }
                catch (Exception ex)
                {
                    userdb = new User(currentUser, Guid.NewGuid(), model.FirstName);
                    userdb.FirstName = model.FirstName;
                    userdb.LastName = model.LastName;
                    userdb.FullName = model.FirstName + " " + model.LastName;
                    userdb.Email = model.Email;
                    userdb.Phone = model.Phone;
                    userdb.Password = "";

                    await _userService.Create(userdb);

                }

                organisation = await _organisationService.GetOrganisationByEmail(model.Email);
                if (organisation == null)
                {
                    var organisationName = model.FirstName + " " + model.LastName;
                    organisation = new Organisation(currentUser, Guid.NewGuid(), organisationName, organisationType, model.Email);
                    //organisation.OperatorYearsOfExp = model.OperatorYearsOfExp;
                    // organisation.OrganisationType = organisationType;
                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    insuranceAttribute.IAOrganisations.Add(organisation);
                    await _organisationService.CreateNewOrganisation(organisation);
                    userdb.SetPrimaryOrganisation(organisation);

                }

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    userdb.SetPrimaryOrganisation(organisation);
                    currentUser.Organisations.Add(organisation);
                    userdb.Organisations.Add(organisation);
                    sheet.Organisation.Add(organisation);
                    model.ID = organisation.Id;
                    //NewMethod(uow);
                    await uow.Commit();
                }

                //model.PartyUseId = organisationId;                                    
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }


        #endregion

        #region BusinessContracts

        [HttpPost]
        public async Task<IActionResult> AddBusinessContract(BusinessContractViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var user = await CurrentUser();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Location - No Client information for " + model.AnswerSheetId);

            BusinessContract businessContract = _businessContractRepository.FindAll().FirstOrDefault(bc => bc.Id == model.BusinessContractId);
            if (businessContract == null)
                businessContract = model.ToEntity(user);
            model.UpdateEntity(businessContract);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.BusinessContracts.Add(businessContract);
                await uow.Commit();
            }

            model.BusinessContractId = businessContract.Id;

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetBusinessContract(Guid answerSheetId, Guid businessContractId)
        {
            BusinessContractViewModel model = new BusinessContractViewModel();
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
            BusinessContract businessContract = sheet.BusinessContracts.FirstOrDefault(bc => bc.Id == businessContractId);
            if (businessContract != null)
            {
                model = BusinessContractViewModel.FromEntity(businessContract);
                model.AnswerSheetId = answerSheetId;
            }
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessContracts(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var businessContracts = sheet.BusinessContracts.Where(bc => bc.Removed == removed && bc.DateDeleted == null).ToList();

            if (_search)
            {
                switch (searchOper)
                {
                    case "eq":
                        businessContracts = businessContracts.Where(searchField + " = \"" + searchString + "\"").ToList();
                        break;
                    case "bw":
                        businessContracts = businessContracts.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
                        break;
                    case "cn":
                        businessContracts = businessContracts.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
                        break;
                }
            }
            businessContracts = businessContracts.ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = 1;
            model.TotalRecords = businessContracts.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                BusinessContract businessContract = businessContracts[i];
                JqGridRow row = new JqGridRow(businessContract.Id);
                row.AddValues(businessContract.Id, businessContract.Year, businessContract.ContractTitle, businessContract.ConstructionValue, businessContract.Fees, businessContract.ContractType, businessContract.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering
            document = model.ToXml();
            return Xml(document);

        }


        [HttpGet]
        public async Task<IActionResult> GetBusinessContractss(Guid informationId, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var businessContracts = sheet.BusinessContracts.Where(bc => bc.Removed == false && bc.DateDeleted == null).ToList();

            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();
            model.Page = 1;
            model.TotalRecords = businessContracts.Count;
            model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

            int offset = rows * (page - 1);
            for (int i = offset; i < offset + rows; i++)
            {
                if (i == model.TotalRecords)
                    break;

                BusinessContract businessContract = businessContracts[i];
                JqGridRow row = new JqGridRow(businessContract.Id);
                row.AddValues(businessContract.Id, businessContract.Year, businessContract.ContractTitle, businessContract.ConstructionValue, businessContract.Fees, businessContract.ContractType, businessContract.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering
            document = model.ToXml();
            return Xml(document);

        }

        [HttpPost]
        public async Task<IActionResult> SetBusinessContractRemovedStatus(Guid businessContractId, bool status)
        {
            BusinessContract businessContract = await _businessContractRepository.GetByIdAsync(businessContractId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                businessContract.Removed = status;
                await uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region CoastGuardSelfReg

        [HttpPost]
        public async Task<IActionResult> CoastGuardSelfRegAsync(string craftType, string membershipNumber, string boatType, string constructionType, string hullConfiguration, string mooredType, string trailered,
            string boatInsuredValue, string quickQuotePremium, string firstName, string lastName, string email, string orgType, string homePhone, string mobilePhone)
        {

            bool hasAccount = true;
            //Add User, Organisation, Information Sheet, Quick Term saving process here
            string organisationName = null;
            string ouname = null;
            string orgTypeName = null;
            if (orgType == "Private") //orgType = "Private", "Company", "Trust", "Partnership"
            {
                organisationName = firstName + " " + lastName;
                ouname = "Home";
            }
            else
            {
                organisationName = "To be completed";
                ouname = "Head Office";
            }
            switch (orgType)
            {
                case "Private":
                    {
                        orgTypeName = "Person - Individual";
                        break;
                    }
                case "Company":
                    {
                        orgTypeName = "Corporation – Limited liability";
                        break;
                    }
                case "Trust":
                    {
                        orgTypeName = "Trust";
                        break;
                    }
                case "Partnership":
                    {
                        orgTypeName = "Partnership";
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Invalid Organisation Type: ", orgType));
                    }
            }
            string phonenumber = null;
            if (homePhone == null)
            {
                phonenumber = homePhone;
            }
            else
            {
                phonenumber = mobilePhone;
            }
            OrganisationType organisationType = null;
            organisationType = await _organisationTypeService.GetOrganisationTypeByName(orgTypeName);
            if (organisationType == null)
            {
                organisationType = await _organisationTypeService.CreateNewOrganisationType(null, orgTypeName);
            }
            Organisation organisation = null;
            organisation = await _organisationService.GetOrganisationByEmail(email);

            //condition for organisation exists
            if (organisation == null)
            {
                organisation = new Organisation(null, Guid.NewGuid(), organisationName, organisationType);
                organisation.Phone = phonenumber;
                organisation.Email = email;
                await _organisationService.CreateNewOrganisation(organisation);

                User user = null;
                User user2 = null;

                try
                {
                    user = await _userService.GetUserByEmail(email);
                    if (!user.Organisations.Contains(organisation))
                        user.Organisations.Add(organisation);
                    var username = user.FirstName;
                }
                catch (Exception ex)
                {
                    string username = firstName + "_" + lastName;

                    try
                    {
                        user2 = await _userService.GetUser(username);

                        if (user2 != null && user == user2)
                        {
                            Random random = new Random();
                            int randomNumber = random.Next(10, 99);
                            username = username + randomNumber.ToString();
                        }
                    }
                    catch (Exception)
                    {

                        try
                        {
                            user = new User(null, Guid.NewGuid(), username);
                            user.FirstName = firstName;
                            user.LastName = lastName;
                            user.FullName = firstName + " " + lastName;
                            user.Email = email;
                            user.Phone = homePhone;
                            user.MobilePhone = mobilePhone;
                            user.Password = "";
                            //user.Organisations.Add (personalOrganisation);
                            // save the new user
                            // creates a new user in the system along with a default organisation
                            await _userService.Create(user);
                        }
                        catch (Exception ex1)
                        {
                            Console.WriteLine(ex1.Message);
                        }

                    }
                }
                finally
                {
                    if (!user.Organisations.Contains(organisation))
                        user.Organisations.Add(organisation);

                    user.SetPrimaryOrganisation(organisation);
                    await _userRepository.UpdateAsync(user);

                }

                var programme = await _programmeService.GetCoastGuardProgramme();
                var clientProgramme = await _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);
                var reference = await _referenceService.GetLatestReferenceId();
                var sheet = await _clientInformationService.IssueInformationFor(user, organisation, clientProgramme, reference);
                await _referenceService.CreateClientInformationReference(sheet);

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    OrganisationalUnit ou = new OrganisationalUnit(user, ouname);
                    Boat vessel = new Boat(user)
                    {
                        BoatType1 = boatType,
                        BoatType2 = craftType,
                        HullConstruction = constructionType,
                        HullConfiguration = hullConfiguration,
                        BoatIsTrailered = trailered,
                        MaxSumInsured = Convert.ToInt32(boatInsuredValue),
                        BoatQuickQuotePremium = Convert.ToDecimal(quickQuotePremium),
                    };
                    sheet.Boats.Add(vessel);
                    organisation.OrganisationalUnits.Add(ou);
                    clientProgramme.BrokerContactUser = programme.BrokerContactUser;
                    clientProgramme.ClientProgrammeMembershipNumber = membershipNumber;
                    sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(user, sheet, null, "First Mate Cover Quick Quote Consuming Process Completed"));
                    try
                    {
                        Thread.Sleep(1000);
                        await uow.Commit();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                }
                //send out login email
                await _emailService.SendSystemEmailLogin(email);
                EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetInstruction");
                if (emailTemplate != null)
                {
                    await _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null);
                }
                //send out information sheet issue notification email
                await _emailService.SendSystemEmailUISIssueNotify(programme.BrokerContactUser, programme, clientProgramme.InformationSheet, organisation);
            }
            else
            {
                hasAccount = false;
            }

            if (hasAccount)
            {

                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }
        }

        #endregion

        #region Advisory
        [HttpPost]
        public async Task<IActionResult> CloseAdvisory(string ClientInformationSheetId)
        {
            var sheet = await _clientInformationService.GetInformation(Guid.Parse(ClientInformationSheetId));
            sheet.Status = "Not Taken Up";
            foreach(var agreement in sheet.Programme.Agreements)
            {
                agreement.Status = "Not Taken Up";
                await _clientAgreementService.UpdateClientAgreement(agreement);
            }

            string domainQueryString = _appSettingService.domainQueryString;

            return Ok("https://" + domainQueryString + "/Home/Index/");
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> IssueUIS(string orgName, Guid programmeID, string firstName, string membershipNumber, string lastName, string email, string orgType, string mobilePhone)
        {

            bool hasAccount = true;
            //Add User, Organisation, Information Sheet, Quick Term saving process here
            string organisationName = null;
            string ouname = null;
            string orgTypeName = null;
            if (orgType == "Private") //orgType = "Private", "Company", "Trust", "Partnership"
            {
                organisationName = firstName + " " + lastName;
                ouname = "Home";
            }
            else
            {
                organisationName = orgName;
                ouname = "Head Office";
            }
            switch (orgType)
            {
                case "Private":
                    {
                        orgTypeName = "Person - Individual";
                        break;
                    }
                case "Company":
                    {
                        orgTypeName = "Corporation – Limited liability";
                        break;
                    }
                case "Trust":
                    {
                        orgTypeName = "Trust";
                        break;
                    }
                case "Partnership":
                    {
                        orgTypeName = "Partnership";
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Invalid Organisation Type: ", orgType));
                    }
            }
            string phonenumber = null;

            phonenumber = mobilePhone;

            OrganisationType organisationType = null;
            organisationType = await _organisationTypeService.GetOrganisationTypeByName(orgTypeName);
            if (organisationType == null)
            {
                organisationType = await _organisationTypeService.CreateNewOrganisationType(null, orgTypeName);
            }
            Organisation organisation = null;
            organisation = await _organisationService.GetOrganisationByEmail(email);

            //condition for organisation exists
            if (organisation == null)
            {
                organisation = new Organisation(null, Guid.NewGuid(), organisationName, organisationType);
                organisation.Phone = phonenumber;
                organisation.Email = email;
                await _organisationService.CreateNewOrganisation(organisation);

                User user = null;
                User user2 = null;

                try
                {
                    user = await _userService.GetUserByEmail(email);
                    if (!user.Organisations.Contains(organisation))
                        user.Organisations.Add(organisation);
                    var username = user.FirstName;
                }
                catch (Exception ex)
                {
                    string username = firstName + "_" + lastName;

                    try
                    {
                        user2 = await _userService.GetUser(username);

                        if (user2 != null && user == user2)
                        {
                            Random random = new Random();
                            int randomNumber = random.Next(10, 99);
                            username = username + randomNumber.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        // create personal organisation
                        //var personalOrganisation = new Organisation (CurrentUser(), Guid.NewGuid (), personalOrganisationName, new OrganisationType (CurrentUser(), "personal"));
                        //_organisationService.CreateNewOrganisation (personalOrganisation);
                        // create user object
                        user = new User(null, Guid.NewGuid(), username);
                        user.FirstName = firstName;
                        user.LastName = lastName;
                        user.FullName = firstName + " " + lastName;
                        user.Email = email;
                        user.MobilePhone = mobilePhone;
                        user.Password = "";
                        //user.Organisations.Add (personalOrganisation);
                        // save the new user
                        // creates a new user in the system along with a default organisation
                        await _userService.Create(user);
                        //Console.WriteLine ("Created User " + user.FullName);
                    }
                }
                finally
                {
                    if (!user.Organisations.Contains(organisation))
                        user.Organisations.Add(organisation);

                    user.SetPrimaryOrganisation(organisation);
                    await _userRepository.UpdateAsync(user);

                }

                var programme = await _programmeService.GetProgramme(programmeID);
                var clientProgramme = await _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);
                try
                {
                    var reference = await _referenceService.GetLatestReferenceId();

                    var sheet = await _clientInformationService.IssueInformationFor(user, organisation, clientProgramme, reference);

                    await _referenceService.CreateClientInformationReference(sheet);



                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        OrganisationalUnit ou = new OrganisationalUnit(user, ouname);

                        organisation.OrganisationalUnits.Add(ou);
                        clientProgramme.BrokerContactUser = programme.BrokerContactUser;
                        clientProgramme.ClientProgrammeMembershipNumber = membershipNumber;
                        sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(user, sheet, null, programme.Name + "UIS issue Process Completed"));
                        try
                        {
                            Thread.Sleep(1000);
                            await uow.Commit();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }

                    }
                    ////send out login email
                    //await _emailService.SendSystemEmailLogin(email);
                    //EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetInstruction");
                    //if (emailTemplate != null)
                    //{
                    //    await _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null);
                    //}
                    ////send out information sheet issue notification email
                    //await _emailService.SendSystemEmailUISIssueNotify(programme.BrokerContactUser, programme, clientProgramme.InformationSheet, organisation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                hasAccount = false;
            }

            if (hasAccount)
            {

                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CoastGuardSelfRegCall(string craftType, string membershipNumber, string boatType, string constructionType, string hullConfiguration, string mooredType, string trailered,
            string boatInsuredValue, string quickQuotePremium, string firstName, string lastName, string email, string orgType, string homePhone, string mobilePhone)
        {
            var emailBody = "First Mate Cover Programme Quick Quote Please Call Request " + "<br/>" +
                " Client Details : <br/>" + 
                " First Name : " + firstName + "<br/>" +
                " Last Name : " + lastName + "<br/>" +
                " Email : " + email + "<br/>" +
                " Mobile phone : " + mobilePhone + "<br/>" +
                " Home Phone : " + homePhone + "<br/>" +
                " Craft type : " + craftType + "<br/>" +
                " Membership Number : " + membershipNumber + "<br/>" +
                " Boat type : " + boatType + "<br/>" +
                " Construction type : " + constructionType + "<br/>" +
                " Hull configuration : " + hullConfiguration + "<br/>" +
                " Moored type : " + mooredType + "<br/>" +
                " Trailered : " + trailered + "<br/>" +
                " Boat insured value : " + boatInsuredValue + "<br/>" +
                " Quick Quote premium : " + quickQuotePremium + "<br/>";
            if (_appSettingService.GetMarineInsuranceSpecialistEmail != "")
                await _emailService.MarshPleaseCallMe(_appSettingService.GetMarineInsuranceSpecialistEmail, "Coastguard Pleasurecraft Insurance Query ", emailBody);

            return Ok();
        }
    }
}
