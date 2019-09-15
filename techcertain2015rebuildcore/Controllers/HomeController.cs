﻿#region Using


using AutoMapper;
using Elmah;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using Microsoft.AspNetCore.Authorization;
using techcertain2019core.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using techcertain2019core.Controllers;

#endregion

namespace techcertain2019core.Controllers
{
    [Route("Home")]
    public class HomeController : BaseController
    {
        ILogger _logger;
        IMapper _mapper;
        IInformationTemplateService _informationService;
        ICilentInformationService _customerInformationService;
        IPrivateServerService _privateServerService;
        ITaskingService _taskingService;

        IRepository<Product> _productRepositoy;
        IRepository<Programme> _programmeRepository;

        public HomeController(ILogger logger, IMapper mapper, IUserService userRepository, IInformationTemplateService informationService,
                              ICilentInformationService customerInformationService, IPrivateServerService privateServerService,
                              ITaskingService taskingService, IRepository<Product> productRepository, IRepository<Programme> programmeRepository)
            : base(userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _informationService = informationService;
            _customerInformationService = customerInformationService;
            _privateServerService = privateServerService;
            _taskingService = taskingService;

            _productRepositoy = productRepository;
            _programmeRepository = programmeRepository;
        }

        // GET: home/index
        public ActionResult Dashboard()
        {
                       
                // Product Repository, Get Available Products

                // If Product is NOT Empty

                // List Products

                // If Product is Empty

                // Loop Through All Available Servers

                // Product Awareness and Migration
                // New System
                // First Login Call all servers to get products and also permissions
                // Save all products returned if not saved 
                // Assign User to the specific product they have permissions to
                // Load Products from Application Database

                // Save List of private servers we maintain

                // Old System
                // Add New User
                // Call New System To Log in
                // Perform 1st Time Log In

                // Give existing User new Product Permissions
                // Push notification to Public Server that new Permissions has been given 
                // Add link between Product and User

                // Take away existing User Product Permissions
                // Push notification to Public Server that Permission to product has been taken away
                // Delete Link between Product and User

                // Add Product
                // Push notification to public server that new product is added
                // Add new Product Record

                // Remove Product            
                // Push notification that a product has been removed 
                // Set product record to deleted and save a time stamp


                return View();
        }

        // GET: home/index
        [Route("")]
        [Route("Index")]
        [Route("/")]
        public ActionResult Index()
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

            if (DemoEnvironment)
            {
                model.DisplayDeals = true;
                model.DisplayProducts = false;
                model.DisplayRole = "Client";

                IList<string> languages = new List<string>();
                languages.Add("nz");

                model.ProgrammeItems = new List<ProgrammeItem>();
                foreach (Programme programme in _programmeRepository.FindAll())
                {
                    var userRoles = CurrentUser.GetRoles().ToArray();
                    var hasRole = false;
                    var hasViewAllRole = userRoles.FirstOrDefault(r => r.Name == "CanViewAllInformation") != null;

                    if (programme.Permissions != null)
                    {
                        var userRoleIds = userRoles.Select(r => r.Id).ToArray();
                        hasRole = userRoleIds.Contains(programme.Permissions.ViewInformationRole.Id);
                    }
                    else if (!hasViewAllRole)
                        continue;

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

                    foreach (var task in _taskingService.GetAllTasksFor(CurrentUser))
                    {
                        var taskItem = _mapper.Map<TaskItem>(task);
                        taskItem.DueDate = LocalizeTime(task.DueDate);
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
                    foreach (var org in CurrentUser.Organisations)
                    {
                        var answerSheets = _customerInformationService.GetAllInformationFor(org).Where(s => s.Product == product).OrderByDescending(s => s.DateCreated);

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
                var privateServers = _privateServerService.GetAllPrivateServers().ToList();

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
        public ActionResult ViewProgramme(Guid id)
        {
            ProgrammeItem model = new ProgrammeItem();

            Programme programme = _programmeRepository.GetById(id);
            List<DealItem> deals = new List<DealItem>();

            if (CurrentUser.PrimaryOrganisation.IsBroker || CurrentUser.PrimaryOrganisation.IsInsurer || CurrentUser.PrimaryOrganisation.IsTC)
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
                foreach (ClientProgramme client in programme.ClientProgrammes.Where(cp => cp.Owner == CurrentUser.PrimaryOrganisation).OrderBy(cp => cp.DateCreated).OrderBy(cp => cp.Owner.Name))
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

            if (CurrentUser.PrimaryOrganisation.IsBroker)
            {
                model.CurrentUserIsBroker = "True";
            } else
            {
                model.CurrentUserIsBroker = "False";
            }
            if (CurrentUser.PrimaryOrganisation.IsInsurer)
            {
                model.CurrentUserIsInsurer = "True";
            }
            else
            {
                model.CurrentUserIsInsurer = "False";
            }
            if (CurrentUser.PrimaryOrganisation.IsTC)
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
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: home/calendar
        [HttpGet]
        public ActionResult Calendar()
        {
            return View();
        }

        // GET: home/google-map
        [HttpGet]
        public ActionResult GoogleMap()
        {
            return View();
        }

        // GET: home/widgets
        [HttpGet]
        public ActionResult Widgets()
        {
            //[TEST] to initialize the theme setter
            //could be called via jQuery or somewhere...
            throw new Exception("Method needs to be re-written");
            //Settings.SetValue<string>("config:CurrentTheme", "smart-style-5");

            return View();
        }

        // GET: home/chat
        [HttpGet]
        public ActionResult Chat()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Customer()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }

        [HttpGet]
        public ActionResult ViewTask(Guid Id)
        {
            UserTask task = _taskingService.GetTask(Id);
            UserTaskViewModel model = new UserTaskViewModel
            {
                Details = task.Details,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                ClientName = task.ClientName,
                Completed = task.Completed,
                CompletedBy = task.CompletedBy,
                For = task.For
            };

            return View("~/ViewTask/"+ task.Id.ToString(), model);
        }

        [HttpPost]
        public ActionResult AddTask(string taskCategory, string taskDueDate, string taskDetail, string taskDescription )
        {
            User user = _userService.GetUser(CurrentUser.Id);
            DateTime time = Convert.ToDateTime(taskDueDate, CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            UserTask task = new UserTask(user, user.PrimaryOrganisation, user.FirstName, time)
            {
                Description = taskDescription,
                Details = taskDetail,
                Priority = Convert.ToInt32(taskCategory),
                TaskUrl = "Home/Task/ViewTask",
                IsActive = true,
                
            };
            _taskingService.CreateTaskFor(task);
            
            return Redirect("~/Home/Index");
        }
    }
}