
using System;
using System.Threading.Tasks;
using DealEngine.Services.Interfaces;
using DealEngine.WebUI.Models.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using DealEngine.Infrastructure.FluentNHibernate;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnv;
        IUserService _userService;
        ISerializerationService _serializerationService;
        IClientInformationService _clientService;
        IProgrammeService _programmeService;
        IOrganisationService _organisationService;
        IDataService _dataService;
        IMapperSession<BindDataCG> _dataRepository;

        public ReportController(
            ISerializerationService serializerationService,
            IUserService userService,  
            IWebHostEnvironment hostingEnv,
            IClientInformationService clientService,
            IOrganisationService organisationService,
            IDataService dataService,
            IProgrammeService programmeService,
            IMapperSession<BindDataCG> dataRepository
            )
            : base(userService)
        {
            _serializerationService = serializerationService;
            _hostingEnv = hostingEnv;
            _userService = userService;
            _clientService = clientService;
            _programmeService = programmeService;
            _organisationService = organisationService;
            _dataService = dataService;
            _dataRepository = dataRepository;
        }


        [HttpGet]
        public async Task<ViewResult> CreateReport()
        {
            var Programmes = await _programmeService.GetAllProgrammes();
            var clients = Programmes.FirstOrDefault().ClientProgrammes;
            ReportViewModel model = new ReportViewModel(Programmes);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection model)
        {
            //Guid clientId = Guid.Parse("82ed739d-0795-4602-a2c8-abab017abcb5");
            //Guid programmeId = Guid.Parse("29dfc24b-845d-4d4c-8d5c-ac46003d1e5a");
            //var test = await _dataService.GetData(programmeId);

            Guid clientId = Guid.Parse(model["Id"]);
            var client = await _programmeService.GetClientProgrammebyId(clientId);
            var user = await CurrentUser();
            BindDataCG clientData = new BindDataCG(user, client.InformationSheet);


            //save to db

            await _dataRepository.AddAsync(clientData);


            var list = new List<object>();
            list.Add(clientData);
            string test = await _serializerationService.GetSerializedObject(clientData);
            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\dealengine\DealEngine.WebUI\wwwroot\Report\test2.json", test);

            #region report writer code
            /*#

            FastReport.Utils.RegisteredObjects.AddConnection(typeof(PostgresDataConnection));
            
            // Get the Data and fill the dataset
            DataSet dataSet1 = new DataSet();
            int rowsAdded = 0;
            string dataconnection = String.Format("Server=localhost;Port=5432;" + "User Id=postgres;Password=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;Database=DealEngine;");
            
            try
            {
                await using var conn = new NpgsqlConnection(dataconnection);
                await conn.OpenAsync();
                using (var dataAdapter = new NpgsqlDataAdapter("SELECT username FROM public.\"User\"", conn))
                    rowsAdded = dataAdapter.Fill(dataSet1);
            }

            catch (System.Exception e)
            {
                Console.WriteLine("Failed because " + e);
            }

            // Create and Import the report (modifying it so it will work)
            Report report = new Report();
            report.Load("wwwroot/Report/templates/userPGRES.frx");           
            var currentConnectionString = report.Dictionary.Connections[0].ConnectionString.ToString();
            currentConnectionString = currentConnectionString.Replace("COMPATIBLE=2.2.7.0;", ""); // remove COMPATIBLE=2.2.7.0; from connection string because it breaks it
            report.Dictionary.Connections[0].ConnectionString = currentConnectionString;

            // Combine Data and Report
            report.RegisterData(dataSet1); 
            report.Prepare();

            // Export an HTML
            HTMLExport html = new HTMLExport();
            html.SinglePage = true;
            html.Navigator = false;           
            report.Export(html, "wwwroot/Report/html/user6.html");

            // Export a JPEG
            ImageExport image = new ImageExport();
            image.ImageFormat = ImageExportFormat.Jpeg;
            image.JpegQuality = 90;
            image.Resolution = 72;
            image.SeparateFiles = false;
            report.Export(image, "wwwroot/Report/jpg/user6.jpg");

            // Export a PNG
            ImageExport image2 = new ImageExport();
            image.ImageFormat = ImageExportFormat.Png;
            image.Resolution = 300;
            image.SeparateFiles = true;
            report.Export(image, "wwwroot/Report/jpg/user6.png");

            //TODO Export a PDF
            //TODO Create WebReport instead (make seperate function and link to different button in UI)

            report.Abort();
            */
            #endregion

            return View("~/Views/Report/CreateReport.cshtml");
        }
    }
}

// dataSet1.WriteXml("wwwroot/Report/data/user2.xml"); 
// dataSet1.ReadXml("wwwroot/Report/data/user2.xml");
// report.Load("wwwroot/Report/templates/userXML.frx");

// WebReport webReport = new WebReport();
// DataSet dataSet2 = new DataSet();
// DataSet dataSet3 = new DataSet(); 
// 2 FastReport.Data.PostgresDataConnection 
// one is in Dictionary > Connections > InnerList[ArrayList] 
// where FastReport.Data.PostgresDataConnection[0].ConnectionString
// one is in Dictionary > Connections > List[IList] 
// where FastReport.Data.PostgresDataConnection[0].ConnectionString

// This has to be changed due to the TestReport's being generated on different machine 
// (so db name is different - this has advantage of giving client a database with different
// credentials to what is actually used in production)

// string connectionString3 = "PORT=5432;SERVER=localhost;USER ID=postgres;PASSWORD=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;DATABASE=DealEngine_nathan;";

// Windows String "PORT=5432;KRBSRVNAME=POSTGRES;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE=2.2.7.0;HOST=localhost;USER ID=postgres;PASSWORD=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;DATABASE=DealEngine"
// Linux String "PORT=5432;HOST=localhost;USER ID=postgres;PASSWORD=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;DATABASE=DealEngine_nathan;"

// PostgresDataConnection reportconnection = new PostgresDataConnection();
// reportconnection.ConnectionString = String.Format("HOST=localhost;PORT=5432;" + "USER ID=postgres;PASSWORD=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;DATABASE=DealEngine_nathan;" /*+ "KRBSRVNAME=POSTGRES;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;"*/);
// reportconnection.CreateAllTables();
// report.Dictionary.Connections.Clear();
// report.Dictionary.Connections.Add(reportconnection);
// string reportconnection = String.Format("HOST=localhost;PORT=5432;" + "USER ID=postgres;PASSWORD=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;DATABASE=DealEngine_nathan;");
// report.Dictionary.Connections[0].ConnectionString = reportconnection;
// "PORT=5432;HOST=localhost;USER ID=postgres;PASSWORD=4CkWMKdlBANp2jlACg8dNjgiPur8wTZc;DATABASE=DealEngine"
// KRBSRVNAME=POSTGRES;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE=2.2.7.0;
// dataSet1.Tables[0].ToString();
// var xml2 = dataSet2.GetXml();
// dataSet3.ReadXml("user.xml");
// Report report2 = new Report();
// report2.Load("tabularTest.frx");
// dataSet2.ReadXml("nwind.xml");
// report2.RegisterData(dataSet2.Tables, "Employees");
// report2.RegisterData(dataSet2.Tables[1], "Customers");
// report2.Prepare();
// report2.Export(html, "report2HTMLTEST.html");
// report2.Export(image, "report2JPGTEST.jpg");
// report2.Export(image, "report2PNGTEST.png");

/*

using (Report report2 = new Report())
{
    // Loading a report
    report.Load("TestReport.frx");
    // Preparing a report
    report.Prepare();
    // Creating the HTML export
    using (HTMLExport html = new HTMLExport())
    {
        // Choose the Jpeg pictures
        html.ImageFormat = ImageFormat.Jpeg;
        // We need the saving in multiple streams
        html.SaveStreams = true;
        // Exporting with fake null stream object, html export will keep all files inside
        report.Export(html, (Stream)null);
        // Checking for the exporting
        if (html.GeneratedFiles.Count > 0)
        {
            // Loop for generated files 
            for (int i = 0; i < html.GeneratedFiles.Count; i++)
            {
                // We have several streams, let's save it in files
                using (FileStream file = new FileStream("wwwroot/Reports/" + html.GeneratedFiles[i], FileMode.Create))
                {
                    // You need reset the internal stream position
                    html.GeneratedStreams[i].Position = 0;
                    // Saving a stream in the file
                    html.GeneratedStreams[i].CopyTo(file);
                }                             
            }
        }
    }
}

    HTMLExport html = new HTMLExport();
    html.SinglePage = true;
    html.Navigator = false;
    report.Export(html, "report3.html");
    // We need embedded pictures inside html
    //html.EmbedPictures = true;
    // Enable all report pages in one html file
    html.SinglePage = true;
    // We don't need a subfolder for pictures and additional files
    //html.SubFolder = false;
    // Enable layered HTML
    //html.Layers = true;
    // Turn off the toolbar with navigation
    html.Navigator = false;
    // Save the report in html
    html.Layers = true;
    report.Export(html, "report3.html");
    ReportPage pg1 = new ReportPage();
    pg1.Name = "Page1";
    report.Pages.Add(pg1);
    pg1.ReportTitle = new ReportTitleBand();
    pg1.ReportTitle.Name = "UserReport";
    // set its height to 1.5cm
    pg1.ReportTitle.Height = Units.Centimeters * 1.5f;
    // create group header
    GroupHeaderBand group1 = new GroupHeaderBand();
    group1.Name = "GroupHeader1";
    group1.Height = Units.Centimeters * 1;
    // set group condition
    group1.Condition = "[Products.ProductName].Substring(0, 1)";
    // add group to the page.Bands collection
    pg1.Bands.Add(group1);
    // create group footer
    group1.GroupFooter = new GroupFooterBand();
    group1.GroupFooter.Name = "GroupFooter1";
    group1.GroupFooter.Height = Units.Centimeters * 1;

*/
