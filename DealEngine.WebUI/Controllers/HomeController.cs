#region Using


using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.Tasking;
using Microsoft.AspNetCore.Authorization;
using DealEngine.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using DealEngine.Infrastructure.FluentNHibernate;

#endregion

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        IClientInformationService _customerInformationService;
        IPrivateServerService _privateServerService;
        ITaskingService _taskingService;
        IClientInformationService _clientInformationService;
        IClientAgreementService _clientAgreementService;
        IHttpClientService _httpClientService;
        IMapper _mapper;
        IAppSettingService _appSettingService;
        IEmailService _emailService;
        IProgrammeService _programmeService;
        IProductService _productService;
        ILogger<HomeController> _logger;
        IApplicationLoggingService _applicationLoggingService;
        IOrganisationService _organisationService;
        IUnitOfWork _unitOfWork;
        public HomeController(
            IOrganisationService organisationService,
            IAppSettingService appSettingService,
            IEmailService emailService,
            IMapper mapper,
            IApplicationLoggingService applicationLoggingService,
            ILogger<HomeController> logger,
            IProductService productService,
            IProgrammeService programmeService,
            IUserService userRepository,
            IHttpClientService httpClientService,
            ITaskingService taskingService,
            IClientInformationService customerInformationService,
            IPrivateServerService privateServerService,
            IClientAgreementService clientAgreementService,
            IClientInformationService clientInformationService,
            IUnitOfWork unitOfWork

            )

            : base(userRepository)
        {
            _organisationService = organisationService;
            _appSettingService = appSettingService;
            _emailService = emailService;
            _applicationLoggingService = applicationLoggingService;
            _logger = logger;
            _productService = productService;
            _programmeService = programmeService;
            _httpClientService = httpClientService;
            _customerInformationService = customerInformationService;
            _privateServerService = privateServerService;
            _taskingService = taskingService;
            _clientInformationService = clientInformationService;
            _clientAgreementService = clientAgreementService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: home/index
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "DealEngine Dashboard";

            DashboardViewModel model = new DashboardViewModel();
            model.ProductItems = new List<ProductItemV2>();
            model.DealItems = new List<ProductItem>();
            model.UserTasks = new List<UserTask>();            

            User user = null;
            try
            {
                user = await CurrentUser();

                model.DisplayDeals = true;
                model.DisplayProducts = false;
                model.CurrentUserType = "Client";
                if (user.PrimaryOrganisation.IsBroker)
                {
                    model.CurrentUserType = "Broker";
                }
                if (user.PrimaryOrganisation.IsInsurer)
                {
                    model.CurrentUserType = "Insurer";
                    model.UserTasks = await _taskingService.GetAllActiveTasksFor(user.PrimaryOrganisation);
                }
                if (user.PrimaryOrganisation.IsTC)
                {
                    model.CurrentUserType = "TC";
                    model.UserTasks = await _taskingService.GetAllActiveTasksFor(user.PrimaryOrganisation);
                }

                IList<string> languages = new List<string>();
                languages.Add("nz");
                List<DealItem> deals = new List<DealItem>();
                IList<Programme> programmeList = new List<Programme>();
                model.ProgrammeItems = new List<ProgrammeItem>();
                if (model.CurrentUserType == "Client")
                {                    
                    var clientProgList = _programmeService.GetClientProgrammesByOwner(user.PrimaryOrganisation.Id).Result.GroupBy(bp => bp.BaseProgramme);
                    foreach (var clientProgramme in clientProgList)
                    {
                        programmeList.Add(clientProgramme.Key);
                    }
                }
                else
                {
                    programmeList = await _programmeService.GetAllProgrammes();
                }

                foreach (Programme programme in programmeList)
                {
                    model.ProgrammeItems.Add(new ProgrammeItem
                    {
                        Deals = deals,
                        Name = programme.Name,
                        Languages = languages,
                        ProgrammeId = programme.Id.ToString(),
                        ProgrammeClaim = programme.Claim
                    });
                }

                return View("IndexNew", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        #region Search
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var companyName = _appSettingService.GetCompanyTitle;
            SearchViewModel model = new SearchViewModel(companyName);
            return View("Search", model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewProgramme(IFormCollection collection)
        {

            var searchTerm = collection["SearchTerm"].ToString();
            var searchValue = collection["SearchValue"].ToString();
            ProgrammeItem model = new ProgrammeItem();
            User user = await CurrentUser();

            if (searchTerm == "Advisory")
            {
                model.Deals = await GetAdvisoryNameSearch(searchValue);
            }
            if (searchTerm == "Boat")
            {
                model.Deals = await GetBoatNameSearch(searchValue);
            }
            if (searchTerm == "Name")
            {
                model.Deals = await GetClientNameSearch(searchValue);
            }
            if (searchTerm == "Reference")
            {
                model.Deals = await GetReferenceSearch(searchValue);
            }

            return View(model);
        }

        private async Task<IList<DealItem>> GetReferenceSearch(string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();

            var informationForList = await _clientInformationService.GetAllInformationFor(searchValue);
            foreach (ClientInformationSheet sheet in informationForList)
            {
                ClientProgramme client = sheet.Programme;

                string status = client.InformationSheet.Status;
                string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy");
                string localDateSubmitted = null;

                if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                {
                    localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy");
                }

                deals.Add(new DealItem
                {
                    Id = client.Id.ToString(),
                    Name = sheet.Programme.BaseProgramme.Name + " for " + client.Owner.Name,
                    LocalDateCreated = localDateCreated,
                    LocalDateSubmitted = localDateSubmitted,
                    Status = status,
                    SubClientProgrammes = client.SubClientProgrammes,
                    ReferenceId = client.InformationSheet.ReferenceId// Move into ClientProgramme?
                });
            }

            ClientAgreement agreement = await _clientAgreementService.GetAgreementbyReferenceNum(searchValue);

            if (agreement != null)
            {
                ClientInformationSheet sheet2 = await _clientInformationService.GetInformation(agreement.ClientInformationSheet.Id);

                if (sheet2 != null)
                {
                    ClientProgramme client = sheet2.Programme;

                    string status = client.InformationSheet.Status;
                    string referenceid = client.InformationSheet.ReferenceId;
                    string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy");//"dd/MM/yyyy h:mm tt"
                    string localDateSubmitted = null;

                    if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy");
                    }

                    deals.Add(new DealItem
                    {
                        Id = client.Id.ToString(),
                        Name = sheet2.Programme.BaseProgramme.Name + " for " + client.Owner.Name,
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        SubClientProgrammes = sheet2.Programme.SubClientProgrammes,
                        ReferenceId = referenceid// Move into ClientProgramme?
                    });

                }
            }

            return deals;
        }

        private async Task<IList<DealItem>> GetClientNameSearch(string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();
            List<ClientProgramme> clients = await _programmeService.FindByOwnerName(searchValue);

            if (clients.Count != 0)
            {
                foreach (var client in clients)
                {
                    string status = client.InformationSheet.Status;
                    string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy");
                    string localDateSubmitted = null;

                    if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy");
                    }

                    deals.Add(new DealItem
                    {
                        Id = client.Id.ToString(),
                        Name = client.BaseProgramme.Name + " for " + client.Owner.Name,
                        ProgrammeAllowUsesChange = client.BaseProgramme.AllowUsesChange,
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = client.InformationSheet.ReferenceId,// Move into ClientProgramme?
                        SubClientProgrammes = client.SubClientProgrammes,
                    });
                }
            }

            return deals;
        }

        private async Task<IList<DealItem>> GetAdvisoryNameSearch(string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();
            List<ClientInformationSheet> clients = await _clientInformationService.FindByAdvisoryName(searchValue);
            if (clients.Count != 0)
            {
                foreach (var client in clients)
                {
                    string status = client.Status;
                    string localDateCreated = LocalizeTime(client.DateCreated.GetValueOrDefault(), "dd/MM/yyyy");
                    string localDateSubmitted = null;

                    if (client.Status != "Not Started" && client.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.SubmitDate, "dd/MM/yyyy");
                    }

                    deals.Add(new DealItem
                    {
                        Id = client.Programme.Id.ToString(),
                        Name = client.Programme.BaseProgramme.Name + " for " + client.Owner.Name,
                        ProgrammeAllowUsesChange = client.Programme.BaseProgramme.AllowUsesChange,
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = client.ReferenceId,// Move into ClientProgramme?
                        SubClientProgrammes = client.Programme.SubClientProgrammes,
                    });
                }
            }

            return deals;
        }

        private async Task<IList<DealItem>> GetBoatNameSearch(string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();
            List<ClientInformationSheet> clients = await _clientInformationService.FindByBoatName(searchValue);

            if (clients.Count != 0)
            {
                foreach (var client in clients)
                {
                    string status = client.Status;
                    string localDateCreated = LocalizeTime(client.DateCreated.GetValueOrDefault(), "dd/MM/yyyy");
                    string localDateSubmitted = null;

                    if (client.Status != "Not Started" && client.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.SubmitDate, "dd/MM/yyyy");
                    }

                    deals.Add(new DealItem
                    {
                        Id = client.Programme.Id.ToString(),
                        Name = client.Programme.BaseProgramme.Name + " for " + client.Owner.Name,
                        ProgrammeAllowUsesChange = client.Programme.BaseProgramme.AllowUsesChange,
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = client.ReferenceId,// Move into ClientProgramme?
                        SubClientProgrammes = client.Programme.SubClientProgrammes,
                    });
                }
            }

            return deals;
        }


        #endregion Search

        [HttpGet]
        public async Task<IActionResult> ViewSubClientProgrammes(string clientProgrammeId)
        {
            ProgrammeItem model = new ProgrammeItem();
            User user = null;
            var clientList = new List<ClientProgramme>();
            try
            {
                user = await CurrentUser();
                ClientProgramme clientprogramme = await _programmeService.GetClientProgramme(Guid.Parse(clientProgrammeId));
                if (clientprogramme.SubClientProgrammes.Any())
                {
                    foreach (var client in clientprogramme.SubClientProgrammes)
                    {
                        clientList.Add(client);
                    }
                }
                else
                {
                    clientList.Add(clientprogramme);
                }

                model = await GetClientProgrammeListModel(user, clientList, true);

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        private async Task<ProgrammeItem> GetClientProgrammeListModel(User user, IList<ClientProgramme> clientList, bool isClient=false)
        {
            ProgrammeItem model = new ProgrammeItem();
            List<DealItem> deals = new List<DealItem>();
            var clientProgramme = clientList.FirstOrDefault();
            if (!isClient)
            {
                var isBaseClientProg = await _programmeService.IsBaseClass(clientProgramme);
                if (isBaseClientProg)
                {
                    clientList = await _programmeService.GetClientProgrammesForProgramme(clientProgramme.BaseProgramme.Id);
                }
            }
            if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
            {
                Boolean Issubclientsubmitted = false;
                foreach (ClientProgramme client in clientList.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {
                    if (client.InformationSheet != null)
                    {
                        model.ProgrammeId = client.BaseProgramme.Id.ToString();
                        string status = client.InformationSheet.Status;
                        string referenceId = client.InformationSheet.ReferenceId;
                        bool nextInfoSheet = false;
                        bool programmeAllowUsesChange = false;
                        string agreementSatus = "";
                        foreach (ClientAgreement agreement in client.Agreements)
                        {
                            if (agreement.ClientInformationSheet.Status != "Not Started" && agreement.ClientInformationSheet.Status != "Started" && agreement.DateDeleted == null && agreement.Status == "Referred")
                            {
                                agreementSatus = "Referred";
                                break;
                            }
                        }
                        if (client.BaseProgramme.AllowUsesChange)
                        {
                            programmeAllowUsesChange = true;
                        }

                        if (null != client.InformationSheet.NextInformationSheet)
                        {
                            nextInfoSheet = true;
                        }

                        string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy h:mm tt");
                        string localDateSubmitted = null;

                        if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                        {
                            localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy h:mm tt");
                        }
                        if(client.SubClientProgrammes.Count > 0)
                        {
                            Issubclientsubmitted = true;
                        }
                        for(var index=0; index <client.SubClientProgrammes.Count; index++)
                        {
                            if (client.SubClientProgrammes[index].InformationSheet.Status != "Submitted")
                                Issubclientsubmitted = false;

                        }
                        deals.Add(new DealItem
                        {
                            Id = client.Id.ToString(),
                            Name = client.BaseProgramme.Name + " for " + client.Owner.Name,
                            NextInfoSheet = nextInfoSheet,
                            ProgrammeAllowUsesChange = programmeAllowUsesChange,
                            LocalDateCreated = localDateCreated,
                            LocalDateSubmitted = localDateSubmitted,
                            Status = status,
                            ReferenceId = referenceId,// Move into ClientProgramme?
                            SubClientProgrammes = client.SubClientProgrammes,
                            AgreementStatus = agreementSatus,
                            IsSubclientSubmitted = Issubclientsubmitted

                        });
                    }

                }
            }
            else
            {
                clientList = await _programmeService.GetClientProgrammesByOwner(user.PrimaryOrganisation.Id);
                foreach (ClientProgramme client in clientList.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {
                    model.ProgrammeId = client.BaseProgramme.Id.ToString();
                    string status = client.InformationSheet.Status;
                    string referenceId = client.InformationSheet.ReferenceId;
                    bool nextInfoSheet = false;
                    bool programmeAllowUsesChange = false;
                    string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy h:mm tt");
                    string localDateSubmitted = null;
                    string agreementSatus = "";
                    foreach (ClientAgreement agreement in client.Agreements)
                    {
                        if (agreement.ClientInformationSheet.Status != "Not Started" && agreement.ClientInformationSheet.Status != "Started" && agreement.DateDeleted == null && agreement.Status == "Referred")
                        {
                            agreementSatus = "Referred";
                            break;
                        }
                    }
                    if (client.BaseProgramme.AllowUsesChange)
                    {
                        programmeAllowUsesChange = true;
                    }

                    if (null != client.InformationSheet.PreviousInformationSheet)
                    {
                        nextInfoSheet = true;
                    }

                    if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy h:mm tt");
                    }

                    deals.Add(new DealItem
                    {
                        Id = client.Id.ToString(),
                        Name = client.BaseProgramme.Name + " for " + client.Owner.Name,
                        NextInfoSheet = nextInfoSheet,
                        ProgrammeAllowUsesChange = programmeAllowUsesChange,
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = referenceId,// Move into ClientProgramme?
                        SubClientProgrammes = client.SubClientProgrammes,
                        AgreementStatus = agreementSatus
                    });
                }
            }
            model.Deals = deals;

            if (user.PrimaryOrganisation.IsBroker)
            {
                model.CurrentUserIsBroker = "True";
            }
            else
            {
                model.CurrentUserIsBroker = "False";
            }
            if (user.PrimaryOrganisation.IsInsurer)
            {
                model.CurrentUserIsInsurer = "True";
            }
            else
            {
                model.CurrentUserIsInsurer = "False";
            }
            if (user.PrimaryOrganisation.IsTC)
            {
                model.CurrentUserIsTC = "True";
            }
            else
            {
                model.CurrentUserIsTC = "False";
            }

            return model;
        }

        [HttpGet]
        public async Task<IActionResult> ViewSubClientProgramme(Guid subClientProgrammeId)
        {
            ProgrammeItem model = new ProgrammeItem();
            User user = null;
            var clientList = new List<ClientProgramme>();
            try
            {
                user = await CurrentUser();
                SubClientProgramme subClientprogramme = await _programmeService.GetSubClientProgrammebyId(subClientProgrammeId);
                clientList.Add(subClientprogramme);
                model = await GetClientProgrammeListModel(user, clientList);

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewProgramme(Guid id)
        {
            ProgrammeItem model = new ProgrammeItem();
            List<DealItem> deals = new List<DealItem>();
            User user = null;
            try
            {
                user = await CurrentUser();
                Programme programme = await _programmeService.GetProgrammeById(id);
                var clientList = await _programmeService.GetClientProgrammesByOwner(user.PrimaryOrganisation.Id);
                if(!clientList.Any())
                {
                    clientList = await _programmeService.GetClientProgrammesForProgramme(id);
                }

                model = await GetClientProgrammeListModel(user, clientList);
                model.ProgrammeId = id.ToString();
                model.IsSubclientEnabled = programme.HasSubsystemEnabled;

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> IssueUIS(string ProgrammeId,string actionname)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                IssueUISViewModel model = new IssueUISViewModel();
                var clientProgrammes = new List<ClientProgramme>();
                Programme programme = await _programmeService.GetProgrammeById(Guid.Parse(ProgrammeId));
                List<ClientProgramme> mainClientProgrammes = await _programmeService.GetClientProgrammesForProgramme(programme.Id);
                List<ClientProgramme> subClientProgrammes = await _programmeService.GetSubClientProgrammesForProgramme(programme.Id);
                
                foreach (var client in mainClientProgrammes.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {
                    if (client.DateDeleted == null && (client.InformationSheet.Status == "Started" || client.InformationSheet.Status == "Not Started"))
                    {
                            clientProgrammes.Add(client);
                    }
                }
                model.ClientProgrammes = clientProgrammes;
                model.ProgrammeId = ProgrammeId;
                model.IsSubUIS = "false";
                if (actionname == "IssueUIS")
                {
                    return View(model);
                }
                else
                {
                    //if (action == "EditClient")
                        return View("EditClient", model);
                }
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> IssueSubUIS(string ProgrammeId)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                IssueUISViewModel model = new IssueUISViewModel();
                var clientProgrammes = new List<ClientProgramme>();
                Programme programme = await _programmeService.GetProgrammeById(Guid.Parse(ProgrammeId));
                List<ClientProgramme> subClientProgrammes = await _programmeService.GetSubClientProgrammesForProgramme(programme.Id);

                foreach (var client in subClientProgrammes.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {
                    if (client.DateDeleted == null && (client.InformationSheet.Status == "Started" || client.InformationSheet.Status == "Not Started"))
                    {
                        clientProgrammes.Add(client);
                    }
                }
                model.IsSubUIS = "true";
                model.ClientProgrammes = clientProgrammes;
                model.ProgrammeId = ProgrammeId;

                    return View("IssueUIS",model);
               
               
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> IssueReminder(string ProgrammeId)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                IssueUISViewModel model = new IssueUISViewModel();
                var clientProgrammes = new List<ClientProgramme>();
                Programme programme = await _programmeService.GetProgrammeById(Guid.Parse(ProgrammeId));

                foreach (var client in programme.ClientProgrammes.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {
                    if (client.DateDeleted == null && (client.InformationSheet.Status == "Started" || client.InformationSheet.Status == "Not Started"))
                    {
                        clientProgrammes.Add(client);
                    }
                }

                model.ClientProgrammes = clientProgrammes;
                model.ProgrammeId = ProgrammeId;

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }       

        [HttpPost]
        public async Task<IActionResult> IssueUIS(IFormCollection formCollection)
        {
            User user = null;
            Programme programme = null;
            string email = null;

            try
            {
                user = await CurrentUser();
                programme = await _programmeService.GetProgramme(Guid.Parse(formCollection["ProgrammeId"]));
                foreach (var key in formCollection.Keys)
                {

                    email = key;
                    var correctEmail = await _userService.GetUserByEmail(email);
                    if (correctEmail != null)
                    {
                        if (programme.ProgEnableEmail)
                        {
                            var clientProgramme = await _programmeService.GetClientProgrammebyId(Guid.Parse(formCollection[key]));
                            clientProgramme.IssueDate = DateTime.Now;
                            await _programmeService.Update(clientProgramme);

                            //send out login instruction email
                            await _emailService.SendSystemEmailLogin(email);
                            //send out information sheet instruction email
                            EmailTemplate emailTemplate = null;
                            var isSubUis = formCollection["IsSubUIS"];

                            if (isSubUis.Contains("true"))
                            {
                                 emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendSubInformationSheetInstruction"); 

                            }
                            else
                            {
                                emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetInstruction");

                            }
                            if (emailTemplate != null)
                            {
                                await _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null, null, null);
                            }
                            //send out uis issue notification email
                            //await _emailService.SendSystemEmailUISIssueNotify(programme.BrokerContactUser, programme, sheet, programme.Owner);
                        }
                    }

                }

                return await RedirectToLocal();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        
             [HttpGet]
        public async Task<IActionResult> GetClientDetails(String OwnerId , string actionName)
        {
            User user = null;
            Organisation ownerorg= null;
            string email = null;
            OrganisationViewModel orgmodel = new OrganisationViewModel();

            try
            {
                user = await CurrentUser();
                ownerorg = await _organisationService.GetOrganisation(Guid.Parse(OwnerId));
                orgmodel.OrganisationName = ownerorg.Name;
                var userList = await _userService.GetAllUserByOrganisation(ownerorg);
                orgmodel.ID = Guid.Parse(OwnerId);
                orgmodel.Email = ownerorg.Email;
                if(actionName == "ClientDetails")
                {
                    return Json(orgmodel);
                }
                else
                {
                    orgmodel.Users = userList;
                    var id = OwnerId;
                    List<User> userlist = userList;
                    return View("getClientDetails", orgmodel);
                }
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DisplayClientUser(Guid Id , List<User> UserList)
        {
            OrganisationViewModel orgmodel = new OrganisationViewModel();
            return View("getClientDetails", orgmodel);
        }


        [HttpPost]
        public async Task<IActionResult> GetUserDetails(Guid UserID)
        {
            User user = null;
            Organisation ownerorg = null;
            string email = null;
            OrganisationViewModel orgmodel = new OrganisationViewModel();

            try
            {
                user = await _userService.GetUserById(UserID);
                orgmodel.ID = user.Id;
                orgmodel.FirstName = user.FirstName;
                orgmodel.LastName = user.LastName;
                orgmodel.Email = user.Email;
                orgmodel.Phone = user.Phone;
                return Json(orgmodel);
               
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddClientUser(IFormCollection formCollection)
        {
            User Currentuser = null;
            Organisation ownerorg = null;
            Organisation primaryorg = null;
            string email = null;
            OrganisationViewModel orgmodel = new OrganisationViewModel();
            User userdb = null;
            User user2 = null;
            var jsdkf = formCollection["Id"];
            string username = "";
            try
            {
                Currentuser = await CurrentUser();
                ownerorg = await _organisationService.GetOrganisation(Guid.Parse(formCollection["Id"]));
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {

                    var Action  = formCollection["Action"];
                    var FirstName = formCollection["FirstName"];
                    var LastName = formCollection["LastName"];
                    var Email = formCollection["Email"];
                    var Phone = formCollection["Phone"];

                    if (Action == "Edit")
                    {
                        userdb = await _userService.GetUserById(Guid.Parse(formCollection["UserId"]));
                        if (userdb == null)
                        {
                             username = FirstName + "_" + LastName;

                            try
                            {
                                user2 = await _userService.GetUser(username);

                                if (user2 != null && userdb == user2)
                                {
                                    Random random = new Random();
                                    int randomNumber = random.Next(10, 99);
                                    username = username + randomNumber.ToString();
                                }
                            }
                            catch (Exception)
                            {
                                username = FirstName + "_" + LastName;
                            }
                        }

                        primaryorg = await _organisationService.GetOrganisation(userdb.PrimaryOrganisation.Id);
                        primaryorg.Email = Email;
                        userdb.FirstName = FirstName;
                        userdb.LastName = LastName;
                        userdb.FullName = FirstName + " " + LastName;
                        userdb.Email = Email;
                        userdb.Phone = Phone;
                        await uow.Commit();

                    }
                    else
                    {
                        if (Action == "Add") {
                        userdb = new User(Currentuser, Guid.NewGuid(), username);
                            userdb.FirstName = FirstName;
                            userdb.LastName = LastName;
                            userdb.FullName = FirstName + " " + LastName;
                            userdb.Email = Email;
                            userdb.Phone = Phone;
                            await _userService.Create(userdb);
                            userdb.Organisations.Add(ownerorg);
                            userdb.SetPrimaryOrganisation(ownerorg);
                            await uow.Commit();
                        }
                    }

                }
               
                return await RedirectToLocal();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, Currentuser, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }



        [HttpPost]
        public async Task<IActionResult> EditClient(IFormCollection formCollection)
        {
            User user = null;
            Organisation organisation = null;
            string email = null;

            try
            {
                foreach (var key in formCollection.Keys)
                {
                    organisation = await _organisationService.GetOrganisation(Guid.Parse(formCollection["Id"]));
                    var userList = await _userService.GetAllUserByOrganisation(organisation);
                    user = userList.Last(user => user.PrimaryOrganisation == organisation);
                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        organisation.ChangeOrganisationName(formCollection["OrganisationName"]);
                        organisation.Email = formCollection["Email"];
                        organisation.Phone = formCollection["Phone"];
                        if(user != null)
                        user.Email= formCollection["Email"];
                        await uow.Commit();
                    }
                }

                return await RedirectToLocal();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> IssueReminder(IFormCollection formCollection)
        {
            User user = null;
            Programme programme = null;
            string email = null;

            try
            {
                user = await CurrentUser();
                programme = await _programmeService.GetProgramme(Guid.Parse(formCollection["ProgrammeId"]));
                foreach (var key in formCollection.Keys)
                {
                    email = key;
                    var correctEmail = await _userService.GetUserByEmail(email);
                    if (correctEmail != null)
                    {
                        if (programme.ProgEnableEmail)
                        {
                            var clientProgramme = await _programmeService.GetClientProgrammebyId(Guid.Parse(formCollection[key]));
                            clientProgramme.ReminderDate = DateTime.Now;
                            await _programmeService.Update(clientProgramme);

                            //send out login instruction email
                            await _emailService.SendSystemEmailLogin(email);
                            //send out information sheet instruction email
                            EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetReminder");
                            if (emailTemplate != null)
                            {
                                await _emailService.SendEmailViaEmailTemplate(email, emailTemplate, null, null, null);
                            }
                        }
                    }

                }

                return await RedirectToLocal();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

    }
}