using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Programme;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using TechCertain.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Authorization;
using TechCertain.Infrastructure.Tasking;
using TechCertain.WebUI.Helpers;

namespace TechCertain.WebUI.Controllers
{
    [Authorize]
    public class InformationController : BaseController
    {

        IActivityService _activityService;
        IInformationItemService _informationItemService;
        IInformationSectionService _informationSectionService;
        IInformationTemplateService _informationTemplateService;
        IFileService _fileService;
        IClientInformationService _clientInformationService;
        IChangeProcessService _changeProcessService;
        IClientAgreementService _clientAgreementService;
        IClientAgreementTermService _clientAgreementTermService;
        IClientAgreementMVTermService _clientAgreementMVTermService;
        IClientAgreementRuleService _clientAgreementRuleService;
        IUWMService _uWMService;
        ITaskingService _taskingService;
        IMapperSession<PolicyDocumentTemplate> _documentRepository;
        IEmailService _emailService;
        IUnitOfWork _unitOfWork;
        IReferenceService _referenceService;
        IMilestoneService _milestoneService;
        IAdvisoryService _advisoryService;
        IMapperSession<Organisation> _organisationRepository;
        IMapperSession<InsuranceAttribute> _InsuranceAttributesRepository;
        IMapperSession<Territory> _territoryRepository;
        IMapperSession<BusinessActivity> _busActivityRespository;
        IProgrammeService _programmeService;
        IBusinessActivityService _businessActivityService;
        IProductService _productService;
        IMapper _mapper;
        IMapperSession<DropdownListItem> _IDropdownListItem;
        IClientInformationAnswerService _clientInformationAnswer;
        IMapperSession<InformationSection> _informationSectionRepository;
        IMapperSession<ChangeReason> _changeReasonRepository;

        public InformationController(
            IActivityService activityService,
            IAdvisoryService advisoryService,
            IUserService userService,
            IInformationItemService informationItemService,
            IChangeProcessService changeProcessService,
            IInformationSectionService informationSectionService,
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
            IMapperSession<Organisation> organisationRepository,
            IMapperSession<InsuranceAttribute> insuranceAttributesRepository,
            IMapperSession<PolicyDocumentTemplate> documentRepository,
            IMapperSession<Territory> territoryRepository,
             IMapperSession<ChangeReason> changeReasonRepository,
            IUnitOfWork unitOfWork,
            IMapperSession<BusinessActivity> busActivityRespository,
            IMapperSession<InformationSection> informationSectionRepository,
            IProgrammeService programmeService,
            IBusinessActivityService businessActivityService,
            IClientInformationAnswerService clientInformationAnswer,
            IMapperSession<DropdownListItem> dropdownListItem,
            IMapper mapper)
            : base (userService)
        {
            _advisoryService = advisoryService;
            _activityService = activityService;
            _userService = userService;
            _changeProcessService = changeProcessService;
            _changeReasonRepository = changeReasonRepository;
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
            _organisationRepository = organisationRepository;
            _InsuranceAttributesRepository = insuranceAttributesRepository;
            _busActivityRespository = busActivityRespository;
            _territoryRepository = territoryRepository;
            _documentRepository = documentRepository;
            _programmeService = programmeService;
            _unitOfWork = unitOfWork;
            _informationSectionRepository = informationSectionRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProgrammes()
        {
            //var informationTemplate = new IList<InformationTemplate>();
            var informationTemplate = await _informationTemplateService.GetAllTemplates();
            // InformationSection informationSection = _informationSectionRepository.GetById(new Guid("3b2ba8c1-48bc-4ec2-b8ef-aaa200bc5376"));
            InformationBuilderViewModel model = new InformationBuilderViewModel();
            var template = new List<InformationTemplate>();

           
          //  IEnumerable<InformationSection> sections = _informationSectionService.GetAllSections();

            //model.Sections = _informationSectionService.GetAllSections();
            foreach (var temp in informationTemplate)
            {
                if(temp.Name != "Coast Guard")
                template.Add(temp);
            }
                
            model.InformationTemplates = template;
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetProgrammeSections(Guid informationTemplateID)
        {
            //var informationTemplate = new IList<InformationTemplate>();

            InformationTemplate template = await _informationTemplateService.GetTemplate(informationTemplateID);

            Information model = new Information();
            var Litems = new List<InformationItems>();
            var sections = new List<InformationSection>();
           // sections = new List<InformationSection>(_informationSectionService.GetAllSections().Where(pos => pos.Position > 0));

            foreach (var item in template.Sections)
            {
                Litems.Add(new InformationItems(){ Id = item.Id, Name= item.Name});
                
            }
            model.informationitem = Litems;
           // model.Sections = sections;
                return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> SectionBuilder(Guid SectionId)
        //{
        //    //  InformationTemplate template = _informationTemplateService.GetAllTemplates().FirstOrDefault(t => t.Id == informationTemplateID);

        //    InformationSection section = await _informationSectionService.GetSection(SectionId);
        //    InformationViewModel model = new InformationViewModel();
          
        //    model.AnswerSheetId = Guid.Parse("fd442ea1-353d-4f98-86cd-aab200d933f4");
        //    return View(section);
        //}


        [HttpPost]
        public async Task<IActionResult> SectionBuilder(Guid SectionId)
        {
            //  InformationTemplate template = _informationTemplateService.GetAllTemplates().FirstOrDefault(t => t.Id == informationTemplateID);

            InformationSection section = _informationSectionService.GetAllSections().FirstOrDefault(t => t.Id == SectionId);
            Information model = new Information();

            var Litems = new List<InformationItems>();



            foreach (var item in section.Items)
            {
                var list = new List<DropdownList>();

                foreach (var dropdown in item.droplistItems)
                {
                    foreach (var option in dropdown.Options)
                    {
                        list.Add(new DropdownList(option.Text, option.Value));

                    }
                    //list.Add(dropdown.)
                }
                Litems.Add(new InformationItems()
                {
                    Id = SectionId,
                    Type = item.Type,
                    Label = item.Label,
                    option = list,
                    ControlId = item.ControlId
                });
            }


            // InformationTemplate informationTemplate = _informationTemplateService.GetTemplate(new Guid("fd442ea1-353d-4f98-86cd-aab200d933f4"));
            // InformationBuilderViewModel model = new InformationBuilderViewModel();
            // var template = new List<InformationTemplate>();

            //template.Add(informationTemplate);

            // model.InformationTemplates = template;
            return Json(Litems);
        }



        [HttpPost]
        public async Task<IActionResult> SectionBuilder1(Guid SectionId)
        {
          //  InformationTemplate template = _informationTemplateService.GetAllTemplates().FirstOrDefault(t => t.Id == informationTemplateID);

            InformationSection section = await _informationSectionService.GetSection(SectionId);
            InformationViewModel model = new InformationViewModel();
            
            model.AnswerSheetId = Guid.Parse("fd442ea1-353d-4f98-86cd-aab200d933f4");
            return View(model);
        }

        [HttpGet]
        public InformationViewModel LoadTemplate()
        {
            var model = new InformationViewModel();

            model.Name = "Wellness and Health Associated Professionals";

            // Important Notices
            var noticeItems = new List<InformationItemViewModel>();
            noticeItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _importantNotices });
            var importantNoticeSection = new InformationSectionViewModel() { Name = "Important Notices", Items = noticeItems };

            // Applicant     
            var itemList = new List<InformationItemViewModel>();

            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "First Name", Name = "fname", Width = 3, Type = ItemType.TEXTBOX });
            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Last Name", Name = "lname", Width = 3, Type = ItemType.TEXTBOX });
            itemList.Add(new InformationItemViewModel() { Control = "email", Icon = "icon-prepend fa fa-envelope-o", Label = "E-mail", Name = "email", Width = 6, Type = ItemType.TEXTBOX });
            itemList.Add(null);

            itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-phone", Label = "Phone", Name = "phone", Width = 3, Type = ItemType.TEXTBOX });
            itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-fax", Label = "Fax", Name = "fax", Width = 3, Type = ItemType.TEXTBOX });
            itemList.Add(null);

            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Postal Address", Name = "paddress", Width = 6, Type = ItemType.TEXTBOX });
            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Street Address", Name = "saddress", Width = 6, Type = ItemType.TEXTBOX });
            itemList.Add(null);

            var applicantSection = new InformationSectionViewModel() { Name = "Applicant", Items = itemList };


            // Parties
            var partiesitemList = new List<InformationItemViewModel>();

            partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Insured", Name = "nameofinsured", Width = 6, Type = ItemType.TEXTBOX });
            partiesitemList.Add(null);
            partiesitemList.Add(null);
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Note: employees, subagents and business partners who are actively involved in providing services to your clients need their own Insurance cover and must complete their own declaration.", Width = 12 });
            partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-book", Label = "Qualifications and date obtained", Name = "qualifications", Width = 6, Type = ItemType.TEXTBOX });
            partiesitemList.Add(null);
            partiesitemList.Add(null);
            partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-briefcase", Label = "Company Name (if applicable)", Name = "companyname", Width = 6, Type = ItemType.TEXTBOX });
            partiesitemList.Add(null);
            partiesitemList.Add(null);

            var companytypeoptions = new List<SelectListItem>();
            companytypeoptions.Add(new SelectListItem() { Text = "Private Limited Liability Company", Value = "1" });
            companytypeoptions.Add(new SelectListItem() { Text = "Public Listed Company", Value = "2" });
            companytypeoptions.Add(new SelectListItem() { Text = "Public Unlisted Company", Value = "3" });
            companytypeoptions.Add(new SelectListItem() { Text = "Co-operative/Mutual", Value = "4" });
            companytypeoptions.Add(new SelectListItem() { Text = "Partnership", Value = "5" });
            companytypeoptions.Add(new SelectListItem() { Text = "Sole Trader", Value = "6" });
            companytypeoptions.Add(new SelectListItem() { Text = "Trust", Value = "7" });
            companytypeoptions.Add(new SelectListItem() { Text = "Charitable Trust", Value = "8" });
            companytypeoptions.Add(new SelectListItem() { Text = "Incorporated/Unincorporated Society", Value = "9" });
            companytypeoptions.Add(new SelectListItem() { Text = "Other", Value = "10" });
            partiesitemList.Add(new InformationItemViewModel() { Label = "Company Type", Name = "companytypeoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = companytypeoptions, Value = "1" });
            partiesitemList.Add(null);
            partiesitemList.Add(null);

            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Insurer:", Width = 3 });
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[InsurerCompany]]", Width = 9 });
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Broker:", Width = 3 });
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[BrokerCompany]]", Width = 9 });

            var partiesSection = new InformationSectionViewModel() { Name = "Parties", Items = partiesitemList };

            // Business Activities
            var businessactivitiesitemList = new List<InformationItemViewModel>();

            var associationoptions = new List<SelectListItem>();
            associationoptions.Add(new SelectListItem() { Text = "Wellness and Health Associated Professionals", Value = "1" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Association you hold a membership with", Name = "associationoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = associationoptions });
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-dollar", Label = "Gross income excluding GST (before payment of any franchise fees, expenses or tax)", Name = "grossincome", Width = 6, Type = ItemType.TEXTBOX });
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(null);
            var businessactivitiesoptions = new List<SelectListItem>();
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Massage Therapies", Value = "1" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Accupuncture", Value = "2" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Aura Soma", Value = "3" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Chinese Cupping", Value = "4" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Chios", Value = "5" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Colour Therapy", Value = "6" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Crystal Therapy", Value = "7" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Dry Needling", Value = "8" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Emotional Freedom Technique", Value = "9" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Facials", Value = "10" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Flower Essences", Value = "11" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Herbal Medicine", Value = "12" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Mediumship", Value = "13" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Motivational Interviewing", Value = "14" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Naturopathy", Value = "15" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Neural Integration Systems (Neurolink)", Value = "16" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Neurostructural Integration Technique (NST)", Value = "17" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Ortho-Bionomy", Value = "18" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Personal Training", Value = "19" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Pilates Teaching", Value = "20" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Polarity", Value = "21" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Scenar Therapy", Value = "22" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Soul Midwiving", Value = "23" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Sound Therapy/Music Therapy", Value = "24" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Supervision", Value = "25" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Theta Healing", Value = "26" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Visceral Manipulation", Value = "27" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Yoga Teacher", Value = "28" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Neuro Linguistic Programming", Value = "29" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Reflexology", Value = "30" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Reiki", Value = "31" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Thai traditional Massage", Value = "32" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Other", Value = "33" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Please indicate which therapies you practice:", Name = "businessactivitiesoptions", Width = 6, Type = ItemType.PERCENTAGEBREAKDOWN, DefaultText = "Select", Options = businessactivitiesoptions });
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(null);
            var overseasworkoptions = new List<SelectListItem>();
            overseasworkoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            overseasworkoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Do you currently perform work outside of NZ, or expect to perform work outside NZ in the next twelve months?", Name = "overseasworkoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = overseasworkoptions });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-comment", Label = "Details of work you expect to perform outside of NZ in the next twelve months", Name = "overseasworkdesc", Width = 6, Type = ItemType.TEXTBOX });

            var businessactivitiesSection = new InformationSectionViewModel() { Name = "Business Activities", Items = businessactivitiesitemList };

            // People Risk
            var peopleriskitemList = new List<InformationItemViewModel>();

            var peoplerisk1options = new List<SelectListItem>();
            peoplerisk1options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            peoplerisk1options.Add(new SelectListItem() { Text = "No", Value = "2" });
            peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you currently have insurance for key person, shareholder protection or other related people risk covers, i.e.income protection?", Name = "peoplerisk1options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk1options });
            peopleriskitemList.Add(null);
            peopleriskitemList.Add(null);

            var peoplerisk2options = new List<SelectListItem>();
            peoplerisk2options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            peoplerisk2options.Add(new SelectListItem() { Text = "No", Value = "2" });
            peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you have key people in your business whom are vital to the ongoing performance of the company?", Name = "peoplerisk2options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk2options });
            peopleriskitemList.Add(null);
            peopleriskitemList.Add(null);

            var peoplerisk3options = new List<SelectListItem>();
            peoplerisk3options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            peoplerisk3options.Add(new SelectListItem() { Text = "No", Value = "2" });
            peopleriskitemList.Add(new InformationItemViewModel() { Label = "Would you like one of our qualified advisers to contact you to discuss key person and relevant people risk covers?", Name = "peoplerisk3options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk3options });
            peopleriskitemList.Add(null);
            peopleriskitemList.Add(null);

            var peopleriskSection = new InformationSectionViewModel() { Name = "People Risk", Items = peopleriskitemList };

            // Insurance History
            var insurancehistoryitemList = new List<InformationItemViewModel>();

            var insurancehistoryoptions = new List<SelectListItem>();
            insurancehistoryoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            insurancehistoryoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
            insurancehistoryitemList.Add(new InformationItemViewModel() { Label = "In relation to the cover being applied for, have you ever had any insurance declined or cancelled; renewal refused; special conditions imposed; excess imposed; or claim rejected?", Name = "insurancehistoryoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = insurancehistoryoptions });
            insurancehistoryitemList.Add(null);
            insurancehistoryitemList.Add(null);

            var insurancehistorySection = new InformationSectionViewModel() { Name = "Insurance History", Items = insurancehistoryitemList };

            // Avaliable Cover
            var avaliablecoveritemList = new List<InformationItemViewModel>();
            avaliablecoveritemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _avaliableCover });
            var avaliablecoverSection = new InformationSectionViewModel() { Name = "Avaliable Cover", Items = avaliablecoveritemList };

            // Declaration
            var declarationItems = new List<InformationItemViewModel>();
            declarationItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _declaration });
            var declarationSection = new InformationSectionViewModel() { Name = "Declaration", Items = declarationItems };

            var sections = new List<InformationSectionViewModel>();

            sections.Add(importantNoticeSection);
            sections.Add(applicantSection);
            sections.Add(partiesSection);
            sections.Add(businessactivitiesSection);
            sections.Add(peopleriskSection);
            sections.Add(insurancehistorySection);
            sections.Add(avaliablecoverSection);
            sections.Add(declarationSection);

            model.Sections = sections;

            return model;
        }

        [HttpGet]
        public async Task<InformationViewModel> LoadHianzTemplate()
        {
            InformationViewModel model = new InformationViewModel();

            model.Name = "HIANZ Motor Vehicle";
            var user = await CurrentUser();
            // Applicant     
            var appItemList = new List<InformationItemViewModel>();

            appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-user", Label = "First Name", Name = "fname", Width = 3, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Last Name", Name = "lname", Width = 3, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Control = "email", Icon = "icon-prepend fa fa-envelope-o", Label = "E-mail", Name = "email", Width = 6, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            appItemList.Add(new InformationItemViewModel { Control = "tel", Icon = "icon-prepend fa fa-phone", Label = "Phone", Name = "phone", Width = 3, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Control = "tel", Icon = "icon-prepend fa fa-fax", Label = "Fax", Name = "fax", Width = 3, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Postal Address", Name = "paddress", Width = 6, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Street Address", Name = "saddress", Width = 6, Type = ItemType.TEXTBOX });
            appItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            // Organisation Location     
            var locItemList = new List<InformationItemViewModel>();
            string organisation = user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Name;
            locItemList.Add(new InformationItemViewModel { Label = "Organisation Name:", Width = 4, Type = ItemType.LABEL });
            locItemList.Add(new InformationItemViewModel { Label = organisation, Width = 8, Type = ItemType.LABEL });
            locItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            string organisationLocation = user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.CommonName + "<br />" +
                user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Street + "<br />" +
                user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Suburb + "<br />" +
                user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Postcode + "<br />" +
                user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.City + "<br />" +
                user.Organisations.FirstOrDefault(uo => uo.OrganisationType.Name == "superuser").Location.Country;
            locItemList.Add(new InformationItemViewModel { Label = "Organisation Location:", Width = 4, Type = ItemType.LABEL });
            locItemList.Add(new InformationItemViewModel { Label = organisationLocation, Width = 8, Type = ItemType.LABEL });
            locItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            // Registered MV     
            var regItemList = new List<InformationItemViewModel>();
            string regVehicle = "regvehicle";
            regItemList.Add(new InformationItemViewModel { Label = "Add Vehicle", Name = regVehicle + "TableResult", Width = 6, Type = ItemType.MOTORVEHICLELIST });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            regItemList.Add(new InformationItemViewModel { Control = "text", Label = "Registration Number", Name = regVehicle, Width = 5, Type = ItemType.TEXTBOX });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
            regItemList.Add(new InformationItemViewModel { Label = "Search", Width = 1, Type = ItemType.JSBUTTON, Value = "SearchMotorVehicle(this, '#" + regVehicle + "');" });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            regItemList.Add(new InformationItemViewModel { Label = "Year:", Width = 2, Type = ItemType.LABEL });
            regItemList.Add(new InformationItemViewModel { Label = "<span id=\"" + regVehicle + "Year\"></span>", Width = 2, Type = ItemType.LABEL });
            regItemList.Add(new InformationItemViewModel { Label = "Make:", Width = 2, Type = ItemType.LABEL });
            regItemList.Add(new InformationItemViewModel { Label = "<span id=\"" + regVehicle + "Make\"></span>", Width = 2, Type = ItemType.LABEL });
            regItemList.Add(new InformationItemViewModel { Label = "Model:", Width = 2, Type = ItemType.LABEL });
            regItemList.Add(new InformationItemViewModel { Label = "<span id=\"" + regVehicle + "Model\"></span>", Width = 2, Type = ItemType.LABEL });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            regItemList.Add(new InformationItemViewModel { Control = "text", Label = "Estimated Market Value", Name = regVehicle + "TBMarketValue", Width = 4, Type = ItemType.TEXTBOX });
            regItemList.Add(new InformationItemViewModel { Control = "text", Label = "Fleet Number", Name = regVehicle + "TBFleetNo", Width = 4, Type = ItemType.TEXTBOX });
            var areaOperationsOptions = new List<SelectListItem> {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Auckland", Value = "1" },
                new SelectListItem { Text = "Wellington", Value = "2" },
                new SelectListItem { Text = "Rest of North Island", Value = "3" },
                new SelectListItem { Text = "Christchurch", Value = "4" },
                new SelectListItem { Text = "Rest of South Island", Value = "5" }
            };
            regItemList.Add(new InformationItemViewModel { Label = "Area of Operation", Name = regVehicle + "DDAreaOp", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = areaOperationsOptions });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            var vehicleTypeOptions = new List<SelectListItem> {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Registered Vehicle", Value = "1" },
                new SelectListItem { Text = "Registered Towed", Value = "2" },
                new SelectListItem { Text = "Registered Plant", Value = "3" },
                new SelectListItem { Text = "Static Vehicle", Value = "5"}
            };
            regItemList.Add(new InformationItemViewModel { Label = "Type of Vehicle", Name = regVehicle + "DDVehicleType", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleTypeOptions });
            var vehicleUsageOptions = new List<SelectListItem> {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Car and Truck Rental Service", Value = "1" },
                new SelectListItem { Text = "General Business Use", Value = "2" },
                new SelectListItem { Text = "Static Vehicle", Value = "3"}
            };
            regItemList.Add(new InformationItemViewModel { Label = "Use", Name = regVehicle + "DDUsage", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleUsageOptions });
            var vehicleSubUsageOptions = new List<SelectListItem> {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Company", Value = "1" },
                new SelectListItem { Text = "Private", Value = "2" },
                new SelectListItem { Text = "Rental", Value = "3" },
                new SelectListItem { Text = "Underage Private (under 25 years)", Value = "4" },
                new SelectListItem { Text = "Static Vehicle", Value = "5"}
            };
            regItemList.Add(new InformationItemViewModel { Label = "Sub use", Name = regVehicle + "DDSubUse", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleSubUsageOptions });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            //var interestedParties = new List<SelectListItem> {
            //	new SelectListItem { Text = "ANZ Bank", Value = "1" },
            //	new SelectListItem { Text = "ASB Bank", Value = "2" },
            //	new SelectListItem { Text = "BNZ Bank", Value = "3" }
            //};
            var interestedParties = new List<SelectListItem>();
            foreach (Organisation org in _organisationRepository.FindAll().Where(o => o.OrganisationType.Name == "financial"))
                interestedParties.Add(new SelectListItem { Text = org.Name, Value = org.Id.ToString() });

            regItemList.Add(new InformationItemViewModel { Label = "Interested Parties", Name = regVehicle + "Parties", Width = 6, Type = ItemType.MULTISELECT, Options = interestedParties });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
            regItemList.Add(new InformationItemViewModel { Control = "textarea", Label = "Notes", Name = regVehicle + "Notes", Width = 6, Type = ItemType.TEXTAREA });
            regItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
            regItemList.Add(new InformationItemViewModel { Label = "Add Vehicle", Name = regVehicle + "Add", Width = 1, Type = ItemType.JSBUTTON, Value = "AddMotorVehicle(this, '#" + regVehicle + "', '#" + regVehicle + "TableResult');" });

            // Unregistered MV/Plant/Other
            var otherItemList = new List<InformationItemViewModel>();
            string othervehicle = "othervehicle";
            otherItemList.Add(new InformationItemViewModel { Label = "Add Vehicle", Name = othervehicle + "TableResult", Width = 6, Type = ItemType.STATICVEHICLEPLANTLIST });
            otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Year", Name = othervehicle + "Year", Width = 4, Type = ItemType.TEXTBOX });
            otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Make", Name = othervehicle + "Make", Width = 4, Type = ItemType.TEXTBOX });
            otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Model", Name = othervehicle + "Model", Width = 4, Type = ItemType.TEXTBOX });
            otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Estimated Market Value", Name = othervehicle + "TBMarketValue", Width = 4, Type = ItemType.TEXTBOX });
            otherItemList.Add(new InformationItemViewModel { Control = "text", Label = "Fleet Number", Name = othervehicle + "TBFleetNo", Width = 4, Type = ItemType.TEXTBOX });
            otherItemList.Add(new InformationItemViewModel { Label = "Area of Operation", Name = othervehicle + "DDAreaOp", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = areaOperationsOptions });
            otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            var otherVehicleTypeOptions = new List<SelectListItem> {
                new SelectListItem { Text = "-- Select --", Value = "" },
                new SelectListItem { Text = "Non-Registered Plant", Value = "4" },
                new SelectListItem { Text = "Static Vehicle", Value = "5"}
            };
            otherItemList.Add(new InformationItemViewModel { Label = "Type of Vehicle", Name = othervehicle + "DDVehicleType", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = otherVehicleTypeOptions });
            otherItemList.Add(new InformationItemViewModel { Label = "Use", Name = othervehicle + "DDUsage", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleUsageOptions });
            otherItemList.Add(new InformationItemViewModel { Label = "Sub use", Name = othervehicle + "DDSubUse", Width = 4, Type = ItemType.DROPDOWNLIST, DefaultText = "-- Select --", Options = vehicleSubUsageOptions });
            otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });

            otherItemList.Add(new InformationItemViewModel { Label = "Interested Parties", Name = othervehicle + "Parties", Width = 6, Type = ItemType.MULTISELECT, Options = interestedParties });
            otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
            otherItemList.Add(new InformationItemViewModel { Control = "textarea", Label = "Notes", Name = othervehicle + "Notes", Width = 6, Type = ItemType.TEXTAREA });
            otherItemList.Add(new InformationItemViewModel { Type = ItemType.SECTIONBREAK });
            otherItemList.Add(new InformationItemViewModel { Label = "Add Unplated Vehicle/Plant", Name = othervehicle + "Add", Width = 1, Type = ItemType.JSBUTTON, Value = "AddMotorVehicle(this, '#" + othervehicle + "', '#" + othervehicle + "TableResult');" });

            // set sections
            model.Sections = new List<InformationSectionViewModel> {
                new InformationSectionViewModel { Items = appItemList, Name = "You" },
                //new InformationSectionViewModel { Items = locItemList, Name = "Location" },
                new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Organisational Units", CustomView = "ICIBHianzOU" },
                new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Location", CustomView = "ICIBHianzLocation" },
                new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Registered Vehicles", CustomView = "ICIBHianzMotor" },
                new InformationSectionViewModel { Items = new List<InformationItemViewModel>(), Name = "Other Vehicles/Mobile Plant", CustomView = "ICIBHianzPlant" }
                //new InformationSectionViewModel { Items = regItemList, Name = "Registered Vehicles" },
				//new InformationSectionViewModel { Items = otherItemList, Name = "Other Vehicles/Mobile Plant" }
			};

            return model;
        }

        [HttpGet]
        public InformationViewModel LoadTestData()
        {
            var model = new InformationViewModel();

            model.Name = "Wellness and Health Associated Professionals";

            // Important Notices
            var noticeItems = new List<InformationItemViewModel>();
            noticeItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _importantNotices });
            var importantNoticeSection = new InformationSectionViewModel() { Name = "Important Notices", Items = noticeItems };

            // Applicant     
            var itemList = new List<InformationItemViewModel>();

            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "First Name", Name = "fname", Width = 3, Type = ItemType.TEXTBOX, Value = "ApplicantFirstName" });
            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Last Name", Name = "lname", Width = 3, Type = ItemType.TEXTBOX, Value = "ApplicantLastName" });
            itemList.Add(new InformationItemViewModel() { Control = "email", Icon = "icon-prepend fa fa-envelope-o", Label = "E-mail", Name = "email", Width = 6, Type = ItemType.TEXTBOX, Value = "TestApplicant@techcertain.com" });
            itemList.Add(null);

            itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-phone", Label = "Phone", Name = "phone", Width = 3, Type = ItemType.TEXTBOX, Value = "091234567" });
            itemList.Add(new InformationItemViewModel() { Control = "tel", Icon = "icon-prepend fa fa-fax", Label = "Fax", Name = "fax", Width = 3, Type = ItemType.TEXTBOX, Value = "091234568" });
            itemList.Add(null);

            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Postal Address", Name = "paddress", Width = 6, Type = ItemType.TEXTBOX, Value = "1 Queen St, CBD, Auckland" });
            itemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-location-arrow ", Label = "Street Address", Name = "saddress", Width = 6, Type = ItemType.TEXTBOX });
            itemList.Add(null);

            var applicantSection = new InformationSectionViewModel() { Name = "Applicant", Items = itemList };


            // Parties
            var partiesitemList = new List<InformationItemViewModel>();

            partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-user", Label = "Insured", Name = "nameofinsured", Width = 6, Type = ItemType.TEXTBOX, Value = "TestClientName" });
            partiesitemList.Add(null);
            partiesitemList.Add(null);
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Note: employees, subagents and business partners who are actively involved in providing services to your clients need their own Insurance cover and must complete their own declaration.", Width = 12 });
            partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-book", Label = "Qualifications and date obtained", Name = "qualifications", Width = 6, Type = ItemType.TEXTBOX });
            partiesitemList.Add(null);
            partiesitemList.Add(null);
            partiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-briefcase", Label = "Company Name (if applicable)", Name = "companyname", Width = 6, Type = ItemType.TEXTBOX, Value = "TestClientCompany" });
            partiesitemList.Add(null);
            partiesitemList.Add(null);

            var companytypeoptions = new List<SelectListItem>();
            companytypeoptions.Add(new SelectListItem() { Text = "Private Limited Liability Company", Value = "1" });
            companytypeoptions.Add(new SelectListItem() { Text = "Public Listed Company", Value = "2" });
            companytypeoptions.Add(new SelectListItem() { Text = "Public Unlisted Company", Value = "3" });
            companytypeoptions.Add(new SelectListItem() { Text = "Co-operative/Mutual", Value = "4" });
            companytypeoptions.Add(new SelectListItem() { Text = "Partnership", Value = "5" });
            companytypeoptions.Add(new SelectListItem() { Text = "Sole Trader", Value = "6" });
            companytypeoptions.Add(new SelectListItem() { Text = "Trust", Value = "7" });
            companytypeoptions.Add(new SelectListItem() { Text = "Charitable Trust", Value = "8" });
            companytypeoptions.Add(new SelectListItem() { Text = "Incorporated/Unincorporated Society", Value = "9" });
            companytypeoptions.Add(new SelectListItem() { Text = "Other", Value = "10" });
            partiesitemList.Add(new InformationItemViewModel() { Label = "Company Type", Name = "companytypeoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = companytypeoptions, Value = "1" });
            partiesitemList.Add(null);
            partiesitemList.Add(null);

            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Insurer:", Width = 3 });
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[InsurerCompany]]", Width = 9 });
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "Broker:", Width = 3 });
            partiesitemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = "[[BrokerCompany]]", Width = 9 });

            var partiesSection = new InformationSectionViewModel() { Name = "Parties", Items = partiesitemList };

            // Business Activities
            var businessactivitiesitemList = new List<InformationItemViewModel>();

            var associationoptions = new List<SelectListItem>();
            associationoptions.Add(new SelectListItem() { Text = "Wellness and Health Associated Professionals", Value = "1" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Association you hold a membership with", Name = "associationoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = associationoptions, Value = "1" });
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-dollar", Label = "Gross income excluding GST (before payment of any franchise fees, expenses or tax)", Name = "grossincome", Width = 6, Type = ItemType.TEXTBOX, Value = "$10,000" });
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(null);
            var businessactivitiesoptions = new List<SelectListItem>();
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Massage Therapies", Value = "1" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Accupuncture", Value = "2" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Aura Soma", Value = "3" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Chinese Cupping", Value = "4" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Chios", Value = "5" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Colour Therapy", Value = "6" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Crystal Therapy", Value = "7" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Dry Needling", Value = "8" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Emotional Freedom Technique", Value = "9" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Facials", Value = "10" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Flower Essences", Value = "11" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Herbal Medicine", Value = "12" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Mediumship", Value = "13" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Motivational Interviewing", Value = "14" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Naturopathy", Value = "15" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Neural Integration Systems (Neurolink)", Value = "16" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Neurostructural Integration Technique (NST)", Value = "17" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Ortho-Bionomy", Value = "18" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Personal Training", Value = "19" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Pilates Teaching", Value = "20" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Polarity", Value = "21" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Scenar Therapy", Value = "22" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Soul Midwiving", Value = "23" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Sound Therapy/Music Therapy", Value = "24" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Supervision", Value = "25" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Theta Healing", Value = "26" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Visceral Manipulation", Value = "27" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Yoga Teacher", Value = "28" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Neuro Linguistic Programming", Value = "29" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Reflexology", Value = "30" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Reiki", Value = "31" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Thai traditional Massage", Value = "32" });
            businessactivitiesoptions.Add(new SelectListItem() { Text = "Other", Value = "33" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Please indicate which therapies you practice:", Name = "businessactivitiesoptions", Width = 6, Type = ItemType.PERCENTAGEBREAKDOWN, DefaultText = "Select", Options = businessactivitiesoptions });
            businessactivitiesitemList.Add(null);
            businessactivitiesitemList.Add(null);
            var overseasworkoptions = new List<SelectListItem>();
            overseasworkoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            overseasworkoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Label = "Do you currently perform work outside of NZ, or expect to perform work outside NZ in the next twelve months?", Name = "overseasworkoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = overseasworkoptions, Value = "2" });
            businessactivitiesitemList.Add(new InformationItemViewModel() { Control = "text", Icon = "icon-prepend fa fa-comment", Label = "Details of work you expect to perform outside of NZ in the next twelve months", Name = "overseasworkdesc", Width = 6, Type = ItemType.TEXTBOX });

            var businessactivitiesSection = new InformationSectionViewModel() { Name = "Business Activities", Items = businessactivitiesitemList };

            // People Risk
            var peopleriskitemList = new List<InformationItemViewModel>();

            var peoplerisk1options = new List<SelectListItem>();
            peoplerisk1options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            peoplerisk1options.Add(new SelectListItem() { Text = "No", Value = "2" });
            peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you currently have insurance for key person, shareholder protection or other related people risk covers, i.e.income protection?", Name = "peoplerisk1options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk1options, Value = "2" });
            peopleriskitemList.Add(null);
            peopleriskitemList.Add(null);

            var peoplerisk2options = new List<SelectListItem>();
            peoplerisk2options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            peoplerisk2options.Add(new SelectListItem() { Text = "No", Value = "2" });
            peopleriskitemList.Add(new InformationItemViewModel() { Label = "Do you have key people in your business whom are vital to the ongoing performance of the company?", Name = "peoplerisk2options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk2options, Value = "2" });
            peopleriskitemList.Add(null);
            peopleriskitemList.Add(null);

            var peoplerisk3options = new List<SelectListItem>();
            peoplerisk3options.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            peoplerisk3options.Add(new SelectListItem() { Text = "No", Value = "2" });
            peopleriskitemList.Add(new InformationItemViewModel() { Label = "Would you like one of our qualified advisers to contact you to discuss key person and relevant people risk covers?", Name = "peoplerisk3options", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = peoplerisk3options, Value = "2" });
            peopleriskitemList.Add(null);
            peopleriskitemList.Add(null);

            var peopleriskSection = new InformationSectionViewModel() { Name = "People Risk", Items = peopleriskitemList };

            // Insurance History
            var insurancehistoryitemList = new List<InformationItemViewModel>();

            var insurancehistoryoptions = new List<SelectListItem>();
            insurancehistoryoptions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            insurancehistoryoptions.Add(new SelectListItem() { Text = "No", Value = "2" });
            insurancehistoryitemList.Add(new InformationItemViewModel() { Label = "In relation to the cover being applied for, have you ever had any insurance declined or cancelled; renewal refused; special conditions imposed; excess imposed; or claim rejected?", Name = "insurancehistoryoptions", Width = 6, Type = ItemType.DROPDOWNLIST, DefaultText = "Select", Options = insurancehistoryoptions, Value = "2" });
            insurancehistoryitemList.Add(null);
            insurancehistoryitemList.Add(null);

            var insurancehistorySection = new InformationSectionViewModel() { Name = "Insurance History", Items = insurancehistoryitemList };

            // Avaliable Cover
            var avaliablecoveritemList = new List<InformationItemViewModel>();
            avaliablecoveritemList.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _avaliableCover });
            var avaliablecoverSection = new InformationSectionViewModel() { Name = "Avaliable Cover", Items = avaliablecoveritemList };

            // Declaration
            var declarationItems = new List<InformationItemViewModel>();
            declarationItems.Add(new InformationItemViewModel() { Type = ItemType.LABEL, Label = _declaration });
            var declarationSection = new InformationSectionViewModel() { Name = "Declaration", Items = declarationItems };

            var sections = new List<InformationSectionViewModel>();

            sections.Add(importantNoticeSection);
            sections.Add(applicantSection);
            sections.Add(partiesSection);
            sections.Add(businessactivitiesSection);
            sections.Add(peopleriskSection);
            sections.Add(insurancehistorySection);
            sections.Add(avaliablecoverSection);
            sections.Add(declarationSection);

            model.Sections = sections;

            return model;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAll()
        {
            IEnumerable<InformationTemplate> templates = await _informationTemplateService.GetAllTemplates();

            //Mapper.CreateMap<InformationTemplate, InformationViewModel>();
            //Mapper.CreateMap<InformationSection, InformationSectionViewModel>();
            //Mapper.CreateMap<InformationItem, InformationItemViewModel>();
            //Mapper.CreateMap<SelectListItem, DropdownListOption>();
            //Mapper.CreateMap<DropdownListOption, SelectListItem>();

            InformationViewAllViewModel model = new InformationViewAllViewModel();
            model.InformationTemplates = _mapper.Map<List<InformationViewModel>>(templates);

            return View(model);
        }

        // GET: Information
        [HttpGet]
        public async Task<IActionResult> ViewInformation(Guid id)
        {
            //            var model = LoadTemplate();
            //
            //            if (Request.RequestContext.RouteData.Values["id"] != null)
            //            {
            //                ViewBag.TestData = true;
            //
            //                model = LoadTestData();
            //            }



            // Postal Address - Multiline Textbox
            // Streed Address - Multiline Textbox
            // Email        

            // Parties

            // Party to be Billed

            // Available Cover -- Customer Insurance Section

            // Section Custom

            // Insurance History  -- Customer Insurance Section

            // Submit 
            InformationViewModel model = await GetClientInformationSheetViewModel(id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> StartInformation(Guid id)
        {
            //ClientInformationSheet sheet = _clientInformationService.GetInformation (id);

            //InformationViewModel model = GetInformationViewModel(sheet.Programme.Id);

            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
            ClientInformationSheet sheet = clientProgramme.InformationSheet;
            InformationViewModel model = await GetInformationViewModel(clientProgramme.BaseProgramme.Id);
            var user = await CurrentUser();
            model.AnswerSheetId = sheet.Id;
            model.OrganisationId = sheet.Owner.Id;

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
                await uow.Commit().ConfigureAwait(false);
            }

            model.SharedData = new SharedDataViewModel();
            //foreach (var answer in sheet.SharedData.SharedAnswers)
            //	model.SharedData.Add (answer.ItemName, answer.Value);

            var boats = new List<BoatViewModel>();
            foreach (Boat b in sheet.Boats)
            {
                boats.Add(BoatViewModel.FromEntity(b));
            }
            model.Boats = boats;

            //var interestedParties = new List<OrganisationViewModel>();
            //foreach (Organisation org in _organisationRepository.FindAll().Result.Where(o => o.OrganisationType != null))
            //{
            //    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
            //    ovm.OrganisationName = org.Name;
            //    interestedParties.Add(ovm);
            //}

            var operators = new List<OrganisationViewModel>();
            foreach (Organisation or in _organisationRepository.FindAll().Where(o => o.OrganisationType.Name == "Skipper"))
            {
                OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(or);
                ovm.OrganisationName = or.Name;
                ovm.OrganisationEmail = or.Email;
                operators.Add(ovm);
                //  operators.Add(OrganisationViewModel.FromEntity(oper));
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
            foreach (Organisation org in _organisationRepository.FindAll().Where(o => o.OrganisationType != null))
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

            var availableProducts = new List<ProductItem>();
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
            model.AvailableProducts = availableProducts;
            model.OrganisationDetails = organisationDetails;
            model.UserDetails = userDetails;

            model.BusinessActivities = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityRespository.FindAll());
            model.RevenueByActivity = _mapper.Map<IEnumerable<RevenueByActivityViewModel>>(sheet.RevenueData);

            //_taskingService.CreateTaskFor (CurrentUser(), sheet.Owner, "Complete Insurance Information", DateTime.UtcNow.AddDays (7));

            return View("InformationWizard", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPanel(Guid panelId, string panelName, int panelPosition)
        {

            using (var uow = _unitOfWork.BeginUnitOfWork())
            {

                if (panelName != null)
                {

                    InformationSection section = await _informationSectionRepository.GetByIdAsync(panelId);
                    section.Position = panelPosition;
                    // TODO: Add these items at templates so it can be clonned properly 
                    await uow.Commit();
                }

            }
            return Json(true);
        }


        [HttpGet]
        public async Task<IActionResult> ViewProgrammeDetails(Guid Id)
        {
            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(Id);
            ClientInformationSheet sheet = clientProgramme.InformationSheet;

            InformationViewModel model = await GetInformationViewModel(clientProgramme.BaseProgramme.Id);
            try
            {
                model.Id = Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            ViewBag.Title = "View Information Sheet ";
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> PartialViewProgramme(String name, Guid id)
        {
            //////BaseListViewModel<ProgrammeInfoViewModel> models = new BaseListViewModel<ProgrammeInfoViewModel>();
            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
            ClientInformationSheet sheet = clientProgramme.InformationSheet;
            var MarinaLocations = new List<OrganisationViewModel>();

            InformationViewModel model = await GetInformationViewModel(clientProgramme.BaseProgramme.Id);
            model.AnswerSheetId = sheet.Id;
            model.IsChange = sheet.IsChange;
            model.SectionView = name;
            model.Id = id;
            var user = await CurrentUser();
            try
            {

                model.OrganisationId = clientProgramme.Owner.Id;

                foreach (var section in model.Sections)
                    foreach (var item in section.Items)
                    {
                        var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
                        if (answer != null)
                            item.Value = answer.Value;
                    }

                model.SharedData = new SharedDataViewModel();

                var boats = new List<BoatViewModel>();
                foreach (Boat b in sheet.Boats)
                {
                    boats.Add(BoatViewModel.FromEntity(b));
                }
                model.Boats = boats;

                var operators = new List<OrganisationViewModel>();
                foreach (Organisation or in _organisationRepository.FindAll().Where(o => o.OrganisationType.Name == "Skipper"))
                {
                    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(or);
                    ovm.OrganisationName = or.Name;
                    operators.Add(ovm);
                    //  operators.Add(OrganisationViewModel.FromEntity(oper));
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

                    // boats.Add(BoatViewModel.FromEntity(sheet.Boats.ElementAtOrDefault(i)));

                }
               
                model.SkipperList = skipperlist;

                //var operators = new List<OperatorViewModel>();
                //foreach (Operator oper in sheet.Operators)
                //{
                //    operators.Add(OperatorViewModel.FromEntity(oper));
                //}
                //model.Operators = operators;

                var claims = new List<ClaimViewModel>();
                foreach (ClaimNotification cl in sheet.ClaimNotifications)
                {
                    claims.Add(ClaimViewModel.FromEntity(cl));
                }
                model.Claims = claims;



                var interestedParties = new List<OrganisationViewModel>();
                try
                {
                    foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Where(ia => ia.InsuranceAttributeName == "Financial" || ia.InsuranceAttributeName == "Private" || ia.InsuranceAttributeName == "CoOwner"))
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
                  
                    foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Where(ia => ia.InsuranceAttributeName == "Marina" || ia.InsuranceAttributeName == "Other Marina"))
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

                //var interestedParties = new List<OrganisationViewModel>();
                //foreach (Organisation org in _organisationRepository.FindAll().Where(o => o.OrganisationType != null))
                //{
                //    //List<SelectListItem> partylist = new List<SelectListItem>();

                //    //try
                //    //{

                //    //        var text = org.Name;
                //    //        var val = org.Id.ToString();

                //    //        list.Add(new SelectListItem
                //    //        {
                //    //            Selected = false,
                //    //            Value = val,
                //    //            Text = text
                //    //        });



                //    //}
                //    //catch (Exception ex)
                //    //{
                //    //    Console.WriteLine(ex.InnerException);
                //    //}

                //    //model.InterestedPartyList = partylist;

                //    OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
                //    ovm.OrganisationName = org.Name;
                //    interestedParties.Add(ovm);
                //}

                var availableProducts = new List<ProductItem>();


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
                model.AvailableProducts = availableProducts;
                model.OrganisationDetails = organisationDetails;
                model.UserDetails = userDetails;

                model.BusinessActivities = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityRespository.FindAll());
                model.RevenueByActivity = _mapper.Map<IEnumerable<RevenueByActivityViewModel>>(sheet.RevenueData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Programme Email Template ";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CloseAdvisory(MilestoneAdvisoryVM milestoneAdvisoryVM)
        {
            await _milestoneService.CloseMileTask(milestoneAdvisoryVM.Id, milestoneAdvisoryVM.Method).ConfigureAwait(false);

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> EditInformation(Guid id)
        {
            //ClientInformationSheet sheet = _clientInformationService.GetInformation (id);

            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
            ClientInformationSheet sheet = clientProgramme.InformationSheet;

            InformationViewModel model = await GetInformationViewModel(clientProgramme.BaseProgramme.Id);

            model.AnswerSheetId = sheet.Id;
            model.IsChange = sheet.IsChange;
            model.Id = id;
            var user = await CurrentUser();
            //add milestone process here

            string advisoryDesc = "";
            if (sheet.Status == "Not Started")
            {
                var milestone = await _milestoneService.GetMilestoneByBaseProgramme(clientProgramme.BaseProgramme.Id);
                if (milestone != null)
                {
                    var activity = await _activityService.GetActivityByName("Agreement Status - Not Started");
                    var advisory = await _advisoryService.GetAdvisoryByMilestone(milestone, activity);
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
            model.OrganisationId = clientProgramme.Owner.Id;

            for (var i = 0; i < model.Sections.Count(); i++)
            {
                for (var j = 0; j < model.Sections.ElementAtOrDefault(i).Items.Count(); j++)
                {
                    var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == model.Sections.ElementAtOrDefault(i).Items.ElementAtOrDefault(j).Name);
                    if (answer != null)
                        model.Sections.ElementAtOrDefault(i).Items.ElementAtOrDefault(j).Value = answer.Value;
                }

            }
            //foreach (var section in model.Sections)
            //    foreach (var item in section.Items)
            //    {
            //        var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
            //        if (answer != null)
            //            item.Value = answer.Value;
            //    }
           
            model.SharedData = new SharedDataViewModel();
            model.RevenueByTerritories = new List<RevenueByTerritoryViewModel>();
            //model.RolesByLocation = new List<RoleDetailViewModel>();
            var roleList = new List<RoleDetailViewModel>();
            //foreach(Role role in _roleService.GetRolesByProgramme(clientProgramme.BaseProgramme.Id))
            //{
            //    RoleDetailViewModel roleDetail = new RoleDetailViewModel()
            //    {
            //        Role = role,
            //    };
            //    roleList.Add(roleDetail);                
            //}
            //model.RolesByLocation = roleList;

            List<RevenueByTerritoryViewModel> Lterritories = new List<RevenueByTerritoryViewModel>();
            var territories = _territoryRepository.FindAll().ToList(); 
            foreach(Territory territory in territories)
            {
                RevenueByTerritoryViewModel revenueByTerritoryViewModel = new RevenueByTerritoryViewModel();
                revenueByTerritoryViewModel.Territory = territory.Location;
                Lterritories.Add(revenueByTerritoryViewModel);
            }
            
            model.RevenueByTerritories = Lterritories;

            //IMapperSession<Territory> _TerritoryRepository;  sdsdsadsadsa
            //foreach (var answer in sheet.SharedData.SharedAnswers)
            //	model.SharedData.Add (answer.ItemName, answer.Value);

            var boats = new List<BoatViewModel>();
            for (var i = 0; i < sheet.Boats.Count(); i++)
            {
                boats.Add(BoatViewModel.FromEntity(sheet.Boats.ElementAtOrDefault(i)));

            }
            //foreach (Boat b in sheet.Boats)
            //{
            //    boats.Add(BoatViewModel.FromEntity(b));
            //}
            model.Boats = boats;

            var operators = new List<OrganisationViewModel>();
            try {

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

            //foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Result.Where(ia => ia.InsuranceAttributeName == "Skipper"))
            //{
            //    foreach(var org in IA.IAOrganisations)
            //    {
            //        if(org.OrganisationType.Name== "Person - Individual")
            //        {
            //            OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
            //            ovm.OrganisationName = org.Name;
            //            ovm.OrganisationEmail = org.Email;
            //            ovm.ID = org.Id;
            //            operators.Add(ovm);
            //        }
            //    }
            //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            //foreach (Organisation org in _organisationRepository.FindAll().Result.Where(o => o.OrganisationType.Name == "Person - Individual"))
            //{
            //    foreach (var str in org.InsuranceAttributes)
            //    {
            //        if (str.InsuranceAttributeName == "Skipper")
            //        {
            //            OrganisationViewModel ovm = _mapper.Map<OrganisationViewModel>(org);
            //            ovm.OrganisationName = org.Name;
            //            ovm.OrganisationEmail = org.Email;
            //            ovm.ID = org.Id;
            //            operators.Add(ovm);
            //        }
            //    }
            //}
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

                // boats.Add(BoatViewModel.FromEntity(sheet.Boats.ElementAtOrDefault(i)));

            }
            //foreach (var ip in model.Operators)
            //{
            //    skipperlist.Add(new SelectListItem
            //    {
            //        Selected = false,
            //        Text = ip.OrganisationName,
            //        Value = ip.ID.ToString(),
            //    });
            //}
            model.SkipperList = skipperlist;



            //var operators = new List<OperatorViewModel>();
            //foreach (Operator oper in sheet.Operators)
            //{
            //    operators.Add(OperatorViewModel.FromEntity(oper));
            //}
            //model.Operators = operators;

            var claims = new List<ClaimViewModel>();
            for (var i = 0; i < sheet.ClaimNotifications.Count; i++)
            {
                claims.Add(ClaimViewModel.FromEntity(sheet.ClaimNotifications.ElementAtOrDefault(i)));
            }

            //foreach (Claim cl in sheet.Claims)
            //{
            //    claims.Add(ClaimViewModel.FromEntity(cl));
            //}
            model.Claims = claims;

            var interestedParties = new List<OrganisationViewModel>();
            try
            {
                foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Where(ia => ia.InsuranceAttributeName == "Financial" || ia.InsuranceAttributeName == "Private" || ia.InsuranceAttributeName == "CoOwner"))
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
            for (var i = 0; i < sheet.BoatUses.Count(); i++)
            {
                boatUses.Add(BoatUseViewModel.FromEntity(sheet.BoatUses.ElementAtOrDefault(i)));

            }

            List<SelectListItem> list = new List<SelectListItem>();

            try
            {
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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
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


            //foreach (OrganisationalUnit ou in sheet.Owner.OrganisationalUnits)
            //{
            //    organisationalUnits.Add(new OrganisationalUnitViewModel
            //    {
            //        OrganisationalUnitId = ou.Id,
            //        Name = ou.Name
            //    });
            //    model.OrganisationalUnitsVM.OrganisationalUnits.Add(new SelectListItem { Text = ou.Name, Value = ou.Id.ToString() });
            //}

            for (var i = 0; i < sheet.Locations.Count(); i++)
            {
                locations.Add(LocationViewModel.FromEntity(sheet.Locations.ElementAtOrDefault(i)));
            }

            //foreach (Location loc in sheet.Locations)
            //{
            //    locations.Add(LocationViewModel.FromEntity(loc));
            //}

            for (var i = 0; i < sheet.Buildings.Count(); i++)
            {
                buildings.Add(BuildingViewModel.FromEntity(sheet.Buildings.ElementAtOrDefault(i)));

            }

            //foreach (Building bui in sheet.Buildings)
            //{
            //    buildings.Add(BuildingViewModel.FromEntity(bui));
            //}
         try
            {
                //InsuranceAttribute insuranceAttribute = _insuranceAttributeService.GetInsuranceAttributeByName("Skipper");
                //if (insuranceAttribute == null)
                //{
                //    insuranceAttribute = new InsuranceAttribute(CurrentUser(), "Skipper");

                //    _insuranceAttributeService.CreateNewInsuranceAttribute(insuranceAttribute);
                //}

                foreach (InsuranceAttribute IA in _InsuranceAttributesRepository.FindAll().Where(ia => ia.InsuranceAttributeName == "Marina" || ia.InsuranceAttributeName == "Other Marina"))
                {
                    foreach (var org in IA.IAOrganisations)
                    {
                        if ( org.OrganisationType.Name == "Corporation – Limited liability")
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

            for (var i = 0; i < sheet.WaterLocations.Count(); i++)
            {
                waterLocations.Add(WaterLocationViewModel.FromEntity(sheet.WaterLocations.ElementAtOrDefault(i)));
            }


            var availableProducts = new List<ProductItem>();          
            var userDetails = _mapper.Map<UserDetailsVM>(user);
            userDetails.PostalAddress = user.Address;
            userDetails.StreetAddress = user.Address;
            userDetails.FirstName = user.FirstName;
            userDetails.Email = user.Email;
            
            var roles = new List<String>();

            //for (var i = 0; i < user.Groups.Count(); i++)
            //{
            //    roles.Add(user.Groups.ElementAtOrDefault(i).Name);
            //}s

            model.UserRole = roles;

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
            model.AvailableProducts = availableProducts;
            model.OrganisationDetails = organisationDetails;
            model.UserDetails = userDetails;

            var businessActivity = await _businessActivityService.GetBusinessActivitiesByClientProgramme(clientProgramme.BaseProgramme.Id);
            model.BusinessActivities = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(businessActivity);
            model.RevenueByActivity = _mapper.Map<IEnumerable<RevenueByActivityViewModel>>(sheet.RevenueData);
            model.Status = sheet.Status;

            List<ClientInformationAnswer> informationAnswers = await _clientInformationAnswer.GetAllClaimHistory();
            informationAnswers.Where(c => c.ClientInformationSheet.Id == sheet.Id);
            model.ClientInformationAnswers = informationAnswers;

            return View("InformationWizard", model);
        }

        [HttpGet]
        public async Task<IActionResult> Unlock(Guid id)
        {
            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
            ClientInformationSheet sheet = clientProgramme.InformationSheet;
            var user = await CurrentUser();
            if (sheet != null)
            {
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (sheet.Status == "Submitted")
                    {
                        sheet.Status = "Started";
                        sheet.UnlockDate = DateTime.UtcNow;
                        sheet.UnlockedBy = user;
                    }
                    await uow.Commit();

                }
            }

            var url = "/Information/EditInformation/" + id;
            return Redirect(url);
        }

        
        [HttpPost]
        public async Task<IActionResult> SaveInformation(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
            {
                ClientInformationSheet sheet = await _clientInformationService.GetInformation(sheetId);
                if (sheet == null)
                    return Json("Failure");

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    await _clientInformationService.SaveAnswersFor(sheet, collection);
                    await _clientInformationService.UpdateInformation(sheet);
                    await uow.Commit();
                }
            }

            return Json("Success");

        }

        [HttpPost]
        public async Task<IActionResult> GetClaimHistory(Guid ClientInformationSheet)
        {
            String[][] ClaimAnswers = new String[5][];
            var count = 0;
            String[] ClaimItem;
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

        [HttpPost]
        public async Task<IActionResult> UpdateClaim(List<string[]> Claims, Guid ClientInformationSheet)
        {
            ClientInformationSheet sheet = null;

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


        [HttpPost]
        public async Task<IActionResult> SubmitInformation(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;
            var user = await CurrentUser();
            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
            {

                sheet = await _clientInformationService.GetInformation(sheetId);

                var reference = await _referenceService.GetLatestReferenceId();

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (sheet.Status != "Submitted" && sheet.Status != "Bound")
                    {
                        //UWM ICIB
                        _uWMService.UWM_ICIBNZIMV(user, sheet, reference);

                        //sheet.Status = "Submitted";
                        await uow.Commit();
                    }

                }
                foreach (ClientAgreement agreement in sheet.Programme.Agreements)
                {
                    await _referenceService.CreateClientAgreementReference(reference, agreement.Id);
                }
            }

            //return View();
            return Content("/Agreement/ViewAgreement/" + sheet.Programme.Id);
            //return Content (sheet.Id.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> QuoteToAgree(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;
            var user = await CurrentUser();
            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
            {
                sheet = await _clientInformationService.GetInformation(sheetId);
                if (sheet.Status != "Submitted" && sheet.Status != "Bound")
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        sheet.Status = "Submitted";
                        sheet.SubmitDate = DateTime.UtcNow;
                        sheet.SubmittedBy = user;
                        await uow.Commit();
                    }                   

                }
                await _emailService.SendSystemEmailUISSubmissionConfirmationNotify(user, sheet.Programme.BaseProgramme, sheet, sheet.Owner);
                //send out information sheet submission notification email
                await _emailService.SendSystemEmailUISSubmissionNotify(user, sheet.Programme.BaseProgramme, sheet, sheet.Owner);
            }

            return Content("/Agreement/ViewAgreementDeclaration/" + sheet.Programme.Id);

        }

        [HttpPost]
        public async Task<IActionResult> PaymentInformation(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;

            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
            {
                sheet = await _clientInformationService.GetInformation(sheetId);
            }

            return Content("/Agreement/ViewPayment/" + sheet.Programme.Id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInformationReason(ChangeReason changeReason)
        {
            var user = await CurrentUser();
            ChangeReason change = _changeProcessService.CreateChangeReason(user,changeReason);

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
            change.EffectiveDate = DateTime.Parse(LocalizeTime(changeReason.EffectiveDate, "d"));
            _changeReasonRepository.AddAsync(change);
            //change.EffectiveDate = DateTime.Parse(changeReason.EffectiveDate., System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")).ToUniversalTime(tzi);
                       return Redirect("/Information/UpdateInformation/"+changeReason.DealId );
        }

        [HttpGet]
        public async Task<IActionResult> UpdateInformation(Guid id)
        {
            var user = await CurrentUser();
            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
            if (clientProgramme == null)
                throw new Exception("ClientProgramme (" + id + ") doesn't belong to User " + user.UserName);

            ClientProgramme newClientProgramme = await _programmeService.CloneForUpdate(clientProgramme, user);

            await _programmeService.Update(newClientProgramme);

            return Redirect("/Information/EditInformation/" + newClientProgramme.Id);
        }

        [HttpGet]
        public async Task<IActionResult> RenewInformation(Guid id)
        {
            ClientProgramme clientProgramme = await _programmeService.GetClientProgramme(id);
            var user = await CurrentUser();
            if (clientProgramme == null)
                throw new Exception("ClientProgramme (" + id + ") doesn't belong to User " + user.UserName);

            ClientProgramme newClientProgramme = await _programmeService.CloneForRewenal(clientProgramme, user);

            await _programmeService.Update(newClientProgramme);

            return Redirect("/Information/StartInformation/" + newClientProgramme.Id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDemoUIS()
        {
            var demoData = LoadTemplate();
            var user = await CurrentUser();
            List<InformationSection> informationSections = new List<InformationSection>();

            foreach (var section in demoData.Sections)
            {

                // Update section Id in view model
                // See Above temporaraly

                List<InformationItem> items = new List<InformationItem>();

                //Loop through Questions
                foreach (var item in section.Items)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        // Create Information Item


                        if (item != null)
                        {
                            string itemTypeName = Enum.GetName(typeof(ItemType), item.Type);
                            InformationItem newItem = null;

                            switch (item.Type)
                            {
                                case ItemType.TEXTAREA:
                                case ItemType.TEXTBOX:
                                    var textboxItem = await _informationItemService.CreateTextboxItem(user, item.Name, item.Label, item.Width, itemTypeName) as TextboxItem;
                                    items.Add(textboxItem);
                                    item.Id = textboxItem.Id;
                                    break;

                                case ItemType.LABEL:
                                    var labelItem = await _informationItemService.CreateLabelItem(user, item.Name, item.Label, item.Width, itemTypeName) as LabelItem;
                                    items.Add(labelItem);
                                    item.Id = labelItem.Id;
                                    break;

                                case ItemType.PERCENTAGEBREAKDOWN:
                                case ItemType.DROPDOWNLIST:
                                    //Mapper.CreateMap<SelectListItem, DropdownListOption>();
                                    //Mapper.CreateMap<DropdownListOption, SelectListItem>()
                                    var options = _mapper.Map<IList<DropdownListOption>>(item.Options);
                                    var newDropdownList = await _informationItemService.CreateDropdownListItem(user, item.Name, item.Label, item.DefaultText, options, item.Width, itemTypeName) as DropdownListItem;
                                    //newDropdownList.AddItems(options);
                                    items.Add(newDropdownList);
                                    item.Id = newDropdownList.Id;
                                    break;

                                case ItemType.MULTISELECT:
                                    options = _mapper.Map<IList<DropdownListOption>>(item.Options);
                                    var multiselectList = await _informationItemService.CreateMultiselectListItem(user, item.Name, item.Label, item.DefaultText, options, item.Width, itemTypeName) as MultiselectListItem;
                                    items.Add(multiselectList);
                                    item.Id = multiselectList.Id;
                                    break;

                                case ItemType.JSBUTTON:
                                    newItem = await _informationItemService.CreateJSButtonItem(user, item.Name, item.Label, item.Width, itemTypeName, item.Value) as JSButtonItem;
                                    break;

                                case ItemType.SUBMITBUTTON:
                                    newItem = await _informationItemService.CreateSubmitButtonItem(user, item.Name, item.Label, item.Width, itemTypeName) as SubmitButtonItem;
                                    break;

                                //case ItemType.SECTIONBREAK:
                                //    var terminatorItem = await _informationItemService.CreateSectionBreakItem(CurrentUser, itemTypeName);
                                //    items.Add(terminatorItem);
                                //    break;


                                case ItemType.STATICVEHICLEPLANTLIST:
                                case ItemType.MOTORVEHICLELIST:
                                    var motorVehicleListItem = await _informationItemService.CreateMotorVehicleListItem(user, item.Name, item.Label, item.Width, itemTypeName) as MotorVehicleListItem;
                                    items.Add(motorVehicleListItem);
                                    item.Id = motorVehicleListItem.Id;
                                    break;

                                default:
                                    newItem = null;
                                    break;
                            }

                            if (newItem != null)
                            {
                                items.Add(newItem);
                                item.Id = newItem.Id;
                                newItem = null;
                            }


                            // Update Inforamtion Item ID in view model
                            // For now see code above, Will fix later with Domain Model
                        }
                        await uow.Commit();
                    }

                }

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    // Create New Section

                    InformationSection informationSection = await _informationSectionService.CreateNewSection(user, section.Name, items);
                    informationSection.CustomView = section.CustomView;
                    informationSections.Add(informationSection);

                    section.Id = informationSection.Id;
                    await uow.Commit();
                }

            }

            using (var uow = _unitOfWork.BeginUnitOfWork())
            {
                InformationTemplate template = await _informationTemplateService.CreateInformationTemplate(user, demoData.Name, informationSections);

                demoData.Id = template.Id;

                await uow.Commit();
            }

            return Json(demoData, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<IActionResult> IssueDemoUIS(string id)
        {
            var user = await CurrentUser();
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

        [HttpGet]
        public async Task<IActionResult> IssueUISForProduct(Guid id)
        {
            InformationTemplate infoTemplate = await _informationTemplateService.GetTemplate(id);
            if (infoTemplate == null)
                throw new Exception("Insurance Product " + id + " lacks question set");

            ClientInformationSheet cis = null;
            using (var uow = _unitOfWork.BeginUnitOfWork())
            {
                var user = await CurrentUser();
                cis = await _clientInformationService.IssueInformationFor(user, user.PrimaryOrganisation, infoTemplate);
                await uow.Commit();
            }

            return Redirect("/Information/StartInformation/" + cis.Id);
        }

        public async Task<InformationViewModel> GetInformationViewModel(Guid programmeId)
        {
            Programme programme = await _programmeService.GetProgramme(programmeId);

            InformationViewModel model = new InformationViewModel
            {
                Name = programme.Name,
                Sections = new List<InformationSectionViewModel>()
            };
            model.Name = programme.Name;
            Product product = programme.Products.FirstOrDefault();
            InformationTemplate informationTemplate = await _informationTemplateService.GetTemplatebyProduct(product.Id);
            
            List<InformationSection> sections = await _informationSectionService.GetInformationSectionsbyTemplateId(informationTemplate.Id);
            foreach (var section in sections)
            {
                section.Items = section.Items.OrderBy(i => i.ItemOrder).ToList();
            }

            //foreach (InformationTemplate template in templates)
            //{
            //    Console.WriteLine(template.Id);
            foreach (var section in informationTemplate.Sections)
            {
                section.Items = section.Items.OrderBy(i => i.ItemOrder).ToList();
            }
               (model.Sections as List<InformationSectionViewModel>).InsertRange(model.Sections.Count(), _mapper.Map<InformationViewModel>(informationTemplate).Sections);


            //}
            model.Section = sections;
            return model;
        }

        public async Task<InformationViewModel> GetClientInformationSheetViewModel(Guid sheetId)
        {
            ClientInformationSheet sheet = await _clientInformationService.GetInformation(sheetId);

            InformationViewModel model = await GetInformationViewModel(sheet.Programme.Id);
            model.Sections = model.Sections.OrderBy(sec => sec.Position);

            model.Status = sheet.Status;
            model.AnswerSheetId = sheet.Id;

            model.OrganisationId = sheet.Owner.Id;

            foreach (var section in model.Sections)
                foreach (var item in section.Items)
                {
                    var answer = sheet.Answers.FirstOrDefault(a => a.ItemName == item.Name);
                    if (answer != null)
                        item.Value = answer.Value;
                }

            return model;
        }

        IEnumerable<InformationSectionViewModel> GetCommonPanels()
        {
            List<InformationSectionViewModel> sections = new List<InformationSectionViewModel>
            {

            };
            return sections;
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
