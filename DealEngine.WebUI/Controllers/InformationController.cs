using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models;
using DealEngine.WebUI.Models.Programme;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using DealEngine.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Authorization;
using DealEngine.Infrastructure.Tasking;
using Microsoft.Extensions.Logging;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class InformationController : BaseController
    {
        IProgrammeService _programmeService;
        IEmailTemplateService _emailTemplateService;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<InformationController> _logger;
        ISharedDataRoleService _sharedDataRoleService;
        IAppSettingService _appSettingService;
        IActivityService _activityService;
        IInformationItemService _informationItemService;
        IInformationSectionService _informationSectionService;
        IInformationTemplateService _informationTemplateService;
        ITerritoryService _territoryService;
        IFileService _fileService;
        IClientInformationService _clientInformationService;
        IChangeProcessService _changeProcessService;
        IClientAgreementService _clientAgreementService;
        IClientAgreementTermService _clientAgreementTermService;
        IClientAgreementMVTermService _clientAgreementMVTermService;
        IClientAgreementRuleService _clientAgreementRuleService;
        IUWMService _uWMService;
        ITaskingService _taskingService;
        IEmailService _emailService;
        IUnitOfWork _unitOfWork;
        IReferenceService _referenceService;
        IMilestoneService _milestoneService;
        IAdvisoryService _advisoryService;
        IOrganisationService _organisationService;
        IInsuranceAttributeService _insuranceAttributeService;
        IBusinessActivityService _businessActivityService;
        IRevenueActivityService _revenueActivityService;
        IProductService _productService;
        IMapper _mapper;
        IMapperSession<DropdownListItem> _IDropdownListItem;
        IClientInformationAnswerService _clientInformationAnswer;
        IOrganisationTypeService _organisationTypeService;


        public InformationController(
            IOrganisationTypeService organisationTypeService,
            IEmailTemplateService emailTemplateService,
            IApplicationLoggingService applicationLoggingService,
            ILogger<InformationController> logger,
            IInformationSectionService informationSectionService,
            IRevenueActivityService revenueActivityService,
            IInsuranceAttributeService insuranceAttributeService,
            IOrganisationService organisationService,
            IActivityService activityService,
            IAppSettingService appSettingService,
            IAdvisoryService advisoryService,
            IUserService userService,
            ITerritoryService territoryService,
            IInformationItemService informationItemService,
            IChangeProcessService changeProcessService,
            IFileService fileService,
            IEmailService emailService,
            IMilestoneService milestoneService,
            IInformationTemplateService informationTemplateService,
            IClientInformationService clientInformationService,
            IClientAgreementService clientAgreementService,
            IClientAgreementTermService clientAgreementTermService,
            IClientAgreementMVTermService clientAgreementMVTermService,
            IClientAgreementRuleService clientAgreementRuleService,
            IUWMService uWMService,
            IReferenceService referenceService,
            IProductService productService,
            ITaskingService taskingService,
            ISharedDataRoleService sharedDataRoleService,
            IUnitOfWork unitOfWork,
            IProgrammeService programmeService,
            IBusinessActivityService businessActivityService,
            IClientInformationAnswerService clientInformationAnswer,
            IMapperSession<DropdownListItem> dropdownListItem,
            IMapper mapper
            )
            : base(userService)
        {
            _organisationTypeService = organisationTypeService;
            _emailTemplateService = emailTemplateService;
            _applicationLoggingService = applicationLoggingService;
            _logger = logger;
            _revenueActivityService = revenueActivityService;
            _insuranceAttributeService = insuranceAttributeService;
            _organisationService = organisationService;
            _appSettingService = appSettingService;
            _sharedDataRoleService = sharedDataRoleService;
            _territoryService = territoryService;
            _advisoryService = advisoryService;
            _activityService = activityService;
            _userService = userService;
            _changeProcessService = changeProcessService;
            _productService = productService;
            _informationItemService = informationItemService;
            _informationSectionService = informationSectionService;
            _clientInformationAnswer = clientInformationAnswer;
            _informationTemplateService = informationTemplateService;
            _clientAgreementService = clientAgreementService;
            _clientAgreementTermService = clientAgreementTermService;
            _clientAgreementMVTermService = clientAgreementMVTermService;
            _clientAgreementRuleService = clientAgreementRuleService;
            _clientInformationService = clientInformationService;
            _referenceService = referenceService;
            _milestoneService = milestoneService;
            _uWMService = uWMService;
            _taskingService = taskingService;
            _fileService = fileService;
            _businessActivityService = businessActivityService;
            _IDropdownListItem = dropdownListItem;
            _programmeService = programmeService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }



        [HttpGet]
        public async Task<IActionResult> GetProgrammeSections(Guid informationTemplateID)
        {
            InformationTemplate template = await _informationTemplateService.GetTemplate(informationTemplateID);
            Information model = new Information();
            var Litems = new List<InformationItems>();
            User user = null;
            try
            {
                user = await CurrentUser();
                foreach (var item in template.Sections)
                {
                    Litems.Add(new InformationItems() { Id = item.Id, Name = item.Name });

                }
                model.informationitem = Litems;
                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        //[HttpGet]
        //public async Task<InformationViewModel> LoadTemplate()
        //{
        //    var model = new InformationViewModel();
        //    model.Name = "Wellness and Health Associated Professionals";
        //    User user = null;
        //    try
        //    {
        //        user = await CurrentUser();

        //        // Important Notices
        //        var noticeItems = new List<InformationItemViewModel>();
        //        noticeItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _importantNotices });
        //        var importantNoticeSection = new InformationSectionViewModel() { Name = "Important Notices", Items = noticeItems };

        //        // Applicant     
        //        var itemList = new List<InformationItemViewModel>();

        //        itemList.Add(new InformationItemViewModel() { ControlType = "text", Icon = "icon-prepend fa fa-user", Label = "First Name", Name = "fname", Width = 3, Type = ItemType.TEXTBOX });
        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Last Name", Name = "lname", Width = 3, Type = ItemType.TEXTBOX });
        //        itemList.Add(new InformationItemViewModel() { Control = "email", Icon = "icon-prepend fa fa-envelope-o", Label = "E-mail", Name = "email", Width = 6, Type = ItemType.TEXTBOX });
        //        itemList.Add(null);

        //        itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-phone", Label = "Phone", Name = "phone", Width = 3, Type = ItemType.TEXTBOX });
        //        itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-fax", Label = "Fax", Name = "fax", Width = 3, Type = ItemType.TEXTBOX });
        //        itemList.Add(null);

        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Postal Address", Name = "paddress", Width = 6, Type = ItemType.TEXTBOX });
        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Street Address", Name = "saddress", Width = 6, Type = ItemType.TEXTBOX });
        //        itemList.Add(null);

        //        var applicantSection = new InformationSectionViewModel() { Name = "Applicant", Items = itemList };


        //        // Parties
        //        var partiesitemList = new List<InformationItemViewModel>();

        //        partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Insured", Name = "nameofinsured", Width = 6, Type = ItemType.TEXTBOX });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Note: employees, subagents and business partners who are actively involved in providing services to your clients need their own Insurance cover and must complete their own declaration.", Width = 12 });
        //        partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-book", Label = "Qualifications and date obtained", Name = "qualifications", Width = 6, Type = ItemType.TEXTBOX });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-briefcase", Label = "Company Name (if applicable)", Name = "companyname", Width = 6, Type = ItemType.TEXTBOX });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);

        //        var companytypeoptions = new List<SelectListItem>();
        //        companytypeoptions.Add(new SelectListItem() { Text = "Private Limited Liability Company", Value = "1" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Public Listed Company", Value = "2" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Public Unlisted Company", Value = "3" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Co-operative/Mutual", Value = "4" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Partnership", Value = "5" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Sole Trader", Value = "6" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Trust", Value = "7" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Charitable Trust", Value = "8" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Incorporated/Unincorporated Society", Value = "9" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Other", Value = "10" });
        //        partiesitemList.Add(new InformationItemViewModel() { Label = "Company Type", Name = "companytypeoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = companytypeoptions, Value = "1" });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);

        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Insurer:", Width = 3 });
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[InsurerCompany]]", Width = 9 });
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Broker:", Width = 3 });
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[BrokerCompany]]", Width = 9 });

        //        var partiesSection = new InformationSectionViewModel() { Name = "Parties", Items = partiesitemList };

        //        // Business Activities
        //        var businessactivitiesitemList = new List<InformationItemViewModel>();

        //        var associationoptions = new List<SelectListItem>();
        //        associationoptions.Add(new SelectListItem() { Text = "Wellness and Health Associated Professionals", Value = "1" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Association you hold a membership with", Name = "associationoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = associationoptions });
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-dollar", Label = "Gross income excluding GST (before payment of any franchise fees, expenses or tax)", Name = "grossincome", Width = 6, Type = ItemType.TEXTBOX });
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(null);
        //        var businessactivitiesoptions = new List<SelectListItem>();
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Massage Therapies", Value = "1" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Accupuncture", Value = "2" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Aura Soma", Value = "3" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Chinese Cupping", Value = "4" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Chios", Value = "5" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Colour Therapy", Value = "6" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Crystal Therapy", Value = "7" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Dry Needling", Value = "8" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Emotional Freedom Technique", Value = "9" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Facials", Value = "10" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Flower Essences", Value = "11" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Herbal Medicine", Value = "12" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Mediumship", Value = "13" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Motivational Interviewing", Value = "14" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Naturopathy", Value = "15" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Neural Integration Systems (Neurolink)", Value = "16" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Neurostructural Integration Technique (NST)", Value = "17" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Ortho-Bionomy", Value = "18" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Personal Training", Value = "19" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Pilates Teaching", Value = "20" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Polarity", Value = "21" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Scenar Therapy", Value = "22" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Soul Midwiving", Value = "23" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Sound Therapy/Music Therapy", Value = "24" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Supervision", Value = "25" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Theta Healing", Value = "26" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Visceral Manipulation", Value = "27" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Yoga Teacher", Value = "28" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Neuro Linguistic Programming", Value = "29" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Reflexology", Value = "30" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Reiki", Value = "31" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Thai traditional Massage", Value = "32" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Other", Value = "33" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Please indicate which therapies you practice:", Name = "businessactivitiesoptions", Width = 6, Type = ItemType.PERCENTAGEBREAKDOWN, DefaultText = "Select", Options = businessactivitiesoptions });
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(null);
        //        var overseasworkoptions = new List<SelectListItem>();
        //        overseasworkoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        overseasworkoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Do you currently perform work outside of NZ, or expect to perform work outside NZ in the next twelve months?", Name = "overseasworkoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = overseasworkoptions });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-comment", Label = "Details of work you expect to perform outside of NZ in the next twelve months", Name = "overseasworkdesc", Width = 6, Type = ItemType.TEXTBOX });

        //        var businessactivitiesSection = new InformationSectionViewModel() { Name = "Business Activities", Items = businessactivitiesitemList };

        //        // People Risk
        //        var peopleriskitemList = new List<InformationItemViewModel>();

        //        var peoplerisk1options = new List<SelectListItem>();
        //        peoplerisk1options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        peoplerisk1options.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you currently have insurance for key person, shareholder protection or other related people risk covers, i.e.income protection?", Name = "peoplerisk1options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk1options });
        //        peopleriskitemList.Add(null);
        //        peopleriskitemList.Add(null);

        //        var peoplerisk2options = new List<SelectListItem>();
        //        peoplerisk2options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        peoplerisk2options.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you have key people in your business whom are vital to the ongoing performance of the company?", Name = "peoplerisk2options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk2options });
        //        peopleriskitemList.Add(null);
        //        peopleriskitemList.Add(null);

        //        var peoplerisk3options = new List<SelectListItem>();
        //        peoplerisk3options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        peoplerisk3options.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        peopleriskitemList.Add(new InformationItemViewModel() { Label = "Would you like one of our qualified advisers to contact you to discuss key person and relevant people risk covers?", Name = "peoplerisk3options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk3options });
        //        peopleriskitemList.Add(null);
        //        peopleriskitemList.Add(null);

        //        var peopleriskSection = new InformationSectionViewModel() { Name = "People Risk", Items = peopleriskitemList };

        //        // Insurance History
        //        var insurancehistoryitemList = new List<InformationItemViewModel>();

        //        var insurancehistoryoptions = new List<SelectListItem>();
        //        insurancehistoryoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        insurancehistoryoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        insurancehistoryitemList.Add(new InformationItemViewModel() { Label = "In relation to the cover being applied for, have you ever had any insurance declined or cancelled; renewal refused; special conditions imposed; excess imposed; or claim rejected?", Name = "insurancehistoryoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = insurancehistoryoptions });
        //        insurancehistoryitemList.Add(null);
        //        insurancehistoryitemList.Add(null);

        //        var insurancehistorySection = new InformationSectionViewModel() { Name = "Insurance History", Items = insurancehistoryitemList };

        //        // Avaliable Cover
        //        var avaliablecoveritemList = new List<InformationItemViewModel>();
        //        avaliablecoveritemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _avaliableCover });
        //        var avaliablecoverSection = new InformationSectionViewModel() { Name = "Avaliable Cover", Items = avaliablecoveritemList };

        //        // Declaration
        //        var declarationItems = new List<InformationItemViewModel>();
        //        declarationItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _declaration });
        //        var declarationSection = new InformationSectionViewModel() { Name = "Declaration", Items = declarationItems };

        //        var sections = new List<InformationSectionViewModel>();

        //        sections.Add(importantNoticeSection);
        //        sections.Add(applicantSection);
        //        sections.Add(partiesSection);
        //        sections.Add(businessactivitiesSection);
        //        sections.Add(peopleriskSection);
        //        sections.Add(insurancehistorySection);
        //        sections.Add(avaliablecoverSection);
        //        sections.Add(declarationSection);

        //        model.Sections = sections;

        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        throw ex;
        //    }
        //}

   //     [HttpGet]
   //     public async Task<InformationViewModel> LoadHianzTemplate()
   //     {
   //         InformationViewModel model = new InformationViewModel();
   //         model.Name = "HIANZ Motor Vehicle";
   //         User user = null;

   //         try
   //         {
   //             user = await CurrentUser();
   //             // Applicant     
   //             var appItemList = new List<InformationItemViewModel>();

   //             appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-user", Label = "First Name", Name = "fname", Width = 3, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Last Name", Name = "lname", Width = 3, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Control = "email", Icon = "icon-prepend fa fa-envelope-o", Label = "E-mail", Name = "email", Width = 6, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             appItemList.Add(new InformationItemViewModel { Control = "tel", Icon = "icon-prepend fa fa-phone", Label = "Phone", Name = "phone", Width = 3, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Control = "tel", Icon = "icon-prepend fa fa-fax", Label = "Fax", Name = "fax", Width = 3, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Postal Address", Name = "paddress", Width = 6, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Street Address", Name = "saddress", Width = 6, Type = ItemType.TEXTBOX });
   //             appItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             // Organisation Location     
   //             var locItemList = new List<InformationItemViewModel>();
   //             string organisation = user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Name;
   //             locItemList.Add(new InformationItemViewModel { Label = "Organisation Name:", Width = 4, Type = ItemType.LABEL });
   //             locItemList.Add(new InformationItemViewModel { Label = organisation, Width = 8, Type = ItemType.LABEL });
   //             locItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             string organisationLocation = user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.CommonName + "<br />" +
   //                 user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Street + "<br />" +
   //                 user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Suburb + "<br />" +
   //                 user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Postcode + "<br />" +
   //                 user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.City + "<br />" +
   //                 user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Country;
   //             locItemList.Add(new InformationItemViewModel { Label = "Organisation Location:", Width = 4, Type = ItemType.LABEL });
   //             locItemList.Add(new InformationItemViewModel { Label = organisationLocation, Width = 8, Type = ItemType.LABEL });
   //             locItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             // Registered MV     
   //             var regItemList = new List<InformationItemViewModel>();
   //             string regVehicle = "regvehicle";
   //             regItemList.Add(new InformationItemViewModel { Label = "Add Vehicle", Name = regVehicle + "TableResult", Width = 6, Type = ItemType.MOTORVEHICLELIST });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             regItemList.Add(new InformationItemViewModel { Control = "text", Label = "Registration Number", Name = regVehicle, Width = 5, Type = ItemType.TEXTBOX });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
   //             regItemList.Add(new InformationItemViewModel { Label = "Search", Width = 1, Type = ItemType.JSBUTTON, Value = "SearchMotorVehicle(this, '#" + regVehicle + "');" });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             regItemList.Add(new InformationItemViewModel { Label = "Year:", Width = 2, Type = ItemType.LABEL });
   //             regItemList.Add(new InformationItemViewModel { Label = "<span id=\"" + regVehicle + "Year\"></span>", Width = 2, Type = ItemType.LABEL });
   //             regItemList.Add(new InformationItemViewModel { Label = "Make:", Width = 2, Type = ItemType.LABEL });
   //             regItemList.Add(new InformationItemViewModel { Label = "<span id=\"" + regVehicle + "Make\"></span>", Width = 2, Type = ItemType.LABEL });
   //             regItemList.Add(new InformationItemViewModel { Label = "Model:", Width = 2, Type = ItemType.LABEL });
   //             regItemList.Add(new InformationItemViewModel { Label = "<span id=\"" + regVehicle + "Model\"></span>", Width = 2, Type = ItemType.LABEL });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             regItemList.Add(new InformationItemViewModel { Control = "text", Label = "Estimated Market Value", Name = regVehicle + "TBMarketValue", Width = 4, Type = ItemType.TEXTBOX });
   //             regItemList.Add(new InformationItemViewModel { Control = "text", Label = "Fleet Number", Name = regVehicle + "TBFleetNo", Width = 4, Type = ItemType.TEXTBOX });
   //             var areaOperationsOptions = new List<SelectListItem> {
   //             new SelectListItem { Text = "-- Select --", Value = "" },
   //             new SelectListItem { Text = "Auckland", Value = "1" },
   //             new SelectListItem { Text = "Wellington", Value = "2" },
   //             new SelectListItem { Text = "Rest of North Island", Value = "3" },
   //             new SelectListItem { Text = "Christchurch", Value = "4" },
   //             new SelectListItem { Text = "Rest of South Island", Value = "5" }
   //         };
   //             regItemList.Add(new InformationItemViewModel { Label = "Area of Operation", Name = regVehicle + "DDAreaOp", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = areaOperationsOptions });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             var vehicleTypeOptions = new List<SelectListItem> {
   //             new SelectListItem { Text = "-- Select --", Value = "" },
   //             new SelectListItem { Text = "Registered Vehicle", Value = "1" },
   //             new SelectListItem { Text = "Registered Towed", Value = "2" },
   //             new SelectListItem { Text = "Registered Plant", Value = "3" },
   //             new SelectListItem { Text = "Static Vehicle", Value = "5"}
   //         };
   //             regItemList.Add(new InformationItemViewModel { Label = "Type of Vehicle", Name = regVehicle + "DDVehicleType", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleTypeOptions });
   //             var vehicleUsageOptions = new List<SelectListItem> {
   //             new SelectListItem { Text = "-- Select --", Value = "" },
   //             new SelectListItem { Text = "Car and Truck Rental Service", Value = "1" },
   //             new SelectListItem { Text = "General Business Use", Value = "2" },
   //             new SelectListItem { Text = "Static Vehicle", Value = "3"}
   //         };
   //             regItemList.Add(new InformationItemViewModel { Label = "Use", Name = regVehicle + "DDUsage", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleUsageOptions });
   //             var vehicleSubUsageOptions = new List<SelectListItem> {
   //             new SelectListItem { Text = "-- Select --", Value = "" },
   //             new SelectListItem { Text = "Company", Value = "1" },
   //             new SelectListItem { Text = "Private", Value = "2" },
   //             new SelectListItem { Text = "Rental", Value = "3" },
   //             new SelectListItem { Text = "Underage Private (under 25 years)", Value = "4" },
   //             new SelectListItem { Text = "Static Vehicle", Value = "5"}
   //         };
   //             regItemList.Add(new InformationItemViewModel { Label = "Sub use", Name = regVehicle + "DDSubUse", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleSubUsageOptions });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             //var interestedParties = new List<SelectListItem> {
   //             //	new SelectListItem { Text = "ANZ Bank", Value = "1" },
   //             //	new SelectListItem { Text = "ASB Bank", Value = "2" },
   //             //	new SelectListItem { Text = "BNZ Bank", Value = "3" }
   //             //};
   //             var interestedParties = new List<SelectListItem>();
   //             var organisationList = await _organisationService.GetAllOrganisations();
   //             foreach (Organisation org in organisationList.Where(o => o.OrganisationType.Name == "financial"))
   //                 interestedParties.Add(new SelectListItem { Text = org.Name, Value = org.Id.ToString() });

   //             regItemList.Add(new InformationItemViewModel { Label = "Interested Parties", Name = regVehicle + "Parties", Width = 6, Type = ItemType.MULTISELECT, Options = interestedParties });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
   //             regItemList.Add(new InformationItemViewModel { Control = "textarea", Label = "Notes", Name = regVehicle + "Notes", Width = 6, Type = ItemType.TEXTAREA });
   //             regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
   //             regItemList.Add(new InformationItemViewModel { Label = "Add Vehicle", Name = regVehicle + "Add", Width = 1, Type = ItemType.JSBUTTON, Value = "AddMotorVehicle(this, '#" + regVehicle + "', '#" + regVehicle + "TableResult');" });

   //             // Unregistered MV/Plant/Other
   //             var otherItemList = new List<InformationItemViewModel>();
   //             string othervehicle = "othervehicle";
   //             otherItemList.Add(new InformationItemViewModel { Label = "Add Vehicle", Name = othervehicle + "TableResult", Width = 6, Type = ItemType.STATICVEHICLEPLANTLIST });
   //             otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Year", Name = othervehicle + "Year", Width = 4, Type = ItemType.TEXTBOX });
   //             otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Make", Name = othervehicle + "Make", Width = 4, Type = ItemType.TEXTBOX });
   //             otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Model", Name = othervehicle + "Model", Width = 4, Type = ItemType.TEXTBOX });
   //             otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Estimated Market Value", Name = othervehicle + "TBMarketValue", Width = 4, Type = ItemType.TEXTBOX });
   //             otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Fleet Number", Name = othervehicle + "TBFleetNo", Width = 4, Type = ItemType.TEXTBOX });
   //             otherItemList.Add(new InformationItemViewModel { Label = "Area of Operation", Name = othervehicle + "DDAreaOp", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = areaOperationsOptions });
   //             otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             var otherVehicleTypeOptions = new List<SelectListItem> {
   //             new SelectListItem { Text = "-- Select --", Value = "" },
   //             new SelectListItem { Text = "Non-Registered Plant", Value = "4" },
   //             new SelectListItem { Text = "Static Vehicle", Value = "5"}
   //         };
   //             otherItemList.Add(new InformationItemViewModel { Label = "Type of Vehicle", Name = othervehicle + "DDVehicleType", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = otherVehicleTypeOptions });
   //             otherItemList.Add(new InformationItemViewModel { Label = "Use", Name = othervehicle + "DDUsage", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleUsageOptions });
   //             otherItemList.Add(new InformationItemViewModel { Label = "Sub use", Name = othervehicle + "DDSubUse", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleSubUsageOptions });
   //             otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

   //             otherItemList.Add(new InformationItemViewModel { Label = "Interested Parties", Name = othervehicle + "Parties", Width = 6, Type = ItemType.MULTISELECT, Options = interestedParties });
   //             otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
   //             otherItemList.Add(new InformationItemViewModel { Control = "textarea", Label = "Notes", Name = othervehicle + "Notes", Width = 6, Type = ItemType.TEXTAREA });
   //             otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
   //             otherItemList.Add(new InformationItemViewModel { Label = "Add Unplated Vehicle/Plant", Name = othervehicle + "Add", Width = 1, Type = ItemType.JSBUTTON, Value = "AddMotorVehicle(this, '#" + othervehicle + "', '#" + othervehicle + "TableResult');" });

   //             // set sections
   //             model.Sections = new List<InformationSectionViewModel> {
   //             new InformationSectionViewModel { Items = appItemList, Name = "You" },
   //             //new InformationSectionViewModel { Items = locItemList, Name = "Location" },
   //             new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Organisational Units", CustomView = "ICIBHianzOU" },
   //             new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Location", CustomView = "ICIBHianzLocation" },
   //             new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Registered Vehicles", CustomView = "ICIBHianzMotor" },
   //             new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Other Vehicles/Mobile Plant", CustomView = "ICIBHianzPlant" }
   //             //new InformationSectionViewModel { Items = regItemList, Name = "Registered Vehicles" },
			//	//new InformationSectionViewModel { Items = otherItemList, Name = "Other Vehicles/Mobile Plant" }
			//};

   //             return model;
   //         }
   //         catch (Exception ex)
   //         {
   //             await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
   //             throw ex;
   //         }
   //     }

        //[HttpGet]
        //public async Task<InformationViewModel> LoadTestData()
        //{
        //    var model = new InformationViewModel();
        //    User user = null;
        //    model.Name = "Wellness and Health Associated Professionals";

        //    try
        //    {
        //        user = await CurrentUser();
        //        // Important Notices
        //        var noticeItems = new List<InformationItemViewModel>();
        //        noticeItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _importantNotices });
        //        var importantNoticeSection = new InformationSectionViewModel() { Name = "Important Notices", Items = noticeItems };

        //        // Applicant     
        //        var itemList = new List<InformationItemViewModel>();

        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "First Name", Name = "fname", Width = 3, Type = ItemType.TEXTBOX, Value = "ApplicantFirstName" });
        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Last Name", Name = "lname", Width = 3, Type = ItemType.TEXTBOX, Value = "ApplicantLastName" });
        //        itemList.Add(new InformationItemViewModel() { Control = "email", Icon = "icon-prepend fa fa-envelope-o", Label = "E-mail", Name = "email", Width = 6, Type = ItemType.TEXTBOX, Value = "TestApplicant@DealEngine.com" });
        //        itemList.Add(null);

        //        itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-phone", Label = "Phone", Name = "phone", Width = 3, Type = ItemType.TEXTBOX, Value = "091234567" });
        //        itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-fax", Label = "Fax", Name = "fax", Width = 3, Type = ItemType.TEXTBOX, Value = "091234568" });
        //        itemList.Add(null);

        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Postal Address", Name = "paddress", Width = 6, Type = ItemType.TEXTBOX, Value = "1 Queen St, CBD, Auckland" });
        //        itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Street Address", Name = "saddress", Width = 6, Type = ItemType.TEXTBOX });
        //        itemList.Add(null);

        //        var applicantSection = new InformationSectionViewModel() { Name = "Applicant", Items = itemList };


        //        // Parties
        //        var partiesitemList = new List<InformationItemViewModel>();

        //        partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Insured", Name = "nameofinsured", Width = 6, Type = ItemType.TEXTBOX, Value = "TestClientName" });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Note: employees, subagents and business partners who are actively involved in providing services to your clients need their own Insurance cover and must complete their own declaration.", Width = 12 });
        //        partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-book", Label = "Qualifications and date obtained", Name = "qualifications", Width = 6, Type = ItemType.TEXTBOX });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-briefcase", Label = "Company Name (if applicable)", Name = "companyname", Width = 6, Type = ItemType.TEXTBOX, Value = "TestClientCompany" });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);

        //        var companytypeoptions = new List<SelectListItem>();
        //        companytypeoptions.Add(new SelectListItem() { Text = "Private Limited Liability Company", Value = "1" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Public Listed Company", Value = "2" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Public Unlisted Company", Value = "3" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Co-operative/Mutual", Value = "4" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Partnership", Value = "5" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Sole Trader", Value = "6" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Trust", Value = "7" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Charitable Trust", Value = "8" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Incorporated/Unincorporated Society", Value = "9" });
        //        companytypeoptions.Add(new SelectListItem() { Text = "Other", Value = "10" });
        //        partiesitemList.Add(new InformationItemViewModel() { Label = "Company Type", Name = "companytypeoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = companytypeoptions, Value = "1" });
        //        partiesitemList.Add(null);
        //        partiesitemList.Add(null);

        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Insurer:", Width = 3 });
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[InsurerCompany]]", Width = 9 });
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Broker:", Width = 3 });
        //        partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[BrokerCompany]]", Width = 9 });

        //        var partiesSection = new InformationSectionViewModel() { Name = "Parties", Items = partiesitemList };

        //        // Business Activities
        //        var businessactivitiesitemList = new List<InformationItemViewModel>();

        //        var associationoptions = new List<SelectListItem>();
        //        associationoptions.Add(new SelectListItem() { Text = "Wellness and Health Associated Professionals", Value = "1" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Association you hold a membership with", Name = "associationoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = associationoptions, Value = "1" });
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-dollar", Label = "Gross income excluding GST (before payment of any franchise fees, expenses or tax)", Name = "grossincome", Width = 6, Type = ItemType.TEXTBOX, Value = "$10,000" });
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(null);
        //        var businessactivitiesoptions = new List<SelectListItem>();
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Massage Therapies", Value = "1" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Accupuncture", Value = "2" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Aura Soma", Value = "3" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Chinese Cupping", Value = "4" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Chios", Value = "5" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Colour Therapy", Value = "6" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Crystal Therapy", Value = "7" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Dry Needling", Value = "8" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Emotional Freedom Technique", Value = "9" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Facials", Value = "10" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Flower Essences", Value = "11" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Herbal Medicine", Value = "12" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Mediumship", Value = "13" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Motivational Interviewing", Value = "14" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Naturopathy", Value = "15" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Neural Integration Systems (Neurolink)", Value = "16" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Neurostructural Integration Technique (NST)", Value = "17" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Ortho-Bionomy", Value = "18" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Personal Training", Value = "19" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Pilates Teaching", Value = "20" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Polarity", Value = "21" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Scenar Therapy", Value = "22" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Soul Midwiving", Value = "23" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Sound Therapy/Music Therapy", Value = "24" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Supervision", Value = "25" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Theta Healing", Value = "26" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Visceral Manipulation", Value = "27" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Yoga Teacher", Value = "28" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Neuro Linguistic Programming", Value = "29" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Reflexology", Value = "30" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Reiki", Value = "31" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Thai traditional Massage", Value = "32" });
        //        businessactivitiesoptions.Add(new SelectListItem() { Text = "Other", Value = "33" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Please indicate which therapies you practice:", Name = "businessactivitiesoptions", Width = 6, Type = ItemType.PERCENTAGEBREAKDOWN, DefaultText = "Select", Options = businessactivitiesoptions });
        //        businessactivitiesitemList.Add(null);
        //        businessactivitiesitemList.Add(null);
        //        var overseasworkoptions = new List<SelectListItem>();
        //        overseasworkoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        overseasworkoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Do you currently perform work outside of NZ, or expect to perform work outside NZ in the next twelve months?", Name = "overseasworkoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = overseasworkoptions, Value = "2" });
        //        businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-comment", Label = "Details of work you expect to perform outside of NZ in the next twelve months", Name = "overseasworkdesc", Width = 6, Type = ItemType.TEXTBOX });

        //        var businessactivitiesSection = new InformationSectionViewModel() { Name = "Business Activities", Items = businessactivitiesitemList };

        //        // People Risk
        //        var peopleriskitemList = new List<InformationItemViewModel>();

        //        var peoplerisk1options = new List<SelectListItem>();
        //        peoplerisk1options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        peoplerisk1options.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you currently have insurance for key person, shareholder protection or other related people risk covers, i.e.income protection?", Name = "peoplerisk1options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk1options, Value = "2" });
        //        peopleriskitemList.Add(null);
        //        peopleriskitemList.Add(null);

        //        var peoplerisk2options = new List<SelectListItem>();
        //        peoplerisk2options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        peoplerisk2options.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you have key people in your business whom are vital to the ongoing performance of the company?", Name = "peoplerisk2options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk2options, Value = "2" });
        //        peopleriskitemList.Add(null);
        //        peopleriskitemList.Add(null);

        //        var peoplerisk3options = new List<SelectListItem>();
        //        peoplerisk3options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        peoplerisk3options.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        peopleriskitemList.Add(new InformationItemViewModel() { Label = "Would you like one of our qualified advisers to contact you to discuss key person and relevant people risk covers?", Name = "peoplerisk3options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk3options, Value = "2" });
        //        peopleriskitemList.Add(null);
        //        peopleriskitemList.Add(null);

        //        var peopleriskSection = new InformationSectionViewModel() { Name = "People Risk", Items = peopleriskitemList };

        //        // Insurance History
        //        var insurancehistoryitemList = new List<InformationItemViewModel>();

        //        var insurancehistoryoptions = new List<SelectListItem>();
        //        insurancehistoryoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
        //        insurancehistoryoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
        //        insurancehistoryitemList.Add(new InformationItemViewModel() { Label = "In relation to the cover being applied for, have you ever had any insurance declined or cancelled; renewal refused; special conditions imposed; excess imposed; or claim rejected?", Name = "insurancehistoryoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = insurancehistoryoptions, Value = "2" });
        //        insurancehistoryitemList.Add(null);
        //        insurancehistoryitemList.Add(null);

        //        var insurancehistorySection = new InformationSectionViewModel() { Name = "Insurance History", Items = insurancehistoryitemList };

        //        // Avaliable Cover
        //        var avaliablecoveritemList = new List<InformationItemViewModel>();
        //        avaliablecoveritemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _avaliableCover });
        //        var avaliablecoverSection = new InformationSectionViewModel() { Name = "Avaliable Cover", Items = avaliablecoveritemList };

        //        // Declaration
        //        var declarationItems = new List<InformationItemViewModel>();
        //        declarationItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _declaration });
        //        var declarationSection = new InformationSectionViewModel() { Name = "Declaration", Items = declarationItems };

        //        var sections = new List<InformationSectionViewModel>();

        //        sections.Add(importantNoticeSection);
        //        sections.Add(applicantSection);
        //        sections.Add(partiesSection);
        //        sections.Add(businessactivitiesSection);
        //        sections.Add(peopleriskSection);
        //        sections.Add(insurancehistorySection);
        //        sections.Add(avaliablecoverSection);
        //        sections.Add(declarationSection);

        //        model.Sections = sections;

        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        throw ex;
        //    }
        //}

        // GET: Information
        [HttpGet]
        public async Task<IActionResult> ViewInformation(Guid id)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                InformationViewModel model = await GetClientInformationSheetViewModel(id);

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StartInformation(Guid id)
        {
            User user = null;
            try
            {
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
                ClientInformationSheet sheet = clientProgramme.InformationSheet;
                InformationViewModel model = await GetInformationViewModel(clientProgramme);
                user = await CurrentUser();
                model.ClientInformationSheet = sheet;
                model.ClientProgramme = clientProgramme;
                model.CompanyName = _appSettingService.GetCompanyTitle;

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (sheet.Status == "Not Started")
                    {
                        sheet.Status = "Started";
                    }
                    foreach (var section in model.Sections)
                        foreach (var item in section.Items.Where(i => (i.Type != ItemType.LABEL && i.Type != ItemType.SECTIONBREAK && i.Type != ItemType.JSBUTTON && i.Type != ItemType.SUBMITBUTTON)))
                        {
                            var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
                            if (answer != null)
                                item.Value = answer.Value;
                            else
                                sheet.AddAnswer(item.Name, "");
                        }
                    await uow.Commit();
                }

                var boats = new List<BoatViewModel>();
                foreach (Boat b in sheet.Boats)
                {
                    boats.Add(BoatViewModel.FromEntity(b));
                }
                model.Boats = boats;

                var operators = new List<OrganisationViewModel>();
                var organisationList = await _organisationService.GetAllOrganisations();
                foreach (Organisation skipper in organisationList.Where(o => o.OrganisationType.Name == "Skipper"))
                {
                    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(skipper);
                    ovm.OrganisationName = skipper.Name;
                    ovm.OrganisationEmail = skipper.Email;
                    operators.Add(ovm);
                }
                model.Operators = operators;

                var claims = new List<ClaimViewModel>();
                foreach (ClaimNotification cl in sheet.ClaimNotifications)
                {
                    claims.Add(ClaimViewModel.FromEntity(cl));
                }
                model.Claims = claims;

                var boatUses = new List<BoatUseViewModel>();
                foreach (BoatUse bu in sheet.BoatUses)
                {
                    boatUses.Add(BoatUseViewModel.FromEntity(bu));
                }
                model.BoatUse = boatUses;

                // TODO - find a better way to pass these in
                model.HasVehicles = sheet.Vehicles.Count > 0;
                var vehicles = new List<VehicleViewModel>();
                foreach (Vehicle v in sheet.Vehicles)
                {
                    vehicles.Add(VehicleViewModel.FromEntity(v));
                }
                model.AllVehicles = vehicles;
                model.RegisteredVehicles = vehicles.Where(v => !string.IsNullOrWhiteSpace(v.Registration));
                model.UnregisteredVehicles = vehicles.Where(v => string.IsNullOrWhiteSpace(v.Registration));

                var organisationalUnits = new List<OrganisationalUnitViewModel>();
                model.OrganisationalUnitsVM = new OrganisationalUnitVM();
                model.OrganisationalUnitsVM.OrganisationalUnits = new List<SelectListItem>();
                var locations = new List<LocationViewModel>();
                var buildings = new List<BuildingViewModel>();
                var waterLocations = new List<WaterLocationViewModel>();
                foreach (OrganisationalUnit ou in sheet.Owner.OrganisationalUnits)
                {
                    organisationalUnits.Add(new OrganisationalUnitViewModel
                    {
                        OrganisationalUnitId = ou.Id,
                        Name = ou.Name
                    });

                    model.OrganisationalUnitsVM.OrganisationalUnits.Add(new SelectListItem { Text = ou.Name, Value = ou.Id.ToString() });

                    foreach (Location loc in ou.Locations)
                    {
                        locations.Add(LocationViewModel.FromEntity(loc));

                        foreach (Building bui in loc.Buildings)
                        {
                            buildings.Add(BuildingViewModel.FromEntity(bui));
                        }

                        foreach (WaterLocation wl in loc.WaterLocations)
                        {
                            waterLocations.Add(WaterLocationViewModel.FromEntity(wl));
                        }
                    }
                }

                var interestedParties = new List<OrganisationViewModel>();
                var orgList = await _organisationService.GetAllOrganisations();
                foreach (Organisation org in orgList.Where(o => o.OrganisationType != null))
                {
                    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
                    ovm.OrganisationName = org.Name;
                    interestedParties.Add(ovm);
                }

                var boatUse = new List<BoatUse>();
                foreach (BoatUse bu in sheet.BoatUses)
                {
                    boatUse.Add(bu);

                }

                //var availableProducts = new List<ProductItem>();
                // TODO verify that this is no longer needed with the Programme Implementation
                //foreach (var otherSheet in _clientInformationService.GetAllInformationFor (sheet.Owner)) {
                //	// skip any information sheet that has been renewed or updated
                //	if (otherSheet.NextInformationSheet != null)
                //		continue;
                //	availableProducts.Add (new ProductItem {
                //		Name = otherSheet.Product.Name + " for " + sheet.Owner.Name,
                //		Status = otherSheet.Status,
                //		RedirectLink = "/Information/EditInformational/" + otherSheet.Id
                //	});
                //}

                var userDetails = _mapper.Map<UserDetailsVM>(CurrentUser());
                userDetails.PostalAddress = user.Address;
                userDetails.StreetAddress = user.Address;

                var organisationDetails = new OrganisationDetailsVM
                {
                    Name = sheet.Owner.Name,
                    Phone = sheet.Owner.Phone,
                    Website = sheet.Owner.Domain
                };

                model.OrganisationalUnits = organisationalUnits;
                model.Locations = locations;
                model.Buildings = buildings;
                model.WaterLocations = waterLocations;
                model.InterestedParties = interestedParties;
                //model.AvailableProducts = availableProducts;
                model.OrganisationDetails = organisationDetails;
                model.UserDetails = userDetails;

                return View("InformationWizard", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditPanel(Guid panelId, string panelName, int panelPosition)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {

                    if (panelName != null)
                    {

                        InformationSection section = await _informationSectionService.GetSection(panelId);
                        section.Position = panelPosition;
                        // TODO: Add these items at templates so it can be clonned properly 
                        await uow.Commit();
                    }

                }
                return Json(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ViewProgrammeDetails(Guid Id)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(Id);
                ClientInformationSheet sheet = clientProgramme.InformationSheet;

                InformationViewModel model = await GetInformationViewModel(clientProgramme);                
                model.ClientProgramme = clientProgramme;
                ViewBag.Title = "View Information Sheet ";
                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartialViewProgramme(String name, Guid id)
        {
            var MarinaLocations = new List<OrganisationViewModel>();
            User user = null;

            try
            {
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
                ClientInformationSheet sheet = clientProgramme.InformationSheet;
                InformationViewModel model = await GetInformationViewModel(clientProgramme);
                model.ClientInformationSheet = sheet;                
                model.SectionView = name;
                model.ClientProgramme = clientProgramme;                
                user = await CurrentUser();
                try
                {                   

                    foreach (var section in model.Sections)
                        foreach (var item in section.Items)
                        {
                            var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
                            if (answer != null)
                                item.Value = answer.Value;
                        }

                    var boats = new List<BoatViewModel>();
                    foreach (Boat b in sheet.Boats)
                    {
                        boats.Add(BoatViewModel.FromEntity(b));
                    }
                    model.Boats = boats;

                    var operators = new List<OrganisationViewModel>();
                    var orgSkipperList = await _organisationService.GetAllOrganisations();
                    foreach (Organisation or in orgSkipperList.Where(o => o.OrganisationType.Name == "Skipper"))
                    {
                        OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(or);
                        ovm.OrganisationName = or.Name;
                        operators.Add(ovm);
                    }
                    model.Operators = operators;

                    List<SelectListItem> skipperlist = new List<SelectListItem>();

                    for (var i = 0; i < model.Operators.Count(); i++)
                    {
                        skipperlist.Add(new SelectListItem
                        {
                            Selected = false,
                            Text = model.Operators.ElementAtOrDefault(i).OrganisationName,
                            Value = model.Operators.ElementAtOrDefault(i).ID.ToString(),
                        });
                    }

                    model.SkipperList = skipperlist;

                    var claims = new List<ClaimViewModel>();
                    foreach (ClaimNotification cl in sheet.ClaimNotifications)
                    {
                        claims.Add(ClaimViewModel.FromEntity(cl));
                    }
                    model.Claims = claims;

                    var interestedParties = new List<OrganisationViewModel>();
                    try
                    {
                        var insuranceAttributesList = await _insuranceAttributeService.GetInsuranceAttributes();
                        foreach (InsuranceAttribute IA in insuranceAttributesList.Where(ia => ia.InsuranceAttributeName == "Financial" || ia.InsuranceAttributeName == "Private" || ia.InsuranceAttributeName == "CoOwner"))
                        {
                            foreach (var org in IA.IAOrganisations)
                            {
                                if (org.OrganisationType.Name == "Person - Individual" || org.OrganisationType.Name == "Corporation – Limited liability")
                                {
                                    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
                                    ovm.OrganisationName = org.Name;
                                    interestedParties.Add(ovm);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    model.InterestedParties = interestedParties;
                    List<SelectListItem> linterestedparty = new List<SelectListItem>();

                    for (var i = 0; i < model.InterestedParties.Count(); i++)
                    {
                        linterestedparty.Add(new SelectListItem
                        {
                            Selected = false,
                            Text = model.InterestedParties.ElementAtOrDefault(i).OrganisationName,
                            Value = model.InterestedParties.ElementAtOrDefault(i).ID.ToString(),
                        });
                    }

                    model.InterestedPartyList = linterestedparty;

                    var boatUses = new List<BoatUseViewModel>();
                    foreach (BoatUse bu in sheet.BoatUses)
                    {
                        boatUses.Add(BoatUseViewModel.FromEntity(bu));
                    }
                    List<SelectListItem> list = new List<SelectListItem>();

                    try
                    {
                        foreach (var bu in boatUses)
                        {
                            var text = bu.BoatUseCategory.Substring(0, 4);
                            var val = bu.BoatUseId.ToString();

                            list.Add(new SelectListItem
                            {
                                Selected = false,
                                Value = val,
                                Text = text
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException);
                    }

                    model.BoatUseslist = list;
                    // TODO - find a better way to pass these in
                    model.HasVehicles = sheet.Vehicles.Count > 0;
                    var vehicles = new List<VehicleViewModel>();
                    foreach (Vehicle v in sheet.Vehicles)
                    {
                        vehicles.Add(VehicleViewModel.FromEntity(v));
                    }
                    model.AllVehicles = vehicles;
                    model.RegisteredVehicles = vehicles.Where(v => !string.IsNullOrWhiteSpace(v.Registration));
                    model.UnregisteredVehicles = vehicles.Where(v => string.IsNullOrWhiteSpace(v.Registration));

                    var organisationalUnits = new List<OrganisationalUnitViewModel>();
                    model.OrganisationalUnitsVM = new OrganisationalUnitVM();
                    model.OrganisationalUnitsVM.OrganisationalUnits = new List<SelectListItem>();
                    var locations = new List<LocationViewModel>();
                    var buildings = new List<BuildingViewModel>();
                    var waterLocations = new List<WaterLocationViewModel>();
                    foreach (OrganisationalUnit ou in sheet.Owner.OrganisationalUnits)
                    {
                        organisationalUnits.Add(new OrganisationalUnitViewModel
                        {
                            OrganisationalUnitId = ou.Id,
                            Name = ou.Name
                        });
                        model.OrganisationalUnitsVM.OrganisationalUnits.Add(new SelectListItem { Text = ou.Name, Value = ou.Id.ToString() });
                    }

                    foreach (Location loc in sheet.Locations)
                    {
                        locations.Add(LocationViewModel.FromEntity(loc));
                    }

                    foreach (Building bui in sheet.Buildings)
                    {
                        buildings.Add(BuildingViewModel.FromEntity(bui));
                    }
                    try
                    {
                        var insuranceAttributeList1 = await _insuranceAttributeService.GetInsuranceAttributes();
                        foreach (InsuranceAttribute IA in insuranceAttributeList1.Where(ia => ia.InsuranceAttributeName == "Marina" || ia.InsuranceAttributeName == "Other Marina"))
                        {
                            foreach (var org in IA.IAOrganisations)
                            {
                                if (org.OrganisationType.Name == "Corporation – Limited liability")
                                {
                                    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
                                    ovm.OrganisationName = org.Name;
                                    MarinaLocations.Add(ovm);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    model.MarinaLocations = MarinaLocations;

                    foreach (WaterLocation wl in sheet.WaterLocations)
                    {
                        waterLocations.Add(WaterLocationViewModel.FromEntity(wl));
                    }

                    var availableProducts = new List<SelectListItem>();


                    var userDetails = _mapper.Map<UserDetailsVM>(await CurrentUser());
                    userDetails.PostalAddress = user.Address;
                    userDetails.StreetAddress = user.Address;

                    var organisationDetails = new OrganisationDetailsVM
                    {
                        Name = sheet.Owner.Name,
                        Phone = sheet.Owner.Phone,
                        Website = sheet.Owner.Domain
                    };

                    model.OrganisationalUnits = organisationalUnits;
                    model.Locations = locations;
                    model.Buildings = buildings;
                    model.WaterLocations = waterLocations;
                    model.InterestedParties = interestedParties;
                    model.ClaimProducts = availableProducts;
                    model.OrganisationDetails = organisationDetails;
                    model.UserDetails = userDetails;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                ViewBag.Title = "Programme Email Template ";
                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetProductName(Guid id)
        {
            List<string> productname = new List<string>();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
                ClientInformationSheet sheet = clientProgramme.InformationSheet;

                foreach (ClientAgreement agreement in clientProgramme.Agreements.Where(a => a.Product.IsMultipleOption == true && a.DateDeleted == null))
                {
                    productname.Add(agreement.Product.Name);
                }

                return Json(productname);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCoverOptions(string[] Answers, Guid ProgrammeId)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(ProgrammeId);

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    foreach (var agreement in clientProgramme.Agreements)
                    {
                        if (agreement.Product.IsMultipleOption)
                        {
                            foreach (var term in agreement.ClientAgreementTerms)
                            {
                                term.Bound = false;
                                await uow.Commit();
                            }
                        }
                    }
                }

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    foreach (var option in Answers)
                    {
                        if (option != "None")
                        {
                            var clientAgreementTerm = await _clientAgreementTermService.GetAllClientAgreementTerm();
                            List<ClientAgreementTerm> listClientAgreementerm = clientAgreementTerm.Where(cagt => cagt.Id == Guid.Parse(option)).ToList();
                            foreach (var term in listClientAgreementerm)
                            {
                                term.Bound = true;
                                await uow.Commit();
                            }
                        }
                    }
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCoverOptions(Guid ProgrammeId)
        {
            List<ClientAgreementTerm> listClientAgreementerm = new List<ClientAgreementTerm>();
            List<Guid> listClientAgreementermid = new List<Guid>();
            var count = 0;
            String[] OptionItem;
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(ProgrammeId);

                String[][] OptionItems = new String[clientProgramme.Agreements.Count][];
                foreach (var agreement in clientProgramme.Agreements)
                {

                    foreach (var term in agreement.ClientAgreementTerms)
                    {
                        OptionItem = new String[2];
                        if (term.Bound)
                        {
                            OptionItem[0] = agreement.Product.Name;
                            OptionItem[1] = "" + term.Id;
                            OptionItems[count] = OptionItem;
                            count++;
                        }
                    }
                }
                return Json(OptionItems);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetClaimHistory(Guid ClientInformationSheet)
        {
            String[][] ClaimAnswers = new String[5][];
            var count = 0;
            String[] ClaimItem;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllClaimHistory().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "Claimexp1" || c.ItemName == "Claimexp2" || c.ItemName == "Claimexp3"
                                                                                                                                                         || c.ItemName == "Claimexp4" || c.ItemName == "Claimexp5")))
                {
                    ClaimItem = new String[3];

                    for (var i = 0; i < 1; i++)
                    {
                        ClaimItem[i] = answer.ItemName;
                        ClaimItem[i + 1] = answer.Value;
                        ClaimItem[i + 2] = answer.ClaimDetails;
                    }

                    ClaimAnswers[count] = ClaimItem;
                    count++;
                }

                return Json(ClaimAnswers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditInformation(Guid id)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var clientProgramme = await _programmeService.GetClientProgramme(id);
                var sheet = clientProgramme.InformationSheet;
                InformationViewModel model = await GetInformationViewModel(clientProgramme);
                SharedRoleViewModel sharedRoleViewModel = await GetSharedRoleViewModel(sheet);
                model.SharedRoleViewModel = sharedRoleViewModel;
                RevenueByActivityViewModel revenueByActivityViewModel = new RevenueByActivityViewModel();                
                if (clientProgramme.BaseProgramme.TerritoryTemplates.Count > 0 && clientProgramme.BaseProgramme.BusinessActivityTemplates.Count > 0)
                {
                    revenueByActivityViewModel = await GetRevenueActivityViewModel(sheet);
                }
                model.RevenueByActivityViewModel = revenueByActivityViewModel;
                model.AnswerSheetId = sheet.Id;
                model.Id = id;
                model.ClientInformationSheet = sheet;
                model.ClientProgramme = clientProgramme;
                model.CompanyName = _appSettingService.GetCompanyTitle;
                //testing dynamic wizard here
                var isSubsystem = await _programmeService.IsBaseClass(clientProgramme);
                if (isSubsystem)
                {
                    model.Wizardsteps = LoadWizardsteps("Subsystem");
                }
                else
                {
                    model.Wizardsteps = LoadWizardsteps("Standard");
                }

                //build models from answers
                foreach(var answer in sheet.Answers) 
                {
                    var value = 0;
                    try
                    {
                        var split = answer.ItemName.Split('.').ToList();
                        if(split.LastOrDefault() == "FormDate")
                        {
                            Console.WriteLine("");
                        }
                        if (split.Count > 1)
                        {
                            
                            var modeltype = typeof(InformationViewModel).GetProperty(split.FirstOrDefault());
                            var infomodel = modeltype.GetValue(model);                            
                            var property = infomodel.GetType().GetProperty(split.LastOrDefault());

                            switch (property.PropertyType.Name)
                            {
                                case "Int32":
                                    int.TryParse(answer.Value, out value);
                                    property.SetValue(infomodel, value);
                                    break;
                                case "IList`1":
                                    var propertylist = (IList<SelectListItem>)property.GetValue(infomodel);
                                    var options = answer.Value.Split(',').ToList();
                                    foreach(var option in options)
                                    {
                                        propertylist.FirstOrDefault(i => i.Value == option).Selected = true;
                                    }                                    
                                    property.SetValue(infomodel, propertylist);
                                    break;
                                case "DateTime":
                                    property.SetValue(infomodel, DateTime.Parse(answer.Value));
                                    break;
                                default:
                                    property.SetValue(infomodel, answer.Value);
                                    break;
                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("");
                    }
                }
               
                foreach (var section in model.Sections)
                    foreach (var item in section.Items.Where(i => (i.Type != ItemType.LABEL && i.Type != ItemType.SECTIONBREAK && i.Type != ItemType.JSBUTTON && i.Type != ItemType.SUBMITBUTTON)))
                    {                        
                        var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
                        if (answer != null)
                        {
                            item.Value = answer.Value;
                        }
                        else
                            sheet.AddAnswer(item.Name, "");
                    }

                string advisoryDesc = "";
                if (sheet.Status == "Not Started")
                {
                    var milestone = await _milestoneService.GetMilestoneByBaseProgramme(clientProgramme.BaseProgramme.Id);
                    if (milestone != null)
                    {
                        var advisoryList = await _advisoryService.GetAdvisorysByMilestone(milestone);
                        var advisory = advisoryList.LastOrDefault(a => a.Activity.Name == "Agreement Status - Not Started" && a.DateDeleted == null);
                        if (advisory != null)
                        {
                            advisoryDesc = advisory.Description;
                        }
                    }

                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {                                
                        sheet.Status = "Started";
                        await uow.Commit();
                    }
                }

                model.Advisory = advisoryDesc;

                for (var i = 0; i < model.Sections.Count(); i++)
                {
                    for (var j = 0; j < model.Sections.ElementAtOrDefault(i).Items.Count(); j++)
                    {
                        var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == model.Sections.ElementAtOrDefault(i).Items.ElementAtOrDefault(j).Name);
                        if (answer != null)
                            model.Sections.ElementAtOrDefault(i).Items.ElementAtOrDefault(j).Value = answer.Value;
                    }
                }

                var boats = new List<BoatViewModel>();
                foreach (var b in sheet.Boats)
                {
                    boats.Add(BoatViewModel.FromEntity(b));
                }

                model.Boats = boats;

                var operators = new List<OrganisationViewModel>();

                foreach (Organisation skipperorg in sheet.Organisation.Where(o => o.OrganisationType.Name == "Person - Individual"))
                {
                    if (skipperorg.InsuranceAttributes.FirstOrDefault(ia => ia.InsuranceAttributeName == "Skipper") != null)
                    {
                        OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(skipperorg);
                        ovm.OrganisationName = skipperorg.Name;
                        ovm.OrganisationEmail = skipperorg.Email;
                        ovm.ID = skipperorg.Id;
                        operators.Add(ovm);
                    }
                }

                if (sheet.Owner.OrganisationType.Name == "Person - Individual")
                {
                    OrganisationViewModel ovmowner = _mapper.Map<OrganisationViewModel>(sheet.Owner);
                    ovmowner.OrganisationName = sheet.Owner.Name;
                    ovmowner.OrganisationEmail = sheet.Owner.Email;
                    ovmowner.ID = sheet.Owner.Id;
                    operators.Add(ovmowner);
                }

                model.Operators = operators;

                List<SelectListItem> skipperlist = new List<SelectListItem>();

                for (var i = 0; i < model.Operators.Count(); i++)
                {
                    skipperlist.Add(new SelectListItem
                    {
                        Selected = false,
                        Text = model.Operators.ElementAtOrDefault(i).OrganisationName,
                        Value = model.Operators.ElementAtOrDefault(i).ID.ToString(),
                    });

                }

                model.SkipperList = skipperlist;

                var claims = new List<ClaimViewModel>();
                for (var i = 0; i < sheet.ClaimNotifications.Count; i++)
                {
                    claims.Add(ClaimViewModel.FromEntity(sheet.ClaimNotifications.ElementAtOrDefault(i)));
                }

                model.Claims = claims;

                var businessContracts = new List<BusinessContractViewModel>();
                for (var i = 0; i < sheet.BusinessContracts.Count; i++)
                {
                    businessContracts.Add(BusinessContractViewModel.FromEntity(sheet.BusinessContracts.ElementAtOrDefault(i)));
                }
                model.BusinessContracts = businessContracts;

                var interestedParties = new List<OrganisationViewModel>();

                var insuranceAttributeList = await _insuranceAttributeService.GetInsuranceAttributes();
                foreach (InsuranceAttribute IA in insuranceAttributeList.Where(ia => ia.InsuranceAttributeName == "Financial" || ia.InsuranceAttributeName == "Private" || ia.InsuranceAttributeName == "CoOwner"))
                {

                    foreach (var org in IA.IAOrganisations)
                    {
                        if (org.OrganisationType.Name == "Person - Individual" || org.OrganisationType.Name == "Corporation – Limited liability" || org.OrganisationType.Name == "Corporation – Unlimited liability" || org.OrganisationType.Name == "Corporation – Public-Listed" ||
                            org.OrganisationType.Name == "Corporation – Public Unlisted" || org.OrganisationType.Name == "Corporation – Overseas" || org.OrganisationType.Name == "Incorporated Society")
                        {
                            OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
                            ovm.OrganisationName = org.Name;
                            interestedParties.Add(ovm);
                        }
                    }
                }

                model.InterestedParties = interestedParties;

                List<SelectListItem> linterestedparty = new List<SelectListItem>();

                for (var i = 0; i < model.InterestedParties.Count(); i++)
                {
                    linterestedparty.Add(new SelectListItem
                    {
                        Selected = false,
                        Text = model.InterestedParties.ElementAtOrDefault(i).OrganisationName,
                        Value = model.InterestedParties.ElementAtOrDefault(i).ID.ToString(),
                    });
                }

                model.InterestedPartyList = linterestedparty;

                var boatUses = new List<BoatUseViewModel>();
                for (var i = 0; i < sheet.BoatUses.Count(); i++)
                {
                    boatUses.Add(BoatUseViewModel.FromEntity(sheet.BoatUses.ElementAtOrDefault(i)));

                }

                List<SelectListItem> list = new List<SelectListItem>();

                for (var i = 0; i < boatUses.Count(); i++)
                {
                    var text = boatUses.ElementAtOrDefault(i).BoatUseCategory.Substring(0, 4);
                    var val = boatUses.ElementAtOrDefault(i).BoatUseId.ToString();

                    list.Add(new SelectListItem
                    {
                        Selected = false,
                        Value = val,
                        Text = text
                    });

                }

                model.BoatUseslist = list;
                // TODO - find a better way to pass these in
                model.HasVehicles = sheet.Vehicles.Count > 0;
                var vehicles = new List<VehicleViewModel>();
                foreach (Vehicle v in sheet.Vehicles.Where(v => v.Removed == false))
                {
                    vehicles.Add(VehicleViewModel.FromEntity(v));
                }
                model.AllVehicles = vehicles;
                model.RegisteredVehicles = vehicles.Where(v => !string.IsNullOrWhiteSpace(v.Registration));
                model.UnregisteredVehicles = vehicles.Where(v => string.IsNullOrWhiteSpace(v.Registration));

                var organisationalUnits = new List<OrganisationalUnitViewModel>();
                model.OrganisationalUnitsVM = new OrganisationalUnitVM();
                model.OrganisationalUnitsVM.OrganisationalUnits = new List<SelectListItem>();
                var locations = new List<LocationViewModel>();
                var buildings = new List<BuildingViewModel>();
                var waterLocations = new List<WaterLocationViewModel>();
                var MarinaLocations = new List<OrganisationViewModel>();
                var organisationalunit = new List<OrganisationalUnit>();


                for (var i = 0; i < sheet.Owner.OrganisationalUnits.Count(); i++)
                {
                    organisationalUnits.Add(new OrganisationalUnitViewModel
                    {
                        OrganisationalUnitId = sheet.Owner.OrganisationalUnits.ElementAtOrDefault(i).Id,
                        Name = sheet.Owner.OrganisationalUnits.ElementAtOrDefault(i).Name
                    });
                }


                for (var i = 0; i < sheet.Locations.Count(); i++)
                {
                    locations.Add(LocationViewModel.FromEntity(sheet.Locations.ElementAtOrDefault(i)));
                }

                for (var i = 0; i < sheet.Buildings.Count(); i++)
                {
                    buildings.Add(BuildingViewModel.FromEntity(sheet.Buildings.ElementAtOrDefault(i)));

                }

                var insuranceAttributeList1 = await _insuranceAttributeService.GetInsuranceAttributes();
                foreach (InsuranceAttribute IA in insuranceAttributeList1.Where(ia => ia.InsuranceAttributeName == "Marina" || ia.InsuranceAttributeName == "Other Marina"))
                {
                    foreach (var org in IA.IAOrganisations)
                    {
                        if (org.OrganisationType.Name == "Corporation – Limited liability" || org.OrganisationType.Name == "Corporation – Unlimited liability" || org.OrganisationType.Name == "Corporation – Public-Listed" ||
                        org.OrganisationType.Name == "Corporation – Public Unlisted" || org.OrganisationType.Name == "Corporation – Overseas" || org.OrganisationType.Name == "Incorporated Society")
                        {
                            OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
                            ovm.OrganisationName = org.Name;
                            MarinaLocations.Add(ovm);
                        }
                    }
                }

                model.MarinaLocations = MarinaLocations;

                for (var i = 0; i < sheet.WaterLocations.Count(); i++)
                {
                    waterLocations.Add(WaterLocationViewModel.FromEntity(sheet.WaterLocations.ElementAtOrDefault(i)));
                }

                var availableProducts = new List<SelectListItem>();

                foreach (Product product in clientProgramme.BaseProgramme.Products)
                {
                    availableProducts.Add(new SelectListItem
                    {
                        Selected = false,
                        Value = "" + product.Id,
                        Text = product.Name
                    });
                }

                var availableorganisation = new List<SelectListItem>();

                foreach (Organisation organisation in  await _organisationService.GetOrganisationPrincipals(sheet))
                {
                    availableorganisation.Add(new SelectListItem
                    {
                        Selected = false,
                        Value = "" + organisation.Id,
                        Text = organisation.Name
                    });
                }
                model.AvailableOrganisations = availableorganisation;

                model.AllVehicles = vehicles;

                var userDetails = _mapper.Map<UserDetailsVM>(user);
                userDetails.PostalAddress = user.Address;
                userDetails.StreetAddress = user.Address;
                userDetails.FirstName = user.FirstName;
                userDetails.Email = user.Email;

                var organisationDetails = new OrganisationDetailsVM
                {
                    Name = sheet.Owner.Name,
                    Phone = sheet.Owner.Phone,
                    Website = sheet.Owner.Domain
                };

                model.OrganisationalUnits = organisationalUnits;
                model.Locations = locations;
                model.Buildings = buildings;
                //model.Buildings.
                model.WaterLocations = waterLocations;
                //model.InterestedParties = interestedParties;


                model.ClaimProducts = availableProducts;
                model.OrganisationDetails = organisationDetails;
                model.UserDetails = userDetails;
                model.Status = sheet.Status;
                List<ClientInformationAnswer> informationAnswers = await _clientInformationAnswer.GetAllClaimHistory();
                informationAnswers.Where(c => c.ClientInformationSheet.Id == sheet.Id);
                model.ClientInformationAnswers = informationAnswers;

                return View("InformationWizard", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        private IList<string> LoadWizardsteps(string wizardType)
        {
            IList<string> steps = new List<string>();
            //convert this to load from the DB?
            if (wizardType == "Standard")
            {
                steps.Add("Details");
                steps.Add("Steptwo");
            }
            else if (wizardType == "Subsystem")
            {
                steps.Add("Details");
                steps.Add("Quote");
                steps.Add("Declaration");
                steps.Add("Payment");
                steps.Add("Documents");
            }
            return steps;
        }

        private async Task<SharedRoleViewModel> GetSharedRoleViewModel(ClientInformationSheet sheet)
        {
            SharedRoleViewModel sharedRoleViewModel = new SharedRoleViewModel();
            var clientProgramme = sheet.Programme;
            var roleList = new List<SharedDataRoleTemplate>();
            var roleListCount = 0;
            var sharedRoles = new List<SelectListItem>();
            var programmeSharedRoles = await _sharedDataRoleService.GetSharedRoleTemplatesByProgramme(clientProgramme.BaseProgramme);

            if (sheet.SharedDataRoles.Count != 0)
            {
                foreach (var sharedRole in sheet.SharedDataRoles)
                {
                    var sharedRoleTemplate = await _sharedDataRoleService.GetSharedRoleTemplateByRoleName(sharedRole.Name);
                    if (sharedRoleTemplate != null)
                    {
                        roleList.Add(sharedRoleTemplate);
                    }

                    if (sharedRole.AdditionalRoleInformation != null)
                    {
                        sharedRoleViewModel.OtherProfessionId = sharedRole.AdditionalRoleInformation.OtherProfessionId;
                    }

                    sharedRoleViewModel.SharedDataRoles.Add(sharedRole);
                }
                roleListCount = roleList.Count;
            }

            foreach (var sharedRoleTemplate in programmeSharedRoles)
            {
                if (!roleList.Contains(sharedRoleTemplate))
                {
                    roleList.Add(sharedRoleTemplate);
                }
            }

            foreach (var template in roleList)
            {
                if (roleList.IndexOf(template) <= roleListCount)
                {
                    sharedRoles.Add(new SelectListItem
                    {
                        Text = template.Name,
                        Value = template.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    sharedRoles.Add(new SelectListItem
                    {
                        Text = template.Name,
                        Value = template.Id.ToString(),
                        Selected = false
                    });
                }
            }

            return sharedRoleViewModel;
        }
        private async Task<RevenueByActivityViewModel> GetRevenueActivityViewModel(ClientInformationSheet sheet)
        {
            RevenueByActivityViewModel revenueByActivityViewModel = new RevenueByActivityViewModel();
            var clientProgramme = sheet.Programme;

            var territoryList = new List<TerritoryTemplate>();
            var nzTemplate = await _territoryService.GetTerritoryTemplateByName("New Zealand");

            var businessActivityList = new List<BusinessActivityTemplate>();
            List<SelectListItem> territoryTemplates = new List<SelectListItem>();
            List<SelectListItem> businessActivityTemplates = new List<SelectListItem>();

            if (sheet.RevenueData != null && (sheet.RevenueData.Territories.Count != 0 || sheet.RevenueData.Activities.Count != 0))
            {
                if (sheet.RevenueData.Territories.Count > 0)
                {
                    foreach (var territory in sheet.RevenueData.Territories)
                    {
                        var template = await _territoryService.GetTerritoryTemplateById(territory.TerritoryTemplateId);
                        territoryList.Add(template);

                        territoryTemplates.Add(new SelectListItem
                        {
                            Value = template.Id.ToString(),
                            Text = template.Location,
                            Selected = true
                        });

                    }
                    foreach (var template in clientProgramme.BaseProgramme.TerritoryTemplates)
                    {
                        if (!territoryList.Contains(template))
                        {
                            territoryTemplates.Add(new SelectListItem
                            {
                                Value = template.Id.ToString(),
                                Text = template.Location,
                                Selected = false
                            });
                        }
                    }                    
                }
                else
                {                    
                    foreach (var template in clientProgramme.BaseProgramme.TerritoryTemplates)
                    {
                        territoryTemplates.Add(new SelectListItem
                        {
                            Value = template.Id.ToString(),
                            Text = template.Location,
                            Selected = false
                        });

                    }
                }

                if (sheet.RevenueData.Activities.Count > 0)
                {
                    foreach (var ba in sheet.RevenueData.Activities)
                    {
                        var template = await _businessActivityService.GetBusinessActivityTemplate(ba.BusinessActivityTemplate);
                        businessActivityList.Add(template);
                        businessActivityTemplates.Add(new SelectListItem
                        {
                            Value = template.Id.ToString(),
                            Text = template.Description,
                            Selected = true
                        });
                    }
                    foreach (var template in clientProgramme.BaseProgramme.BusinessActivityTemplates)
                    {
                        if (!businessActivityList.Contains(template))
                        {
                            businessActivityTemplates.Add(new SelectListItem
                            {
                                Value = template.Id.ToString(),
                                Text = template.Description,
                                Selected = false
                            });
                        }
                    }
                }
                else
                {
                    foreach (var bat in clientProgramme.BaseProgramme.BusinessActivityTemplates)
                    {
                        businessActivityTemplates.Add(new SelectListItem
                        {
                            Value = bat.Id.ToString(),
                            Text = bat.Description,
                            Selected = false
                        });
                    }
                }

                if (sheet.RevenueData.AdditionalActivityInformation != null)
                {
                    revenueByActivityViewModel.AdditionalInformation = new AdditionalActivityViewModel
                    {
                        CanterburyEarthquakeRebuildWorkId = sheet.RevenueData.AdditionalActivityInformation.CanterburyEarthquakeRebuildWorkId,
                        ValuationTextId = sheet.RevenueData.AdditionalActivityInformation.ValuationTextId,
                        OtherActivitiesTextId = sheet.RevenueData.AdditionalActivityInformation.OtherActivitiesTextId,
                        ValuationTextId2 = sheet.RevenueData.AdditionalActivityInformation.ValuationTextId2,
                        InspectionReportTextId = sheet.RevenueData.AdditionalActivityInformation.InspectionReportTextId,
                        OtherProjectManagementTextId = sheet.RevenueData.AdditionalActivityInformation.OtherProjectManagementTextId,
                        NonProjectManagementTextId = sheet.RevenueData.AdditionalActivityInformation.NonProjectManagementTextId,
                        ConstructionTextId = sheet.RevenueData.AdditionalActivityInformation.ConstructionTextId,
                    };

                    if (sheet.RevenueData.AdditionalActivityInformation.ValuationBoolId > 0)
                        revenueByActivityViewModel.AdditionalInformation.ValuationBoolId.FirstOrDefault(l => l.Value == sheet.RevenueData.AdditionalActivityInformation.ValuationBoolId.ToString()).Selected = true;

                    if (sheet.RevenueData.AdditionalActivityInformation.InspectionReportBoolId > 0)
                        revenueByActivityViewModel.AdditionalInformation.InspectionReportBoolId.FirstOrDefault(l => l.Value == sheet.RevenueData.AdditionalActivityInformation.InspectionReportBoolId.ToString()).Selected = true;

                    if (sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId > 0)
                        revenueByActivityViewModel.AdditionalInformation.SchoolsDesignWorkBoolId.FirstOrDefault(l => l.Value == sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId.ToString()).Selected = true;

                    if (sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId2 > 0)
                        revenueByActivityViewModel.AdditionalInformation.SchoolsDesignWorkBoolId2.FirstOrDefault(l => l.Value == sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId2.ToString()).Selected = true;

                    if (sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId3 > 0)
                        revenueByActivityViewModel.AdditionalInformation.SchoolsDesignWorkBoolId3.FirstOrDefault(l => l.Value == sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId3.ToString()).Selected = true;

                    if (sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId4 > 0)
                        revenueByActivityViewModel.AdditionalInformation.SchoolsDesignWorkBoolId4.FirstOrDefault(l => l.Value == sheet.RevenueData.AdditionalActivityInformation.SchoolsDesignWorkBoolId4.ToString()).Selected = true;
                }
                else
                {
                    revenueByActivityViewModel.AdditionalInformation = new AdditionalActivityViewModel();
                }

                revenueByActivityViewModel.CurrentYear = sheet.RevenueData.CurrentYear;
                revenueByActivityViewModel.NextFincialYear = sheet.RevenueData.NextFinancialYear;
                revenueByActivityViewModel.LastFinancialYear = sheet.RevenueData.LastFinancialYear;
                revenueByActivityViewModel.RevenueData = sheet.RevenueData;
            }
            else
            {                
                foreach(var template in clientProgramme.BaseProgramme.TerritoryTemplates)
                {
                    territoryTemplates.Add(new SelectListItem
                    {
                        Value = template.Id.ToString(),
                        Text = template.Location,
                        Selected = false
                    });
                }
                foreach (var bat in clientProgramme.BaseProgramme.BusinessActivityTemplates)
                {
                    businessActivityTemplates.Add(new SelectListItem
                    {
                        Value = bat.Id.ToString(),
                        Text = bat.Description,
                        Selected = false
                    });
                }

            }
            revenueByActivityViewModel.AdditionalInformation = new AdditionalActivityViewModel();

            if (territoryTemplates.Where(sl=>sl.Text == nzTemplate.Location).ToList().Count == 0)
            {
                territoryTemplates.Add(new SelectListItem
                {
                    Value = nzTemplate.Id.ToString(),
                    Text = nzTemplate.Location,
                    Selected = false
                });
            }

            revenueByActivityViewModel.Territories = territoryTemplates;
            revenueByActivityViewModel.Activities = businessActivityTemplates.OrderBy(ba => ba.Text).ToList();

            return revenueByActivityViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> Unlock(Guid id)
        {
            User user = null;
            try
            {
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
                ClientInformationSheet sheet = clientProgramme.InformationSheet;
                user = await CurrentUser();
                if (sheet != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (sheet.Status == "Submitted")
                        {
                            sheet.Status = "Started";
                            sheet.Answers.FirstOrDefault(i => i.ItemName == "ClientInformationSheet.Status").Value = "Started";
                            sheet.UnlockDate = DateTime.UtcNow;
                            sheet.UnlockedBy = user;
                        }
                        await uow.Commit();

                    }
                }

                var url = "/Information/EditInformation/" + id;
                return Redirect(url);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Resume(Guid id)
        {
            User user = null;
            try
            {
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
                ClientInformationSheet sheet = clientProgramme.InformationSheet;
                user = await CurrentUser();
                if (sheet != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (sheet.Status == "Not Taken Up")
                        {
                            sheet.Status = "Started";
                            sheet.LastModifiedOn = DateTime.UtcNow;
                            sheet.LastModifiedBy = user;
                        }
                        await uow.Commit();

                    }
                }

                var url = "/Information/EditInformation/" + id;
                return Redirect(url);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveInformation(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            User user = null;
            try
            {
                user = await CurrentUser();
                sheetId = Guid.Parse(collection["ClientInformationSheet.Id"]);
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(sheetId);
                if (sheet == null)
                    return Json("Failure");

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    await _clientInformationService.SaveAnswersFor(sheet, collection);
                    await _clientInformationService.UpdateInformation(sheet);
                    await uow.Commit();
                }

                return Json("Success");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDNO(Guid ClientInformationSheet)
        {
            string[][] DNOAnwers = new String[15][];
            var count = 0;
            string[] DNOItem;
            User user = null;
            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "DNO1" || c.ItemName == "DNO2" || c.ItemName == "DNO3"
                                                                                                                                                          || c.ItemName == "DNO4" || c.ItemName == "DNO5" || c.ItemName == "DNO6" || c.ItemName == "DNO7"
                                                                                                                                                          || c.ItemName == "DNO8" || c.ItemName == "DNO9" || c.ItemName == "DNO10" || c.ItemName == "DNO11"
                                                                                                                                                          || c.ItemName == "DNO12" || c.ItemName == "DNO13" || c.ItemName == "DNO14" || c.ItemName == "DNO15")))
                {
                    DNOItem = new String[2];

                    for (var i = 0; i < 1; i++)
                    {
                        DNOItem[i] = answer.ItemName;
                        DNOItem[i + 1] = answer.Value;
                    }

                    DNOAnwers[count] = DNOItem;
                    count++;
                }

                return Json(DNOAnwers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetStatutoryLiability(Guid ClientInformationSheet)
        {
            String[][] GeneralLiabilityAnwers = new String[6][];
            var count = 0;
            String[] GeneralLiability;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "StatutoryLiability1" || c.ItemName == "StatutoryLiability2" || c.ItemName == "StatutoryLiability3"
                                                                                                                                                      || c.ItemName == "StatutoryLiability4" || c.ItemName == "StatutoryLiability5" || c.ItemName == "StatutoryLiability6")))
                {
                    GeneralLiability = new String[2];
                    for (var i = 0; i < 1; i++)
                    {
                        GeneralLiability[i] = answer.ItemName;
                        GeneralLiability[i + 1] = answer.Value;
                    }

                    GeneralLiabilityAnwers[count] = GeneralLiability;
                    count++;
                }

                return Json(GeneralLiabilityAnwers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetProfessionalIdemnity(Guid ClientInformationSheet)
        {
            String[][] ProfessionalIndemnityAnswer = new String[11][];
            var count = 0;
            String[] ProfessionalIndemnity;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "ProfessionalIndemnity1" || c.ItemName == "ProfessionalIndemnity2" || c.ItemName == "ProfessionalIndemnity11"
                                                                                                                                || c.ItemName == "ProfessionalIndemnity3" || c.ItemName == "ProfessionalIndemnity4" || c.ItemName == "ProfessionalIndemnity5" || c.ItemName == "ProfessionalIndemnity10"
                                                                                                                                || c.ItemName == "ProfessionalIndemnity6" || c.ItemName == "ProfessionalIndemnity7" || c.ItemName == "ProfessionalIndemnity8" || c.ItemName == "ProfessionalIndemnity9")))
                {
                    ProfessionalIndemnity = new String[2];
                    for (var i = 0; i < 1; i++)
                    {
                        ProfessionalIndemnity[i] = answer.ItemName;
                        ProfessionalIndemnity[i + 1] = answer.Value;
                    }
                    ProfessionalIndemnityAnswer[count] = ProfessionalIndemnity;
                    count++;
                }
                return Json(ProfessionalIndemnityAnswer);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPMINZProfessionalIdemnity(Guid ClientInformationSheet)
        {
            String[][] ProfessionalIndemnityAnswer = new String[20][];
            var count = 0;
            String[] ProfessionalIndemnity;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "PMINZPI1" || c.ItemName == "PMINZPI2" || c.ItemName == "PMINZPI3" || c.ItemName == "PMINZPI4" || c.ItemName == "PMINZPI5" || c.ItemName == "PMINZPI6"
                                                                                                                                                         || c.ItemName == "PMINZPI7"  || c.ItemName == "PMINZPI8" || c.ItemName == "PMINZPI9" || c.ItemName == "PMINZPI10" || c.ItemName == "PMINZPI11" || c.ItemName == "PMINZPI12"
                                                                                                                                                         || c.ItemName == "PMINZPI13" || c.ItemName == "PMINZPI14" || c.ItemName == "PMINZPI5" || c.ItemName == "PMINZPI6" || c.ItemName == "PMINZPI17" || c.ItemName == "PMINZPI18" || c.ItemName == "PMINZPI19" || c.ItemName == "PMINZPI20")))
                {
                    ProfessionalIndemnity = new String[3];
                    for (var i = 0; i < 1; i++)
                    {
                        ProfessionalIndemnity[i] = answer.ItemName;
                        ProfessionalIndemnity[i + 1] = answer.Value;
                        ProfessionalIndemnity[i + 2] = answer.ClaimDetails;
                    }
                    ProfessionalIndemnityAnswer[count] = ProfessionalIndemnity;
                    count++;
                }
                return Json(ProfessionalIndemnityAnswer);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetEmployerLiability(Guid ClientInformationSheet)
        {
            String[][] GeneralLiabilityAnwers = new String[6][];
            var count = 0;
            String[] GeneralLiability;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "EmployerLiabilityInsurance1" || c.ItemName == "EmployerLiabilityInsurance2"
                                                                                                                                                     || c.ItemName == "EmployerLiabilityInsurance3" || c.ItemName == "EmployerLiabilityInsurance4"
                                                                                                                                                     || c.ItemName == "EmployerLiabilityInsurance5" || c.ItemName == "EmployerLiabilityInsurance6")))
                {
                    GeneralLiability = new String[2];

                    for (var i = 0; i < 1; i++)
                    {
                        GeneralLiability[i] = answer.ItemName;
                        GeneralLiability[i + 1] = answer.Value;
                    }

                    GeneralLiabilityAnwers[count] = GeneralLiability;
                    count++;
                }

                return Json(GeneralLiabilityAnwers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployerPracticesLiability(Guid ClientInformationSheet)
        {
            String[][] GeneralLiabilityAnwers = new String[6][];
            var count = 0;
            String[] GeneralLiability;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "EmployerPracticesLiability1" || c.ItemName == "EmployerPracticesLiability2"
                                                                                                                                                     || c.ItemName == "EmployerPracticesLiability3" || c.ItemName == "EmployerPracticesLiability4"
                                                                                                                                                     || c.ItemName == "EmployerPracticesLiability5" || c.ItemName == "EmployerPracticesLiability6")))
                {
                    GeneralLiability = new String[2];

                    for (var i = 0; i < 1; i++)
                    {
                        GeneralLiability[i] = answer.ItemName;
                        GeneralLiability[i + 1] = answer.Value;
                    }

                    GeneralLiabilityAnwers[count] = GeneralLiability;
                    count++;
                }

                return Json(GeneralLiabilityAnwers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGeneralLiability(Guid ClientInformationSheet)
        {
            String[][] GeneralLiabilityAnwers = new String[7][];
            var count = 0;
            String[] GeneralLiability;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "GeneralLiabilityInsurance1" || c.ItemName == "GeneralLiabilityInsurance2" || c.ItemName == "GeneralLiabilityInsurance3"
                                                                                                                                                     || c.ItemName == "GeneralLiabilityInsurance4" || c.ItemName == "GeneralLiabilityInsurance5" || c.ItemName == "GeneralLiabilityInsurance6" || c.ItemName == "GeneralLiabilityInsurance7")))

                {
                    GeneralLiability = new String[2];

                    for (var i = 0; i < 1; i++)
                    {
                        GeneralLiability[i] = answer.ItemName;
                        GeneralLiability[i + 1] = answer.Value;
                    }

                    GeneralLiabilityAnwers[count] = GeneralLiability;
                    count++;
                }

                return Json(GeneralLiabilityAnwers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDirectorsandOfficersLiability(Guid ClientInformationSheet)
        {
            String[][] GeneralLiabilityAnwers = new String[7][];
            var count = 0;
            String[] GeneralLiability;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var answer in _clientInformationAnswer.GetAllSheetAns().Result.Where(c => c.ClientInformationSheet.Id == ClientInformationSheet && (c.ItemName == "DirectorsandOfficers1" || c.ItemName == "DirectorsandOfficers2" || c.ItemName == "DirectorsandOfficers3"
                                                                                                                                                     || c.ItemName == "DirectorsandOfficers4" || c.ItemName == "DirectorsandOfficers5" || c.ItemName == "DirectorsandOfficers6" || c.ItemName == "DirectorsandOfficers7")))

                {
                    GeneralLiability = new String[2];

                    for (var i = 0; i < 1; i++)
                    {
                        GeneralLiability[i] = answer.ItemName;
                        GeneralLiability[i + 1] = answer.Value;
                    }

                    GeneralLiabilityAnwers[count] = GeneralLiability;
                    count++;
                }

                return Json(GeneralLiabilityAnwers);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAnswer(List<string[]> Answers, Guid ClientInformationSheet)
        {
            ClientInformationSheet sheet = null;
            User user = null;

            try
            {
                foreach (var item in Answers)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (item[1] != null)
                        {


                            for (var x = 0; x < item.Length - 1; x++)
                            {
                                ClientInformationAnswer answer = await _clientInformationAnswer.GetSheetAnsByName(item[0], ClientInformationSheet);

                                if (answer != null)
                                {
                                    answer.Value = item[1];
                                    if (item.Length > 2)
                                        answer.ClaimDetails = item[2];
                                    //answer.ClaimDetails = item[2];
                                }
                                else
                                {
                                    sheet = await _clientInformationService.GetInformation(ClientInformationSheet);
                                    if (item.Length > 2)
                                    {
                                        await _clientInformationAnswer.CreateNewSheetPMINZAns(item[0], item[1], item[2], sheet);
                                    }
                                    else
                                    {
                                        await _clientInformationAnswer.CreateNewSheetAns(item[0], item[1], sheet);

                                    }

                                }
                                //if (answer != null)
                                //{
                                //    answer.Value = item[1];
                                //    //answer.ClaimDetails = item[2];
                                //}
                                //else
                                //{
                                //    sheet = await _clientInformationService.GetInformation(ClientInformationSheet);
                                //    await _clientInformationAnswer.CreateNewSheetAns(item[0], item[1], sheet);
                                //}
                            }
                        }
                        await uow.Commit();
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePMINZPIAnswer(List<string[]> Answers, Guid ClientInformationSheet)
        {
            ClientInformationSheet sheet = null;
            User user = null;

            try
            {
                foreach (var item in Answers)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (item[1] != null)
                        {
                            //for (var x = 0; x < item.Length - 1; x++)
                           // {

                                
                                ClientInformationAnswer answer = await _clientInformationAnswer.GetSheetAnsByName(item[0], ClientInformationSheet);
                                if (answer != null)
                                {
                                    answer.Value = item[1];
                                    if(item.Length > 2)
                                    answer.ClaimDetails = item[2];
                                    //answer.ClaimDetails = item[2];
                                }
                                else
                                {
                                    sheet = await _clientInformationService.GetInformation(ClientInformationSheet);
                                    if (item.Length > 2)
                                    {
                                        await _clientInformationAnswer.CreateNewSheetPMINZAns(item[0], item[1], item[2], sheet);
                                    }
                                    else
                                    {
                                        await _clientInformationAnswer.CreateNewSheetAns(item[0], item[1], sheet);

                                    }

                                }
                            //}
                        }
                        await uow.Commit();
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClaim(List<string[]> Claims, Guid ClientInformationSheet)
        {
            ClientInformationSheet sheet = null;
            User user = null;

            try
            {
                user = await CurrentUser();
                foreach (var item in Claims)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        for (var x = 0; x < item.Length - 1; x++)
                        {
                            ClientInformationAnswer answer = _clientInformationAnswer.GetClaimHistoryByName(item[0], ClientInformationSheet).Result;
                            if (answer != null)
                            {
                                answer.Value = item[1];
                                answer.ClaimDetails = item[2];
                            }
                            else
                            {
                                sheet = _clientInformationService.GetInformation(ClientInformationSheet).Result;
                                await _clientInformationAnswer.CreateNewClaimHistory(item[0], item[1], item[2], sheet);
                            }
                        }
                        await uow.Commit();
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitInformation(string clientInformationId)
        {
            ClientInformationSheet sheet = null;
            User user = null;

            try
            {
                user = await CurrentUser();
                sheet = await _clientInformationService.GetInformation(Guid.Parse(clientInformationId));
                var isBaseSheet = await _clientInformationService.IsBaseClass(sheet);
                if (isBaseSheet)
                {
                    var programme = sheet.Programme.BaseProgramme;
                    var reference = await _referenceService.GetLatestReferenceId();

                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (sheet.Status != "Submitted" && sheet.Status != "Bound")
                        {
                            //UWM
                            _uWMService.UWM(user, sheet, reference);

                            //sheet.Status = "Submitted";
                            await uow.Commit();
                        }
                    }

                    foreach (ClientAgreement agreement in sheet.Programme.Agreements)
                    {
                        await _referenceService.CreateClientAgreementReference(agreement.ReferenceId, agreement.Id);
                    }

                    return Content("/Agreement/ViewAgreement/" + sheet.Programme.Id);
                }
                else
                {
                    return Redirect("/Information/QuoteToAgree?id=" + sheet.Programme.Id);
                }

            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuoteToAgree(string id)
        {
            Guid sheetId = Guid.Empty;
            User user = null;

            try
            {
                user = await CurrentUser();
                var clientProgramme = await _programmeService.GetClientProgrammebyId(Guid.Parse(id));
                var sheet = clientProgramme.InformationSheet;
                if (sheet.Status != "Submitted" && sheet.Status != "Bound")
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        sheet.Status = "Submitted";
                        ClientInformationAnswer clientInformationAnswer = sheet.Answers.FirstOrDefault(i => i.ItemName == "ClientInformationSheet.Status");
                        sheet.Answers.FirstOrDefault(i => i.ItemName == "ClientInformationSheet.Status").Value = "Submitted";
                        sheet.SubmitDate = DateTime.UtcNow;
                        sheet.SubmittedBy = user;
                        await uow.Commit();
                    }

                }
                //sheet owner is null
                //await _emailService.SendSystemEmailUISSubmissionConfirmationNotify(user, sheet.Programme.BaseProgramme, sheet, sheet.Owner);
                //send out information sheet submission notification email
                //await _emailService.SendSystemEmailUISSubmissionNotify(user, sheet.Programme.BaseProgramme, sheet, sheet.Owner);

                return Content("/Agreement/ViewAgreementDeclaration/" + sheet.Programme.Id);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> PaymentInformation(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;
            User user = null;

            try
            {
                user = await CurrentUser();
                if (Guid.TryParse(HttpContext.Request.Form["ClientInformationSheet.Id"], out sheetId))
                {
                    sheet = await _clientInformationService.GetInformation(sheetId);
                }

                return Content("/Agreement/ViewPayment/" + sheet.Programme.Id);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInformation(ChangeReason changeReason)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                await _changeProcessService.CreateChangeReason(user, changeReason);

                changeReason.EffectiveDate = DateTime.Parse(LocalizeTime(changeReason.EffectiveDate, "d"));

                await _changeProcessService.CreateChangeReason(user, changeReason);

                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(changeReason.DealId);
                if (clientProgramme == null)
                    throw new Exception("ClientProgramme (" + changeReason.DealId + ") doesn't belong to User " + user.UserName);

                ClientProgramme newClientProgramme = await _programmeService.CloneForUpdate(clientProgramme, user, changeReason);

                await _programmeService.Update(newClientProgramme);

                var url = "/Information/EditInformation/" + newClientProgramme.Id;
                return Json(new { url });
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RenewInformation(Guid id)
        {
            User user = null;

            try
            {
                ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
                user = await CurrentUser();
                if (clientProgramme == null)
                    throw new Exception("ClientProgramme (" + id + ") doesn't belong to User " + user.UserName);

                ClientProgramme newClientProgramme = await _programmeService.CloneForRewenal(clientProgramme, user);

                await _programmeService.Update(newClientProgramme);

                return Redirect("/Information/StartInformation/" + newClientProgramme.Id);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateDemoUIS()
        //{
        //    User user = null;

        //    try
        //    {
        //        var demoData = null;//await LoadTemplate();
        //        user = await CurrentUser();
        //        List<InformationSection> informationSections = new List<InformationSection>();

        //        foreach (var section in demoData.Sections)
        //        {

        //            // Update section Id in view model
        //            // See Above temporaraly

        //            List<InformationItem> items = new List<InformationItem>();

        //            //Loop through Questions
        //            foreach (var item in section.Items)
        //            {
        //                using (var uow = _unitOfWork.BeginUnitOfWork())
        //                {
        //                    // Create Information Item


        //                    if (item != null)
        //                    {
        //                        string itemTypeName = Enum.GetName(typeof(ItemType), item.Type);
        //                        InformationItem newItem = null;

        //                        switch (item.Type)
        //                        {
        //                            case ItemType.TEXTAREA:
        //                            case ItemType.TEXTBOX:
        //                                var textboxItem = await _informationItemService.CreateTextboxItem(user, item.Name, item.Label, item.Width, itemTypeName) as TextboxItem;
        //                                items.Add(textboxItem);
        //                                item.Id = textboxItem.Id;
        //                                break;

        //                            case ItemType.LABEL:
        //                                var labelItem = await _informationItemService.CreateLabelItem(user, item.Name, item.Label, item.Width, itemTypeName) as LabelItem;
        //                                items.Add(labelItem);
        //                                item.Id = labelItem.Id;
        //                                break;

        //                            case ItemType.PERCENTAGEBREAKDOWN:
        //                            case ItemType.DROPDOWNLIST:
        //                                //Mapper.CreateMap<SelectListItem, DropdownListOption>();
        //                                //Mapper.CreateMap<DropdownListOption, SelectListItem>()
        //                                var options = _mapper.Map<IList<DropdownListOption>>(item.Options);
        //                                var newDropdownList = await _informationItemService.CreateDropdownListItem(user, item.Name, item.Label, item.DefaultText, options, item.Width, itemTypeName) as DropdownListItem;
        //                                //newDropdownList.AddItems(options);
        //                                items.Add(newDropdownList);
        //                                item.Id = newDropdownList.Id;
        //                                break;

        //                            //case ItemType.MULTISELECT:
        //                            //    options = _mapper.Map<IList<DropdownListOption>>(item.Options);
        //                            //    var multiselectList = await _informationItemService.CreateMultiselectListItem(user, item.Name, item.Label, item.DefaultText, options, item.Width, itemTypeName) as MultiselectListItem;
        //                            //    items.Add(multiselectList);
        //                            //    item.Id = multiselectList.Id;
        //                            //    break;

        //                            case ItemType.JSBUTTON:
        //                                newItem = await _informationItemService.CreateJSButtonItem(user, item.Name, item.Label, item.Width, itemTypeName, item.Value) as JSButtonItem;
        //                                break;

        //                            case ItemType.SUBMITBUTTON:
        //                                newItem = await _informationItemService.CreateSubmitButtonItem(user, item.Name, item.Label, item.Width, itemTypeName) as SubmitButtonItem;
        //                                break;

        //                            //case ItemType.SECTIONBREAK:
        //                            //    var terminatorItem = await _informationItemService.CreateSectionBreakItem(CurrentUser, itemTypeName);
        //                            //    items.Add(terminatorItem);
        //                            //    break;

        //                            default:
        //                                newItem = null;
        //                                break;
        //                        }

        //                        if (newItem != null)
        //                        {
        //                            items.Add(newItem);
        //                            item.Id = newItem.Id;
        //                            newItem = null;
        //                        }


        //                        // Update Inforamtion Item ID in view model
        //                        // For now see code above, Will fix later with Domain Model
        //                    }
        //                    await uow.Commit();
        //                }

        //            }

        //            using (var uow = _unitOfWork.BeginUnitOfWork())
        //            {
        //                // Create New Section

        //                InformationSection informationSection = await _informationSectionService.CreateNewSection(user, section.Name, items);
        //                informationSection.CustomView = section.CustomView;
        //                informationSections.Add(informationSection);

        //                section.Id = informationSection.Id;
        //                await uow.Commit();
        //            }

        //        }

        //        using (var uow = _unitOfWork.BeginUnitOfWork())
        //        {
        //            InformationTemplate template = await _informationTemplateService.CreateInformationTemplate(user, demoData.Name, informationSections);

        //            demoData.Id = template.Id;

        //            await uow.Commit();
        //        }

        //        return Json(demoData);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        return RedirectToAction("Error500", "Error");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> IssueDemoUIS(string id)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                if (!string.IsNullOrWhiteSpace(id))
                    user = await _userService.GetUser(id);

                // issues a demo UIS for every template in the system, assuming it hasn't been issued yet
                var templates = await _informationTemplateService.GetAllTemplates();
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    foreach (var template in templates)
                    {
                        await _clientInformationService.IssueInformationFor(user, user.PrimaryOrganisation, template);
                    }
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

        [HttpPost]
        public async Task<IActionResult> SaveSharedRoleTabOne(string[] SharedDataRoles, string ClientInformationSheetId)
        {
            User user = null;

            try
            {
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(ClientInformationSheetId));
                user = await CurrentUser();
                sheet.SharedDataRoles.Clear();
                foreach (var id in SharedDataRoles)
                {
                    var template = await _sharedDataRoleService.GetSharedRoleTemplateById(Guid.Parse(id));
                    var newSharedRole = new SharedDataRole(user);
                    newSharedRole.Name = template.Name;
                    await _sharedDataRoleService.CreateSharedDataRole(newSharedRole);
                    sheet.SharedDataRoles.Add(newSharedRole);
                }

                await _clientInformationService.UpdateInformation(sheet);

                return Json("OK");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSharedRoleTabTwo(string TableSerialised, string ClientInformationSheetId)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(ClientInformationSheetId));
                foreach (var sharedRole in sheet.SharedDataRoles)
                {
                    string[] tableRow = TableSerialised.Split('&');
                    foreach (var str in tableRow)
                    {
                        string[] valueId = str.Split('=');
                        var sharedRoleTemplate = await _sharedDataRoleService.GetSharedRoleTemplateById(Guid.Parse(valueId[0]));
                        if (sharedRoleTemplate != null)
                        {
                            if (sharedRoleTemplate.Name == sharedRole.Name)
                            {
                                sharedRole.Count = int.Parse(valueId[1]);
                            }
                        }
                        else
                        {
                            if (valueId[0] == sharedRole.Id.ToString())
                            {
                                sharedRole.Count = int.Parse(valueId[1]);
                            }
                        }
                        await _sharedDataRoleService.UpdateSharedRole(sharedRole);
                    }
                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSharedRoleTabThree(IFormCollection form)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var clientInformationSheetIdFormString = form["ClientInformationSheetId"].ToString();
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(clientInformationSheetIdFormString));
                var additionalRoleInformation = new AdditionalRoleInformation(user);
                var serialisedAdditionalInformationTableFormString = form["SerialisedAdditionalRoleInformationTable"].ToString();
                var FormString = serialisedAdditionalInformationTableFormString.Split('&');
                if (sheet.SharedDataRoles.Count == 0)
                {
                    throw new Exception("Please complete Activities Tab");
                }

                //loop through form
                foreach (var questionFormString in FormString)
                {
                    var questionSplit = questionFormString.Split("=");
                    switch (questionSplit[0])
                    {
                        case "OtherProfessionId":
                            additionalRoleInformation.OtherProfessionId = questionSplit[1];
                            break;
                        default:
                            throw new Exception("Add more form question 'cases'");
                    }
                }

                foreach (var role in sheet.SharedDataRoles)
                {
                    if (role.Name == "Other Professions")
                    {
                        role.AdditionalRoleInformation = additionalRoleInformation;
                        await _sharedDataRoleService.UpdateSharedRole(role);
                    }
                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRevenueDataTabOne(IFormCollection form)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var sheetId = form["ClientInformationSheetId"];
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(sheetId[0]));
                if (sheet.RevenueData == null)
                {
                    sheet.RevenueData = new RevenueByActivity(user);
                    await _revenueActivityService.AddRevenueByActivity(sheet.RevenueData);
                }
                else
                {
                    foreach (Territory territory in sheet.RevenueData.Territories)
                    {
                        territory.DateDeleted = DateTime.Now;
                        territory.DeletedBy = user;
                        await _territoryService.UpdateTerritory(territory);
                    }
                    sheet.RevenueData.Territories.Clear();
                }

                var territoryForm = form["form"];
                var territoryFormString = territoryForm[0];
                var territorySplit = territoryFormString.Split("&");
                foreach (var str in territorySplit)
                {
                    var strSpit = str.Split('=');
                    if (strSpit[0] != "Territories")
                    {                        
                        var territorytemplate = await _territoryService.GetTerritoryTemplateById(Guid.Parse(strSpit[0]));
                        var newTerritory = new Territory(user);
                        newTerritory.Location = territorytemplate.Location;
                        newTerritory.Pecentage = decimal.Parse(strSpit[1]);
                        newTerritory.TerritoryTemplateId = territorytemplate.Id;
                        await _territoryService.AddTerritory(newTerritory);
                        sheet.RevenueData.Territories.Add(newTerritory);
                    }
                }
                await _clientInformationService.UpdateInformation(sheet);

                return Json("OK");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRevenueDataTabTwo(IFormCollection form)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var sheetId = form["ClientInformationSheetId"];
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(sheetId[0]));
                if (sheet.RevenueData == null)
                {
                    sheet.RevenueData = new RevenueByActivity(user);
                }
                else
                {
                    foreach (BusinessActivity businessActivity in sheet.RevenueData.Activities)
                    {
                        businessActivity.DateDeleted = DateTime.Now;
                        businessActivity.DeletedBy = user;
                        await _businessActivityService.UpdateBusinessActivity(businessActivity);
                    }
                    sheet.RevenueData.Activities.Clear();
                }

                var activityForm = form["form"];
                var activityFormString = activityForm[0];
                var activitySplit = activityFormString.Split("&");
                foreach (var str in activitySplit)
                {
                    var strSpit = str.Split('=');
                    if (strSpit[0] == "currentYear")
                    {
                        sheet.RevenueData.CurrentYear = decimal.Parse(strSpit[1]);
                    }
                    else if (strSpit[0] == "lastFinancialYear")
                    {
                        sheet.RevenueData.LastFinancialYear = decimal.Parse(strSpit[1]);
                    }
                    else if (strSpit[0] == "nextFinancialYear")
                    {
                        sheet.RevenueData.NextFinancialYear = decimal.Parse(strSpit[1]);
                    }
                    else if (strSpit[0] == "Activities")
                    {

                    }
                    else
                    {
                        var businessActivityTemplate = await _businessActivityService.GetBusinessActivityTemplate(Guid.Parse(strSpit[0]));
                        var newBusinessActivity = new BusinessActivity(user)
                        {
                            AnzsciCode = businessActivityTemplate.AnzsciCode,
                            Classification = businessActivityTemplate.Classification,
                            Description = businessActivityTemplate.Description,
                            BusinessActivityTemplate = businessActivityTemplate.Id,
                            Pecentage = decimal.Parse(strSpit[1])
                        };
                        await _businessActivityService.CreateBusinessActivity(newBusinessActivity);
                        sheet.RevenueData.Activities.Add(newBusinessActivity);
                    }
                }

                await _clientInformationService.UpdateInformation(sheet);
                return Json("OK");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRevenueDataTabThree(IFormCollection form)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                var clientInformationSheetIdFormString = form["ClientInformationSheetId"].ToString();
                var sheet = await _clientInformationService.GetInformation(Guid.Parse(clientInformationSheetIdFormString));
                var additionalInformation = new AdditionalActivityInformation(user);
                var serialisedAdditionalInformationTableFormString = form["form"].ToString();
                var FormString = serialisedAdditionalInformationTableFormString.Split('&');

                if (sheet.RevenueData == null)
                {
                    throw new Exception("Please complete Activities Tab");
                }

                //loop through form
                foreach (var questionFormString in FormString)
                {
                    var questionSplit = questionFormString.Split("=");
                    try
                    {
                        switch (questionSplit[0])
                        {
                            case "InspectionReportTextId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.InspectionReportTextId = questionSplit[1];
                                }
                                break;
                            case "InspectionReportBoolId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.InspectionReportBoolId = int.Parse(questionSplit[1]);
                                }
                                break;
                            case "ValuationTextId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ValuationTextId = questionSplit[1];
                                }
                                break;
                            case "ValuationTextId2":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ValuationTextId2 = questionSplit[1];
                                }

                                break;
                            case "ValuationBoolId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ValuationBoolId = int.Parse(questionSplit[1]);
                                }
                                break;
                            case "SchoolsDesignWorkBoolId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.SchoolsDesignWorkBoolId = int.Parse(questionSplit[1]);
                                }
                                break;
                            case "SchoolsDesignWorkBoolId2":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.SchoolsDesignWorkBoolId2 = int.Parse(questionSplit[1]);
                                }
                                break;
                            case "SchoolsDesignWorkBoolId3":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.SchoolsDesignWorkBoolId3 = int.Parse(questionSplit[1]);
                                }
                                break;
                            case "SchoolsDesignWorkBoolId4":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.SchoolsDesignWorkBoolId4 = int.Parse(questionSplit[1]);
                                }
                                break;
                            case "OtherActivitiesTextId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.OtherActivitiesTextId = questionSplit[1];
                                }
                                break;
                            case "CanterburyEarthquakeRebuildWorkId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.CanterburyEarthquakeRebuildWorkId = questionSplit[1];
                                }
                                break;
                            case "OtherProjectManagementTextId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.OtherProjectManagementTextId = questionSplit[1];
                                }
                                break;
                            case "NonProjectManagementTextId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.NonProjectManagementTextId = questionSplit[1];
                                }
                                break;
                            case "ConstructionCommercial":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ConstructionCommercial = decimal.Parse(questionSplit[1]);
                                }
                                break;
                            case "ConstructionDwellings":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ConstructionDwellings = decimal.Parse(questionSplit[1]);
                                }
                                break;
                            case "ConstructionIndustrial":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ConstructionIndustrial = decimal.Parse(questionSplit[1]);
                                }
                                break;
                            case "ConstructionInfrastructure":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ConstructionInfrastructure = decimal.Parse(questionSplit[1]);
                                }
                                break;
                            case "ConstructionSchool":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ConstructionSchool = decimal.Parse(questionSplit[1]);
                                }
                                break;
                            case "ConstructionTextId":
                                if (questionSplit[1] != "")
                                {
                                    additionalInformation.ConstructionTextId = questionSplit[1];
                                }
                                break;
                            default:
                                throw new Exception("Add more form question 'cases'");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                sheet.RevenueData.AdditionalActivityInformation = additionalInformation;
                await _clientInformationService.UpdateInformation(sheet);
                return Json("OK");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        public async Task<InformationViewModel> GetInformationViewModel(ClientProgramme clientProgramme)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                Programme programme = clientProgramme.BaseProgramme;
                InformationViewModel model = new InformationViewModel
                {
                    Name = programme.Name,
                    Sections = new List<InformationSectionViewModel>()
                };
                model.Name = programme.Name;
                Product product = null;
                if (programme.Products.Count > 1)
                {
                    product = programme.Products.FirstOrDefault(progp => progp.IsMasterProduct);
                }
                else
                {
                    product = programme.Products.FirstOrDefault();
                }

                InformationTemplate informationTemplate;
                List<InformationSection> sections = new List<InformationSection>();
                var isSubsystem = await _programmeService.IsBaseClass(clientProgramme);
                if (!isSubsystem)
                {
                    informationTemplate = product.SubInformationTemplate;
                }
                else
                {
                    //remove after checking with ray
                    if (product.InformationTemplate == null)
                    {
                        informationTemplate = await _informationTemplateService.GetTemplatebyProduct(product.Id);
                        product.InformationTemplate = informationTemplate;
                        await _productService.UpdateProduct(product);
                    }
                    informationTemplate = product.InformationTemplate;
                }
                sections = await _informationSectionService.GetInformationSectionsbyTemplateId(informationTemplate.Id);
                foreach (var section in sections)
                {
                    section.Items = section.Items.OrderBy(i => i.ItemOrder).ToList();
                }

                foreach (var section in informationTemplate.Sections)
                {
                    section.Items = section.Items.OrderBy(i => i.ItemOrder).ToList();
                }

                (model.Sections as List<InformationSectionViewModel>).InsertRange(model.Sections.Count(), _mapper.Map<InformationViewModel>(informationTemplate).Sections);

                model.Section = sections;
                return model;
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                throw ex;
            }
        }

        public async Task<InformationViewModel> GetClientInformationSheetViewModel(Guid sheetId)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(sheetId);
                InformationViewModel model = await GetInformationViewModel(sheet.Programme);
                model.Sections = model.Sections.OrderBy(sec => sec.Position);                
                model.ClientInformationSheet = sheet;                

                foreach (var section in model.Sections)
                    foreach (var item in section.Items)
                    {
                        var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
                        if (answer != null)
                            item.Value = answer.Value;
                    }

                return model;
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendOnlineAcceptance(string ClientAgreement)
        {
            var clientAgreement = await _clientAgreementService.GetAgreement(Guid.Parse(ClientAgreement));
            var programme = clientAgreement.ClientInformationSheet.Programme.BaseProgramme;


            EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendAgreementOnlineAcceptanceInstructions");
            if (emailTemplate != null)
            {
                await _emailService.SendEmailViaEmailTemplate(programme.Owner.Email, emailTemplate, null, null, null);
                clientAgreement.SentOnlineAcceptance = true;
                await _clientAgreementService.UpdateClientAgreement(clientAgreement);
            }

            return await RedirectToLocal();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInformationSheet(IFormCollection form)
        {
            User currentUser = null;
            //Add User, Organisation, Information Sheet, Quick Term saving process here
            string organisationName = null;
            string ouname = null;
            string orgTypeName = null;

            try
            {
                var orgType = form["cgradioselect"];
                var orgName = form["fname"].ToList().First();
                var firstName = form["fname"].ToList().Last();
                var lastName = form["lname"];                
                var mobilePhone = form["mphon"];
                var programmeList = await _programmeService.GetAllProgrammes();
                var programme = programmeList.LastOrDefault();
                var email = form["email"];
                var membershipNumber = form["memno"];

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
                    
                    var clientProgramme = await _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);
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
                            await uow.Commit();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }                        
                    }                    
                }
                return NoContent();
            }                          
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, currentUser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }            
        }

        // Important Notices - CK Editor

        // Oualification - Name, DateObtained

        // Parties - Party, Org Name, Org Type, Qualification

        #region Test Data
        private string _importantNotices = @"<table id=""Table3"" border=""1"" cellpadding=""4"" cellspacing=""0"" width=""100%"">
	                                            <tbody>
		                                            <tr>
			                                            <td style=""padding-top: 10px;"">
				                                            <h2 align=""center"">
					                                            <strong>
						                                            <font size=""5"">[[BrokerCompanyShort]] Professional Liability Package</font>
					                                            </strong>
				                                            </h2>
				                                            <p align=""center"">
					                                            <img src=""/Content/img/demo/WAHAP.jpg"">
                                                                    </p>
						                                            <h3 align=""center"">
							                                            <strong>
								                                            <font size=""3"">NOTICE TO THE PROPOSED INSURED</font>
							                                            </strong>
						                                            </h3>
					                                            </td>
				                                            </tr>
			                                            </tbody>
		                                            </table>
                                                    <p style=""padding-top: 10px;"">
                                                                                            &nbsp;
		                                            </p>
		                                            <div style=""text-align: center; width: 100%"">
			                                            <p/>
			                                            <table border=""0"" cellpadding=""0"" cellspacing=""0""  align=""center"">
				                                            <tbody>
					                                            <tr>
						                                            <td style=""text-align: center; border-right: red 2px solid; border-top: red 2px solid;
                                                                        border-left: red 2px solid; border-bottom: red 2px solid; padding-right: 10px;
                                                                        padding-top: 10px; padding-bottom: 10px; padding-left: 10px; width: 550px"" valign=""middle"">
							                                            <strong>Note:</strong> You can log off your proposal and return to it later at any stage.
                                                                                                            Any information you have already entered will be saved until you return to complete it.
						                                            </td>
					                                            </tr>
				                                            </tbody>
			                                            </table>
		                                            </div>
		                                            <p style=""padding-top: 10px;"">
			                                            <strong>Dear Health Professional,</strong>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <strong>Please familiarise yourself with the following: </strong>
		                                            </p>
		                                            <h3 style=""padding-top: 10px;"">
			                                            <strong>1. DISCLOSURE OF RELEVANT FACTS</strong>
		                                            </h3>
		                                            <p style=""padding-top: 10px;"">
			                                            <strong>
				                                            <span lang=""EN-GB"">Your Duty of </span>
				                                            <!--?xml namespace="""" ns=""urn:schemas-microsoft-com:office:smarttags"" prefix=""st1"" ?-->
				                                            <st1:personname>
					                                            <span lang=""EN-GB""/>
				                                            </st1:personname>
				                                            <span lang=""EN-GB"">Disclosure</span>
			                                            </strong>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang=""EN-GB"">
                                                                                                Before you enter into a contract of general insurance with an insurer, you have a duty to disclose
                                                                                                to the insurer every matter which you know, or could reasonably be expected to know, is
                                                                                                relevant to the insurer's decision whether to accept the risk of the insurance and, if so,
                                                                                                on what terms.
			                                            </span>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang=""EN-GB"">
                                                                                                You have the same duty to disclose those matters before you renew, extend,
                                                                                                vary or reinstate a contract of insurance.
			                                            </span>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang=""EN-GB"">
                                                                                                Your duty however does not require disclosure of a matter:
			                                            </span>
		                                            </p>
		                                            <p style=""padding-top: 10px; padding-left: 40px;""> <ul type=""disc"">
                                        <li class=""MsoNormal"" style=""margin: 0cm 0cm 0pt; mso-margin-top-alt: auto; mso-margin-bottom-alt: auto;
                    mso-list: l0 level1 lfo1; tab-stops: list 36.0pt"" ><span lang=""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"" > that diminishes the risk to be undertaken by the insurer </span></li>
                                        <li class=""MsoNormal"" style=""margin: 0cm 0cm 0pt; mso-margin-top-alt: auto; mso-margin-bottom-alt: auto;
                    mso-list: l0 level1 lfo1; tab-stops: list 36.0pt"" ><span lang=""EN-GB"" style=""padding-top: 10px; padding-left: 40px;""> that is common knowledge </span></li>
                                        <li class=""MsoNormal"" style=""margin: 0cm 0cm 0pt; mso-margin-top-alt: auto; mso-margin-bottom-alt: auto;
                    mso-list: l0 level1 lfo1; tab-stops: list 36.0pt"" >
                                            <span lang = ""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                that the insurer knows or, in the ordinary course of business as an insurer, ought
                                                to know
                                            </span>
                                        </li>
                                        <li class=""MsoNormal"" style=""margin: 0cm 0cm 0pt; mso-margin-top-alt: auto; mso-margin-bottom-alt: auto;
                    mso-list: l0 level1 lfo1; tab-stops: list 36.0pt"" ><span lang=""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">as to which compliance with your duty is waived by the insurer.</span></li>
                                    </ul></p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang = ""EN-GB"">
				                                            <strong>NON-DISCLOSURE</strong>
			                                            </span>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang = ""EN-GB"">
                                                                                                If you fail to comply with your duty of disclosure, the insurer may be entitled to reduce
                                                                                                its liability under the contract in respect of a claim or may cancel the contract.
			                                            </span>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang = ""EN-GB"">
                                                                                                If your non-disclosure is fraudulent, the insurer may also have the option
                                                                                                of avoiding the contract from its beginning.
                                                        </span>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang = ""EN-GB"">
				                                            <strong>COMMENT</strong>
			                                            </span>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
			                                            <em>
				                                            <b>
					                                            <span lang = ""EN-GB"">
                                                                                                        The requirement of full and frank disclosure of anything which may
                                                                                                        be material to the risk for which you seek cover (eg.claims, whether founded or unfounded),
                                                                                                        or to the magnitude of the risk, is of the utmost importance with this type of insurance.
                                                                                                        It is better to err on the side of caution by disclosing anything which might conceivably
                                                                                                        influence the insurer's consideration of your proposal.
					                                            </span>
				                                            </b>
			                                            </em>
		                                            </p>
		                                            <p style=""padding-top: 10px;"">
                                                                                            &nbsp;
		                                            </p>
		                                            <table id = ""Table2"" border= ""1"" cellpadding= ""0"" cellspacing= ""0"" width= ""100%"">
			                                            <tbody>
				                                            <tr>
					                                            <td style=""padding-top: 10px;"">
						                                            <h3>
							                                            <strong>2. FAILURE TO COMPLETE PROPOSAL FORM</strong>
						                                            </h3>
						                                            <p style=""padding-top: 10px;"">
                                                                                                            Failure to fully complete or return this completed proposal form
                                                                                                            will result in you having no current cover.
						                                            </p>
						                                            <p style=""padding-top: 10px;"">
                                                                                                            Should any of the questions not be completed and left blank, this proposal
                                                                                                            form will be returned to you immediately for completion and return. Cover
                                                                                                            will not be granted until such time the fully completed proposal form is returned.
						                                            </p>
					                                            </td>
				                                            </tr>
			                                            </tbody>
		                                            </table>
		                                            <h3 style=""padding-top: 10px;"">
			                                            <strong>3. CLAIMS MADE POLICY</strong>
		                                            </h3>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang = ""EN-GB"">
                                                                                                This proposal is for a ""claims made"" policy of insurance.This means that the policy covers you for claims made against you
                                                                                                and notified to the insurer during the period of cover. This policy does not provide cover in relation to:
			                                            </span>
		                                            </p>
		                                            <ul >
			                                            <li >
				                                            <span lang = ""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                                                                    events that occurred prior to the retroactive date of the policy
                                                                                                    (if such a date is specified);
				                                            </span>
			                                            </li>
			                                            <li >
				                                            <span lang=""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                                                                    claims made after the expiry of the period of cover even though the event giving
                                                                                                    rise to the claim may have occurred during the period of cover;
				                                            </span>
			                                            </li>
			                                            <li >
				                                            <span lang = ""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                                                                    claims notified or arising out of facts or circumstances notified(or which ought
                                                                                                    reasonably to have been notified) under any previous policy;
				                                            </span>
			                                            </li>
			                                            <li >
				                                            <span lang = ""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                                                                    claims made, threatened or intimated against you prior to the commencement of the
                                                                                                    period of cover;
				                                            </span>
			                                            </li>
			                                            <li >
				                                            <span lang = ""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                                                                    facts or circumstances which you first became aware of prior to the period of cover,
                                                                                                    and which you knew or ought reasonably to have known had the potential to give rise
                                                                                                    to a claim under this policy;
				                                            </span>
			                                            </li>
			                                            <li >
				                                            <span lang = ""EN-GB"" style=""padding-top: 10px; padding-left: 40px;"">
                                                                                                    claims arising out of circumstances noted on the proposal form for the current period
                                                                                                    of cover or on any previous proposal form.
				                                            </span>
			                                            </li>
		                                            </ul>
		                                            <p style=""padding-top: 10px;"">
			                                            <span lang = ""EN-GB"">
                                                                                                However, where you give notice in writing to the insurer of any facts that might give rise to a claim against you as soon as
                                                                                                reasonably practicable after you become aware of those facts but before the expiry of the period of cover,
                                                                                                the policy will, subject to the terms and conditions, cover you notwithstanding that a claim is only made after the expiry of the period of cover.
                                                        </span>
		                                            </p>
                                                    <p style=""padding-top: 10px;"">
                                                                                            &nbsp;
		                                            </p>
		                                            <div style = ""text-align: center; width: 100%"">
			                                            <table border = ""0"" cellpadding=""0"" cellspacing=""0"" align=""center"">
				                                            <tbody>
					                                            <tr>
						                                            <td style = ""text-align: center; border-right: red 2px solid; border-top: red 2px solid;
        border-left: red 2px solid; border-bottom: red 2px solid; padding-right: 10px;
                                                                        padding-top: 10px; padding-bottom: 10px; padding-left: 10px; width: 550px"" valign=""middle"">
							                                            <strong>Note:</strong> You can log off your proposal and return to it later at any
                                                                                                            stage. Any information you have already entered will be saved until you return to
                                                                                                            complete it.
						                                            </td>
					                                            </tr>
				                                            </tbody>
			                                            </table>";

        private string _avaliableCover = @"<table id=""tblQ"" cellspacing=""0"" cellpadding=""8"" border=""0"">
                                            <tr>
                                                <td colspan = ""2"" style=""padding-top: 10px;"" >
                                                    <table id=""tblCover"" cellspacing=""0"" cellpadding=""8"" border=""1"" width=""50%"" >
                                                        <tr>
                                                            <td style = ""background-color: silver"" style=""padding-top: 10px;"">
                                                                <strong> Insurance Package</strong>
                                                            </td>
                                                            <td style = ""background-color: silver"" style=""padding-top: 10px;"">
                                                                <strong> Your Insurance package will contain the following policies and limits of indemnity:</strong>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style=""padding-top: 10px;"">
                                                                Medical Malpractice
                                                            </td>
                                                            <td style=""padding-top: 10px;"">$500,000 any one claim and in the aggregate</td>
                                                        </tr>
                                                        <tr>
                                                            <td style=""padding-top: 10px;"">Public Liability</td>
                                                            <td style=""padding-top: 10px;"">$1,000,000 any one occurrence</td>
                                                        </tr>
                                                        <tr>
                                                            <td style=""padding-top: 10px;"">Statutory Liability</td>
                                                            <td style=""padding-top: 10px;"">$500,000 any one claim and in the aggregate</td>
                                                        </tr>
                                                    </table>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td style=""padding-top: 10px;"">
                                                    <strong>Annual Package Premium:</strong> $294.11 inc GST, Credit Card surcharge and broker fees per member
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style=""padding-top: 10px;"">
                                                    <h2 style = ""text-align: center"" style=""padding-top: 10px;"">
                                                        <span style= ""color: #ff0000"" >
                                                            <strong>Important Notice</strong>
                                                        </span>
                                                    </h2>
                                                    <p style=""padding-top: 10px;"">
                                                        <strong>
                                                            Thank you for completing your Application.
                                                        </strong>
                                                    </p>
                                                    <p style=""padding-top: 10px;"">
                                                        <strong>

                                                            If your application is referred to [[InsurerCompanyShort]] and [[BrokerCompanyShort]] for manual processing [[BrokerCompanyShort]] will contact you for more information shortly,
                                                            otherwise your cover will be bound and your Insurance will be in place on receipt of your payment.
                                                            Your policy documents will be sent to you in due course.
                                                        </strong>
                                                    </p>
                                                    <p style=""padding-top: 10px;"">
                                                        <strong>
                                                            Please check the policies and ensure the Insurance is issued in accordance with your requirements.
                                                            If you have any questions please contact [[BrokerName]] at [[BrokerCompanyShort]] via email at
                                                        </strong><a href = ""[[BrokerEmail]]"" ><strong> [[BrokerEmail]]</strong></a><strong>.</strong>
                                                    </p>

                                                </td>
                                            </tr>
                                        </table>";

        private string _declaration = @"<P style=""padding-bottom: 20px;""><STRONG>On behalf of the applicant:</STRONG></P>
                                        <OL>
                                            <LI>
                                                <STRONG>
                                                    I declare that I am the person named above and I am authorised to submit this
                                                    proposal on behalf of the Applicant.<br />
                                                    &nbsp;
                                                </STRONG>
                                            </LI>
                                            <LI>
                                                <STRONG>
                                                    I declare that the information and answers given in this proposal have been
                                                    checked and are true and complete in every respect and the Applicant is not
                                                    aware of any other information that may be material in considering this
                                                    proposal.
                                                </STRONG>
                                                <br />
                                                &nbsp;
                                            </LI>
                                            <LI>
                                                <STRONG>
                                                    I acknowledge that this proposal, declaration and any other information
                                                    supplied in support of this proposal constitutes representations to, and will
                                                    be relied on as the basis of contract by, insurers requested to quote on this
                                                    proposal. We undertake to inform these insurers through our broker of any
                                                    material alteration to this information whether occurring before or after the
                                                    completion of any insurance contract.<br />
                                                    &nbsp;
                                                </STRONG>
                                            </LI>
                                            <LI>
                                                <STRONG>
                                                    I acknowledge that misrepresentations or material non-disclosure of
                                                    relevant information, whether made through this proposal or otherwise, may
                                                    result in the insurance not being available to meet a claim and/or cancellation
                                                    of relevant insurance contract(s), in addition to other remedies.<br />
                                                    &nbsp;
                                                </STRONG>
                                            </LI>
                                            <LI>
                                                <STRONG>
                                                    I confirm that the applicant authorises the disclosure to insurers
                                                    requested to quote on this proposal, of information held by other insurers or
                                                    insurance brokers.<br />
                                                    &nbsp;
                                                </STRONG>
                                            </LI>
                                            <LI>
                                                <STRONG>
                                                    I declare that I am a current member or employee of <br />
                                                    &nbsp;&nbsp;&nbsp;a) Wellness and Health Associated Professionals (NZ) Inc
                                                </STRONG>
                                            </LI>
                                        </OL>
                                        <p align=""center"">
                                        <button class=""btn btn-primary"" type = ""submit"" style=""width:150px"">Agree and Submit</button>
                                        </p>";
        #endregion
    }
}
