using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using AutoMapper;
using TechCertain.Services.Interfaces;
using SystemDocument = TechCertain.Domain.Entities.Document;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Programme;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.WebUI.Models.Product;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;

namespace TechCertain.WebUI.Controllers
{
    //[Authorize]
    public class ProgrammeController : BaseController
    {        
        IInformationTemplateService _informationService;
        IUnitOfWork _unitOfWork;

        IMapperSession<Programme> _programmeRepository;
        IMapperSession<Product> _productRepository;
        IMapperSession<RiskCategory> _riskRepository;
        IMapperSession<RiskCover> _riskCoverRepository;
        IMapperSession<Organisation> _organisationRepository;
        IBusinessActivityService _busActivityService;
        IMapperSession<Document> _documentRepository;
        IProgrammeService _programmeService;
        IFileService _fileService;
        IEmailService _emailService;
        IRuleService _RuleService;
        IRoleService _roleService;
        IMapper _mapper;
        IHttpContextAccessor _httpContextAccessor;

        public ProgrammeController(DealEngineDBContext dealEngineDBContext, ISignInManager<DealEngineUser> signInManager, IUserService userRepository, IHttpContextAccessor httpContextAccessor, IInformationTemplateService informationService,
                                 IUnitOfWork unitOfWork, IMapperSession<Product> productRepository, IMapperSession<RiskCategory> riskRepository,
                                 IMapperSession<RiskCover> riskCoverRepository, IMapperSession<Organisation> organisationRepository, IRoleService roleService,
                                 IRuleService ruleService, IMapperSession<Document> documentRepository, IMapperSession<Programme> programmeRepository, IBusinessActivityService busActivityService,
                                 IProgrammeService programmeService, IFileService fileService, IEmailService emailService, IMapper mapper)
            : base(userRepository, dealEngineDBContext, signInManager, httpContextAccessor)
        {            
            _informationService = informationService;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _riskRepository = riskRepository;
            _riskCoverRepository = riskCoverRepository;
            _roleService = roleService;
            _organisationRepository = organisationRepository;
            _busActivityService = busActivityService;
            _documentRepository = documentRepository;
            _programmeRepository = programmeRepository;
            _programmeService = programmeService;
            _unitOfWork = unitOfWork;
            _RuleService = ruleService;
            _fileService = fileService;
            _emailService = emailService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult MyProgrammes()
        {
            try {
                var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser.PrimaryOrganisation);
                var userRoles = CurrentUser.GetRoles().ToArray();
                var hasViewAllRole = userRoles.FirstOrDefault(r => r.Name == "CanViewAllInformation") != null;
                if (hasViewAllRole) {
                    programmes = _programmeRepository.FindAll();
                }
                BaseListViewModel<ProgrammeInfoViewModel> models = new BaseListViewModel<ProgrammeInfoViewModel>();
                foreach (Programme programme in programmes) {
                    ProgrammeInfoViewModel model = new ProgrammeInfoViewModel
                    {
                        DateCreated = LocalizeTime(programme.DateCreated.GetValueOrDefault()),
                        //LocalDateSubmitted = LocalizeTime(programme.DateDeleted.GetValueOrDefault()),
                        //Status= programme.InformationSheet.Status,
                        Id = programme.Id,
                        Name = programme.Name + " for " + programme.Owner.Name,
                        OwnerCompany = programme.Owner.Name,
                    };
                    models.Add(model);

                }
                return View("AllProgrammes", models);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return View("AllProgrammes");
        }

        [HttpGet]
        public ActionResult AllProgrammes()
        {
            try {
                var programmes = _programmeRepository.FindAll().Where(p => p.DateDeleted == null);
                BaseListViewModel<ProgrammeInfoViewModel> models = new BaseListViewModel<ProgrammeInfoViewModel>();
                foreach (Programme p in programmes) {
                    ProgrammeInfoViewModel model = new ProgrammeInfoViewModel
                    {
                        DateCreated = LocalizeTime(p.DateCreated.GetValueOrDefault()),
                        Id = p.Id,
                        Name = p.Name,
                        OwnerCompany = p.Owner.Name,
                    };
                    // ClientProgramme programme = _programmeService.GetClientProgrammesForProgramme(Id);

                    models.Add(model);
                }
                return View(models);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return View();
        }

        [HttpGet]
        public ActionResult ManageClient(Guid Id)
        {
            //BaseListViewModel<ProgrammeInfoViewModel> model = new BaseListViewModel<ProgrammeInfoViewModel>();
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();

            List<ClientProgramme> clientProgrammes = new List<ClientProgramme>();
            List<Organisation> Owners = new List<Organisation>();
            List<Organisation> Ownerlist = new List<Organisation>();

            try
            {
                // ClientProgramme programme = _programmeService.GetClientProgrammesForProgramme(Id);
                foreach (var programme in _programmeService.GetClientProgrammesForProgramme(Id))
                {
                    Ownerlist.Add(programme.Owner);
                    clientProgrammes.Add(programme);
                    model.programmeName = programme.BaseProgramme.Name;

                }
                Ownerlist.Select(x => x.Name).Distinct();

                foreach (var owner in Ownerlist)
                {
                    Owners.Add(owner);
                }
                //Notes.Select(x => x.Author).Distinct();
                model.Id = Id;
                model.Owner = Owners;
                model.clientProgrammes = clientProgrammes;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Term Sheet Template ";
            return View(model);

        }

        //throw new Exception("Method will need to be re-written");
        //[HttpPost]
        //public ActionResult UploadDataFiles(HttpPostedFileWrapper uploadedBusinessActivityData)
        //{
        //    byte[] buffer;
        //    IList<BusinessActivity> BAList = new List<BusinessActivity>();
        //    if (uploadedBusinessActivityData != null)
        //    {
        //        buffer = new byte[uploadedBusinessActivityData.ContentLength];
        //        uploadedBusinessActivityData.InputStream.Read(buffer, 0, buffer.Length);
        //        string lines = _fileService.FromBytes(buffer);
        //        using (System.IO.StringReader reader = new System.IO.StringReader(lines))
        //        {
        //            string line = string.Empty;
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                string[] parts = line.Split(',');
        //                BusinessActivity ba = new BusinessActivity(CurrentUser);

        //                if (!string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]))
        //                {
        //                    //classification 1
        //                    ba.Classification = 1;
        //                    ba.AnzsciCode = parts[0];
        //                    ba.Description = parts[1];
        //                    Debug.WriteLine(parts[0]);
        //                    Debug.WriteLine(parts[1]);

        //                }
        //                if (!string.IsNullOrEmpty(parts[1]) && !string.IsNullOrEmpty(parts[2]))
        //                {
        //                    //classification 2
        //                    ba.Classification = 2;
        //                    ba.AnzsciCode = parts[1];
        //                    ba.Description = parts[2];
        //                    Debug.WriteLine(parts[1]);
        //                    Debug.WriteLine(parts[2]);
        //                }
        //                if (!string.IsNullOrEmpty(parts[2]) && !string.IsNullOrEmpty(parts[3]))
        //                {
        //                    //classification 3
        //                    ba.Classification = 3;
        //                    ba.AnzsciCode = parts[2];
        //                    ba.Description = parts[3];
        //                    Debug.WriteLine(parts[2]);
        //                    Debug.WriteLine(parts[3]);

        //                }
        //                if (!string.IsNullOrEmpty(parts[3]) && !string.IsNullOrEmpty(parts[4]))
        //                {
        //                    //classification 4
        //                    ba.Classification = 4;
        //                    ba.AnzsciCode = parts[3];
        //                    ba.Description = parts[4];
        //                    Debug.WriteLine(parts[3]);
        //                    Debug.WriteLine(parts[4]);
        //                }

        //                BAList.Add(ba);
        //            }
        //        }

        //        foreach (BusinessActivity businessActivity in BAList)
        //        {
        //            _busActivityService.CreateBusinessActivity(businessActivity);
        //        }
        //    }

        //    return Redirect("/Programme/ActivityBuilder");
        //}

        [HttpPost]
        public ActionResult CreateProgrammeActivities(ActivityViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("", "Form has not been completed");
            //    throw new Exception("Form has not been completed");
            //}

            try
            {
                Programme programme = _programmeService.GetProgramme(Guid.Parse(model.ActivityAttach.SelectedProgramme[0]));
                foreach (string str in model.Builder.SelectedActivities)
                {
                    BusinessActivity businessActivity = _busActivityService.GetBusinessActivity(Guid.Parse(str));                    
                    _busActivityService.AttachClientProgrammeToActivities(programme, businessActivity);
                }
          
                return Redirect("~/Programme/ActivityBuilder");

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ActivityBuilder()
        {
            ActivityViewModel model = new ActivityViewModel
            {
                Builder = new ActivityBuilderVM
                {
                    Ispublic = false,
                    //Activities = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityService.GetBusinessActivities()),
                    Activities = new List <SelectListItem>(),
                    Level1Classifications = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityService.GetBusinessActivitiesByClassification(1)),
                    Level2Classifications = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityService.GetBusinessActivitiesByClassification(2)),
                    Level3Classifications = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityService.GetBusinessActivitiesByClassification(3)),
                    Level4Classifications = _mapper.Map<IEnumerable<BusinessActivityViewModel>>(_busActivityService.GetBusinessActivitiesByClassification(4)),
                },
                ActivityCreate = new ActivityModal()
            };

            var classification = _busActivityService.GetBusinessActivities().GroupBy(ba => ba.Classification);
            foreach (var group in classification)
            {
                var optionGroup = new SelectListGroup() { Name = group.Key.ToString() };
                foreach (var item in group)
                {
                    model.Builder.Activities.Add(new SelectListItem()
                    {
                        Value = item.Id.ToString(),
                        Text = item.AnzsciCode + " --- " + item.Description,
                        Group = optionGroup,

                    });
                }
            }

            List<SelectListItem> proglist = new List<SelectListItem>();
            foreach (Programme programme in _programmeRepository.FindAll().Where(p => p.IsPublic == true || p.Owner.Id == CurrentUser.PrimaryOrganisation.Id))
            {
                proglist.Add(new SelectListItem
                {
                    Selected = false,
                    Text = programme.Name,
                    Value = programme.Id.ToString(),
                });

            }
           
            model.ActivityAttach = new ActivityAttachVM()
            {
                BaseProgList = proglist
            };
        
            return View("ActivityBuilder", model);
        }

        [HttpPost]
        public ActionResult CreateBusinessActivity(ActivityModal model)
        {
            //TODO: tidy up code use a list to loop through model 
            IList<BusinessActivity> BAList = new List<BusinessActivity>();
            if (model.ClassOne != null)
            {
                BusinessActivity ba1 = new BusinessActivity(CurrentUser)
                {
                    AnzsciCode = model.ClassOne.AnzsciCode,
                    Description = model.ClassOne.Description,
                    Classification = model.ClassOne.Classification
                };
                BAList.Add(ba1);
            }

            if (model.ClassTwo != null)
            {
                BusinessActivity ba2 = new BusinessActivity(CurrentUser)
                {
                    AnzsciCode = model.ClassTwo.AnzsciCode,
                    Description = model.ClassTwo.Description,
                    Classification = model.ClassTwo.Classification
                };
                BAList.Add(ba2);
            }

            if (model.ClassThree != null)
            {
                BusinessActivity ba3 = new BusinessActivity(CurrentUser)
                {
                    AnzsciCode = model.ClassThree.AnzsciCode,
                    Description = model.ClassThree.Description,
                    Classification = model.ClassThree.Classification
                };
                BAList.Add(ba3);
            }

            if (model.ClassFour != null)
            {
                BusinessActivity ba4 = new BusinessActivity(CurrentUser)
                {
                    AnzsciCode = model.ClassFour.AnzsciCode,
                    Description = model.ClassFour.Description,
                    Classification = model.ClassFour.Classification
                };
                BAList.Add(ba4);
            }

            foreach (BusinessActivity businessActivity in BAList)
            {
                _busActivityService.CreateBusinessActivity(businessActivity);
            }

            return Redirect("/Programme/ActivityBuilder");
        }

        [HttpPost]
        public ActionResult CreateRole(string title)
        {
            _roleService.CreateRole(title);
            return Redirect("/Programme/RoleBuilder");
        }

        [HttpPost]
        public ActionResult CreateProgrammeRoles(RoleViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("", "Form has not been completed");
            //    throw new Exception("Form has not been completed");
            //}

            try
            {
                Programme programme = _programmeService.GetProgramme(Guid.Parse(model.RoleAttach.SelectedProgramme[0]));
                foreach (string str in model.Builder.SelectedRoles)
                {
                    Role role = _roleService.GetRole(Guid.Parse(str));
                    _roleService.AttachClientProgrammeToRole(programme, role);
                }

                return Redirect("~/Programme/ActivityBuilder");

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult RoleBuilder()
        {
            List<SelectListItem> proglist = new List<SelectListItem>();
            foreach (Programme programme in _programmeRepository.FindAll().Where(p => p.IsPublic == true || p.Owner.Id == CurrentUser.PrimaryOrganisation.Id))
            {
                proglist.Add(new SelectListItem
                {
                    Selected = false,
                    Text = programme.Name,
                    Value = programme.Id.ToString(),
                });

            }

            List<SelectListItem> roleList = new List<SelectListItem>();
            var roles = _roleService.GetRoles();
            foreach (Role role in roles)
            {
                roleList.Add(new SelectListItem
                {
                    Selected = false,
                    Text = role.Title,
                    Value = role.Id.ToString(),
                });

            }

            RoleViewModel model = new RoleViewModel
            {
                Builder = new RoleBuilderVM()
                {
                    Roles = roleList,
                },
                RoleAttach = new RoleAttachVM()
                {
                    BaseProgList = proglist,
                }

            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SendInvoice(Guid programmeId)
        {
            ClientProgramme programme = _programmeService.GetClientProgramme(programmeId);
            if (programme.EGlobalClientNumber == null)
            {
                throw new NullReferenceException("Client number is null");
            }

            var status = "Bound and invoiced";

            EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");
            if(emailTemplate == null)
            {
                throw new NullReferenceException("send policy documents template email not set up");
            }

            foreach (ClientAgreement agreement in programme.Agreements)
            {
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (agreement.Status != status)
                    {
                        agreement.Status = status;
                        uow.Commit();
                    }
                }

                var documents = new List<SystemDocument>();

                var invoiceDoc = agreement.Documents.FirstOrDefault(d => d.DateDeleted == null && d.DocumentType == 4);
                if (invoiceDoc != null)
                {
                    
                    SystemDocument renderedDoc = _fileService.RenderDocument(CurrentUser, invoiceDoc, agreement);
                    renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                    documents.Add(renderedDoc);
                    _emailService.SendEmailViaEmailTemplate(programme.BrokerContactUser.Email, emailTemplate, documents);
                    _emailService.SendSystemSuccessInvoiceConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                }    
                else
                    throw new NullReferenceException("No Invoice file");
                
            }
            return Redirect("/EditBillingConfiguration" + programmeId);
        }

        [HttpGet]
        public ActionResult OwnerProgrammes(Guid ownerId, Guid Id)
        {
            //BaseListViewModel<ProgrammeInfoViewModel> model = new BaseListViewModel<ProgrammeInfoViewModel>();
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();

            //ClientProgramme clientprogramme = new ClientProgramme();
            List<ClientProgramme> clientProgrammes = new List<ClientProgramme>();
            List<Organisation> Owners = new List<Organisation>();

            try
            {
                foreach (var programme in _programmeService.GetClientProgrammesByOwner(ownerId))
                {
                    clientProgrammes.Add(programme);

                }

                model.OwnerId = ownerId;
                model.Id = Id;
                model.clientProgrammes = clientProgrammes;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Term Sheet Template ";
            return View(model);

        }



        [HttpGet]
        public ActionResult ClientProgrammeDetails(Guid programmeId, Guid Id,Guid ownerId)
        {
            //BaseListViewModel<ProgrammeInfoViewModel> model = new BaseListViewModel<ProgrammeInfoViewModel>();
          //  ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            ClientProgrammeInfoViewModel clientviewmodel = new ClientProgrammeInfoViewModel();
            //ClientProgramme clientprogramme = new ClientProgramme();
            List<ClientProgramme> clientProgrammes = new List<ClientProgramme>();
            //List<Organisation> Owners = new List<Organisation>();ProgramId
            clientviewmodel.Id = Id;
            clientviewmodel.OwnerId = ownerId;
            clientviewmodel.ProgramId = programmeId;

            try
            {
                ClientProgramme programme = _programmeService.GetClientProgramme(programmeId);
                //foreach (var owner in programme)
                //{
                clientviewmodel.Name = programme.Owner.Name;
                clientviewmodel.Phone = programme.Owner.Phone;
                clientviewmodel.Email = programme.Owner.Email;
                clientviewmodel.DateCreated = (DateTime)programme.Owner.DateCreated;
                clientviewmodel.Status = programme.InformationSheet.Status;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Term Sheet Template ";
            return View(clientviewmodel);

        }

        [HttpGet]
        public ActionResult EditBillingConfiguration(Guid programmeId)
        {
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            ClientProgramme programme = _programmeService.GetClientProgramme(programmeId);
            model.Id = programme.Id;
            model.BrokerContactUser = programme.BrokerContactUser;
            model.EGlobalBranchCode = programme.EGlobalBranchCode;
            model.EGlobalClientNumber = programme.EGlobalClientNumber;
            model.EGlobalClientStatus = programme.EGlobalClientStatus;
            model.HasEGlobalCustomDescription = programme.HasEGlobalCustomDescription;
            model.EGlobalCustomDescription = programme.EGlobalCustomDescription;

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveBillingConfiguration(string[] billingConfig, Guid programmeId)
        {
            ClientProgramme programme = _programmeService.GetClientProgramme(programmeId);
            programme.EGlobalBranchCode = billingConfig[0];
            programme.EGlobalClientNumber = billingConfig[1];
            programme.EGlobalClientStatus = billingConfig[2];
            if (billingConfig[3] != "")
            {
                programme.HasEGlobalCustomDescription = true;
                programme.EGlobalCustomDescription = billingConfig[3];
            }
            else
            {
                programme.HasEGlobalCustomDescription = false;
                programme.EGlobalCustomDescription = null;
            }
                    
            _programmeService.Update(programme);

            return Redirect("/EditBillingConfiguration" + programmeId);
        }

        [HttpGet]
        public ActionResult EditClientProgrammeDetails(Guid programmeId, Guid Id, Guid OwnerId)
        {
            ClientProgrammeInfoViewModel clientviewmodel = new ClientProgrammeInfoViewModel();

            try
            {
                ClientProgramme programme = _programmeService.GetClientProgramme(programmeId);
                //foreach (var owner in programme)
                //{
                clientviewmodel.Name = programme.Owner.Name;
                clientviewmodel.Email = programme.Owner.Email;
                clientviewmodel.Phone = programme.Owner.Phone;
                clientviewmodel.OwnerId = OwnerId;
                clientviewmodel.ProgramId = programmeId;
                clientviewmodel.Id = Id;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Term Sheet Template ";
            return View(clientviewmodel);
        }

        [HttpPost]
        public ActionResult SaveClientProgrammeDetails(Guid programme_id, ClientProgrammeInfoViewModel clientviewmodel, Guid id)
        {
            ClientProgramme programme = _programmeService.GetClientProgramme(programme_id);

            //clientviewmodel.Id = id;
            Guid ownerid = clientviewmodel.OwnerId;
            try
            {

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())

                {
                    var owner = programme.Owner;
                    //owner.Name = clientviewmodel.Name;
                    owner.Phone = clientviewmodel.Phone;
                    owner.Email = clientviewmodel.Email;
                    NewMethod(uow);

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Term Sheet Template ";

            return RedirectToAction("ClientProgrammeDetails", new { programmeId = programme_id, Id= id , ownerId = ownerid});

            //return View("ClientProgrammeDetails",clientviewmodel);
        }


        private static void NewMethod(IUnitOfWork uow)
        {
            uow.Commit();
        }
        //[HttpGet]

        //               using (var uow = _unitOfWork.BeginUnitOfWork())
        //            {
        //                //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
        //                bvTerm.TermLimit = clientAgreementBVTerm.TermLimit;
        //                bvTerm.Excess = clientAgreementBVTerm.Excess;
        //                bvTerm.Premium = clientAgreementBVTerm.Premium;
        //                bvTerm.FSL = clientAgreementBVTerm.FSL;
        //                NewMethod(uow);
        //}
        //            return RedirectToAction("EditTerms", new { id = clientAgreementId });
        //public ActionResult OwnerDetails(List<Organisation> ownerlist, Guid Id)
        //{
        //    //BaseListViewModel<ProgrammeInfoViewModel> model = new BaseListViewModel<ProgrammeInfoViewModel>();
        //    ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();

        //    //ClientProgramme clientprogramme = new ClientProgramme();
        //    //List<ClientProgramme> clientProgrammes = new List<ClientProgramme>();
        //    List<Organisation> Owners = new List<Organisation>();

        //    try
        //    {
        //        // ClientProgramme programme = _programmeService.GetClientProgrammesForProgramme(Id);
        //        foreach (var owner in ownerlist)
        //        {
        //            //Owners.Add(owner.ownerlist);
        //            //model.clientProgrammes.
        //            //clientProgrammes.Add(programme);
        //            //Organisation owner = programme.Owner;
        //        }
        //        model.Id = Id;
        //        model.Owner = Owners;

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);

        //    }
        //    ViewBag.Title = "Proposalonline - Term Sheet Template ";
        //    return View(model);

        //}

       



        [HttpGet]
        public ActionResult EmailTemplate(Guid Id)
        {
            //BaseListViewModel<ProgrammeInfoViewModel> models = new BaseListViewModel<ProgrammeInfoViewModel>();
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();


            try
            {

                model.Id = Id;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            ViewBag.Title = "Programme Email Template ";
            return View(model);

        }

        [HttpGet]
        public ActionResult TermSheetTemplate(Guid Id)
        {
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            try
            {
                model.Id = Id;                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            ViewBag.Title = "Term Sheet Template " ;
            return View(model);
        }

        
        [HttpGet]
        public ActionResult ProductRules(Guid Id, Guid productId)
        {
            Programme programme = _programmeRepository.GetById(Id);

            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            var rules = new List<Rule>();
            model.Id = Id;
            model.ProductId = productId;
            var product = _productRepository.GetById(productId);
            
            foreach (var rule in product.Rules)
            {
                rules.Add(rule);
            }
            model.Rules = rules;

            ViewBag.Title = "Manage Product Rules";

            return View("ProductRules", model);
        }

        [HttpPost]
        public ActionResult EditRule(Guid Id , ClientAgreementRuleViewModel rule )
        {
            //Programme programme = _programmeRepository.GetById(programmeId);

            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            var rules = new List<Rule>();
            //model.Id = Id;
            Rule Rule = _RuleService.GetRuleByID(rule.ClientAgreementRuleID);
            if(Rule != null)
            {
                try
                {
                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        Rule.Name = rule.Name;
                        Rule.Description = rule.Description;
                        Rule.OrderNumber = rule.OrderNumber;
                        Rule.Value = rule.Value;
                        uow.Commit();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            model.Rules = rules;

            ViewBag.Title = "Manage Product Rules";
            return Json(model);
            //return RedirectToAction("EditTerms", new { id = Id , productId = ProductId });
        }


        [HttpGet]
        public ActionResult ManageRules(Guid Id)
        {
            Programme programme = _programmeRepository.GetById(Id);

            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            var product = new List<ProductInfoViewModel>();
            model.BrokerContactUser = programme.BrokerContactUser;
            model.Id = Id;
            if(programme.Products != null)
            {
                foreach (var prod in programme.Products)
                {
                    product.Add(new ProductInfoViewModel()
                    {
                        Id = prod.Id,
                        Name = prod.Name
                        
                    });

                }
            }
            model.Product= product;
           
            ViewBag.Title = "Add/Edit Programme Email Template";

            return View("ProgrammeRules", model);
        }

        //[HttpPost]
        //public ActionResult AddselectedParty(Guid Id)
        //{
        //    var orguser = new List<string>();
        //    Programme programme = _programmeRepository.GetById(Id);
        //    ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
        //    model.Id = Id;
        //    model.Parties = programme.Parties;

        //    //model.OrgUser = _organisationRepository.GetById(programme.Parties.);
        //    //var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser.PrimaryOrganisation);

        //    ViewBag.Title = "Add/Edit Programme Email Template";

        //    return View("IssueNotification", model);
        //}


        [HttpPost]
        public ActionResult AddselectedParty(string[] selectedParty,Guid informationId)
        {
            PartyUserViewModel model = new PartyUserViewModel();
            Programme programme = _programmeRepository.GetById(informationId);
            //model.Id = informationId;
            //model.Parties = programme.Parties;
           
            if (programme != null)
            {
                try
                {
                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        foreach (var party in selectedParty)
                        {
                            var user = _userService.GetUserByEmail(party);
                            programme.UISIssueNotifyUsers.Add(user);
                        }
                        uow.Commit();
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return Json(model);
        }


        [HttpPost]
        public ActionResult selectedParty(Guid selectedParty, Guid informationId)
        {
            //PartyUserViewModel model = new PartyUserViewModel();
            PartyUserViewModel model = new PartyUserViewModel();

            Programme programme = _programmeRepository.GetById(informationId);
            //model.Id = informationId;
            //model.Parties = programme.Parties;
            Organisation organisation = _organisationRepository.GetById(selectedParty);
            List<PartyUserViewModel> selectedorg = new List<PartyUserViewModel>();
            if ("organisation" != null)
            {
                try
                {
                    foreach (var ip in organisation.OrganisationalUnits)
                    {
                        foreach (var org in ip.Users)
                        {
                            selectedorg.Add(new PartyUserViewModel()
                            {
                                Name = org.FirstName,
                                Id = org.Id.ToString(),
                                Email =org.Email,
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return Json(selectedorg);
        }


        //[HttpPost]
        //public ActionResult AddselectedParty(Guid selectedParty, Guid informationId)
        //{
        //    Programme programme = _programmeRepository.GetById(informationId);
        //    ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
        //    model.Id = informationId;
        //    model.Parties = programme.Parties;
        //    Organisation organisation = _organisationRepository.GetById(selectedParty);
            
        //    List<SelectListItem> selectedorg = new List<SelectListItem>();
        //    try
        //    {
        //        using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())

        //        {
        //          //  foreach (var ip in organisation.OrganisationalUnits)
        //          //{
        //          //  foreach (var org in ip.Users)
        //          //  {
        //          //      selectedorg.Add(new SelectListItem
        //          //      {
        //          //          Selected = false,
        //          //          Text = org.FirstName,
        //          //          Value = org.Id.ToString(),
        //          //      });
        //          //  }
        //          //}
        //          //   model.OrgUser = selectedorg;
                    
        //            NewMethod(uow);

        //        }


        //        ////model.OrgUser = _organisationRepository.GetById(programme.Parties.);
        //        ////var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser.PrimaryOrganisation);
        //        //using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
        //        //{
        //        //    uow.Commit();
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
            
        //    return Json(model);
        //}


        [HttpGet]
        public ActionResult IssueNotification(Guid Id)
        {
            var orguser = new List<string>();
            Programme programme = _programmeRepository.GetById(Id);
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            model.Id = Id;
            model.Parties = programme.Parties;

            //model.OrgUser = _organisationRepository.GetById(programme.Parties.);
            //var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser.PrimaryOrganisation);

            ViewBag.Title = "Add/Edit Programme Email Template";

            return View("IssueNotification", model);
        }


        [HttpGet]
        public ActionResult SendEmailTemplates(Guid Id, String type,String description)
        {
            Programme programme = _programmeRepository.GetById(Id);
           
            EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == type);
            
            EmailTemplateViewModel model = new EmailTemplateViewModel();

            model.Description = description;
            model.BaseProgrammeID = Id;
            model.Type = type;

            if (emailTemplate != null)
            {
                model.Name = emailTemplate.Name;
                model.Subject = emailTemplate.Subject;
                model.Body = System.Net.WebUtility.HtmlDecode(emailTemplate.Body);
                
            }
            else
            {
                model.Name = "";
                model.Subject = "";
                model.Body = "";
            }

            ViewBag.Title = "Add/Edit Programme Email Template";

            return View("SendEmailTemplates", model);
        }

        [HttpPost]
        public ActionResult SendEmailTemplates(EmailTemplateViewModel model)
        {
            Programme programme = _programmeRepository.GetById(model.BaseProgrammeID);

            EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == model.Type);

            string emailtemplatename = null;

            switch (model.Type)
            {
                case "SendSchemeEmail":
                    {
                        emailtemplatename = "Scheme Email";
                        break;
                    }
                case "SendInformationSheetInstruction":
                    {
                        emailtemplatename = "Information Sheet Instruction";
                        break;
                    }
                case "SendInformationSheetReminder":
                    {
                        emailtemplatename = "Information Sheet Reminder";
                        break;
                    }
                case "SendInformationSheetRenewalInstruction":
                    {
                        emailtemplatename = "Information Sheet Instructions For Renewals";
                        break;
                    }
                case "SendPolicyDocuments":
                    {
                        emailtemplatename = "Agreement Policy Documents Covering Text";
                        break;
                    }
                case "SendQuoteDocuments":
                    {
                        emailtemplatename = "Agreement Quote Documents Covering Text";
                        break;
                    }
                case "SendAgreementOnlineAcceptanceInstructions":
                    {
                        emailtemplatename = "Agreement Online Acceptance Instructions";
                        break;
                    }
                case "ResendPolicyDocuments":
                    {
                        emailtemplatename = "Agreement Policy Documents Resend Covering Text";
                        break;
                    }
                case "SendAgreementAcceptanceConfirmation":
                    {
                        emailtemplatename = "Agreement Acceptance Confirmation";
                        break;
                    }
                case "SendOnlinePaymentInstructions":
                    {
                        emailtemplatename = "Online Payment Instructions";
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Invalid Email Template Type for Programme ID: ", model.BaseProgrammeID));
                    }
            }

            if (emailTemplate != null)
            {
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    emailTemplate.Subject = model.Subject;
                    emailTemplate.Body = model.Body;
                    emailTemplate.LastModifiedBy = CurrentUser;
                    emailTemplate.LastModifiedOn = DateTime.UtcNow;

                    uow.Commit();
                }
            }
            else
            {
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    emailTemplate = new EmailTemplate(CurrentUser, emailtemplatename, model.Type, model.Subject, model.Body, null, programme);
                    programme.EmailTemplates.Add(emailTemplate);
                    uow.Commit();
                }
            }

            return RedirectToAction("SendEmailTemplates", new { Id = programme.Id, type = model.Type, description = model.Description });

        }

        /*[HttpGet]
		public ActionResult ViewProgramme (Guid id)
		{
			ProductViewModel model = new ProductViewModel ();
			Product product = _productRepository.GetById (id);
			if (product != null) {
				model.Description = new ProductDescriptionVM {
					DateCreated = LocalizeTime (product.DateCreated.GetValueOrDefault ()),
					Description = product.Description,
					Name = product.Name,
					SelectedLanguages = product.Languages.ToArray (),
					Public = product.Public
				};
				model.Risks = new ProductRisksVM ();
				foreach (RiskCover risk in product.RiskCategoriesCovered)
					model.Risks.Add (new RiskEntityViewModel { Insured = risk.BaseRisk.Name, CoverAll = risk.CoverAll, CoverLoss = risk.Loss, 
							CoverInterruption = risk.Interuption, CoverThirdParty = risk.ThirdParty });
			}
			return View (model);
		}*/


    }
}
