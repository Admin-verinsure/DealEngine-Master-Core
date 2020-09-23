using AutoMapper;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.WebUI.Models;
using DealEngine.WebUI.Models.Organisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class OrganisationController : BaseController
    {
        ISerializerationService _serialiserService;
        IOrganisationService _organisationService;
        IClientInformationService _clientInformationService;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<OrganisationController> _logger;        
        IMilestoneService _milestoneService;
        IProgrammeService _programmeService;
        IMapper _mapper;

        public OrganisationController(
            IProgrammeService programmeService,
            IMilestoneService milestoneService,
            ISerializerationService serialiserService,
            ILogger<OrganisationController> logger,
            IClientInformationService clientInformationService,
            IApplicationLoggingService applicationLoggingService,
            IOrganisationService organisationService,
            IUserService userRepository,
            IMapper mapper
            )
            : base (userRepository)
        {
            _mapper = mapper;
            _programmeService = programmeService;
            _milestoneService = milestoneService;
            _serialiserService = serialiserService;
            _clientInformationService = clientInformationService;
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _organisationService = organisationService;                    
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOrganisationEmail(IFormCollection collection)
        {
            var email = collection["OrganisationViewModel.User.Email"].ToString();
            bool ValidBackEndEmail;
            Guid.TryParse(collection["OrganisationViewModel.Organisation.Id"].ToString(), out Guid OrganisationId);
            Guid.TryParse(collection["ClientInformationSheet.Id"].ToString(), out Guid SheetId);
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(SheetId);
            Organisation organisation = await _organisationService.GetOrganisationByEmail(email);

            
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                ValidBackEndEmail = addr.Address == email;

                if (organisation != null)
                {
                    if (OrganisationId == Guid.Empty)
                    {
                        return Json(true);
                    }
                    if (sheet.Owner.Id == OrganisationId)
                    {
                        return Json(false);
                    }
                    if (sheet.Organisation.Contains(organisation))
                    {
                        return Json(false);
                    }
                }
                return Json(false);
            }
            catch
            {
                return Json(true);
            }
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
        public async Task<IActionResult> ManageOrganisations(Guid Id)
        {
            Programme programme = await _programmeService.GetProgrammeById(Id);
            OrganisationViewModel model = new OrganisationViewModel(null, null);
            var marinas = await _organisationService.GetAllMarinas();
            foreach(var mar in marinas)
            {
                model.Organisations.Add(mar);
            }
            
            var institutes = await _organisationService.GetFinancialInstitutes();
            foreach (var inst in institutes)
            {
                model.Organisations.Add(inst);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetMarina(IFormCollection model)
        {
            Organisation organisation = await _organisationService.GetOrganisation(Guid.Parse(model["Id"]));
            Dictionary<string, object> JsonObjects = new Dictionary<string, object>();
            if (organisation != null)
            {
                var unit = (MarinaUnit)organisation.OrganisationalUnits.FirstOrDefault();
                JsonObjects.Add("Marina", organisation);
                JsonObjects.Add("WaterLocation", unit.WaterLocation);
                var jsonObj = await _serialiserService.GetSerializedObject(JsonObjects);
                return Json(jsonObj);
            }
            return NoContent();
        }
        

        [HttpPost]
        public async Task<IActionResult> PostMarina(IFormCollection model)
        {
            Organisation organisation = await _organisationService.GetOrganisation(Guid.Parse(model["Organisation.Id"]));
            MarinaUnit marinaUnit = (MarinaUnit)organisation.OrganisationalUnits.FirstOrDefault();
            var jsonOrganisation = (Organisation)await _serialiserService.GetDeserializedObject(typeof(Organisation), model);
            var jsonWaterLocation = (WaterLocation)await _serialiserService.GetDeserializedObject(typeof(WaterLocation), model);
            organisation = _mapper.Map(jsonOrganisation, organisation);
            marinaUnit.WaterLocation = _mapper.Map(jsonWaterLocation, marinaUnit.WaterLocation);
            //add fields

            //add new organisation using devtools example


            await _organisationService.Update(organisation);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> GetInstitute(IFormCollection model)
        {
            Organisation organisation = await _organisationService.GetOrganisation(Guid.Parse(model["Id"]));
            Dictionary<string, object> JsonObjects = new Dictionary<string, object>();
            if (organisation != null)
            {
                var unit = (InterestedPartyUnit)organisation.OrganisationalUnits.FirstOrDefault();
                JsonObjects.Add("Institute", organisation);
                JsonObjects.Add("Location", unit.Location);
                var jsonObj = await _serialiserService.GetSerializedObject(JsonObjects);
                return Json(jsonObj);
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostInstitute(IFormCollection model)
        {
            Organisation organisation = await _organisationService.GetOrganisation(Guid.Parse(model["Organisation.Id"]));
            InterestedPartyUnit unit = (InterestedPartyUnit)organisation.OrganisationalUnits.FirstOrDefault();
            var jsonOrganisation = (Organisation)await _serialiserService.GetDeserializedObject(typeof(Organisation), model);
            var jsonLocation = (Location)await _serialiserService.GetDeserializedObject(typeof(Location), model);
            organisation = _mapper.Map(jsonOrganisation, organisation);
            unit.Location = _mapper.Map(jsonLocation, unit.Location);
            //add fields

            //add new organisation using devtools example


            await _organisationService.Update(organisation);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganisationSkipperAPI(IFormCollection collection)
        {
            User currentUser = null;
            try
            {
                string FirstName = collection["FirstName"].ToString();
                string Email = collection["Email"].ToString();
                string LastName = collection["LastName"].ToString();
                currentUser = await CurrentUser();
                Guid.TryParse(collection["AnswerSheetId"], out Guid SheetId);
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(SheetId);
                OrganisationType organisationType = new OrganisationType(currentUser, "Person - Individual");
                InsuranceAttribute insuranceAttribute = new InsuranceAttribute(currentUser, "Skipper");
                OrganisationalUnit organisationalUnit = new OrganisationalUnit(currentUser, "Person - Individual");
                InterestedPartyUnit interestedPartyUnit = new InterestedPartyUnit(currentUser, "Skipper", "Person - Individual", null);
                Organisation organisation = new Organisation(currentUser, Guid.NewGuid())
                {
                    OrganisationType = organisationType,
                    Email = Email,
                    Name = FirstName + " " + LastName
                };

                organisation.OrganisationalUnits.Add(organisationalUnit);
                organisation.OrganisationalUnits.Add(interestedPartyUnit);
                organisation.InsuranceAttributes.Add(insuranceAttribute);

                Random random = new Random();
                string UserName = FirstName.Replace(" ", string.Empty)
                    + "_"
                    + LastName.Replace(" ", string.Empty)
                    + random.Next(1000);

                User user = new User(currentUser, UserName)
                {
                    FirstName = collection["FirstName"].ToString(),
                    LastName = collection["LastName"].ToString(),
                    Email = collection["Email"].ToString(),
                    FullName = FirstName + " " + LastName,
                    Id = Guid.NewGuid()
                };
                user.SetPrimaryOrganisation(organisation);

                if (!sheet.Organisation.Contains(organisation))
                {
                    sheet.Organisation.Add(organisation);
                }

                await _clientInformationService.UpdateInformation(sheet);

                return Json(organisation);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganisationInterestedPartyAPI(IFormCollection collection)
        {
            User currentUser = null;
            try
            {
                string FirstName = collection["FirstName"].ToString();
                string Email = collection["OrganisationEmail"].ToString();
                string Name = collection["OrganisationName"].ToString();
                string InsuranceAttribute = collection["InsuranceAttribute"].ToString();
                string OrganisationType = collection["OrganisationTypeName"].ToString();
                currentUser = await CurrentUser();
                Guid.TryParse(collection["AnswerSheetId"], out Guid SheetId);
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(SheetId);
                OrganisationType organisationType = new OrganisationType(currentUser, OrganisationType);
                InsuranceAttribute insuranceAttribute = new InsuranceAttribute(currentUser, InsuranceAttribute);
                OrganisationalUnit organisationalUnit = new OrganisationalUnit(currentUser, OrganisationType);
                InterestedPartyUnit interestedPartyUnit = new InterestedPartyUnit(currentUser, InsuranceAttribute, OrganisationType, null);
                Organisation organisation = new Organisation(currentUser, Guid.NewGuid())
                {
                    OrganisationType = organisationType,
                    Email = Email,
                    Name = Name
                };

                organisation.OrganisationalUnits.Add(organisationalUnit);
                organisation.OrganisationalUnits.Add(interestedPartyUnit);
                organisation.InsuranceAttributes.Add(insuranceAttribute);

                if (!sheet.Organisation.Contains(organisation))
                {
                    sheet.Organisation.Add(organisation);
                }

                await _clientInformationService.UpdateInformation(sheet);

                return Json(organisation);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AttachOrganisation(Guid ProgrammeId, Guid OrganisationId)
        {
            var Programme = await _programmeService.GetProgrammeById(ProgrammeId);
            var RemovedOrg = await _organisationService.GetOrganisation(OrganisationId);            
            AttachOrganisationViewModel model = new AttachOrganisationViewModel(Programme.ClientProgrammes, RemovedOrg);
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

        [HttpPost]
        public async Task<IActionResult> RemoveOrganisation(IFormCollection collection)
        {
            User user = await CurrentUser();
            Guid Id = Guid.Parse(collection["OrganisationId"]);
            Organisation organisation = await _organisationService.GetOrganisation(Id);
            var organisationUser = await _userService.GetUserPrimaryOrganisation(organisation);
            organisation.Removed = true;
            await _organisationService.Update(organisation);

            //if (user.UserName == "JDillon")
            //{
            //    if (organisationUser != null)
            //    {
            //        Guid.TryParse(collection["ProgrammeId"].ToString(), out Guid ProgrammeId);
            //        var Programme = await _programmeService.GetProgramme(ProgrammeId);
            //        await _milestoneService.CreateJoinOrganisationTask(user, organisationUser, Programme);
            //    }
            //}

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RestoreOrganisation(IFormCollection collection)
        {
            Guid Id = Guid.Parse(collection["OrganisationId"]);
            Organisation organisation = await _organisationService.GetOrganisation(Id);
            organisation.Removed = false;
            await _organisationService.Update(organisation);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> RejoinProgramme(Guid ProgrammeId, Guid OrganisationId)
        {
            User user = await CurrentUser();
            Programme programme = await _programmeService.GetProgrammeById(ProgrammeId);
            await _milestoneService.JoinOrganisationTask(user, programme);
            return RedirectToAction("Index", "Home");
        }

    }
}
