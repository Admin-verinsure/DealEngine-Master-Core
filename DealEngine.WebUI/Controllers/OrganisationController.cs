using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models;
using DealEngine.WebUI.Models.Organisation;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class OrganisationController : BaseController
    {
        ISerializerationService _serialiserService;
        IOrganisationService _organisationService;
        IOrganisationTypeService _organisationTypeService;              
        IUnitOfWork _unitOfWork;
        IClientInformationService _clientInformationService;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<OrganisationController> _logger;

        public OrganisationController(
            ISerializerationService serialiserService,
            ILogger<OrganisationController> logger,
            IClientInformationService clientInformationService,
            IApplicationLoggingService applicationLoggingService,
            IOrganisationService organisationService,
            IOrganisationTypeService organisationTypeService, 
            IUnitOfWork unitOfWork,           
            IUserService userRepository
            )
            : base (userRepository)
        {
            _serialiserService = serialiserService;
            _clientInformationService = clientInformationService;
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _organisationService = organisationService;            
            _organisationTypeService = organisationTypeService;
            _unitOfWork = unitOfWork;          
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOrganisationEmail(IFormCollection collection)
        {
            var email = collection["OrganisationViewModel.User.Email"].ToString();
            Guid.TryParse(collection["OrganisationViewModel.Organisation.Id"].ToString(), out Guid OrganisationId);
            Guid.TryParse(collection["ClientInformationSheet.Id"].ToString(), out Guid SheetId);
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(SheetId);
            Organisation organisation = await _organisationService.GetOrganisationByEmail(email);

            if(organisation != null)
            {
                if (OrganisationId == Guid.Empty)
                {
                    return Json(true);
                }
                if(sheet.Owner.Id == OrganisationId)
                {
                    return Json(false);
                }
                if (sheet.Organisation.Contains(organisation))
                {                    
                    return Json(false);
                }

                //if (organisation.Id != OrganisationId && OrganisationId != sheet.Owner.Id)
                //{
                //    return Json(true);
                //}
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<IActionResult> GetOrganisation(OrganisationViewModel model)
        {
            User user = null;
            Guid OrganisationId = Guid.Parse(model.ID.ToString());//Guid.Parse(collection["OrganisationId"]);
            Dictionary<string, object> JsonObjects = new Dictionary<string, object>();
            try
            {
                Organisation organisation = await _organisationService.GetOrganisation(OrganisationId);
                User orgUser = await _userService.GetUserPrimaryOrganisation(organisation);
                JsonObjects.Add("Organisation", organisation);
                JsonObjects.Add("User", orgUser);
                var jsonObj = await _serialiserService.GetSerializedObject(JsonObjects);

                return Json(jsonObj);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganisation(IFormCollection collection)
        {
            User currentUser = await CurrentUser();
            Guid.TryParse(collection["OrganisationViewModel.Organisation.Id"], out Guid OrganisationId);
            Guid.TryParse(collection["ClientInformationSheet.Id"], out Guid Id);
            ClientInformationSheet Sheet = await _clientInformationService.GetInformation(Id);

            var jsonOrganisation = (Organisation)await _serialiserService.GetDeserializedObject(typeof(Organisation), collection);
            var jsonUser = (User)await _serialiserService.GetDeserializedObject(typeof(User), collection);
            string TypeName = collection["OrganisationViewModel.InsuranceAttribute"].ToString();
            string OrganisationTypeName = collection["OrganisationViewModel.OrganisationType"].ToString();
            Organisation organisation = await _organisationService.GetOrganisation(OrganisationId);
            //condition for organisation exists
            try
            {

                if (organisation == null)
                {
                    organisation = await _organisationService.CreateOrganisation(jsonUser.Email, TypeName, jsonOrganisation.Name, OrganisationTypeName, jsonUser.FirstName, jsonUser.LastName, currentUser, collection);
                }

                await _organisationService.PostOrganisation(collection, organisation);

                if (!Sheet.Organisation.Contains(organisation))
                    Sheet.Organisation.Add(organisation);

                await _clientInformationService.UpdateInformation(Sheet);
                //return Ok();
                return Redirect("../Information/EditInformation?Id=" + Sheet.Programme.Id.ToString());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageOrganisations()
        {            
            return View("ManageOrganisations");
        }


        [HttpGet]
        public async Task<IActionResult> AttachOrganisation()
        {
            var Owners = await _clientInformationService.GetAllInformationSheets();
            var RemovedOrganisations = await _organisationService.GetAllRemovedOrganisations();
            AttachOrganisationViewModel model = new AttachOrganisationViewModel(Owners, RemovedOrganisations);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemovePrincipalAdvisors(IFormCollection collection)
        {
            User currentUser = await CurrentUser();
            Guid Id = Guid.Parse(collection["ClientInformationSheet.Id"]);
            string Name = "Advisor";
            ClientInformationSheet Sheet = await _clientInformationService.GetInformation(Id);
            foreach(var organisation in Sheet.Organisation)
            {
                var advisorUnit = (AdvisorUnit)organisation.OrganisationalUnits.FirstOrDefault(i => i.Name == Name);
                if(advisorUnit != null)
                {
                    advisorUnit.IsPrincipalAdvisor = false;
                }

                await _organisationService.Update(organisation);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganisation()
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var orgUser = await _userService.GetUser("TCMarinaAdmin1");
                var orgType = Request.Form["OrganisationType"];
                var selectedMooredType = Request.Form["OrganisationMarinaOrgMooredType"].ToString().Split(',');

                OrganisationType organisationType = await _organisationTypeService.GetOrganisationTypeByName(orgType);
                if (organisationType == null)
                {
                    organisationType = await _organisationTypeService.CreateNewOrganisationType(user, orgType);
                }

                var insuranceAttributeName = Request.Form["InsuranceAttributeName"];
                //InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(insuranceAttributeName);
                //if (insuranceAttribute == null)
                //{
                //    insuranceAttribute = await _insuranceAttributeService.CreateNewInsuranceAttribute(user, insuranceAttributeName);
                //}

                Organisation organisation = await _organisationService.GetOrganisationByEmail(Request.Form["OrganisationEmail"]);
                if (organisation == null)
                {
                    //organisation = new Organisation(user, Guid.NewGuid(), Request.Form["OrganisationName"], organisationType);
                    //organisation.Phone = Request.Form["OrganisationPhone"];
                    //organisation.Email = Request.Form["OrganisationEmail"];
                    //organisation.Domain = Request.Form["OrganisationWebsite"];
                    //organisation.InsuranceAttributes.Add(insuranceAttribute);
                    //organisation.IsApproved = insuranceAttributeName == "Marina" ? true : false;

                    //foreach (string MooredType in selectedMooredType)
                    //{
                    //    organisation.Marinaorgmooredtype.Add(MooredType);
                    //}

                    //organisation.InsuranceAttributes.Add(insuranceAttribute);
                    //insuranceAttribute.IAOrganisations.Add(organisation);
                    await _organisationService.CreateNewOrganisation(organisation);
                }

                Location location = new Location(user)
                {
                    CommonName = Request.Form["LocationCommonName"],
                    Country = Request.Form["LocationCountry"],
                    Suburb = Request.Form["LocationSuburb"],
                    Street = Request.Form["LocationStreetAddress"],
                    City = Request.Form["LocationCity"],
                    Postcode = Request.Form["LocationPostCode"]
                };

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    OrganisationalUnit ou = new OrganisationalUnit(user, "Main Entrance");
                    organisation.OrganisationalUnits.Add(ou);
                    location.OrganisationalUnits.Add(ou);
                    ou.Locations.Add(location);
                    await uow.Commit();
                }
                
                return RedirectToAction("AddNewOrganisation", "Organisation");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        public async Task<IActionResult> SetPrimary(Guid id)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Organisation org = user.Organisations.FirstOrDefault(o => o.Id == id);
                if (org != null)
                {
                    user.SetPrimaryOrganisation(org);
                    await _userService.Update(user);
                }

                return Redirect("~/Organisation/Index");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }    
        
    }
}
