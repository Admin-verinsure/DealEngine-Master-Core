using System;
using System.Collections.Generic;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using System.Xml.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models;
using DealEngine.WebUI.Models.ControlModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack;
using System.Threading;
using System.Threading.Tasks;
using DealEngine.WebUI.Helpers;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;

namespace DealEngine.WebUI.Controllers
{

    public class ServicesController : BaseController
    {
        IClientInformationService _clientInformationService;
        IClientAgreementService _clientAgreementService;
        IOrganisationalUnitService _organisationalUnitService;
        ILocationService _locationService;
        IMapperSession<WaterLocation> _waterLocationRepository;
        IMapperSession<Boat> _boatRepository;        
        IBoatUseService _boatUseService;
        IVehicleService _vehicleService;
        IOrganisationService _organisationService;        
        IMapperSession<Building> _buildingRepository;              
        IMapperSession<BusinessInterruption> _businessInterruptionRepository;
        IMapperSession<MaterialDamage> _materialDamageRepository;        
        IClaimNotificationService _claimNotificationService;
        IProductService _productService;        
        IProgrammeService _programmeService;
        IOrganisationTypeService _organisationTypeService;
        IUnitOfWork _unitOfWork;        
        IReferenceService _referenceService;
        IEmailService _emailService;
        IAppSettingService _appSettingService;
        IInsuranceAttributeService _insuranceAttributeService;
        IBusinessContractService _businessContractService;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<ServicesController> _logger;
        IMapper _mapper;


        public ServicesController(
            IMapper mapper,
            ILogger<ServicesController> logger,
            IApplicationLoggingService applicationLoggingService,
            IBusinessContractService businessContractService,
            IUserService userService, 
            IClientAgreementService clientAgreementService, 
            IAppSettingService appSettingService,             
            IClientInformationService clientInformationService,            
            IOrganisationalUnitService organisationalUnitService,            
            ILocationService locationService,
            IMapperSession<WaterLocation> waterLocationRepository, 
            IMapperSession<Building> buildingRepository, 
            IMapperSession<BusinessInterruption> businessInterruptionRepository,
            IMapperSession<MaterialDamage> materialDamageRepository,
            IClaimNotificationService claimNotificationService,
            IProductService productService,
            IVehicleService vehicleService, 
            IMapperSession<Boat> boatRepository,
            IOrganisationService organisationService, 
            IBoatUseService boatUseService, 
            IProgrammeService programeService, 
            IOrganisationTypeService organisationTypeService,             
            IEmailService emailService,             
            IUnitOfWork unitOfWork, 
            IInsuranceAttributeService insuranceAttributeService, 
            IReferenceService referenceService
            )

            : base(userService)
        {
            _mapper = mapper;
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _clientAgreementService = clientAgreementService;
            _appSettingService = appSettingService;            
            _clientInformationService = clientInformationService;
            _organisationalUnitService = organisationalUnitService;            
            _vehicleService = vehicleService;
            _locationService = locationService;
            _waterLocationRepository = waterLocationRepository;
            _boatRepository = boatRepository;            
            _organisationService = organisationService;
            _boatUseService = boatUseService;
            _buildingRepository = buildingRepository;
            _businessInterruptionRepository = businessInterruptionRepository;
            _materialDamageRepository = materialDamageRepository;
            _claimNotificationService = claimNotificationService;
            _productService = productService;
            _programmeService = programeService;
            _organisationTypeService = organisationTypeService;
            _unitOfWork = unitOfWork;            
            _referenceService = referenceService;
            _emailService = emailService;            
            _insuranceAttributeService = insuranceAttributeService;
            _businessContractService = businessContractService;

        }

        #region Vehicle

        [HttpPost]
        public async Task<IActionResult> SearchVehicle(string registration)
        {
            VehicleViewModel model = new VehicleViewModel();
            User user = null;
            try
            {
                user = await CurrentUser();
                model.Registration = registration;
                model.Make = "Not Found";
                model.VehicleModel = "Not Found";
                model.SumInsured = 5000;


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
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleViewModel model)
        {
            User user = null;
            try
            {
                user = await CurrentUser();

                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Vehicle - No Client information for " + model.AnswerSheetId);

                // get existing vehicle (if any)
                Vehicle vehicle = await _vehicleService.GetVehicleById(model.VehicleId);
                // no vehicle, so create new
                if (vehicle == null)
                    vehicle = model.ToEntity(user);
                model.UpdateEntity(vehicle);

                if (model.VehicleLocation != Guid.Empty)
                    vehicle.GarageLocation = await _locationService.GetLocationById(model.VehicleLocation);

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
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }                       
        }

        [HttpPost]
        public async Task<IActionResult> GetVehicle(Guid answerSheetId, Guid vehicleId)
        {
            VehicleViewModel model = new VehicleViewModel();
            User user = null;
            try
            {
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                Vehicle vehicle = sheet.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
                if (vehicle != null)
                {
                    model = VehicleViewModel.FromEntity(vehicle);
                    model.AnswerSheetId = answerSheetId;

                    if (vehicle.GarageLocation != null)
                        model.VehicleLocation = vehicle.GarageLocation.Id;
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicles(Guid informationId, bool validated, bool removed, bool transfered, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;
            var vehicles = new List<Vehicle>();

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
                if (sheet == null)
                    throw new Exception("No valid information for id " + informationId);

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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }


        [HttpGet]
        public async Task<IActionResult> GetPrincipalPartners(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;
            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);

                if (sheet == null)
                    throw new Exception("No valid information for id " + informationId);

                var organisations = await _organisationService.GetOrganisationPrincipals(sheet);
                
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
                document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeletedPartners(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                       string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                XDocument document = null;
                JqGridViewModel model = new JqGridViewModel();
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);

                var userList = await _userService.GetAllUsers();
                User userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
                if (sheet == null)
                    throw new Exception("No valid information for id " + informationId);

                var organisations = new List<Organisation>();
                 foreach ( var org in sheet.Organisation.Where(o => o.Removed == true))
                        {
                            organisations.Add(org);
                        }

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
                document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }


        
        [HttpGet]
        public async Task<IActionResult> GetPminzNamedParties(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;
            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
                if (sheet == null)
                    throw new Exception("No valid information for id " + informationId);

                var organisations = new List<Organisation>();
                foreach (var org in sheet.Organisation.Where(o => o.Removed == removed))
                {
                    organisations.Add(org);
                }
                

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

                document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetNamedParties(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;
            XDocument document = null;
            JqGridViewModel model = new JqGridViewModel();

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
                if (sheet == null)
                    throw new Exception("No valid information for id " + informationId);

                var organisations = new List<Organisation>();
                for (var i = 0; i < sheet.Organisation.Count; i++)
                {
                    organisations.Add(sheet.Organisation.ElementAtOrDefault(i));
                }

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

                document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> SetVehicleRemovedStatus(Guid vehicleId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Vehicle vehicle = await _vehicleService.GetVehicleById(vehicleId);
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    vehicle.Removed = status;
                    await uow.Commit();
                }
                //return new JsonResult(true);
                return new JsonResult(new { status = true, id = vehicleId });
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> SetVehicleCeasedStatus(Guid vehicleId, bool status, DateTime ceaseDate, int ceaseReason)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Vehicle vehicle = await _vehicleService.GetVehicleById(vehicleId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    vehicle.VehicleCeaseDate = DateTime.Parse(LocalizeTime(ceaseDate, "d"));
                    vehicle.VehicleCeaseReason = ceaseReason;
                    await uow.Commit().ConfigureAwait(false);
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetVehicleTransferedStatus(Guid vehicleId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Vehicle vehicle = await _vehicleService.GetVehicleById(vehicleId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    vehicle.VehicleCeaseDate = DateTime.MinValue;
                    vehicle.VehicleCeaseReason = '0';
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevalidateVehicle(Guid vehicleId)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Vehicle vehicle = await _vehicleService.GetVehicleById(vehicleId);
                if (vehicle == null)
                    throw new Exception("Vehicle is null");
                if (vehicle.Validated == false)
                {
                    if (string.IsNullOrWhiteSpace(vehicle.Registration))
                        throw new Exception("Unable to revalidate a non registered & validated vehicle");
                }

                return null;
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region Organisational Units

        [HttpPost]
        public async Task<IActionResult> SearchOrganisationalUnit(Guid answerSheetId, string name)
        {
            OrganisationalUnitViewModel model = new OrganisationalUnitViewModel();
            User user = null;
            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganisationalUnit(OrganisationalUnitViewModel model)
        {
            User user = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Organisational Unit - No Client information for " + model.AnswerSheetId);

                OrganisationalUnit ou = await _organisationalUnitService.GetOrganisationalUnit(model.OrganisationalUnitId);
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
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> GetOrganisationalUnit(Guid answerSheetId, Guid unitId)
        {
            OrganisationalUnitViewModel model = new OrganisationalUnitViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisationalUnits(Guid informationId, bool _search, string nd, int rows, int page, string sidx, string sord,
                                                     string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisationalUnitName(string term)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var organisationalUnitNameList = await _organisationalUnitService.GetAllOrganisationalUnitsName();
                var results = organisationalUnitNameList.Where(n => n.ToLower().Contains(term.ToLower()));

                return new JsonResult(results.ToArray());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region Locations

        [HttpPost]
        public async Task<IActionResult> SearchLocationStreet(Guid answerSheetId, string street)
        {
            LocationViewModel model = new LocationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                model.Street = street;
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                Location location = await _locationService.GetLocationByStreet(street);
                if (location != null)
                {
                    model = LocationViewModel.FromEntity(location);
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationViewModel model)
        {
            User user = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Location - No Client information for " + model.AnswerSheetId);

                Location location = await _locationService.GetLocationById(model.LocationId);
                if (location == null)
                    location = model.ToEntity(user);
                model.UpdateEntity(location);
                var OUList = new List<OrganisationalUnit>();

                if (sheet.Owner.OrganisationalUnits.Count > 0)
                    OUList.Add(sheet.Owner.OrganisationalUnits.ElementAtOrDefault(0));

                location.OrganisationalUnits = OUList;
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.Locations.Add(location);
                    await uow.Commit();
                }

                model.LocationId = location.Id;

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }                        
        }

        [HttpPost]
        public async Task<IActionResult> GetLocation(Guid answerSheetId, Guid locationId)
        {
            LocationViewModel model = new LocationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
           
        }


        [HttpGet]
        public async Task<IActionResult> GetLocationss(Guid informationId, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(informationId);
                if (sheet == null)
                    throw new Exception("No valid information for id " + informationId);

                var locations = sheet.Owner.OrganisationalUnits.SelectMany(ou => ou.Locations).Distinct().ToList();

                locations = sheet.Locations.Where(loc => loc.Removed == false && loc.DateDeleted == null).ToList();

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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationStreet(string term)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                var locationStreetList = await _locationService.GetLocationStreetList();
                var results = locationStreetList.Where(n => n.ToLower().Contains(term.ToLower()));
                return new JsonResult(results.ToArray());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationList(Guid answerSheetId)
        {
            List<LocationViewModel> models = new List<LocationViewModel>();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                foreach (var location in sheet.Locations)
                    models.Add(LocationViewModel.FromEntity(location));

                return new JsonResult(models.ToArray());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationsByCountry(Guid answerSheetId)
        {
            User user = null;
            List<LocationViewModel> models = new List<LocationViewModel>();

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                List<Location> countries = sheet.Locations.GroupBy(loc => loc.Country)
                                              .Select(grp => grp.First())
                                              .ToList();

                foreach (var location in countries)
                    models.Add(LocationViewModel.FromEntity(location));

                return new JsonResult(models.ToArray());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetLocationRemovedStatus(Guid locationId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Location location = await _locationService.GetLocationById(locationId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    location.Removed = status;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> SetPrincipalRemovedStatus(Guid answersheetId, Guid principalId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Organisation org = await _organisationService.GetOrganisation(principalId);
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answersheetId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    org.Removed = status;
                  
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region Buildings

        [HttpPost]
        public async Task<IActionResult> AddBuilding(BuildingViewModel model)
        {
            User user = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Building - No Client information for " + model.AnswerSheetId);

                Building building = _buildingRepository.FindAll().FirstOrDefault(bui => bui.Id == model.BuildingId);
                if (building == null)
                    building = model.ToEntity(user);
                model.UpdateEntity(building);

                if (model.BuildingLocation != null)
                    building.Location = await _locationService.GetLocationById(model.BuildingLocation);

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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetBuilding(Guid answerSheetId, Guid buildingId)
        {
            BuildingViewModel model = new BuildingViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBuildings(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetBuildingRemovedStatus(Guid buildingId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Building building = await _buildingRepository.GetByIdAsync(buildingId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    building.Removed = status;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        #endregion

        #region WaterLocations

        [HttpPost]
        public async Task<IActionResult> SearchWaterLocationName(Guid answerSheetId, string waterLocationName)
        {
            WaterLocationViewModel model = new WaterLocationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                model.WaterLocationName = waterLocationName;
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                WaterLocation waterLocation = _waterLocationRepository.FindAll().FirstOrDefault(wl => wl.WaterLocationName == waterLocationName);
                if (waterLocation != null)
                {
                    model = WaterLocationViewModel.FromEntity(waterLocation);
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddWaterLocation(WaterLocationViewModel model)
        {
            User user = null;
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Water Location - No Client information for " + model.AnswerSheetId);

                WaterLocation waterLocation = _waterLocationRepository.FindAll().FirstOrDefault(wloc => wloc.Id == model.WaterLocationId);
                if (waterLocation == null)
                    waterLocation = model.ToEntity(user);
                model.UpdateEntity(waterLocation);

                if (model.WaterLocationLocation != Guid.Empty)
                    waterLocation.WaterLocationLocation = await _locationService.GetLocationById(model.WaterLocationLocation);
                if (model.WaterLocationMarinaLocation != null)
                {
                    waterLocation.WaterLocationMarinaLocation = await _organisationService.GetOrganisation(model.WaterLocationMarinaLocation);

                }
                if (model.OrganisationalUnit != null)
                {
                    waterLocation.OrganisationalUnit = await _organisationalUnitService.GetOrganisationalUnit(model.OrganisationalUnit);

                }
                // waterLocation.OrganisationalUnit = OrganisationalUnit;

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.WaterLocations.Add(waterLocation);
                    await uow.Commit();
                }

                model.WaterLocationId = waterLocation.Id;
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> GetWaterLocation(Guid answerSheetId, Guid waterLocationId)
        {
            WaterLocationViewModel model = new WaterLocationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        //removing this through new bootstrap
        [HttpGet]
        public async Task<IActionResult> GetWaterLocations(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetWaterLocationName(string term)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var waterLocationNameList = _waterLocationRepository.FindAll().Select(wl => wl.WaterLocationName);
                var results = waterLocationNameList.Where(n => n.ToLower().Contains(term.ToLower()));
                return new JsonResult(results.ToArray());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetWaterLocationList(Guid answerSheetId)
        {
            List<WaterLocationViewModel> models = new List<WaterLocationViewModel>();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                foreach (var waterLocation in sheet.WaterLocations)
                    models.Add(WaterLocationViewModel.FromEntity(waterLocation));

                return new JsonResult(models.ToArray());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetWaterLocationRemovedStatus(Guid waterLocationId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                WaterLocation waterLocation = await _waterLocationRepository.GetByIdAsync(waterLocationId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    waterLocation.Removed = status;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region BusinessInterruption

        [HttpPost]
        public async Task<IActionResult> AddBusinessInterruption(BusinessInterruptionViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save BusinessInterruption - No Client information for " + model.AnswerSheetId);

                BusinessInterruption businessInterruption = _businessInterruptionRepository.FindAll().FirstOrDefault(bi => bi.Id == model.BusinessInterruptionId);
                if (businessInterruption == null)
                    businessInterruption = model.ToEntity(user);
                model.UpdateEntity(businessInterruption);

                if (model.BusinessInterruptionLocation != null)
                    businessInterruption.Location = await _locationService.GetLocationById(model.BusinessInterruptionLocation);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.BusinessInterruptions.Add(businessInterruption);
                    await uow.Commit();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> GetBusinessInterruption(Guid answerSheetId, Guid businessInterruptionId)
        {
            BusinessInterruptionViewModel model = new BusinessInterruptionViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessInterruptions(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> SetBusinessInterruptionRemovedStatus(Guid businessInterruptionId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                BusinessInterruption businessInterruption = await _businessInterruptionRepository.GetByIdAsync(businessInterruptionId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    businessInterruption.Removed = status;
                    await uow.Commit();
                }


                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region MaterialDamage

        [HttpPost]
        public async Task<IActionResult> AddMaterialDamage(MaterialDamageViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save BusinessInterruption - No Client information for " + model.AnswerSheetId);

                MaterialDamage materialDamage = _materialDamageRepository.FindAll().FirstOrDefault(md => md.Id == model.MaterialDamageId);
                if (materialDamage == null)
                    materialDamage = model.ToEntity(user);
                model.UpdateEntity(materialDamage);

                if (model.MaterialDamageLocation != null)
                    materialDamage.Location = await _locationService.GetLocationById(model.MaterialDamageLocation);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.MaterialDamages.Add(materialDamage);
                    await uow.Commit();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetMaterialDamage(Guid answerSheetId, Guid materialDamageId)
        {
            MaterialDamageViewModel model = new MaterialDamageViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialDamages(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetMaterialDamageRemovedStatus(Guid materialDamageId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                MaterialDamage materialDamage = await _materialDamageRepository.GetByIdAsync(materialDamageId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    materialDamage.Removed = status;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region Boat

        [HttpPost]
        public async Task<IActionResult> AddUsetoBoat(string[] Boatuse, Guid BoatId)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Boat boat = await _boatRepository.GetByIdAsync(BoatId);

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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddBoat(BoatViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat - No Client information for " + model.AnswerSheetId);
                // get existing boat (if any)
                Boat boat = await _boatRepository.GetByIdAsync(model.BoatId);
                // no boat, so create new
                if (boat == null)
                    boat = model.ToEntity(user);
                model.UpdateEntity(boat);
                if (model.BoatLandLocation != Guid.Empty)
                    boat.BoatLandLocation = await _buildingRepository.GetByIdAsync(model.BoatLandLocation);

                if (model.BoatOperator != Guid.Empty)
                    boat.BoatOperator = await _organisationService.GetOrganisation(model.BoatOperator);
                boat.BoatWaterLocation = null;

                if (model.BoatWaterLocation != Guid.Empty)
                    boat.BoatWaterLocation = await _organisationService.GetOrganisation(model.BoatWaterLocation);

                if (model.OtherMarinaName != null)
                {
                    boat.OtherMarinaName = model.OtherMarinaName;
                    boat.OtherMarina = true;
                }
                else
                {
                    boat.OtherMarina = false;

                }
                if (model.SelectedBoatUse != null )
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

                if (model.SelectedInterestedParty != null )
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

                if (model.BoatTrailer != Guid.Empty)
                    boat.BoatTrailer = await _vehicleService.GetVehicleById(model.BoatTrailer);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.Boats.Add(boat);
                    await uow.Commit();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> GetOriginalVehicle(Guid answerSheetId, Guid vehicleId)
        {
            VehicleViewModel model = new VehicleViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> GetOriginalBoat(Guid answerSheetId, Guid boatId)
        {
            BoatViewModel model = new BoatViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetBoat(Guid answerSheetId, Guid boatId)
        {
            BoatViewModel model = new BoatViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
                        model.BoatselectedVal = new List<String>();

                    model.BoatselectedText = new List<Guid>();

                    foreach (var boatuse in boat.BoatUse)
                    {
                        model.BoatselectedVal.Add(boatuse.BoatUseCategory);
                        model.BoatselectedText.Add(boatuse.Id);
                    }
                }

                model.BoatpartyVal = new List<string>();
                model.BoatpartyText = new List<Guid>();

                foreach (var boatparty in boat.InterestedParties)
                {
                    model.BoatpartyVal.Add(boatparty.Name);
                    model.BoatpartyText.Add(boatparty.Id);
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }


        [HttpGet]
        public async Task<IActionResult> GetBoats(Guid informationId, bool validated, bool removed, bool transfered, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser(); 
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatRemovedStatus(Guid boatId, bool status)
        {
            User user = null;

            try
            {
                bool hasTrailer = false;
                user = await CurrentUser();
                Boat boat = await _boatRepository.GetByIdAsync(boatId);

                if(boat.BoatTrailer != null)
                {
                    hasTrailer = true;
                }

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    boat.Removed = status;
                    await uow.Commit();
                }
                return Json(new { HasTrailer = hasTrailer, Success = true });                
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }



        [HttpPost]
        public async Task<IActionResult> UndoBoatRemovedStatus(BoatViewModel removedboat)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Boat boat = await _boatRepository.GetByIdAsync(removedboat.BoatId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    boat.Removed = removedboat.Removed;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }           
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatCeasedStatus(Guid boatId, bool status, DateTime ceaseDate, int ceaseReason)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Boat boat = await _boatRepository.GetByIdAsync(boatId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {

                    boat.BoatCeaseDate = DateTime.Parse(LocalizeTime(ceaseDate, "d"));

                    boat.BoatCeaseReason = ceaseReason;
                    await uow.Commit().ConfigureAwait(false);
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatTransferedStatus(Guid boatId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Boat boat = await _boatRepository.GetByIdAsync(boatId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    boat.BoatCeaseDate = DateTime.MinValue;
                    boat.BoatCeaseReason = '0';
                    await uow.Commit().ConfigureAwait(false);
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region BoatUse

        [HttpPost]
        public async Task<IActionResult> AddBoatUse(BoatUseViewModel model)
        {
            User user = null;
            BoatUse boatUse = null;
            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

                // get existing boat (if any)
                if (model.BoatUseId != Guid.Parse("00000000-0000-0000-0000-000000000000")) //to use Edit mode to add new org
                {
                     boatUse = await _boatUseService.GetBoatUse(model.BoatUseId);
                    if (boatUse == null)
                        boatUse = model.ToEntity(user);
                }
                else
                {
                    boatUse = model.ToEntity(user);
                }

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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> AddPrincipalDirectors(OrganisationViewModel model)
        {
            User currentUser = null;
            try
            {
                currentUser = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                    organisation.OtherCompanyname = model.Othercompanyname;
                    organisation.Activities = model.Activities;
                    organisation.Email = userdb.Email;
                    organisation.Type = model.Type;
                    organisation.IsIPENZmember = model.IsIPENZmember;
                    organisation.CPEngQualified = model.CPEngQualified;
                    if (model.DateofBirth != null)
                    {
                        organisation.DateofBirth = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofBirth), "d"));
                    }

                    if (model.DateofRetirement != null)
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
                        userdb.Organisations.Add(organisation);
                        sheet.Organisation.Add(organisation);
                        model.ID = organisation.Id;
                        await uow.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }



        [HttpPost]
        public async Task<IActionResult> AddCEASPrincipalDirectors(OrganisationViewModel model)
        {
            User currentUser = null;
            try
            {
                currentUser = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save - No Client information for " + model.AnswerSheetId);
                string orgTypeName = "";

                try
                {
                    if (model.Type != "Principal")
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
                    else
                    {
                        orgTypeName = "Person - Individual";
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                    organisation = _mapper.Map<Organisation>(model);
                    //organisation.Qualifications = model.Qualifications;
                    //organisation.IsNZIAmember = model.IsNZIAmember;
                    //organisation.NZIAmembership = model.NZIAmembership;
                    //organisation.IsADNZmember = model.IsADNZmember;
                    //organisation.IsRetiredorDecieved = model.IsRetiredorDecieved;
                    //organisation.IsLPBCategory3 = model.IsLPBCategory3;
                    //organisation.YearofPractice = model.YearofPractice;
                    //organisation.PrevPractice = model.prevPractice;
                    //organisation.IsOtherdirectorship = model.IsOtherdirectorship;
                    //organisation.OtherCompanyname = model.Othercompanyname;
                    //organisation.Activities = model.Activities;
                    //organisation.Email = userdb.Email;
                    //organisation.Type = model.Type;
                    //organisation.IsIPENZmember = model.IsIPENZmember;
                    //organisation.CPEngQualified = model.CPEngQualified;
                    if (model.DateofBirth != null)
                    {
                        organisation.DateofBirth = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofBirth), "d"));
                    }

                    if (model.DateofRetirement != null)
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
                        userdb.Organisations.Add(organisation);
                        sheet.Organisation.Add(organisation);
                        model.ID = organisation.Id;
                        await uow.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> EditPrincipalDirectors(OrganisationViewModel model)
        {
            User currentUser = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                currentUser = await CurrentUser();
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
                    if(model.ID != Guid.Parse("00000000-0000-0000-0000-000000000000")) //to use Edit mode to add new org
                    {
                        organisation = await _organisationService.GetOrganisation(model.ID);

                    }
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

                        }else {

                        var userList = await _userService.GetAllUsers();
                        userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
                         }

                    }catch (Exception ex)
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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

                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (organisation != null)
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
                            organisation.OtherCompanyname = model.Othercompanyname;
                            organisation.IsRetiredorDecieved = model.IsRetiredorDecieved;

                            organisation.Activities = model.Activities;
                            organisation.Email = userdb.Email;
                            organisation.Type = model.Type;
                            if (model.DateofRetirement != null)
                            {
                                organisation.DateofRetirement = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofRetirement), "d"));
                            }
                            if (model.DateofDeceased != null)
                            {
                                organisation.DateofDeceased = DateTime.Parse(LocalizeTime(DateTime.Parse(model.DateofDeceased), "d"));
                            }
                        }
                        else
                        {
                          
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
                            organisation.OtherCompanyname = model.Othercompanyname;
                            organisation.Activities = model.Activities;
                            organisation.Email = userdb.Email;
                            organisation.Type = model.Type;
                            if (model.DateofRetirement != null)
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
                            userdb.Organisations.Add(organisation);
                            sheet.Organisation.Add(organisation);
                            model.ID = organisation.Id;
                                
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddPMINZNamedParty(OrganisationViewModel model)
        {
            User currentUser = null;
            try
            {
                currentUser = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save - No Client information for " + model.AnswerSheetId);
                string orgTypeName = "";
                if(model.Type == "project management personnel")
                {
                    model.OrganisationTypeName = "Person - Individual";
                }

                try
                {
                    switch (model.OrganisationTypeName)
                    {
                        case "Person - Individual":
                            {
                                orgTypeName = "Person - Individual";
                                break;
                            }
                        case "Corporation – Limited liability":
                            {
                                orgTypeName = "Corporation – Limited liability";
                                break;
                            }
                        case "Trust":
                            {
                                orgTypeName = "Corporation – Limited liability";
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

                    User userdb = null;
                    Organisation organisation = null;
                    if (model.ID != Guid.Parse("00000000-0000-0000-0000-000000000000")) //to use Edit mode to add new org
                    {
                        organisation = await _organisationService.GetOrganisation(model.ID);

                    }
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                    organisation.IsAffiliation = model.isaffiliation;
                    organisation.AffiliationDetails = model.affiliationdetails;
                    organisation.ProfAffiliation = model.ProfAffiliation;
                    organisation.JobTitle = model.JobTitle;
                    organisation.Email = model.Email;
                    organisation.InsuredEntityRelation = model.InsuredEntityRelation;
                    organisation.IsContractorInsured = model.IsContractorInsured;
                    organisation.IsInsuredRequired = model.IsInsuredRequired;
                    organisation.IsCurrentMembership = model.IsCurrentMembership;
                    organisation.PMICert = model.PMICert;
                    organisation.CurrentMembershipNo = model.CurrentMembershipNo;
                    organisation.CertType = model.CertType;
                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    insuranceAttribute.IAOrganisations.Add(organisation);
                    await _organisationService.CreateNewOrganisation(organisation);

                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        userdb.Organisations.Add(organisation);
                        sheet.Organisation.Add(organisation);
                        model.ID = organisation.Id;
                        await uow.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }



        [HttpPost]
        public async Task<IActionResult> EditPMINZNamedParty(OrganisationViewModel model)
        {
            User currentUser = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                currentUser = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);
                string orgTypeName = "";
                if (model.Type == "project management personnel")
                {
                    model.OrganisationTypeName = "Person - Individual";
                }
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
                            case "Corporation – Limited liability":
                                {
                                    orgTypeName = "Corporation – Limited liability";
                                    break;
                                }
                            case "Trust":
                                {
                                    orgTypeName = "Corporation – Limited liability";
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
                    if (model.ID != Guid.Parse("00000000-0000-0000-0000-000000000000")) //to use Edit mode to add new org
                    {
                        organisation = await _organisationService.GetOrganisation(model.ID);

                    }
                    try
                    {
                        if (orgTypeName == "Person - Individual")
                        {
                            userdb = await _userService.GetUserByEmail(model.Email);
                            if (userdb == null)
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
                        else
                        {

                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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
                            var userList = await _userService.GetAllUsers();
                            userdb = userList.FirstOrDefault(user => user.PrimaryOrganisation == sheet.Owner);
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

                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (organisation != null)
                        {
                            organisation.ChangeOrganisationName(organisationName);
                            organisation.Qualifications = model.Qualifications;
                            organisation.Type = model.Type;
                            organisation.IsAffiliation = model.isaffiliation;
                            organisation.AffiliationDetails = model.affiliationdetails;
                            organisation.ProfAffiliation = model.ProfAffiliation;
                            organisation.JobTitle = model.JobTitle;
                            organisation.Email = model.Email;
                            organisation.InsuredEntityRelation = model.InsuredEntityRelation;
                            organisation.IsContractorInsured = model.IsContractorInsured;
                            organisation.IsInsuredRequired = model.IsInsuredRequired;
                            organisation.PMICert = model.PMICert;
                            organisation.CertType = model.CertType;
                           
                        }
                        else
                        {

                            organisation = new Organisation(currentUser, Guid.NewGuid(), organisationName, organisationType, userdb.Email);
                            organisation.Qualifications = model.Qualifications;
                            organisation.Type = model.Type;
                            organisation.IsAffiliation = model.isaffiliation;
                            organisation.PartyName = model.PartyName;
                            organisation.AffiliationDetails = model.affiliationdetails;
                            organisation.ProfAffiliation = model.ProfAffiliation;
                            organisation.JobTitle = model.JobTitle;
                            organisation.Email = model.Email;
                            organisation.InsuredEntityRelation = model.InsuredEntityRelation;
                            organisation.IsContractorInsured = model.IsContractorInsured;
                            organisation.IsInsuredRequired = model.IsInsuredRequired;
                            organisation.PMICert = model.PMICert;
                            organisation.CertType = model.CertType;
                            organisation.CurrentMembershipNo = model.CurrentMembershipNo;
                            organisation.InsuranceAttributes.Add(insuranceAttribute);
                            insuranceAttribute.IAOrganisations.Add(organisation);
                            await _organisationService.CreateNewOrganisation(organisation);
                            userdb.Organisations.Add(organisation);
                            sheet.Organisation.Add(organisation);
                            model.ID = organisation.Id;

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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }


        [HttpPost]
        public async Task<IActionResult> GetPMINZNamedParties(Guid answerSheetId, Guid partyID)
        {
            OrganisationViewModel model = new OrganisationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                Organisation org = sheet.Organisation.FirstOrDefault(o => o.Id == partyID);
                if (org != null)
                {
                    User userdb = await _userService.GetUserByEmail(org.Email);

                    model.ID = partyID;
                    model.FirstName = userdb.FirstName;
                    model.LastName = userdb.LastName;
                    model.Email = org.Email;
                    model.Qualifications = org.Qualifications;
                    model.isaffiliation = org.IsAffiliation;
                    model.affiliationdetails = org.AffiliationDetails;
                    model.ProfAffiliation = org.ProfAffiliation;
                    model.JobTitle = org.JobTitle;
                    model.InsuredEntityRelation = org.InsuredEntityRelation;
                    model.PartyName = org.PartyName;
                    model.IsContractorInsured = org.IsContractorInsured;
                    model.IsInsuredRequired = org.IsInsuredRequired;
                    model.PMICert = org.PMICert;
                    model.CertType = org.CertType;
                    model.OrganisationTypeName = org.OrganisationType.Name;
                    model.Type = org.InsuranceAttributes.First().InsuranceAttributeName;
                    model.IsCurrentMembership = org.IsCurrentMembership;
                    model.CurrentMembershipNo = org.CurrentMembershipNo;
                    model.OrganisationName = org.Name;
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

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }



        [HttpPost]
        public async Task<IActionResult> EditPrincipalDirectorsOwner(OrganisationViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                Organisation org = await _organisationService.GetOrganisation(sheet.Owner.Id);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    org.Email = model.Email;
                    org.ChangeOrganisationName(model.OrganisationName);
                    await uow.Commit();

                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPrincipalPartners(Guid answerSheetId, Guid partyID)
        {
            OrganisationViewModel model = new OrganisationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                Organisation org = sheet.Organisation.FirstOrDefault(o => o.Id == partyID);
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
                    if (org.DateofBirth != null)
                    {
                        model.DateofBirth = (org.DateofBirth > DateTime.MinValue) ? org.DateofBirth.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "";
                    }
                    model.IsIPENZmember = org.IsIPENZmember;
                    model.CPEngQualified = org.CPEngQualified;

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
                    model.Othercompanyname = org.OtherCompanyname;
                    model.Type = org.InsuranceAttributes.First().InsuranceAttributeName;
                    model.DateofDeceased = (org.DateofDeceased > DateTime.MinValue) ? org.DateofDeceased.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "";
                    model.DateofRetirement = (org.DateofRetirement > DateTime.MinValue) ? org.DateofRetirement.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "";
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

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


       
        [HttpPost]
        public async Task<IActionResult> AddNamedParty(OrganisationViewModel model)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

                Organisation organisation = null;

                organisation = await _organisationService.GetOrganisation(model.ID);
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    organisation.ChangeOrganisationName(model.OrganisationName);
                    organisation.Phone = model.OrganisationPhone;
                    organisation.Email = model.OrganisationEmail;
                    await uow.Commit();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetNamedParty(Guid answerSheetId, Guid partyID)
        {
            OrganisationViewModel model = new OrganisationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }


        [HttpPost]
        public async Task<IActionResult> AddMarina(OrganisationViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);


                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName("Other Marina");
                user = await CurrentUser();
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
                    await uow.Commit();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetMooredType(Guid OrgID)
        {
            Organisation organisation = null;
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> OUSelected(Guid OUselect)
        {
            OrganisationalUnit orgunit = null;
            var locations = new List<LocationViewModel>();
            User user = null;

            try
            {
                user = await CurrentUser();
                orgunit = await _organisationalUnitService.GetOrganisationalUnit(OUselect);
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
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
                return Json(userName);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddInterestedParty(OrganisationViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

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
                    
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetBoatUse(Guid answerSheetId, Guid boatUseId)
        {
            BoatUseViewModel model = new BoatUseViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                BoatUse boatUse = sheet.BoatUses.FirstOrDefault(bu => bu.Id == boatUseId);
                if (boatUse != null)
                {
                    model = BoatUseViewModel.FromEntity(boatUse);
                    model.AnswerSheetId = answerSheetId;
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBoatUses(Guid informationId, bool removed, bool ceased, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatUseRemovedStatus(Guid boatUseId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                BoatUse boatUse = await _boatUseService.GetBoatUse(boatUseId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    boatUse.Removed = status;
                    await uow.Commit();
                }
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetBoatUseCeasedStatus(Guid boatUseId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                BoatUse boatUse = await _boatUseService.GetBoatUse(boatUseId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    boatUse.BoatUseCeaseDate = DateTime.MinValue;
                    boatUse.BoatUseCeaseReason = '0';
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region Claim

        [HttpPost]
        public async Task<IActionResult> AddClaim(ClaimViewModel model)
        {
            User user = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();

                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Claim - No Client information for " + model.AnswerSheetId);

                ClaimNotification claimNotification = await _claimNotificationService.GetClaimNotificationById(model.ClaimId);
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
                {
                    var productList = await _productService.GetAllProducts();
                    claimNotification.ClaimProducts = productList.Where(pro => model.ClaimProducts.Contains(pro.Id)).ToList();
                }

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    sheet.ClaimNotifications.Add(claimNotification);
                    await uow.Commit();
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> GetClaim(Guid answerSheetId, Guid claimId)
        {
            ClaimViewModel model = new ClaimViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                ClientProgramme clientProgramme =  sheet.Programme;
                ClaimNotification claim = sheet.ClaimNotifications.FirstOrDefault(c => c.Id == claimId);
                if (claim != null)
                {
                    model = ClaimViewModel.FromEntity(claim);
                    model.AnswerSheetId = answerSheetId;
                }
                var claimProducts = new List<Product>();
                List<SelectListItem> ClaimProducts = new List<SelectListItem>();
               
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClaims(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                         string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetClaimRemovedStatus(Guid claimId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                ClaimNotification claim = await _claimNotificationService.GetClaimNotificationById(claimId);

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    claim.Removed = status;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        #endregion

        #region Operators

        [HttpPost]
        public async Task<IActionResult> AddOperator(OrganisationViewModel model)
        {
            User currentUser = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                currentUser = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Boat Use - No Client information for " + model.AnswerSheetId);

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
                finally
                {
                    organisation = await _organisationService.GetOrganisationByEmail(model.Email);
                    if (organisation == null)
                    {
                        var organisationName = model.FirstName + " " + model.LastName;
                        organisation = new Organisation(currentUser, Guid.NewGuid(), organisationName, organisationType, model.Email);
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
                        await uow.Commit();
                    }
                    
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        #endregion

        #region BusinessContracts

        [HttpPost]
        public async Task<IActionResult> AddBusinessContract(BusinessContractViewModel model)
        {
            User user = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Location - No Client information for " + model.AnswerSheetId);

                BusinessContract businessContract = await _businessContractService.GetBusinessContractById(model.BusinessContractId);
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCEASProject(BusinessContractViewModel model)
        {
            User user = null;

            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(model.AnswerSheetId);
                if (sheet == null)
                    throw new Exception("Unable to save Location - No Client information for " + model.AnswerSheetId);

                BusinessContract businessContract = await _businessContractService.GetBusinessContractById(model.BusinessContractId);
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCEASProject(Guid answerSheetId, Guid CEASProjectId)
        {
            BusinessContractViewModel model = new BusinessContractViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                BusinessContract businessContract = sheet.BusinessContracts.FirstOrDefault(bc => bc.Id == CEASProjectId);
                if (businessContract != null)
                {
                    model = BusinessContractViewModel.FromEntity(businessContract);
                    model.AnswerSheetId = answerSheetId;
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetBusinessContract(Guid answerSheetId, Guid businessContractId)
        {
            BusinessContractViewModel model = new BusinessContractViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(answerSheetId);
                BusinessContract businessContract = sheet.BusinessContracts.FirstOrDefault(bc => bc.Id == businessContractId);
                if (businessContract != null)
                {
                    model = BusinessContractViewModel.FromEntity(businessContract);
                    model.AnswerSheetId = answerSheetId;
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Getceasprojects(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
                    row.AddValues(businessContract.Id, businessContract.ContractTitle, businessContract.ProjectDescription, businessContract.Fees,  businessContract.Id);
                    model.AddRow(row);
                }

                // convert model to XDocument for rendering
                document = model.ToXml();
                return Xml(document);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBusinessContracts(Guid informationId, bool removed, bool _search, string nd, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBusinessContractss(Guid informationId, int rows, int page, string sidx, string sord,
                                          string searchField, string searchString, string searchOper, string filters)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> SetBusinessContractRemovedStatus(Guid businessContractId, bool status)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                BusinessContract businessContract = await _businessContractService.GetBusinessContractById(businessContractId);
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    businessContract.Removed = status;
                    await uow.Commit();
                }

                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        #endregion

        #region CoastGuardSelfReg

        [HttpPost]
        public async Task<IActionResult> CoastGuardSelfRegAsync(string craftType, string membershipNumber, string boatType, string constructionType, string hullConfiguration, string mooredType, string trailered,
            string boatInsuredValue, string quickQuotePremium, string firstName, string lastName, string email, string orgType, string homePhone, string mobilePhone)
        {
            User currentUser = null;
            bool hasAccount = true;            
            string organisationName = null;
            string ouname = null;
            string orgTypeName = null;

            try
            {
                currentUser = await CurrentUser();
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
                        await _userService.Update(user);

                        try
                        {
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

                            if (programme.ProgEnableEmail)
                            {
                                //send out login email
                                await _emailService.SendSystemEmailLogin(email);
                                EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetInstruction");
                                if (emailTemplate != null)
                                {
                                    await _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null, sheet, null);
                                }
                                //send out information sheet issue notification email
                                await _emailService.SendSystemEmailUISIssueNotify(programme.BrokerContactUser, programme, clientProgramme.InformationSheet, organisation);
                            }
                            
                        }
                        catch(Exception ex)
                        {
                            await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);                            
                        }
                        
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        #endregion

        #region Advisory
        [HttpGet]
        public async Task<IActionResult> CloseAdvisory(string ClientInformationSheetId)
        {
            User user = await CurrentUser();
            try
            {
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(ClientInformationSheetId));
                sheet.Status = "Not Taken Up";
                foreach (var agreement in sheet.Programme.Agreements)
                {
                    agreement.Status = "Not Taken Up";
                    await _clientAgreementService.UpdateClientAgreement(agreement);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> IssueUIS(string orgName, Guid programmeID, string firstName, string membershipNumber, string lastName, string email, string orgType, string mobilePhone)
        {
            User currentUser = null;
            bool hasAccount = true;
            //Add User, Organisation, Information Sheet, Quick Term saving process here
            string organisationName = null;
            string ouname = null;
            string orgTypeName = null;

            try
            {
                currentUser = await CurrentUser();
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
                        await _userService.Update(user);



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
                                if (!string.IsNullOrWhiteSpace(membershipNumber))
                                {
                                    clientProgramme.ClientProgrammeMembershipNumber = membershipNumber;
                                }                                

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
                            //    await _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null, null, null);
                            //}
                            ////send out information sheet issue notification email
                            //await _emailService.SendSystemEmailUISIssueNotify(programme.BrokerContactUser, programme, clientProgramme.InformationSheet, organisation);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        [HttpPost]
        public async Task<IActionResult> CoastGuardSelfRegCall(string craftType, string membershipNumber, string boatType, string constructionType, string hullConfiguration, string mooredType, string trailered,
            string boatInsuredValue, string quickQuotePremium, string firstName, string lastName, string email, string orgType, string homePhone, string mobilePhone)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
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
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

    }
}
