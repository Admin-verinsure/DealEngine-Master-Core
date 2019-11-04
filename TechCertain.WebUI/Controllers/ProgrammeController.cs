using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using AutoMapper;
using TechCertain.Services.Interfaces;
using SystemDocument = TechCertain.Domain.Entities.Document;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Programme;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.WebUI.Models.Product;
using System.Threading.Tasks;
using TechCertain.Infrastructure.Payment.EGlobalAPI;

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
        IMapper _mapper;
        IHttpClientService _httpClientService;

        public ProgrammeController(IUserService userRepository, IInformationTemplateService informationService,
                                 IUnitOfWork unitOfWork, IMapperSession<Product> productRepository, IMapperSession<RiskCategory> riskRepository,
                                 IMapperSession<RiskCover> riskCoverRepository, IMapperSession<Organisation> organisationRepository, IRuleService ruleService, IMapperSession<Document> documentRepository,
                                 IMapperSession<Programme> programmeRepository, IBusinessActivityService busActivityService,
                                 IProgrammeService programmeService, IFileService fileService, IEmailService emailService, IMapper mapper, IHttpClientService httpClientService)
            : base (userRepository)
        {            
            _informationService = informationService;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _riskRepository = riskRepository;
            _riskCoverRepository = riskCoverRepository;
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
            _httpClientService = httpClientService;
        }

        [HttpGet]
        public async Task<IActionResult> MyProgrammes()
        {
            try {
                var user = await CurrentUser();
                var programmes = _programmeRepository.FindAll().Where(p => p.Owner == user.PrimaryOrganisation);
                var userRoles = user.GetRoles().ToArray();
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
        public async Task<IActionResult> AllProgrammes()
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
        public async Task<IActionResult> ManageClient(Guid Id)
        {
            //BaseListViewModel<ProgrammeInfoViewModel> model = new BaseListViewModel<ProgrammeInfoViewModel>();
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();

            List<ClientProgramme> clientProgrammes = new List<ClientProgramme>();
            List<Organisation> Owners = new List<Organisation>();
            List<Organisation> Ownerlist = new List<Organisation>();

            try
            {
                // ClientProgramme programme = _programmeService.GetClientProgrammesForProgramme(Id);
                var clientProgrammeList = await _programmeService.GetClientProgrammesForProgramme(Id);
                foreach (var programme in clientProgrammeList)
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
        //public async Task<IActionResult> UploadDataFiles(HttpPostedFileWrapper uploadedBusinessActivityData)
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
        //                BusinessActivity ba = new BusinessActivity(CurrentUser());

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
        public async Task<IActionResult> CreateProgrammeActivities(ActivityViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("", "Form has not been completed");
            //    throw new Exception("Form has not been completed");
            //}

            try
            {
                Programme programme = await _programmeService.GetProgramme(Guid.Parse(model.ActivityAttach.SelectedProgramme[0]));
                foreach (string str in model.Builder.SelectedActivities)
                {
                    BusinessActivity businessActivity = await _busActivityService.GetBusinessActivity(Guid.Parse(str));                    
                    await _busActivityService.AttachClientProgrammeToActivities(programme, businessActivity);
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
        public async Task<IActionResult> ActivityBuilder()
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
            var user = await CurrentUser();
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
            foreach (Programme programme in _programmeRepository.FindAll().Where(p => p.IsPublic == true || p.Owner.Id == user.PrimaryOrganisation.Id))
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
        public async Task<IActionResult> CreateBusinessActivity(ActivityModal model)
        {
            //TODO: tidy up code use a list to loop through model 
            var user = await CurrentUser();
            IList<BusinessActivity> BAList = new List<BusinessActivity>();
            if (model.ClassOne != null)
            {
                BusinessActivity ba1 = new BusinessActivity(user)
                {
                    AnzsciCode = model.ClassOne.AnzsciCode,
                    Description = model.ClassOne.Description,
                    Classification = model.ClassOne.Classification
                };
                BAList.Add(ba1);
            }

            if (model.ClassTwo != null)
            {
                BusinessActivity ba2 = new BusinessActivity(user)
                {
                    AnzsciCode = model.ClassTwo.AnzsciCode,
                    Description = model.ClassTwo.Description,
                    Classification = model.ClassTwo.Classification
                };
                BAList.Add(ba2);
            }

            if (model.ClassThree != null)
            {
                BusinessActivity ba3 = new BusinessActivity(user)
                {
                    AnzsciCode = model.ClassThree.AnzsciCode,
                    Description = model.ClassThree.Description,
                    Classification = model.ClassThree.Classification
                };
                BAList.Add(ba3);
            }

            if (model.ClassFour != null)
            {
                BusinessActivity ba4 = new BusinessActivity(user)
                {
                    AnzsciCode = model.ClassFour.AnzsciCode,
                    Description = model.ClassFour.Description,
                    Classification = model.ClassFour.Classification
                };
                BAList.Add(ba4);
            }

            foreach (BusinessActivity businessActivity in BAList)
            {
               await _busActivityService.CreateBusinessActivity(businessActivity);
            }

            return Redirect("/Programme/ActivityBuilder");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string title)
        {
            //_roleService.CreateRole(title);
            return Redirect("/Programme/RoleBuilder");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProgrammeRoles(RoleViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("", "Form has not been completed");
            //    throw new Exception("Form has not been completed");
            //}

            try
            {
                Programme programme = await _programmeService.GetProgramme(Guid.Parse(model.RoleAttach.SelectedProgramme[0]));
                foreach (string str in model.Builder.SelectedRoles)
                {
                    //Role role = _roleService.GetRole(Guid.Parse(str));
                    //_roleService.AttachClientProgrammeToRole(programme, role);
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

        [HttpPost]
        public async Task<IActionResult> SendInvoice(Guid programmeId)
        {
            ClientProgramme programme = await _programmeService.GetClientProgramme(programmeId);
            if (programme.EGlobalClientNumber == null)
            {
                throw new NullReferenceException("Client number is null");
            }
            var user = await CurrentUser();

            //var status = "Bound and invoiced";

            //EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");
            //if(emailTemplate == null)
            //{
            //    throw new NullReferenceException("send policy documents template email not set up");
            //}

            //foreach (ClientAgreement agreement in programme.Agreements)
            //{
            //    using (var uow = _unitOfWork.BeginUnitOfWork())
            //    {
            //        if (agreement.Status != status)
            //        {
            //            agreement.Status = status;
            //            await uow.Commit().ConfigureAwait(false);
            //        }
            //    }

            //    var documents = new List<SystemDocument>();

            //    var invoiceDoc = agreement.Documents.FirstOrDefault(d => d.DateDeleted == null && d.DocumentType == 4);
            //    if (invoiceDoc != null)
            //    {

            //        SystemDocument renderedDoc = _fileService.RenderDocument(CurrentUser(), invoiceDoc, agreement).Result;
            //        renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
            //        documents.Add(renderedDoc);
            //        await _emailService.SendEmailViaEmailTemplate(programme.BrokerContactUser.Email, emailTemplate, documents).ConfigureAwait(false);
            //        await _emailService.SendSystemSuccessInvoiceConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner).ConfigureAwait(false);
            //    }    
            //    else
            //        throw new NullReferenceException("No Invoice file");

            //}

            var eGlobalSerializer = new EGlobalSerializerAPI();

            //check Eglobal parameters
            if (string.IsNullOrEmpty(programme.EGlobalClientNumber))
            {
                throw new Exception(nameof(programme.EGlobalClientNumber) + " EGlobal client number");
            }

            var xmlPayload = eGlobalSerializer.SerializePolicy(programme, user, _unitOfWork);

            var byteResponse = await _httpClientService.CreateEGlobalInvoice(xmlPayload);

            eGlobalSerializer.DeSerializeResponse(byteResponse, programme, user, _unitOfWork);

            if (programme.ClientAgreementEGlobalResponses.Count > 0)
            {
                EGlobalResponse eGlobalResponse = programme.ClientAgreementEGlobalResponses.Where(er => er.DateDeleted == null && er.ResponseType == "update").OrderByDescending(er => er.VersionNumber).FirstOrDefault();
                if (eGlobalResponse != null)
                {
                    var documents = new List<SystemDocument>();
                    foreach (ClientAgreement agreement in programme.Agreements)
                    {
                        if (agreement.MasterAgreement && (agreement.ReferenceId == eGlobalResponse.MasterAgreementReferenceID))
                        {
                            foreach (SystemDocument doc in agreement.Documents.Where(d => d.DateDeleted == null && d.DocumentType == 4))
                            {
                                doc.Delete(user);
                            }
                            foreach (SystemDocument template in agreement.Product.Documents)
                            {
                                //render docs invoice
                                if (template.DocumentType == 4)
                                {
                                    SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement);
                                    renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                                    agreement.Documents.Add(renderedDoc);
                                    documents.Add(renderedDoc);
                                    await _fileService.UploadFile(renderedDoc);
                                }
                            }
                        }
                    }

                    var status = "Bound and invoiced";
                    foreach (ClientAgreement agreement in programme.Agreements)
                    {
                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {
                            if (agreement.Status != status)
                            {
                                agreement.Status = status;
                                await uow.Commit().ConfigureAwait(false);
                            }
                        }
                        agreement.Status = status;
                    }
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (programme.InformationSheet.Status != status)
                        {
                            programme.InformationSheet.Status = status;
                            await uow.Commit().ConfigureAwait(false);
                        }
                    }
                }

            }

            //return Redirect("/EditBillingConfiguration" + programmeId);
            var url = "/Agreement/ViewAcceptedAgreement/" + programme.Id;
            return Json(new { url });
        }

        [HttpGet]
        public async Task<IActionResult> OwnerProgrammes(Guid ownerId, Guid Id)
        {
            //BaseListViewModel<ProgrammeInfoViewModel> model = new BaseListViewModel<ProgrammeInfoViewModel>();
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();

            //ClientProgramme clientprogramme = new ClientProgramme();
            List<ClientProgramme> clientProgrammes = new List<ClientProgramme>();
            List<Organisation> Owners = new List<Organisation>();

            try
            {
                var programeListByOwner = await _programmeService.GetClientProgrammesByOwner(ownerId);
                foreach (var programme in programeListByOwner)
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
        public async Task<IActionResult> ClientProgrammeDetails(Guid programmeId, Guid Id,Guid ownerId)
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
                ClientProgramme programme = await _programmeService.GetClientProgramme(programmeId);
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
        public async Task<IActionResult> EditBillingConfiguration(Guid programmeId)
        {
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            ClientProgramme programme = await _programmeService.GetClientProgramme(programmeId);
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
        public async Task<IActionResult> SaveBillingConfiguration(string[] billingConfig, Guid programmeId)
        {
            ClientProgramme programme = await _programmeService.GetClientProgramme(programmeId);
            programme.EGlobalBranchCode = billingConfig[0];
            programme.EGlobalClientNumber = billingConfig[1];
            programme.EGlobalClientStatus = billingConfig[2];
            if (string.IsNullOrEmpty(billingConfig[3]))
            {
                programme.HasEGlobalCustomDescription = billingConfig[4] == "True"? true:false; 
                programme.EGlobalCustomDescription = billingConfig[3];
            }
            else
            {
                programme.HasEGlobalCustomDescription = billingConfig[4] == "True" ? true : false;
                programme.EGlobalCustomDescription = billingConfig[3]; 
            }
                    
            await _programmeService.Update(programme).ConfigureAwait(false);

            return Redirect("EditBillingConfiguration" + programmeId);
        }

        [HttpGet]
        public async Task<IActionResult> EditClientProgrammeDetails(Guid programmeId, Guid Id, Guid OwnerId)
        {
            ClientProgrammeInfoViewModel clientviewmodel = new ClientProgrammeInfoViewModel();

            try
            {
                ClientProgramme programme = await _programmeService.GetClientProgramme(programmeId);
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
        public async Task<IActionResult> SaveClientProgrammeDetails(Guid programme_id, ClientProgrammeInfoViewModel clientviewmodel, Guid id)
        {
            ClientProgramme programme = await _programmeService.GetClientProgramme(programme_id);

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
                    await uow.Commit().ConfigureAwait(false);

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
        //public async Task<IActionResult> OwnerDetails(List<Organisation> ownerlist, Guid Id)
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
        public async Task<IActionResult> EmailTemplate(Guid Id)
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
        public async Task<IActionResult> TermSheetTemplate(Guid Id)
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
        public async Task<IActionResult> TermSheetConfirguration(Guid Id)
        {
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            Programme programme = await _programmeRepository.GetByIdAsync(Id);
            try
            {
                model.Id = Id;
                model.programmeName = programme.Name;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            ViewBag.Title = "Term Sheet Template ";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductRules(Guid Id, Guid productId)
        {
            Programme programme = await _programmeRepository.GetByIdAsync(Id);

            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            var rules = new List<Rule>();
            model.Id = Id;
            model.ProductId = productId;
            var product = await _productRepository.GetByIdAsync(productId);
            
            foreach (var rule in product.Rules)
            {
                rules.Add(rule);
            }
            model.Rules = rules;

            ViewBag.Title = "Manage Product Rules";

            return View("ProductRules", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRule(Guid Id , ClientAgreementRuleViewModel rule )
        {
            //Programme programme = _programmeRepository.GetById(programmeId);

            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            var rules = new List<Rule>();
            //model.Id = Id;
            Rule Rule = await _RuleService.GetRuleByID(rule.ClientAgreementRuleID);
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
                        await uow.Commit();
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
        public async Task<IActionResult> ManageRules(Guid Id)
        {
            Programme programme = await _programmeRepository.GetByIdAsync(Id);

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
        //public async Task<IActionResult> AddselectedParty(Guid Id)
        //{
        //    var orguser = new List<string>();
        //    Programme programme = _programmeRepository.GetById(Id);
        //    ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
        //    model.Id = Id;
        //    model.Parties = programme.Parties;

        //    //model.OrgUser = _organisationRepository.GetById(programme.Parties.);
        //    //var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser().PrimaryOrganisation);

        //    ViewBag.Title = "Add/Edit Programme Email Template";

        //    return View("IssueNotification", model);
        //}


        [HttpPost]
        public async Task<IActionResult> AddselectedParty(string[] selectedParty,Guid informationId)
        {
            PartyUserViewModel model = new PartyUserViewModel();
            Programme programme = await _programmeRepository.GetByIdAsync(informationId);
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
                            var user = await _userService.GetUserByEmail(party);
                            programme.UISIssueNotifyUsers.Add(user);
                        }
                        await uow.Commit();
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
        public async Task<IActionResult> selectedParty(Guid selectedParty, Guid informationId)
        {
            //PartyUserViewModel model = new PartyUserViewModel();
            PartyUserViewModel model = new PartyUserViewModel();

            Programme programme = await _programmeRepository.GetByIdAsync(informationId);
            //model.Id = informationId;
            //model.Parties = programme.Parties;
            Organisation organisation = await _organisationRepository.GetByIdAsync(selectedParty);
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
        //public async Task<IActionResult> AddselectedParty(Guid selectedParty, Guid informationId)
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
        //        ////var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser().PrimaryOrganisation);
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
        public async Task<IActionResult> IssueNotification(Guid Id)
        {
            var orguser = new List<string>();
            Programme programme = await _programmeRepository.GetByIdAsync(Id);
            ProgrammeInfoViewModel model = new ProgrammeInfoViewModel();
            model.Id = Id;
            model.Parties = programme.Parties;

            //model.OrgUser = _organisationRepository.GetById(programme.Parties.);
            //var programmes = _programmeRepository.FindAll().Where(p => p.Owner == CurrentUser().PrimaryOrganisation);

            ViewBag.Title = "Add/Edit Programme Email Template";

            return View("IssueNotification", model);
        }


        [HttpGet]
        public async Task<IActionResult> SendEmailTemplates(Guid Id, String type,String description)
        {
            Programme programme = await _programmeRepository.GetByIdAsync(Id);
           
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
        public async Task<IActionResult> SendEmailTemplates(EmailTemplateViewModel model)
        {
            Programme programme = await _programmeRepository.GetByIdAsync(model.BaseProgrammeID);

            EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == model.Type);

            string emailtemplatename = null;
            var user = await CurrentUser();
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
                    emailTemplate.LastModifiedBy = user;
                    emailTemplate.LastModifiedOn = DateTime.UtcNow;
                    await uow.Commit().ConfigureAwait(false);
                }
            }
            else
            {
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    emailTemplate = new EmailTemplate(user, emailtemplatename, model.Type, model.Subject, model.Body, null, programme);
                    programme.EmailTemplates.Add(emailTemplate);
                    await uow.Commit();
                }
            }

            return RedirectToAction("SendEmailTemplates", new { Id = programme.Id, type = model.Type, description = model.Description });

        }

        /*[HttpGet]
		public async Task<IActionResult> ViewProgramme (Guid id)
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
