using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Interfaces;
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
using DealEngine.Infrastructure.Identity.Data;

namespace TechCertain.WebUI.Controllers
{

    public class ServicesController : BaseController
    {        
        ICilentInformationService _clientInformationService;
        IMapperSession<Vehicle> _vehicleRepository;
        IMapperSession<OrganisationalUnit> _organisationalUnitRepository;
        IMapperSession<Location> _locationRepository;
        IMapperSession<WaterLocation> _waterLocationRepository;
        IMapperSession<Boat> _boatRepository;
        IMapperSession<BoatUse> _boatUseRepository;
        IVehicleService _vehicleService;
        IOrganisationService _organisationService;
        IBoatUseService _boatUseService;
        IMapperSession<Building> _buildingRepository;
        IMapperSession<BusinessInterruption> _businessInterruptionRepository;
        IMapperSession<MaterialDamage> _materialDamageRepository;
        IMapperSession<Claim> _claimRepository;
        IMapperSession<Product> _productRepository;
        IProgrammeService _programmeService;
        IOrganisationTypeService _organisationTypeService;
        IUnitOfWork _unitOfWork;
        IMapperSession<Organisation> _OrganisationRepository;
        IReferenceService _referenceService;
        IEmailService _emailService;
        IInsuranceAttributeService _insuranceAttributeService;
        IMapper _mapper;


        public ServicesController(IUserService userService, DealEngineDBContext dealEngineDBContext, ICilentInformationService clientInformationService, IMapperSession<Vehicle> vehicleRepository, IMapperSession<BoatUse> boatUseRepository,
            IMapperSession<OrganisationalUnit> organisationalUnitRepository, IMapperSession<Location> locationRepository, IMapperSession<WaterLocation> waterLocationRepository, IMapperSession<Building> buildingRepository, IMapperSession<BusinessInterruption> businessInterruptionRepository,
            IMapperSession<MaterialDamage> materialDamageRepository, IMapperSession<Claim> claimRepository, IMapperSession<Product> productRepository, IVehicleService vehicleService, IMapperSession<Boat> boatRepository,
            IOrganisationService organisationService, IBoatUseService boatUseService, /*IMapperSession<Operator> operatorRepository,*/ IProgrammeService programeService, IOrganisationTypeService organisationTypeService,
            IMapperSession<Organisation> OrganisationRepository, IEmailService emailService, IMapper mapper,IUnitOfWork unitOfWork, IInsuranceAttributeService insuranceAttributeService, IReferenceService referenceService)

            : base(userService, dealEngineDBContext)
        {
            
            _clientInformationService = clientInformationService;
            _vehicleRepository = vehicleRepository;
            _organisationalUnitRepository = organisationalUnitRepository;
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
            // _operatorRepository = operatorRepository;
            _programmeService = programeService;
            _organisationTypeService = organisationTypeService;
            _unitOfWork = unitOfWork;
            _OrganisationRepository = OrganisationRepository;
            _referenceService = referenceService;
            _emailService = emailService;
            _mapper = mapper;

            _insuranceAttributeService = insuranceAttributeService;

        }

        #region Vehicle

        [HttpPost]
        public ActionResult SearchVehicle(string registration)
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
        public ActionResult AddVehicle(VehicleViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Vehicle - No Client information for " + model.AnswerSheetId);

            // get existing vehicle (if any)
            Vehicle vehicle = _vehicleRepository.FindAll().FirstOrDefault(v => v.Id == model.VehicleId);
            // no vehicle, so create new
            if (vehicle == null)
                vehicle = model.ToEntity(CurrentUser);
            model.UpdateEntity(vehicle);

            //	vehicle = new Vehicle (CurrentUser, model.Registration, model.Make, model.VehicleModel);
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
                vehicle.GarageLocation = _locationRepository.GetById(model.VehicleLocation);

            //var orgs = _organisationService.GetAllOrganisations ().ToList();
            if (model.InterestedParties != null)
                vehicle.InterestedParties = _organisationService.GetAllOrganisations().Where(org => model.InterestedParties.Contains(org.Id)).ToList();

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {

                sheet.Vehicles.Add(vehicle);
                uow.Commit();
            }

            model.VehicleId = vehicle.Id;


            return Json(model);
            //return Json ("Success");
        }

        [HttpPost]
        public ActionResult GetVehicle(Guid answerSheetId, Guid vehicleId)
        {
            VehicleViewModel model = new VehicleViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetVehicles(Guid informationId, bool validated, bool removed, bool ceased, bool transfered, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var vehicles = new List<Vehicle>();

            if (ceased)
            {
                vehicles = sheet.Vehicles.Where(v => v.Validated == validated && v.Removed == removed && v.DateDeleted == null && v.VehicleCeaseDate > DateTime.MinValue && v.VehicleCeaseReason != 4).ToList();
            }
            else if (transfered)
            {
                vehicles = sheet.Vehicles.Where(v => v.Validated == validated && v.Removed == removed && v.DateDeleted == null && v.VehicleCeaseDate > DateTime.MinValue && v.VehicleCeaseReason == 4).ToList();
            }
            else
            {
                vehicles = sheet.Vehicles.Where(v => v.Validated == validated && v.Removed == removed && v.DateDeleted == null && v.VehicleCeaseDate == DateTime.MinValue).ToList();
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
                    row.AddValues(vehicle.Id, vehicle.Year, vehicle.Registration, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured, vehicle.Id);
                }
                else
                {
                    if (sheet.Programme.BaseProgramme.Products.First().Id == new Guid("e2eae6d8-d68e-4a40-b50a-f200f393777a")) // Marsh Coastguard
                    {
                        row.AddValues(vehicle.Id, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured, vehicle.Id);
                    }
                    else
                    {
                        row.AddValues(vehicle.Id, vehicle.Year, vehicle.FleetNumber, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured, vehicle.Id);
                    }

                }


                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }


        [HttpGet]
        public ActionResult GetNamedParties(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult GetVehiclePlates(string term)
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
        public ActionResult SetVehicleRemovedStatus(Guid vehicleId, bool status)
        {
            Vehicle vehicle = _vehicleRepository.GetById(vehicleId);
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                vehicle.Removed = status;
                uow.Commit();
            }
            throw new Exception("Method needs to be re-written");
            //return new JsonResult { Data = new { status = true, id = vehicleId } };
        }

        [HttpPost]
        public ActionResult SetVehicleCeasedStatus(Guid vehicleId, bool status)
        {
            Vehicle vehicle = _vehicleRepository.GetById(vehicleId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                vehicle.VehicleCeaseDate = DateTime.MinValue;
                vehicle.VehicleCeaseReason = '0';
                uow.Commit();
            }
            throw new Exception("Method needs to be re-written");
            //return new JsonResult { Data = true };
        }

        [HttpPost]
        public ActionResult SetVehicleTransferedStatus(Guid vehicleId, bool status)
        {
            Vehicle vehicle = _vehicleRepository.GetById(vehicleId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                vehicle.VehicleCeaseDate = DateTime.MinValue;
                vehicle.VehicleCeaseReason = '0';
                uow.Commit();
            }

            throw new Exception("Method needs to be re-written");
            //return new JsonResult { Data = true };
        }

        [HttpPost]
        public ActionResult RevalidateVehicle(Guid vehicleId)
        {
            Vehicle vehicle = _vehicleRepository.GetById(vehicleId);
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
        public ActionResult SearchOrganisationalUnit(Guid answerSheetId, string name)
        {
            OrganisationalUnitViewModel model = new OrganisationalUnitViewModel();
            model.Name = name;
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult AddOrganisationalUnit(OrganisationalUnitViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Organisational Unit - No Client information for " + model.AnswerSheetId);

            OrganisationalUnit ou = _organisationalUnitRepository.GetById(model.OrganisationalUnitId);
            if (ou == null)
                ou = new OrganisationalUnit(CurrentUser, model.Name);
            ou.Name = model.Name;

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Owner.OrganisationalUnits.Add(ou);
                uow.Commit();
            }
            model.OrganisationalUnitId = ou.Id;

            return Json(model);
            //return Json("Success");
        }

        [HttpPost]
        public ActionResult GetOrganisationalUnit(Guid answerSheetId, Guid unitId)
        {
            OrganisationalUnitViewModel model = new OrganisationalUnitViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetOrganisationalUnits(Guid informationId, bool _search, string nd, int rows, int page, string sidx, string sord,
                                                     string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public IActionResult GetOrganisationalUnitName(string term)
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
        public ActionResult SearchLocationStreet(Guid answerSheetId, string street)
        {
            LocationViewModel model = new LocationViewModel();
            model.Street = street;
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
            Location location = _locationRepository.FindAll().FirstOrDefault(l => l.Street == street);
            if (location != null)
            {
                model = LocationViewModel.FromEntity(location);
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult AddLocation(LocationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Location - No Client information for " + model.AnswerSheetId);

            Location location = _locationRepository.FindAll().FirstOrDefault(loc => loc.Id == model.LocationId);
            if (location == null)
                location = model.ToEntity(CurrentUser);
            model.UpdateEntity(location);
            var OUList = new List<OrganisationalUnit>();

            if (sheet.Owner.OrganisationalUnits.Count() > 0)
                OUList.Add(sheet.Owner.OrganisationalUnits.ElementAtOrDefault(0));

            location.OrganisationalUnits = OUList;
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Locations.Add(location);
                uow.Commit();
            }

            model.LocationId = location.Id;

            return Json(model);
            //return Json("Success");
        }

        [HttpPost]
        public ActionResult GetLocation(Guid answerSheetId, Guid locationId)
        {
            LocationViewModel model = new LocationViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetLocations(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult GetLocationss(Guid informationId, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult GetLocationStreet(string term)
        {
            var locationStreetList = _locationRepository.FindAll().Select(l => l.Street);
            var results = locationStreetList.Where(n => n.ToLower().Contains(term.ToLower()));
            return new JsonResult(results.ToArray());
        }

        [HttpPost]
        public ActionResult GetLocationList(Guid answerSheetId)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);

            List<LocationViewModel> models = new List<LocationViewModel>();
            foreach (var location in sheet.Locations)
                models.Add(LocationViewModel.FromEntity(location));

            return new JsonResult(models.ToArray());
        }

        [HttpPost]
        public ActionResult GetLocationsByCountry(Guid answerSheetId)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);

            List<Location> countries = sheet.Locations.GroupBy(loc => loc.Country)
                                          .Select(grp => grp.First())
                                          .ToList();

            List<LocationViewModel> models = new List<LocationViewModel>();
            foreach (var location in countries)
                models.Add(LocationViewModel.FromEntity(location));

            return new JsonResult(models.ToArray());
        }

        [HttpPost]
        public ActionResult SetLocationRemovedStatus(Guid locationId, bool status)
        {
            Location location = _locationRepository.GetById(locationId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                location.Removed = status;
                uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Buildings

        [HttpPost]
        public ActionResult AddBuilding(BuildingViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Building - No Client information for " + model.AnswerSheetId);

            Building building = _buildingRepository.FindAll().FirstOrDefault(bui => bui.Id == model.BuildingId);
            if (building == null)
                building = model.ToEntity(CurrentUser);
            model.UpdateEntity(building);

            if (model.BuildingLocation != null)
                building.Location = _locationRepository.GetById(model.BuildingLocation);

            if (model.InterestedParties != null)
                building.InterestedParties = _organisationService.GetAllOrganisations().Where(org => model.InterestedParties.Contains(org.Id)).ToList();

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.Buildings.Add(building);
                uow.Commit();
            }

            model.LocationStreet = building.Location.Street;
            model.BuildingId = building.Id;


            return Json(model);
        }

        [HttpPost]
        public ActionResult GetBuilding(Guid answerSheetId, Guid buildingId)
        {
            BuildingViewModel model = new BuildingViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetBuildings(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult SetBuildingRemovedStatus(Guid buildingId, bool status)
        {
            Building building = _buildingRepository.GetById(buildingId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                building.Removed = status;
                uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region WaterLocations

        [HttpPost]
        public ActionResult SearchWaterLocationName(Guid answerSheetId, string waterLocationName)
        {
            WaterLocationViewModel model = new WaterLocationViewModel();
            model.WaterLocationName = waterLocationName;
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
            WaterLocation waterLocation = _waterLocationRepository.FindAll().FirstOrDefault(wl => wl.WaterLocationName == waterLocationName);
            if (waterLocation != null)
            {
                model = WaterLocationViewModel.FromEntity(waterLocation);
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult AddWaterLocation(WaterLocationViewModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Water Location - No Client information for " + model.AnswerSheetId);

                WaterLocation waterLocation = _waterLocationRepository.FindAll().FirstOrDefault(wloc => wloc.Id == model.WaterLocationId);
                if (waterLocation == null)
                    waterLocation = model.ToEntity(CurrentUser);
                model.UpdateEntity(waterLocation);

                if (model.WaterLocationLocation != Guid.Empty)
                    waterLocation.WaterLocationLocation = _locationRepository.GetById(model.WaterLocationLocation);
                if (model.WaterLocationMarinaLocation != null)
                {
                    waterLocation.WaterLocationMarinaLocation = _OrganisationRepository.GetById(model.WaterLocationMarinaLocation);

                }
                if (model.OrganisationalUnit != null)
                {
                    waterLocation.OrganisationalUnit = _organisationalUnitRepository.GetById(model.OrganisationalUnit);

                }
                // waterLocation.OrganisationalUnit = OrganisationalUnit;

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.WaterLocations.Add(waterLocation);
                    uow.Commit();
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
        public ActionResult GetWaterLocation(Guid answerSheetId, Guid waterLocationId)
        {
            WaterLocationViewModel model = new WaterLocationViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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

            organisation = _organisationService.GetOrganisation(waterLocation.WaterLocationMarinaLocation.Id);
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
        public ActionResult GetWaterLocations(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult GetWaterLocationName(string term)
        {
            var waterLocationNameList = _waterLocationRepository.FindAll().Select(wl => wl.WaterLocationName);
            var results = waterLocationNameList.Where(n => n.ToLower().Contains(term.ToLower()));
            return new JsonResult(results.ToArray());
        }

        [HttpPost]
        public ActionResult GetWaterLocationList(Guid answerSheetId)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);

            List<WaterLocationViewModel> models = new List<WaterLocationViewModel>();
            foreach (var waterLocation in sheet.WaterLocations)
                models.Add(WaterLocationViewModel.FromEntity(waterLocation));

            return new JsonResult(models.ToArray());
        }

        [HttpPost]
        public ActionResult SetWaterLocationRemovedStatus(Guid waterLocationId, bool status)
        {
            WaterLocation waterLocation = _waterLocationRepository.GetById(waterLocationId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                waterLocation.Removed = status;
                uow.Commit();
            }

            return new JsonResult(true); 
        }

        #endregion

        #region BusinessInterruption

        [HttpPost]
        public ActionResult AddBusinessInterruption(BusinessInterruptionViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save BusinessInterruption - No Client information for " + model.AnswerSheetId);

            BusinessInterruption businessInterruption = _businessInterruptionRepository.FindAll().FirstOrDefault(bi => bi.Id == model.BusinessInterruptionId);
            if (businessInterruption == null)
                businessInterruption = model.ToEntity(CurrentUser);
            model.UpdateEntity(businessInterruption);

            if (model.BusinessInterruptionLocation != null)
                businessInterruption.Location = _locationRepository.GetById(model.BusinessInterruptionLocation);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.BusinessInterruptions.Add(businessInterruption);
                uow.Commit();
            }

            return Json(model);
        }

        [HttpPost]
        public ActionResult GetBusinessInterruption(Guid answerSheetId, Guid businessInterruptionId)
        {
            BusinessInterruptionViewModel model = new BusinessInterruptionViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetBusinessInterruptions(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult SetBusinessInterruptionRemovedStatus(Guid businessInterruptionId, bool status)
        {
            BusinessInterruption businessInterruption = _businessInterruptionRepository.GetById(businessInterruptionId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                businessInterruption.Removed = status;
                uow.Commit();
            }


            return new JsonResult(true);
        }

        #endregion

        #region MaterialDamage

        [HttpPost]
        public ActionResult AddMaterialDamage(MaterialDamageViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save BusinessInterruption - No Client information for " + model.AnswerSheetId);

            MaterialDamage materialDamage = _materialDamageRepository.FindAll().FirstOrDefault(md => md.Id == model.MaterialDamageId);
            if (materialDamage == null)
                materialDamage = model.ToEntity(CurrentUser);
            model.UpdateEntity(materialDamage);

            if (model.MaterialDamageLocation != null)
                materialDamage.Location = _locationRepository.GetById(model.MaterialDamageLocation);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.MaterialDamages.Add(materialDamage);
                uow.Commit();
            }

            return Json(model);
        }

        [HttpPost]
        public ActionResult GetMaterialDamage(Guid answerSheetId, Guid materialDamageId)
        {
            MaterialDamageViewModel model = new MaterialDamageViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetMaterialDamages(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult SetMaterialDamageRemovedStatus(Guid materialDamageId, bool status)
        {
            MaterialDamage materialDamage = _materialDamageRepository.GetById(materialDamageId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                materialDamage.Removed = status;
                uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Boat

        [HttpPost]
        public ActionResult AddUsetoBoat(string[] Boatuse, Guid BoatId)
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
                    boat.BoatUse.Add(_boatUseService.GetBoatUse(Guid.Parse(useid)));

                }

                uow.Commit();

            }
            return Json(boat);

        }


        //[HttpPost]
        //public ActionResult AddBoat(BoatViewModel model)
        //{
        //    if (model == null)
        //        throw new ArgumentNullException(nameof(model));

        //    ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
        //    if (sheet == null)
        //        throw new Exception("Unable to save Boat - No Client information for " + model.AnswerSheetId);

        //    // get existing boat (if any)
        //    Boat boat = _boatRepository.FindAll().FirstOrDefault(b => b.Id == model.BoatId);
        //    // no boat, so create new
        //    try
        //    {
        //        if (boat == null)
        //        boat = model.ToEntity(CurrentUser);
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
        public ActionResult AddBoat(BoatViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat - No Client information for " + model.AnswerSheetId);

            // get existing boat (if any)
            Boat boat = _boatRepository.FindAll().FirstOrDefault(b => b.Id == model.BoatId);
            // no boat, so create new
            if (boat == null)
                boat = model.ToEntity(CurrentUser);
            model.UpdateEntity(boat);
            if (model.BoatLandLocation != Guid.Empty)
                boat.BoatLandLocation = _buildingRepository.GetById(model.BoatLandLocation);
            //if (model.BoatWaterLocation != Guid.Empty)
            //    boat.BoatWaterLocation = _waterLocationRepository.GetById(model.BoatWaterLocation);
            //if (model.InterestedParties != null)
            //    boat.InterestedParties = _organisationService.GetAllOrganisations().Where(org => model.InterestedParties.Contains(org.Id)).ToList();
            if (model.BoatOperator != Guid.Empty)
                boat.BoatOperator = _OrganisationRepository.GetById(model.BoatOperator);
            boat.BoatWaterLocation = null;

            if (model.BoatWaterLocation != Guid.Empty)
                boat.BoatWaterLocation = _OrganisationRepository.GetById(model.BoatWaterLocation);

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
                        boat.BoatUse.Add(_boatUseService.GetBoatUse(Guid.Parse(useid)));
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
                        boat.InterestedParties.Add(_organisationService.GetOrganisation(Guid.Parse(useid)));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (model.BoatTrailer != Guid.Empty)
                boat.BoatTrailer = _vehicleRepository.GetById(model.BoatTrailer);
            //if (model.BoatOperator != Guid.Empty)
            //    boat.BoatOperator = _operatorRepository.GetById(model.BoatOperator);
            try
            {

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    //boat.BoatUse = model.BoatUse;

                    sheet.Boats.Add(boat);
                    uow.Commit();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }


            return Json(model);
        }



        [HttpPost]
        public ActionResult GetBoat(Guid answerSheetId, Guid boatId)
        {
            BoatViewModel model = new BoatViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
            if (boat.InterestedParties != null)
            {
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
            }
            return Json(model);
        }


        [HttpGet]
        public ActionResult GetBoats(Guid informationId, bool validated, bool removed, bool ceased, bool transfered, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
            NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;

            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var boats = new List<Boat>();

            if (ceased)
            {
                boats = sheet.Boats.Where(b => b.Removed == removed && b.DateDeleted == null && b.BoatCeaseDate > DateTime.MinValue && b.BoatCeaseReason != 4).ToList();
            }
            else if (transfered)
            {
                boats = sheet.Boats.Where(b => b.Removed == removed && b.DateDeleted == null && b.BoatCeaseDate > DateTime.MinValue && b.BoatCeaseReason == 4).ToList();
            }
            else
            {
                boats = sheet.Boats.Where(b => b.Removed == removed && b.DateDeleted == null && b.BoatCeaseDate == DateTime.MinValue).ToList();
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
                row.AddValues(boat.Id, boat.BoatName, boat.YearOfManufacture, string.Format(currencyFormat, "{0:c}", boat.MaxSumInsured), boat.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }



        [HttpPost]
        public ActionResult SetBoatRemovedStatus(Guid boatId, bool status)
        {
            Boat boat = _boatRepository.GetById(boatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boat.Removed = status;
                uow.Commit();
            }
            return new JsonResult(true);
        }

        [HttpPost]
        public ActionResult SetBoatCeasedStatus(Guid boatId, bool status)
        {
            Boat boat = _boatRepository.GetById(boatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boat.BoatCeaseDate = DateTime.MinValue;
                boat.BoatCeaseReason = '0';
                uow.Commit();
            }

            return new JsonResult(true);
        }

        [HttpPost]
        public ActionResult SetBoatTransferedStatus(Guid boatId, bool status)
        {
            Boat boat = _boatRepository.GetById(boatId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boat.BoatCeaseDate = DateTime.MinValue;
                boat.BoatCeaseReason = '0';
                uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region BoatUse

        [HttpPost]
        public ActionResult AddBoatUse(BoatUseViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

            // get existing boat (if any)
            BoatUse boatUse = _boatUseRepository.FindAll().FirstOrDefault(bu => bu.Id == model.BoatUseId);
            // no boatUse, so create new

            if (boatUse == null)
                boatUse = model.ToEntity(CurrentUser);
            model.UpdateEntity(boatUse);
            //if (model.BoatUseBoat != Guid.Empty)
            //    boatUse.BoatUseBoat = _boatRepository.GetById(model.BoatUseBoat);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                sheet.BoatUses.Add(boatUse);
                uow.Commit();
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
        public ActionResult AddNamedParty(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

            try
            {

                Organisation organisation = null;

                organisation = _organisationService.GetOrganisation(model.ID);
                //{
                //    organisation = new Organisation(CurrentUser, Guid.NewGuid(), model.OrganisationName);
                //    _organisationService.CreateNewOrganisation(organisation);
                //}
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    organisation.ChangeOrganisationName(model.OrganisationName);
                    organisation.Phone = model.OrganisationPhone;
                    organisation.Email = model.OrganisationEmail;
                    uow.Commit();
                }

                //model.PartyUseId = organisation.Id;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        //[HttpPost]
        //public ActionResult AddAddNamedParty(OrganisationViewModel model)
        //{
        //    if (model == null)
        //        throw new ArgumentNullException(nameof(model));

        //    ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
        //    if (sheet == null)
        //        throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

        //    try
        //    {

        //        Organisation organisation = null;

        //        //organisation = _organisationService.GetOrganisation(model.ID);
        //        organisation = _organisationService.GetOrganisationByEmail(model.OrganisationName);
        //        if (organisation == null)
        //        {
        //            organisation = new Organisation(CurrentUser, Guid.NewGuid(), model.OrganisationName, model.OrganisationType);
        //            _organisationService.CreateNewOrganisation(organisation);
        //        }

        //        model.ID = organisation.Id;
        //        using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
        //        {
        //            sheet.Organisation.Add(organisation);
        //            //NewMethod(uow);
        //            uow.Commit();
        //        }

             
        //        //model.PartyUseId = organisation.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Write(ex.Message);
        //    }
        //    return Json(model);
        //}

        [HttpPost]
        public ActionResult GetNamedParty(Guid answerSheetId, Guid partyID)
        {
            OrganisationViewModel model = new OrganisationViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
            Organisation org = sheet.Organisation.FirstOrDefault(o => o.Id == partyID);
            if (org != null)
            {
                model.ID = partyID;
                model.OrganisationName = org.Name;
                model.OrganisationPhone = org.Phone;
                model.Email= org.Email;
                model.OperatorYearsOfExp = org.SkipperExp;
                model.AnswerSheetId = answerSheetId;
            }
            else
            {
                if(partyID == sheet.Owner.Id)
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
        public ActionResult AddMarina(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

            try
            {
                OrganisationType organisationType = _organisationTypeService.GetOrganisationTypeByName("Other Marina");

                if (organisationType == null)
                {                    
                    organisationType =_organisationTypeService.CreateNewOrganisationType(CurrentUser, "Other Marina");                    
                }
                Organisation organisation = null;

                organisation = _organisationService.GetOrganisationByEmail(model.OrganisationName);
                if (organisation == null)
                {
                    organisation = new Organisation(CurrentUser, Guid.NewGuid(), model.OrganisationName, organisationType);
                    _organisationService.CreateNewOrganisation(organisation);
                }

                model.ID = organisation.Id;
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.Organisation.Add(organisation);
                    //NewMethod(uow);
                    uow.Commit();
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
        public ActionResult GetMooredType(Guid OrgID)
        {
            Organisation organisation = null;

            organisation = _organisationService.GetOrganisation(OrgID);
            var organisationalUnits = new List<OrganisationalUnitViewModel>();
            List<SelectListItem> mooredtypes = new List<SelectListItem>();

            foreach (var mooredtype in organisation.marinaorgmooredtype)
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

        //[HttpPost]
        //public ActionResult GetOU(Guid OrgID)
        //{
        //    Organisation organisation = null;

        //    organisation = _organisationService.GetOrganisation(OrgID);
        //    var organisationalUnits = new List<OrganisationalUnitViewModel>();

        //    foreach (OrganisationalUnit ou in organisation.OrganisationalUnits)
        //    {
        //        organisationalUnits.Add(new OrganisationalUnitViewModel
        //        {
        //            OrganisationalUnitId = ou.Id,
        //            Name = ou.Name
        //        });
        //    }

        //    return Json(organisationalUnits);
        //}

        [HttpPost]
        public ActionResult OUSelected(Guid OUselect)
        {
            OrganisationalUnit orgunit = null;

            orgunit = _organisationalUnitRepository.GetById(OUselect);
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
            //foreach (var ou in orgunit.Locations)
            //{
            //   // Locations.Add(ou);
            //    organisationalUnits.Add(new LocationViewModel
            //    {
            //        LocationId = ou.LocationId,
            //        Street = ou.Street
            //    });
            //}

            return Json(locations);
        }

        [HttpPost]
        public ActionResult getUserEmail(String Useremail)
        {
            User user = null;
            var userName = "";

                    try
                    {
                       user = _userService.GetUserByEmail(Useremail);
                       userName = user.FirstName;
                      
                    }
                    catch (Exception ex)
                    {
                            if(user == null)
                           userName = "NotFound";

                    }
         
            return Json(userName);
        }




        [HttpPost]
        public ActionResult AddInterestedParty(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);
            try
            {
                InsuranceAttribute insuranceAttribute = _insuranceAttributeService.GetInsuranceAttributeByName(model.InsuranceAttribute);
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = _insuranceAttributeService.CreateNewInsuranceAttribute(CurrentUser, model.InsuranceAttribute);
                }

                OrganisationType organisationType = _organisationTypeService.GetOrganisationTypeByName(model.OrganisationTypeName);
                if (organisationType == null)
                {                    
                    organisationType = _organisationTypeService.CreateNewOrganisationType(CurrentUser, model.OrganisationTypeName);
                }


                //OrganisationType organisationType = _organisationTypeService.GetOrganisationTypeByName(model.OrganisationTypeName);
                //if (organisationType == null)
                //{
                //    organisationType = new OrganisationType(CurrentUser, model.OrganisationTypeName);
                //    _organisationTypeService.CreateNewOrganisationType(organisationType);

                //}
                Organisation organisation = null;
                User user = null;
                //if (model.InsuranceAttribute.EqualsIgnoreCase("Financial"))
                //{
                organisation = _organisationService.GetOrganisationByEmail(model.OrganisationEmail);
                if (organisation == null)
                {
                    organisation = new Organisation(CurrentUser, Guid.NewGuid(), model.OrganisationName, organisationType);
                    organisation.Phone = model.OrganisationPhone;
                    organisation.Email = model.OrganisationEmail;
                    _organisationService.CreateNewOrganisation(organisation);
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
                            user = _userService.GetUserByEmail(CurrentUser.Email);
                        }
                        else
                        {
                            user = _userService.GetUserByEmail(model.Email);
                        }
                    }
                    catch (Exception ex)
                    {
                        user = new User(CurrentUser, Guid.NewGuid(), model.FirstName);
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.FullName = model.FirstName + " " + model.LastName;
                        user.Email = model.Email;
                        user.Phone = model.Phone;
                        user.Password = "";
                        _userService.Create(user);

                    }

                    //    //if (user == null)
                    //    //{
                    //    //    user = new User(CurrentUser, Guid.NewGuid(), model.FirstName);
                    //    //    user.FirstName = model.FirstName;
                    //    //    user.LastName = model.LastName;
                    //    //    user.Email = model.Email;
                    //    //    user.Phone = model.Phone;
                    //    //    _userService.Create(user);
                    //    //}
                    //    //organisation = _organisationService.GetOrganisationByEmail(model.OrganisationEmail);
                    //    //if (organisation == null)
                    //    //{
                    //    //    organisation = new Organisation(CurrentUser, Guid.NewGuid(), model.OrganisationName, organisationType, user);
                    //    //    organisation.Phone = model.OrganisationPhone;
                    //    //    organisation.Email = model.OrganisationEmail;
                    //    //    _organisationService.CreateNewOrganisation(organisation);
                    //    //    user.SetPrimaryOrganisation(organisation);
                    //    //    user.Organisations.Add(organisation);

                    //    //}

                    }

                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        CurrentUser.Organisations.Add(organisation);
                        sheet.Organisation.Add(organisation);
                        model.ID = organisation.Id;

                        //NewMethod(uow);
                        uow.Commit();
                    }

                    //model.PartyUseId = organisation.Id;
                }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        //private static void NewMethod(IUnitOfWork uow)
        //{
        //    try
        //    {
        //        uow.Commit();

        //    }catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}


        [HttpPost]
        public ActionResult GetBoatUse(Guid answerSheetId, Guid boatUseId)
        {
            BoatUseViewModel model = new BoatUseViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
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
        public ActionResult GetBoatUses(Guid informationId, bool removed, bool ceased, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
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
        public ActionResult SetBoatUseRemovedStatus(Guid boatUseId, bool status)
        {
            BoatUse boatUse = _boatUseRepository.GetById(boatUseId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boatUse.Removed = status;
                uow.Commit();
            }
            return new JsonResult(true);
        }

        [HttpPost]
        public ActionResult SetBoatUseCeasedStatus(Guid boatUseId, bool status)
        {
            BoatUse boatUse = _boatUseRepository.GetById(boatUseId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                boatUse.BoatUseCeaseDate = DateTime.MinValue;
                boatUse.BoatUseCeaseReason = '0';
                uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Claim

        [HttpPost]
        public ActionResult AddClaim(ClaimViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Claim - No Client information for " + model.AnswerSheetId);

            Claim claim = _claimRepository.FindAll().FirstOrDefault(c => c.Id == model.ClaimId);
            // no claim, so create new
            if (claim == null)
                claim = model.ToEntity(CurrentUser);
            model.UpdateEntity(claim);

            if (model.OrganisationId != Guid.Empty)
            {
                Organisation org = _organisationService.GetOrganisation(model.OrganisationId);
                claim.Organisation = org;
            }

            if (model.ClaimProducts != null)
                claim.ClaimProducts = _productRepository.FindAll().Where(pro => model.ClaimProducts.Contains(pro.Id)).ToList();

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {

                sheet.Claims.Add(claim);
                uow.Commit();
            }

            return Json(model);
        }

        [HttpPost]
        public ActionResult GetClaim(Guid answerSheetId, Guid claimId)
        {
            ClaimViewModel model = new ClaimViewModel();
            ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
            Claim claim = sheet.Claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                model = ClaimViewModel.FromEntity(claim);
                model.AnswerSheetId = answerSheetId;
            }
            return Json(model);
        }

        [HttpGet]
        public ActionResult GetClaims(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
            if (sheet == null)
                throw new Exception("No valid information for id " + informationId);

            var claims = new List<Claim>();

            claims = sheet.Claims.Where(c => c.Removed == removed && c.DateDeleted == null).ToList();

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

                Claim claim = claims[i];
                JqGridRow row = new JqGridRow(claim.Id);
                row.AddValues(claim.Id, claim.ClaimTitle, claim.ClaimDescription, claim.ClaimReference, claim.Claimant, claim.Id);
                model.AddRow(row);
            }

            // convert model to XDocument for rendering.
            document = model.ToXml();
            return Xml(document);
        }

        [HttpPost]
        public ActionResult SetClaimRemovedStatus(Guid claimId, bool status)
        {
            Claim claim = _claimRepository.GetById(claimId);

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                claim.Removed = status;
                uow.Commit();
            }

            return new JsonResult(true);
        }

        #endregion

        #region Operators

        [HttpPost]
        public ActionResult AddOperator(OrganisationViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
            if (sheet == null)
                throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);
            try
            {
                InsuranceAttribute insuranceAttribute = _insuranceAttributeService.GetInsuranceAttributeByName("Skipper");
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = _insuranceAttributeService.CreateNewInsuranceAttribute(CurrentUser, "Skipper");
                }
                OrganisationType organisationType = _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
                if (organisationType == null)
                {
                    organisationType = _organisationTypeService.CreateNewOrganisationType(CurrentUser, "Person - Individual");
                }

                Organisation organisation = null;
                User user = null;
                User currentuser = null;
                try
                {
                            //if (model.IsAdmin.EqualsIgnoreCase("Yes"))
                            //{
                            //    user = _userService.GetUserByEmail(CurrentUser.Email);

                            //}
                            //else
                            //{

                    user = _userService.GetUserByEmail(model.Email);

                            //}
                }
                catch (Exception ex)
                {
                    user = new User(CurrentUser, Guid.NewGuid(), model.FirstName);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.FullName = model.FirstName + " " + model.LastName;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Password = "";

                    _userService.Create(user);

                }
                currentuser = _userService.GetUserByEmail(CurrentUser.Email);

                organisation = _organisationService.GetOrganisationByEmail(model.Email);
                if (organisation == null)
                {
                    var organisationName = model.FirstName + " " + model.LastName;
                    organisation = new Organisation(CurrentUser, Guid.NewGuid(), organisationName, organisationType, model.Email);
                            //organisation.OperatorYearsOfExp = model.OperatorYearsOfExp;
                            // organisation.OrganisationType = organisationType;
                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    insuranceAttribute.IAOrganisations.Add(organisation);
                    _organisationService.CreateNewOrganisation(organisation);
                    user.SetPrimaryOrganisation(organisation);

                }



                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    user.SetPrimaryOrganisation(organisation);
                    currentuser.Organisations.Add(organisation);
                    user.Organisations.Add(organisation);
                    sheet.Organisation.Add(organisation);
                    model.ID = organisation.Id;
                            //NewMethod(uow);
                    uow.Commit();
                }

                        //model.PartyUseId = organisationId;                                    
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Json(model);
        }

        //[HttpPost]
        //public ActionResult AddOperator1(OperatorViewModel model)
        //{
        //    if (model == null)
        //        throw new ArgumentNullException(nameof(model));

        //    ClientInformationSheet sheet = _clientInformationService.GetInformation(model.AnswerSheetId);
        //    if (sheet == null)
        //        throw new Exception("Unable to save Operator - No Client information for " + model.AnswerSheetId);

        //    Operator operato = _operatorRepository.FindAll().FirstOrDefault(oper => oper.Id == model.OperatorId);
        //    if (operato == null)
        //        operato = model.ToEntity(CurrentUser);
        //    model.UpdateEntity(operato);

        //    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
        //    {
        //        sheet.Operators.Add(operato);
        //        uow.Commit();
        //    }
        //    model.OperatorId = operato.Id;


        //    return Json(model);
        //}

        //[HttpPost]
        //public ActionResult GetOperator(Guid answerSheetId, Guid operatorId)
        //{
        //    OperatorViewModel model = new OperatorViewModel();
        //    ClientInformationSheet sheet = _clientInformationService.GetInformation(answerSheetId);
        //    Operator operato = sheet.Operators.FirstOrDefault(oper => oper.Id == operatorId);
        //    if (operato != null)
        //    {
        //        model = OperatorViewModel.FromEntity(operato);
        //        model.AnswerSheetId = answerSheetId;
        //    }
        //    return Json(model);
        //}

        //[HttpGet]
        //public ActionResult GetOperators(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
        //                                  string searchField, string searchString, string searchOper, string filters)
        //{
        //    ClientInformationSheet sheet = _clientInformationService.GetInformation(informationId);
        //    if (sheet == null)
        //        throw new Exception("No valid information for id " + informationId);

        //    var operators = new List<Operator>();

        //    operators = sheet.Operators.Where(oper => oper.Removed == removed && oper.DateDeleted == null).ToList();

        //    if (_search)
        //    {
        //        switch (searchOper)
        //        {
        //            case "eq":
        //                operators = operators.Where(searchField + " = \"" + searchString + "\"").ToList();
        //                break;
        //            case "bw":
        //                operators = operators.Where(searchField + ".StartsWith(\"" + searchString + "\")").ToList();
        //                break;
        //            case "cn":
        //                operators = operators.Where(searchField + ".Contains(\"" + searchString + "\")").ToList();
        //                break;
        //        }
        //    }
        //    operators = operators.OrderBy(sidx + " " + sord).ToList();

        //    XDocument document = null;
        //    JqGridViewModel model = new JqGridViewModel();
        //    model.Page = page;
        //    model.TotalRecords = operators.Count;
        //    model.TotalPages = ((model.TotalRecords - 1) / rows) + 1;

        //    int offset = rows * (page - 1);
        //    for (int i = offset; i < offset + rows; i++)
        //    {
        //        if (i == model.TotalRecords)
        //            break;

        //        Operator operato = operators[i];
        //        JqGridRow row = new JqGridRow(operato.Id);
        //        row.AddValues(operato.Id, operato.OperatorFirstName, operato.OperatorLastName, operato.Id);
        //        model.AddRow(row);
        //    }

        //    // convert model to XDocument for rendering.
        //    document = model.ToXml();
        //    return Xml(document);

        //}

        //[HttpPost]
        //public ActionResult SetOperatorRemovedStatus(Guid operatorId, bool status)
        //{
        //    Operator operato = _operatorRepository.GetById(operatorId);

        //    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
        //    {
        //        operato.Removed = status;
        //        uow.Commit();
        //    }

        //    return new JsonResult { Data = true };
        //}

        #endregion

        #region CoastGuardSelfReg

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> CoastGuardSelfRegAsync(string craftType, string membershipNumber, string boatType, string constructionType, string hullConfiguration, string mooredType, string trailered,
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
            organisationType = _organisationTypeService.GetOrganisationTypeByName(orgTypeName);
            if (organisationType == null)
            {
                organisationType = _organisationTypeService.CreateNewOrganisationType(null, orgTypeName);               
            }
            Organisation organisation = null;
            organisation = _organisationService.GetOrganisationByEmail(email);

            //condition for organisation exists
            if (organisation == null)
            {
                organisation = new Organisation(null, Guid.NewGuid(), organisationName, organisationType);
                organisation.Phone = phonenumber;
                organisation.Email = email;
                _organisationService.CreateNewOrganisation(organisation);                

                User user = null;
                User user2 = null;

                try
                {
                    user = _userService.GetUserByEmail(email);
                }
                catch (Exception)
                {
                    string username = firstName + "_" + lastName;

                    try
                    {
                        user2 = _userService.GetUser(username);

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
                        //var personalOrganisation = new Organisation (CurrentUser, Guid.NewGuid (), personalOrganisationName, new OrganisationType (CurrentUser, "personal"));
                        //_organisationService.CreateNewOrganisation (personalOrganisation);
                        // create user object
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
                        _userService.Create(user);
                        //Console.WriteLine ("Created User " + user.FullName);
                    }
                }
                finally
                {
                    if (!user.Organisations.Contains(organisation))
                        user.Organisations.Add(organisation);

                    _userService.Update(user);
                    user.SetPrimaryOrganisation(organisation);
                }

                var programme = _programmeService.GetAllProgrammes().FirstOrDefault(p => p.Name == "Demo Coastguard Programme"); //Marsh Coastguard
                var clientProgramme = _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);

                User user3 = _userService.GetUserByEmail(email);
                var reference = _referenceService.GetLatestReferenceId();
                var sheet = _clientInformationService.IssueInformationFor(user3, organisation, clientProgramme, reference);
                _referenceService.CreateClientInformationReference(sheet);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    OrganisationalUnit ou = new OrganisationalUnit(user3, ouname);
                    Boat vessel = new Boat(user3)
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
                    sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(user, sheet, null, "Quick Quote Consuming Process Completed"));
                    uow.Commit();
                }

                //send out login email
                _emailService.SendSystemEmailLogin(email);
                //send out instruction email
                EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetInstruction");
                if (emailTemplate != null)
                {
                    _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null);
                }
                else
                {
                    throw new Exception("There is no Information Sheet Instruction email template been set up.");
                }
                //send out information sheet issue notification email
               _emailService.SendSystemEmailUISIssueNotify(programme.BrokerContactUser, programme, clientProgramme.InformationSheet, organisation);
                
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
    }
}
