#region Using


using AutoMapper;
using Elmah;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using Microsoft.AspNetCore.Authorization;
using TechCertain.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

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
        IMapperSession<Product> _productRepositoy;
        IMapperSession<Programme> _programmeRepository;
        IClientInformationService _clientInformationService;
        IClientAgreementService _clientAgreementService;
        IMapper _mapper;

        public HomeController(IMapper mapper, IUserService userRepository,
            ITaskingService taskingService, IClientInformationService customerInformationService, IPrivateServerService privateServerService, IClientAgreementService clientAgreementService, IClientInformationService clientInformationService,
            IMapperSession<Product> productRepository, IMapperSession<Programme> programmeRepository)

            : base (userRepository)
        {            

            _customerInformationService = customerInformationService;
            _privateServerService = privateServerService;
            _taskingService = taskingService;
            _clientInformationService = clientInformationService;
            _productRepositoy = productRepository;
            _clientAgreementService = clientAgreementService;
            _mapper = mapper;
            _programmeRepository = programmeRepository;
        }

        // GET: home/index
        public async Task<IActionResult> Dashboard()
        {
                       
                // Product Repository, Get Available Products

                // If Product is NOT Empty

               

                return View();
        }

        // GET: home/index
        //[Route("")]
        //[Route("Index")]
        //[Route("/")]

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Proposalonline Dashboard";

            //var tzs = TimeZoneInfo.GetSystemTimeZones ();
            //foreach (var tz in tzs)
            //	Console.WriteLine (tz.DisplayName + " " + tz.Id);

            DashboardViewModel model = new DashboardViewModel();
            model.ProductItems = new List<ProductItemV2>();
            model.DealItems = new List<ProductItem>();
            model.CriticalTaskItems = new List<TaskItem>();
            model.ImportantTaskItems = new List<TaskItem>();
            model.CompletedTaskItems = new List<TaskItem>();

            var c = System.Threading.Thread.CurrentThread.CurrentCulture;
            var user = await CurrentUser();
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
                foreach (Programme programme in _programmeRepository.FindAll())
                {

                    //if (!hasRole && !hasViewAllRole)
                        //continue;

                    List<DealItem> deals = new List<DealItem>();
                    //foreach (ClientProgramme client in programme.ClientProgrammes.OrderBy(cp => cp.Owner.Name).OrderBy(cp => cp.DateCreated))
                    //{

                    //string status = client.InformationSheet.Status;

                    //deals.Add(new DealItem
                    //{
                    //    Id = client.InformationSheet.Id.ToString(),
                    //    Name = programme.Name + " for " + client.Owner.Name,
                    //    Status = (status == "Submitted") ? "Submitted on " + client.InformationSheet.SubmitDate.ToString("g") : status			// Move into ClientProgramme?
                    //});

                    //if (CurrentUser.Organisations.Contains(client.Owner))
                    //{
                    //    model.DealItems.Add(new ProductItem
                    //    {
                    //        Languages = languages,
                    //        Name = client.BaseProgramme.Name + " for " + client.Owner.Name,
                    //        Description = proposal.Description,
                    //        RedirectLink = proposal.URL,
                    //        Status = status
                    //    });
                    //}
                    //}

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



                foreach (var product in _productRepositoy.FindAll().Where(p => p.Public && !p.IsBaseProduct))
                {
                    foreach (var org in user.Organisations)
                    {
                        var informationForList = await _customerInformationService.GetAllInformationFor(org);
                        var answerSheets = informationForList.Where(s => s.Product == product).OrderByDescending(s => s.DateCreated);

                        // conditions
                        // no uis = display start
                        // started uis = display started
                        // finished uis = display view/update/renew
                        // updated/renewed uis = display view only

                        // assume no uis
                        //if (answerSheets != null && answerSheets.Count () > 0) {
                        //	foreach (var answerSheet in answerSheets) {
                        //		// clear buttons
                        //		var productItem = new ProductItem {
                        //			Buttons = new List<ButtonItem> (),
                        //			Description = product.Description,
                        //			Languages = product.Languages,
                        //			Name = product.Name + " for " + org.Name,
                        //			Status = answerSheet.Status
                        //		};
                        //		if (answerSheet.Status == "Submitted") {
                        //			productItem.Buttons.Add (new ButtonItem { Classes = "fa fa-search", RedirectLink = "" + answerSheet.Id, Text = "View" });
                        //			if (answerSheet.NextInformationSheet == null) {
                        //				productItem.Buttons.Add (new ButtonItem { Classes = "fa fa-pencil", RedirectLink = "" + answerSheet.Id, Text = "Update" });
                        //				productItem.Buttons.Add (new ButtonItem { Classes = "fa fa-repeat", RedirectLink = "" + answerSheet.Id, Text = "Renew" });
                        //			}
                        //		}
                        //		else if (answerSheet.Status == "Started")
                        //			productItem.Buttons.Add (new ButtonItem { Classes = "fa fa-arrow-right", RedirectLink = "" + answerSheet.Id, Text = "Go" });
                        //		else
                        //			productItem.Buttons.Add (new ButtonItem { Classes = "fa fa-arrow-right", RedirectLink = "" + answerSheet.Id, Text = "Go" });

                        //		model.ProductItems.Add (productItem);
                        //	}
                        //}
                        //else {
                        //	model.ProductItems.Add (new ProductItem {
                        //		Buttons = new List<ButtonItem> {
                        //			new ButtonItem { Classes = "fa fa-pencil", RedirectLink = "" + product.Id, Text = "Get" }
                        //		},
                        //		Description = product.Description,
                        //		Languages = product.Languages,
                        //		Name = product.Name + " for " + org.Name,
                        //		Status = "Not Subscribed"
                        //	});
                        //}

                        if (answerSheets != null && answerSheets.Count() > 0)
                        {
                            foreach (var answerSheet in answerSheets)
                            {
                                if (answerSheet.NextInformationSheet != null)
                                    continue;

                                ProductItemV2 pItem = new ProductItemV2
                                {
                                    Description = product.Description,
                                    Languages = product.Languages,
                                    Name = product.Name + " for " + org.Name,
                                    SheetHistory = new List<KeyValuePair<string, Guid>>(),
                                    SheetId = "" + answerSheet.Id,
                                    Status = answerSheet.Status
                                };

                                ClientInformationSheet currentSheet = answerSheet;
                                do
                                {
                                    pItem.SheetHistory.Add(new KeyValuePair<string, Guid>(string.Format("{0} for {1} (Submitted at: {2})", currentSheet.Product.Name, org.Name,
                                                                                                           LocalizeTime(currentSheet.SubmitDate)), currentSheet.Id));
                                    currentSheet = currentSheet.PreviousInformationSheet;
                                }
                                while (currentSheet != null);

                                model.ProductItems.Add(pItem);
                            }
                        }
                    }
                }

                //foreach (var task in _taskingService.GetAllTasksFor(CurrentUser))
                //{
                //    var taskItem = Mapper.Map<TaskItem>(task);
                //    taskItem.DueDate = LocalizeTime(task.DueDate);
                //    if (task.Completed)
                //    {
                //        model.CompletedTaskItems.Add(taskItem);
                //        continue;
                //    }
                //    if (task.Priority == 1)
                //        model.CriticalTaskItems.Add(taskItem);
                //    else
                //        model.ImportantTaskItems.Add(taskItem);
                //}

                return View("Index_DEMO", model);
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

            //removed as we are no longer using servicestack
            //foreach (string server in servers)
            //{
            //    JsonServiceClient client = null;

            //    IList<string> languages = new List<string>();
            //    languages.Add("nz");

            //    bool staging = server.Contains("demo") || server.Contains("staging");
            //    bool isAuthenticatedForServer = true;


            //    //removed as we are no longer using servicestack
            //    try
            //    {
            //        client = new JsonServiceClient(server)
            //        {
            //            UserName = "admin",
            //            Password = "admin"
            //        };

            //        client.Timeout = TimeSpan.FromSeconds(10);
            //        client.AlwaysSendBasicAuthHeader = true;

            //        SchemesResponse schemesResponse = client.Post(new SchemesRequest() { UserId = CurrentUser.Id });

            //        foreach (var scheme in schemesResponse.Schemes)
            //        {
            //            model.ProductItems.Add(new ProductItemV2
            //            {
            //                Languages = languages,
            //                Name = scheme.Name + (staging ? " (Staging)" : string.Empty),
            //                Description = scheme.Description,
            //                RedirectLink = scheme.URL
            //            });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        WebServiceException webEx = ex as WebServiceException;
            //        if (webEx != null && webEx.StatusCode == 403)
            //            isAuthenticatedForServer = false;
            //        else
            //            _logger.Error(ex, "Failed to connect to: " + server);
            //    }

            //    //removed as we are no longer using servicestack
            //    //if we aren't authenticated for this server, don't bother asking for proposals
            //    if (isAuthenticatedForServer)
            //        {
            //            try
            //            {
            //                ProposalsResponse proposalResponse = client.Post(new ProposalsRequest() { UserId = CurrentUser.Id });

            //                foreach (var proposal in proposalResponse.Proposals)
            //                {
            //                    model.DealItems.Add(new ProductItem
            //                    {
            //                        Languages = languages,
            //                        Name = proposal.CompanyName,
            //                        Description = proposal.Description,
            //                        RedirectLink = proposal.URL,
            //                        Status = proposal.Status
            //                    });
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                ErrorSignal.FromCurrentContext().Raise(ex);
            //            }
            //        }

            //}


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

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View("Search");

        }

        [HttpPost]
        public async Task<IActionResult> Search(String Value)
        {
            //return Redirect("/EditBillingConfiguration" + programmeId);
            //return Content("/Home/ViewProgrammeByFilter/" + value);
            return Content("/Home/ViewProgrammeByFilter/?value=" + Value);

            //return Json(Value);
            //return RedirectToAction("ViewProgrammeByFilter", new { value = Value });


        }
        [HttpGet]
        public async Task<IActionResult> ViewProgrammeByFilter( String value)
        {
            ProgrammeItem model = new ProgrammeItem();

            List<DealItem> deals = new List<DealItem>();
            var user = await CurrentUser();

            if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
            {
                var informationForList = await _clientInformationService.GetAllInformationFor(value);
                foreach (ClientInformationSheet sheet in informationForList)
                {

                    ClientProgramme client = sheet.Programme;

                        string status = client.InformationSheet.Status;
                        string referenceid = client.InformationSheet.ReferenceId;
                        string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy h:mm tt");
                        string localDateSubmitted = null;

                        if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                        {
                            localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy h:mm tt");
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
                
                if(sheet2 != null)
                {
                    ClientProgramme client = sheet2.Programme;

                    string status = client.InformationSheet.Status;
                    string referenceid = client.InformationSheet.ReferenceId;
                    string localDateCreated = LocalizeTime(client.InformationSheet.DateCreated.GetValueOrDefault(), "dd/MM/yyyy h:mm tt");
                    string localDateSubmitted = null;

                    if (client.InformationSheet.Status != "Not Started" && client.InformationSheet.Status != "Started")
                    {
                        localDateSubmitted = LocalizeTime(client.InformationSheet.SubmitDate, "dd/MM/yyyy h:mm tt");
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


        [HttpGet]
        public async Task<IActionResult> ViewProgramme(Guid id)
        {
            ProgrammeItem model = new ProgrammeItem();
            var user = await CurrentUser();
            Programme programme = await _programmeRepository.GetByIdAsync(id);
            List<DealItem> deals = new List<DealItem>();

            if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsInsurer || user.PrimaryOrganisation.IsTC)
            {
                foreach (ClientProgramme client in programme.ClientProgrammes.OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {

                    string status = client.InformationSheet.Status;
                    string referenceid = client.InformationSheet.ReferenceId;
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
                        LocalDateCreated = localDateCreated,
                        LocalDateSubmitted = localDateSubmitted,
                        Status = status,
                        ReferenceId = referenceid// Move into ClientProgramme?
                    });
                }
            } else
            {
                foreach (ClientProgramme client in programme.ClientProgrammes.Where(cp => cp.Owner == user.PrimaryOrganisation).OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
                {

                    string status = client.InformationSheet.Status;
                    string referenceid = client.InformationSheet.ReferenceId;
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
            } else
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

        // GET: home/inbox
        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            return View();
        }

        // GET: home/calendar
        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            return View();
        }

        // GET: home/google-map
        [HttpGet]
        public async Task<IActionResult> GoogleMap()
        {
            return View();
        }

        // GET: home/widgets
        [HttpGet]
        public async Task<IActionResult> Widgets()
        {
            //[TEST] to initialize the theme setter
            //could be called via jQuery or somewhere...
            throw new Exception("Method needs to be re-written");
            //Settings.SetValue<string>("config:CurrentTheme", "smart-style-5");

            return View();
        }

        // GET: home/chat
        [HttpGet]
        public async Task<IActionResult> Chat()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Customer()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewTask(Guid Id)
        {
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

        //[HttpPost]
        //public async Task<IActionResult> AddTask(string taskCategory, string taskDueDate, string taskDetail, string taskDescription )
        //{
        //    User user = _userService.GetUser(CurrentUser.Id);
        //    DateTime time = Convert.ToDateTime(taskDueDate, CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        //    UserTask task = new UserTask(user, user.PrimaryOrganisation, user.FirstName, time)
        //    {
        //        Description = taskDescription,
        //        Details = taskDetail,
        //        Priority = Convert.ToInt32(taskCategory),
        //        TaskUrl = "Home/Task/ViewTask",
        //        IsActive = true,

        //    };
        //    _taskingService.CreateTaskFor(task);

        //    return Redirect("~/Home/Index");
        //}
    }
}