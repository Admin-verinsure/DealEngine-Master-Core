#region Using


using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using Microsoft.AspNetCore.Authorization;
using TechCertain.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#endregion

namespace TechCertain.WebUI.Controllers
{
    //[Route("Home")]
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
        IProgrammeService _programmeService;
        IProductService _productService;
        ILogger<HomeController> _logger;
        IApplicationLoggingService _applicationLoggingService;

        public HomeController(
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
            IClientInformationService clientInformationService
            )

            : base (userRepository)
        {
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
            model.CriticalTaskItems = new List<TaskItem>();
            model.ImportantTaskItems = new List<TaskItem>();
            model.CompletedTaskItems = new List<TaskItem>();

            User user = null;
            try
            {
                user = await CurrentUser();
                if (DemoEnvironment)
                {
                    model.DisplayDeals = true;
                    model.DisplayProducts = false;
                    model.DisplayRole = "Client";

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

                    IList<string> languages = new List<string>();
                    languages.Add("nz");

                    model.ProgrammeItems = new List<ProgrammeItem>();
                    var programmeList = await _programmeService.GetAllProgrammes();
                    foreach (Programme programme in programmeList)
                    {
                        List<DealItem> deals = new List<DealItem>();
                        var taskList = await _taskingService.GetAllTasksFor(user);
                        foreach (var task in taskList)
                        {
                            if (task.IsActive)
                            {
                                var taskItem = new TaskItem
                                {
                                    Id = task.Id,
                                    Details = task.Details,
                                    Description = task.Description,
                                    DueDate = LocalizeTime(task.DueDate)
                                };
                                if (task.Completed)
                                {
                                    model.CompletedTaskItems.Add(taskItem);
                                    continue;
                                }
                                if (task.Priority == 1)
                                    model.CriticalTaskItems.Add(taskItem);
                                else
                                    model.ImportantTaskItems.Add(taskItem);
                            }

                        }

                        model.ProgrammeItems.Add(new ProgrammeItem
                        {
                            Deals = deals,
                            Name = programme.Name,
                            Languages = languages,
                            ProgrammeId = programme.Id.ToString()
                        });
                    }

                    return View("IndexNew", model);
                }


                // Server URL
                // Server Username
                // Server Password

                var servers = new List<string>();

                if (!DemoEnvironment)
                {
                    var privateServers = await _privateServerService.GetAllPrivateServers();

                    foreach (var individualprivateServer in privateServers)
                    {
                        servers.Add(individualprivateServer.ServerAddress + "/api");
                    }

                }

                if (model.DisplayDeals = model.DealItems.Count > 0 || DemoEnvironment)
                {
                    model.DealItems = model.DealItems.OrderBy(o => o.Description).ToList();
                }

                if (model.DisplayProducts = model.ProductItems.Count > 0 || DemoEnvironment)
                {
                    model.ProductItems = model.ProductItems.OrderBy(o => o.Name).ToList();
                }

                if (User.Identity.Name == "TCustomer")
                    return View("Customer");

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View("Search");

        }

        [HttpPost]
        public async Task<IActionResult> Search(String Value)
        {
            return Content("/Home/ViewProgrammeByFilter/?value=" + Value);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProgrammeByFilter( String value)
        {
            ProgrammeItem model = new ProgrammeItem();
            List<DealItem> deals = new List<DealItem>();
            User user = null;
            try
            {
                user = await CurrentUser();

                if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
                {
                    var informationForList = await _clientInformationService.GetAllInformationFor(value);
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

                    ClientAgreement agreement = await _clientAgreementService.GetAgreementbyReferenceNum(value);

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


                model.ProgrammeId = id.ToString();
                if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
                {
                    foreach (ClientProgramme client in programme.ClientProgrammes.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                    {

                        string status = client.InformationSheet.Status;
                        string referenceid = client.InformationSheet.ReferenceId;
                        bool nextInfoSheet = false;
                        bool programmeAllowUsesChange = false;

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
                            Name = programme.Name + " for " + client.Owner.Name,
                            NextInfoSheet = nextInfoSheet,
                            ProgrammeAllowUsesChange = programmeAllowUsesChange,
                            LocalDateCreated = localDateCreated,
                            LocalDateSubmitted = localDateSubmitted,
                            Status = status,
                            ReferenceId = referenceid// Move into ClientProgramme?
                        });
                    }
                }
                else
                {
                    foreach (ClientProgramme client in programme.ClientProgrammes.Where(cp => cp.Owner == user.PrimaryOrganisation).OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                    {

                        string status = client.InformationSheet.Status;
                        string referenceid = client.InformationSheet.ReferenceId;
                        Boolean nextInfoSheet = false;
                        Boolean programmeAllowUsesChange = false;

                        string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy h:mm tt");
                        string localDateSubmitted = null;

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
                            Name = programme.Name + " for " + client.Owner.Name,
                            NextInfoSheet = nextInfoSheet,
                            ProgrammeAllowUsesChange = programmeAllowUsesChange,
                            LocalDateCreated = localDateCreated,
                            LocalDateSubmitted = localDateSubmitted,
                            Status = status,
                            ReferenceId = referenceid// Move into ClientProgramme?
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

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> IssueUIS(string ProgrammeId)
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
                    clientProgrammes.Add(client);                    
                }
                model.ClientProgrammes = clientProgrammes;

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }                       
        }


        [HttpGet]
        public async Task<IActionResult> ViewTask(Guid Id)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                UserTask task = await _taskingService.GetTask(Id);
                UserTaskViewModel model = new UserTaskViewModel
                {
                    Details = task.Details,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Completed = task.Completed,
                    CompletedBy = task.CompletedBy,
                    For = task.For
                };

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

    }
}