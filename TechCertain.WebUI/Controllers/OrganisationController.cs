using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechCertain.WebUI.Controllers
{
    public class OrganisationController : BaseController
    {
        private readonly IOrganisationService _organisationService;
        private readonly IOrganisationTypeService _organisationTypeService;
        IInsuranceAttributeService _insuranceAttributeService;
        IMapperSession<InsuranceAttribute> _InsuranceAttributesRepository;
        IMapper _mapper;
        IUnitOfWork _unitOfWork;

        //private readonly ICompanyService _companyService;

        public OrganisationController(IOrganisationService organisationService,
            IOrganisationTypeService organisationTypeService, 
            IUnitOfWork unitOfWork, 
            IInsuranceAttributeService insuranceAttributeService,
            IMapper mapper,
            IMapperSession<InsuranceAttribute> insuranceAttributesRepository,
            IUserService userRepository)
            : base (userRepository)
        {
            _organisationService = organisationService;
            _InsuranceAttributesRepository = insuranceAttributesRepository;
            _organisationTypeService = organisationTypeService;
            _insuranceAttributeService = insuranceAttributeService;
            _unitOfWork = unitOfWork;
            _insuranceAttributeService = insuranceAttributeService;
            //_companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            BaseListViewModel<OrganisationViewModel> organisations = new BaseListViewModel<OrganisationViewModel>();

            User user = await CurrentUser();
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
        public async Task<IActionResult> Update(OrganisationViewModel model)
        {
            var user = await CurrentUser();
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

        [HttpGet]
        public async Task<IActionResult> ManageOrganisations()
        {
            return View("ManageOrganisations");
        }

        [HttpGet]
        public async Task<IActionResult> AddNewOrganisation(Guid programmeId)
        {
            OrganisationViewModel organisationViewModel = new OrganisationViewModel();
            organisationViewModel.ProgrammeId = programmeId;
            //var insuranceAttributes = new List<InsuranceAttribute>();
            //try
            //{
            //    foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Where(ia => ia.InsuranceAttributeName == "Financial" || ia.InsuranceAttributeName == "Marina" || ia.InsuranceAttributeName == "Other Marina"))
            //    {

            //        foreach (var org in IA.IAOrganisations)
            //        {
            //            if (org.OrganisationType.Name == "Corporation – Limited liability" || org.OrganisationType.Name == "Corporation – Limited liability")
            //            {
            //                foreach(var ia in org.InsuranceAttributes)
            //                {
            //                    insuranceAttributes.Add(ia);

            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //organisationViewModel.InsuranceAttributes = new List<SelectListItem>
            //   {
            //    new SelectListItem {Text = "Select", Value = "Select"},
            //    new SelectListItem {Text = "Marina", Value = "Marina"},
            //   new SelectListItem {Text = "Other Marina", Value = "Other Marina"},
            //   new SelectListItem {Text = "Financial", Value = "Financial"},
            //   };




            //InsuranceAttribute insuranceAttribute = new InsuranceAttribute();
            organisationViewModel.OrgMooredType = new List<SelectListItem>
            {
                new SelectListItem {Text = "Berthed", Value = "Berthed"},
                new SelectListItem {Text = "Pile", Value = "Pile"},
                new SelectListItem {Text = "Swing", Value = "Swing"},
            };

            return View("AddNewOrganisation", organisationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganisation()
        {
            var user = await _userService.GetUser("TCMarinaAdmin1");
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
                    organisation.marinaorgmooredtype.Add(MooredType);
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

            return Redirect("~/Organisation/AddNewOrganisation");            
        }


        public async Task<IActionResult> SetPrimary(Guid id)
        {
            var user = await CurrentUser();
            Organisation org = user.Organisations.FirstOrDefault(o => o.Id == id);
            if (org != null)
            {
                //CurrentUser().Organisations.Remove (org);
                //CurrentUser().Organisations.Insert (0, org);
                user.SetPrimaryOrganisation(org);

                await _userService.Update(user);
            }

            return Redirect("~/Organisation/Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editing_Popup()
        {
            return View();
        }

        //public async Task<IActionResult> EditingPopup_Read ([DataSourceRequest] DataSourceRequest request)
        //{
        //IEnumerable<OrganisationViewModel> items = _companyService.GetCompanies ()
        //	.Select (x => new OrganisationViewModel {
        //	Name = x.Name,
        //	ID = x.Id				
        //});

        //return Json (items.ToDataSourceResult (request));

        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public async Task<IActionResult> EditingPopup_Create ([DataSourceRequest] DataSourceRequest request, OrganisationViewModel company)
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
        //public async Task<IActionResult> EditingPopup_Update ([DataSourceRequest] DataSourceRequest request, OrganisationViewModel company)
        //{
        //    if (company != null && ModelState.IsValid) {

        //        //Organisation updateCompany = _companyService.GetCompany (company.ID);

        //        //updateCompany.Name = company.Name;

        //        //_companyService.UpdateExistingCompany (updateCompany);
        //    }

        //    return Json (new[] { company }.ToDataSourceResult (request, ModelState));
        //}

        //[AcceptVerbs (HttpVerbs.Post)]
        //public async Task<IActionResult> EditingPopup_Destroy ([DataSourceRequest] DataSourceRequest request, OrganisationViewModel company)
        //{
        //    if (company != null) {

        //        //Organisation updateCompany = _companyService.GetCompany (company.ID);

        //        //updateCompany.DeletedDate = DateTime.UtcNow;

        //        //_companyService.UpdateExistingCompany (updateCompany);
        //    }

        //    return Json (new[] { company }.ToDataSourceResult (request, ModelState));
        //}

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            OrganisationViewModel organisationViewModel = new OrganisationViewModel();

            organisationViewModel.OrganisationTypes = _organisationTypeService.GetOrganisationTypes().Select(x => x.Name);

            return View(organisationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(OrganisationViewModel organisationViewModel)
        {
            var user = await CurrentUser();
            _organisationService.CreateNewOrganisation(organisationViewModel.OrganisationName,
                                                       new OrganisationType(user, organisationViewModel.OrganisationTypeName),
                                                       organisationViewModel.FirstName,
                                                       organisationViewModel.LastName,
                                                       organisationViewModel.Email);

            return View();
        }

        public async Task<IActionResult> CreateDefault()
        {
            var user = await CurrentUser();
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
    }
}
