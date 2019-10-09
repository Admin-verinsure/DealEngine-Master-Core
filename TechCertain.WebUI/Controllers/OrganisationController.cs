using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Controllers
{
    public class OrganisationController : BaseController
    {
        private readonly IOrganisationService _organisationService;
        private readonly IOrganisationTypeService _organisationTypeService;
        IInsuranceAttributeService _insuranceAttributeService;
        IHttpContextAccessor _httpContextAccessor;
        IUnitOfWork _unitOfWork;

        //private readonly ICompanyService _companyService;

        public OrganisationController(IHttpContextAccessor httpContextAccessor,
            SignInManager<DealEngineUser> signInManager,
            IOrganisationService organisationService, 
            DealEngineDBContext dealEngineDBContext, 
            IOrganisationTypeService organisationTypeService, 
            IUnitOfWork unitOfWork, 
            IInsuranceAttributeService insuranceAttributeService,
            IUserService userRepository)//(ICompanyService companyService)
            : base(userRepository, dealEngineDBContext, signInManager, httpContextAccessor)
        {
            _organisationService = organisationService;
            _organisationTypeService = organisationTypeService;
            _insuranceAttributeService = insuranceAttributeService;
            _unitOfWork = unitOfWork;
            _insuranceAttributeService = insuranceAttributeService;
            //_companyService = companyService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            BaseListViewModel<OrganisationViewModel> organisations = new BaseListViewModel<OrganisationViewModel>();

            User user = CurrentUser;
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

        [HttpPost]
        public ActionResult Update(OrganisationViewModel model)
        {
            Organisation org = CurrentUser.Organisations.FirstOrDefault(o => o.Id == model.ID);
            if (org != null)
            {
                org.ChangeOrganisationName(model.OrganisationName);
                // Org type here
                org.Domain = (model.Website != "Empty") ? model.Website : "";
                org.Email = (model.Email != "Empty") ? model.Email : "";
                org.Phone = (model.Phone != "Empty") ? model.Phone : "";

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    _organisationService.UpdateOrganisation(org);
                    uow.Commit();
                }
                return Content("success");
            }

            throw new Exception("No organisation found with Id '" + model.ID + "'");
        }

        [HttpGet]
        public ActionResult ManageOrganisations()
        {
            return View("ManageOrganisations");
        }

        [HttpGet]
        public ActionResult AddNewOrganisation()
        {
            OrganisationViewModel organisationViewModel = new OrganisationViewModel();
            throw new Exception("Method will need to be re-written");
            //    if (WebConfigurationManager.AppSettings["DemoEnvironment"].ToString() == "true")
            //    {
            //        User user = _userService.GetUser("TCMarinaAdmin1");
            //        List<User> TCMarinaAdmin = new List<User>();
            //        TCMarinaAdmin.Add(user);
            //        organisationViewModel.Users = TCMarinaAdmin;
            //    }

            //organisationViewModel.OrgMooredType = new List<SelectListItem>
            //{
            //    new SelectListItem {Text = "Berthed", Value = "Berthed"},
            //    new SelectListItem {Text = "Pile", Value = "Pile"},
            //    new SelectListItem {Text = "Swing", Value = "Swing"},
            //};

            //return View("AddNewOrganisation", organisationViewModel);
        }

        [HttpPost]
        public ActionResult CreateOrganisation()
        {
            User user = null;
            var orgType = Request.Form["OrganisationType"];
            var selectedMooredType = Request.Form["OrganisationMarinaOrgMooredType"].ToString().Split(',');          

            OrganisationType organisationType = _organisationTypeService.GetOrganisationTypeByName(orgType);
            if (organisationType == null)
            {
                organisationType = _organisationTypeService.CreateNewOrganisationType(CurrentUser, orgType);
            }

            var insuranceAttributeName = Request.Form["InsuranceAttributeName"];
            InsuranceAttribute insuranceAttribute = _insuranceAttributeService.GetInsuranceAttributeByName(insuranceAttributeName);
            if (insuranceAttribute == null)
            {
                insuranceAttribute = _insuranceAttributeService.CreateNewInsuranceAttribute(CurrentUser, insuranceAttributeName);
            }

            Organisation organisation = _organisationService.GetOrganisationByEmail(Request.Form["OrganisationEmail"]);
            if (organisation == null)
            {
                organisation = new Organisation(CurrentUser, Guid.NewGuid(), Request.Form["OrganisationName"], organisationType);
                organisation.Phone = Request.Form["OrganisationPhone"];
                organisation.Email = Request.Form["OrganisationEmail"];
                organisation.Domain = Request.Form["OrganisationWebsite"];
                organisation.InsuranceAttributes.Add(insuranceAttribute);
                organisation.IsApproved = insuranceAttributeName == "Marina" ? true : false;

                foreach (string MooredType in selectedMooredType)
                {
                    organisation.marinaorgmooredtype.Add(MooredType);
                }

                organisation.InsuranceAttributes.Add(insuranceAttribute);
                insuranceAttribute.IAOrganisations.Add(organisation);
                _organisationService.CreateNewOrganisation(organisation);
            }

            Random random = new Random();
            int rand = random.Next(10000);
            var userName = "TCMarinaAdmin"+ rand;
            try
            {
                user = _userService.GetUser(Request.Form["OrganisationUser"]);
            }
            catch (Exception)
            {
                    user = new User(CurrentUser, Guid.NewGuid(), userName);
                    user.FirstName = Request.Form["UserFirstName"];
                    user.LastName = Request.Form["UserLastName"];
                    user.FullName = user.FirstName + " " + user.LastName;
                    user.Email = Request.Form["UserEmail"];
                    user.Phone = Request.Form["UserPhone"];
                    _userService.Create(user);
            }

            if (!user.Organisations.Contains(organisation))
                user.Organisations.Add(organisation);

            //_userService.Update(user);

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
                uow.Commit();
            }

            return Redirect("~/Organisation/AddNewOrganisation");            
        }


        public ActionResult SetPrimary(Guid id)
        {
            Organisation org = CurrentUser.Organisations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                //CurrentUser.Organisations.Remove (org);
                //CurrentUser.Organisations.Insert (0, org);
                CurrentUser.SetPrimaryOrganisation(org);

                _userService.Update(CurrentUser);
            }

            return Redirect("~/Organisation/Index");
        }

        [HttpGet]
        public ActionResult Editing_Popup()
        {
            return View();
        }

        //public ActionResult EditingPopup_Read ([DataSourceRequest] DataSourceRequest request)
        //{
        //IEnumerable<OrganisationViewModel> items = _companyService.GetCompanies ()
        //	.Select (x => new OrganisationViewModel {
        //	Name = x.Name,
        //	ID = x.Id				
        //});

        //return Json (items.ToDataSourceResult (request));

        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public ActionResult EditingPopup_Create ([DataSourceRequest] DataSourceRequest request, OrganisationViewModel company)
        //{
        //    if (company != null && ModelState.IsValid) {

        //        //_companyService.CreateNewCompany (new Company () {
        //        //    ID = company.ID,
        //        //    Name = company.Name
        //        //});
        //    }

        //    return Json (new[] { company }.ToDataSourceResult (request, ModelState));
        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public ActionResult EditingPopup_Update ([DataSourceRequest] DataSourceRequest request, OrganisationViewModel company)
        //{
        //    if (company != null && ModelState.IsValid) {

        //        //Organisation updateCompany = _companyService.GetCompany (company.ID);

        //        //updateCompany.Name = company.Name;

        //        //_companyService.UpdateExistingCompany (updateCompany);
        //    }

        //    return Json (new[] { company }.ToDataSourceResult (request, ModelState));
        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public ActionResult EditingPopup_Destroy ([DataSourceRequest] DataSourceRequest request, OrganisationViewModel company)
        //{
        //    if (company != null) {

        //        //Organisation updateCompany = _companyService.GetCompany (company.ID);

        //        //updateCompany.DeletedDate = DateTime.UtcNow;

        //        //_companyService.UpdateExistingCompany (updateCompany);
        //    }

        //    return Json (new[] { company }.ToDataSourceResult (request, ModelState));
        //}

        [HttpGet]
        public ActionResult Register()
        {
            OrganisationViewModel organisationViewModel = new OrganisationViewModel();

            organisationViewModel.OrganisationTypes = _organisationTypeService.GetOrganisationTypes().Select(x => x.Name);

            return View(organisationViewModel);
        }

        [HttpPost]
        public ActionResult Register(OrganisationViewModel organisationViewModel)
        {
            _organisationService.CreateNewOrganisation(organisationViewModel.OrganisationName,
                                                       new OrganisationType(CurrentUser, organisationViewModel.OrganisationTypeName),
                                                       organisationViewModel.FirstName,
                                                       organisationViewModel.LastName,
                                                       organisationViewModel.Email);

            return View();
        }

        public ActionResult CreateDefault()
        {
            OrganisationType ot = new OrganisationType(CurrentUser, "financial");
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _organisationService.UpdateOrganisation(new Organisation(CurrentUser, Guid.NewGuid(), "ANZ Bank", ot));
                _organisationService.UpdateOrganisation(new Organisation(CurrentUser, Guid.NewGuid(), "ASB Bank", ot));
                _organisationService.UpdateOrganisation(new Organisation(CurrentUser, Guid.NewGuid(), "BNZ Bank", ot));

                uow.Commit();
            }

            return Redirect("~/Home/Index");
        }
    }
}
