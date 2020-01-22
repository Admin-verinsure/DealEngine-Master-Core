using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace TechCertain.WebUI.Controllers
{
    public class OrganisationController : BaseController
    {
        IOrganisationService _organisationService;
        IOrganisationTypeService _organisationTypeService;
        IInsuranceAttributeService _insuranceAttributeService;               
        IUnitOfWork _unitOfWork;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<OrganisationController> _logger;

        public OrganisationController(
            ILogger<OrganisationController> logger,
            IApplicationLoggingService applicationLoggingService,
            IOrganisationService organisationService,
            IOrganisationTypeService organisationTypeService, 
            IUnitOfWork unitOfWork, 
            IInsuranceAttributeService insuranceAttributeService,            
            IUserService userRepository
            )
            : base (userRepository)
        {
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _organisationService = organisationService;            
            _organisationTypeService = organisationTypeService;
            _insuranceAttributeService = insuranceAttributeService;
            _unitOfWork = unitOfWork;
            _insuranceAttributeService = insuranceAttributeService;            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            BaseListViewModel<OrganisationViewModel> organisations = new BaseListViewModel<OrganisationViewModel>();
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (Organisation org in user.Organisations)
                {
                    OrganisationViewModel model = new OrganisationViewModel
                    {
                        ID = org.Id,
                        OrganisationName = org.Name,
                        OrganisationTypeName = org.OrganisationType != null ? org.OrganisationType.Name : string.Empty,
                        Website = org.Domain,
                        Phone = org.Phone,
                        Email = org.Email,
                        IsPrimary = org.Id == user.PrimaryOrganisation.Id
                    };
                    organisations.Add(model);
                }

                return View(organisations);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(OrganisationViewModel model)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Organisation org = user.Organisations.FirstOrDefault(o => o.Id == model.ID);
                if (org != null)
                {
                    org.ChangeOrganisationName(model.OrganisationName);
                    // Org type here
                    org.Domain = (model.Website != "Empty") ? model.Website : "";
                    org.Email = (model.Email != "Empty") ? model.Email : "";
                    org.Phone = (model.Phone != "Empty") ? model.Phone : "";

                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        await _organisationService.UpdateOrganisation(org);
                        await uow.Commit();
                    }
                    return Content("success");
                }
                throw new Exception("No organisation found with Id '" + model.ID + "'");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageOrganisations()
        {
            return View("ManageOrganisations");
        }

        [HttpGet]
        public async Task<IActionResult> AddNewOrganisation(Guid programmeId)
        {
            OrganisationViewModel organisationViewModel = new OrganisationViewModel();
            User user = null;

            try
            {
                organisationViewModel.ProgrammeId = programmeId;

                organisationViewModel.OrgMooredType = new List<SelectListItem>()
                {
                    new SelectListItem {Text = "Berthed", Value = "Berthed"},
                    new SelectListItem {Text = "Pile", Value = "Pile"},
                    new SelectListItem {Text = "Swing", Value = "Swing"},
                };

                return View("AddNewOrganisation", organisationViewModel);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
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
                InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(insuranceAttributeName);
                if (insuranceAttribute == null)
                {
                    insuranceAttribute = await _insuranceAttributeService.CreateNewInsuranceAttribute(user, insuranceAttributeName);
                }

                Organisation organisation = await _organisationService.GetOrganisationByEmail(Request.Form["OrganisationEmail"]);
                if (organisation == null)
                {
                    organisation = new Organisation(user, Guid.NewGuid(), Request.Form["OrganisationName"], organisationType);
                    organisation.Phone = Request.Form["OrganisationPhone"];
                    organisation.Email = Request.Form["OrganisationEmail"];
                    organisation.Domain = Request.Form["OrganisationWebsite"];
                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    organisation.IsApproved = insuranceAttributeName == "Marina" ? true : false;

                    foreach (string MooredType in selectedMooredType)
                    {
                        organisation.Marinaorgmooredtype.Add(MooredType);
                    }

                    organisation.InsuranceAttributes.Add(insuranceAttribute);
                    insuranceAttribute.IAOrganisations.Add(organisation);
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

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            OrganisationViewModel organisationViewModel = new OrganisationViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                organisationViewModel.OrganisationTypes = _organisationTypeService.GetOrganisationTypes().Select(x => x.Name);
                return View(organisationViewModel);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(OrganisationViewModel organisationViewModel)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                _organisationService.CreateNewOrganisation(organisationViewModel.OrganisationName,
                                                           new OrganisationType(user, organisationViewModel.OrganisationTypeName),
                                                           organisationViewModel.FirstName,
                                                           organisationViewModel.LastName,
                                                           organisationViewModel.Email);

                return View();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        public async Task<IActionResult> CreateDefault()
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                OrganisationType ot = new OrganisationType(user, "financial");
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    await _organisationService.UpdateOrganisation(new Organisation(user, Guid.NewGuid(), "ANZ Bank", ot));
                    await _organisationService.UpdateOrganisation(new Organisation(user, Guid.NewGuid(), "ASB Bank", ot));
                    await _organisationService.UpdateOrganisation(new Organisation(user, Guid.NewGuid(), "BNZ Bank", ot));

                    await uow.Commit();
                }

                return Redirect("~/Home/Index");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }
    }
}
