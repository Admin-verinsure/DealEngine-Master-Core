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
        IEmailService _emailService;
        IProgrammeService _programmeService;
        IProductService _productService;
        ILogger<HomeController> _logger;
        IApplicationLoggingService _applicationLoggingService;
        IOrganisationService _organisationService;
        IUnitOfWork _unitOfWork;


        public HomeController(
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
            IUnitOfWork unitOfWork,
            IOrganisationService organisationService

            )

            : base(userRepository)
        {
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
            _organisationService = organisationService;
            _mapper = mapper;
        }

        // GET: home/index
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Proposalonline Dashboard";

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
            return View("Search");

        }

        [HttpPost]
        public async Task<IActionResult> ViewProgramme(IFormCollection collection)
        {
            //enum 
            //1-Reference No
            //2-Insured First Name
            //3-Insured Last Name
            //4-Boat Name
            var searchTerm = collection["SearchTerm"].ToString();
            var searchValue = collection["SearchValue"].ToString();

            ProgrammeItem model = new ProgrammeItem();
            User user = null;

            var isValidInput = ValidateSearchInput(searchTerm, searchValue);
            if (isValidInput)
            {
                try
                {
                    user = await CurrentUser();

                    if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
                    {
                        if (searchTerm == "1")
                        {
                            searchValue = searchValue.Replace(" ", string.Empty);
                            model.Deals = await GetReferenceIdSearch(user, searchValue);
                        }
                        else if (searchTerm == "2" || searchTerm == "3")
                        {
                            searchValue = searchValue.Replace(" ", string.Empty);
                            model.Deals = await GetInsuredNameSearch(user, searchValue);
                        }
                        else if (searchTerm == "4")
                        {
                            model.Deals = await GetBoatNameSearch(user, searchValue);
                        }
                    }

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

                    return View(model);
                }
                catch (Exception ex)
                {
                    await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                    return RedirectToAction("Error500", "Error");
                }
            }

            model.Deals = new List<DealItem>();
            return View(model);
        }

        private async Task<IList<DealItem>> GetBoatNameSearch(User user, string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();
            List<ClientInformationSheet> clients = await _clientInformationService.FindByBoatName(searchValue);

            if (clients.Count != 0)
            {
                foreach (var client in clients)
                {
                    string status = client.Status;
                    string referenceid = client.ReferenceId;
                    string localDateCreated = LocalizeTime(client.DateCreated.GetValueOrDefault(), "dd/MM/yyyy");
                    string localDateSubmitted = null;

                    if (client.Status != "Not Started" && client.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.SubmitDate, "dd/MM/yyyy");
                    }

                    deals.Add(new DealItem
                    {
                        Id = client.Id.ToString(),
                        //Name = client.BaseProgramme.Name + " for " + client.Owner.Name,
                        Name = client.Programme.BaseProgramme.Name + " for " + client.Owner.Name,
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = referenceid// Move into ClientProgramme?
                    });
                }
            }

            return deals;
        }

        private bool ValidateSearchInput(string value1, string value2)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(value1))
            {
                list.Add(value1);
            }
            if (!string.IsNullOrWhiteSpace(value2))
            {
                list.Add(value2);
            }

            if (list.Count != 2)
            {
                return false;
            }
            return true;
        }

        private async Task<IList<DealItem>> GetInsuredNameSearch(User user, string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();
            List<ClientProgramme> clients = await _programmeService.FindByOwnerName(searchValue);

            if (clients.Count != 0)
            {
                foreach (var client in clients)
                {
                    string status = client.InformationSheet.Status;
                    string referenceid = client.InformationSheet.ReferenceId;
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
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = referenceid// Move into ClientProgramme?
                    });
                }
            }

            return deals;
        }

        private async Task<IList<DealItem>> GetReferenceIdSearch(User user, string searchValue)
        {
            List<DealItem> deals = new List<DealItem>();

            var informationForList = await _clientInformationService.GetAllInformationFor(searchValue);
            foreach (ClientInformationSheet sheet in informationForList)
            {
                ClientProgramme client = sheet.Programme;

                string status = client.InformationSheet.Status;
                string referenceid = client.InformationSheet.ReferenceId;
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
                    ReferenceId = referenceid// Move into ClientProgramme?
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
                        ReferenceId = referenceid// Move into ClientProgramme?
                    });

                }
            }

            return deals;
        }
        #endregion Search

        [HttpGet]
        public async Task<IActionResult> ViewSubClientProgrammes(Guid clientProgrammeId)
        {
            ProgrammeItem model = new ProgrammeItem();
            User user = null;
            var clientList = new List<ClientProgramme>();
            try
            {
                user = await CurrentUser();
                ClientProgramme clientprogramme = await _programmeService.GetClientProgramme(clientProgrammeId);
                foreach (var client in clientprogramme.SubClientProgrammes)
                {
                    clientList.Add(client);
                }
                model = await GetClientProgrammeListModel(user, clientList);

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        private async Task<ProgrammeItem> GetClientProgrammeListModel(User user, IList<ClientProgramme> clientList)
        {
            ProgrammeItem model = new ProgrammeItem();
            List<DealItem> deals = new List<DealItem>();
            var clientProgramme = clientList.FirstOrDefault();
            var isBaseClientProg = await _programmeService.IsBaseClass(clientProgramme);
            if (isBaseClientProg)
            {
                clientList = await _programmeService.GetClientProgrammesForProgramme(clientProgramme.BaseProgramme.Id);
            }
            if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
            {                
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
                if(clientList.Count == 0)
                {
                    clientList = await _programmeService.GetClientProgrammesForProgramme(id);
                }

                model = await GetClientProgrammeListModel(user, clientList);
                model.ProgrammeId = id.ToString();

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

                foreach (var client in programme.ClientProgrammes.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {
                    if (client.DateDeleted == null && (client.InformationSheet.Status == "Started" || client.InformationSheet.Status == "Not Started"))
                    {
                        clientProgrammes.Add(client);
                    }
                }

                model.ClientProgrammes = clientProgrammes;
                model.ProgrammeId = ProgrammeId;

                if(actionname == "IssueUIS")
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

        //[HttpGet]
        //public async Task<IActionResult> EditClient(Guid ProgrammeId ,IList<DealItem> Deals)
        //{
        //    User user = null;
        //    try
        //    {
        //        user = await CurrentUser();
        //        ProgrammeItem model = new ProgrammeItem();

        //        model.Deals =Deals;
        //        var clientProgrammes = new List<ClientProgramme>();
               
               

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
        //        return RedirectToAction("Error500", "Error");
        //    }
        //}
       

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
                            EmailTemplate emailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendInformationSheetInstruction");
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

        
             [HttpPost]
        public async Task<IActionResult> getClientDetails(Guid OwnerId)
        {
            User user = null;
            Organisation ownerorg= null;
            string email = null;
            OrganisationViewModel orgmodel = new OrganisationViewModel();

            try
            {
                user = await CurrentUser();
                ownerorg = await _organisationService.GetOrganisation(OwnerId);
                orgmodel.OrganisationName = ownerorg.Name;
                var userList = await _userService.GetAllUserByOrganisation(ownerorg);
                //user = userList.FirstOrDefault(user => user.PrimaryOrganisation == ownerorg);
                orgmodel.ID = OwnerId;
                orgmodel.Users = userList;
                //orgmodel.FirstName = user.FirstName;
                //orgmodel.LastName = user.LastName;
                //orgmodel.Email = user.Email;
                //orgmodel.Phone = user.Phone;

                return Json(orgmodel);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditClient(IFormCollection formCollection)
        {
            User user = null;
            Programme programme = null;
            Organisation organisation = null;
            string email = null;

            try
            {
                programme = await _programmeService.GetProgramme(Guid.Parse(formCollection["ProgrammeId"]));

                foreach (var key in formCollection.Keys)
                {
                    organisation = await _organisationService.GetOrganisation(Guid.Parse(formCollection["Id"]));
                    var userList = await _userService.GetAllUserByOrganisation(organisation);
                    user = userList.FirstOrDefault(user => user.PrimaryOrganisation == organisation);
                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        organisation.ChangeOrganisationName(formCollection["OrganisationName"]);
                        organisation.Email = formCollection["Email"];
                        organisation.Phone = formCollection["Phone"];
                        user.FirstName = formCollection["FirstName"];
                        user.LastName = formCollection["LastName"];
                        user.Email = formCollection["Email"];
                        await uow.Commit();
                    }



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