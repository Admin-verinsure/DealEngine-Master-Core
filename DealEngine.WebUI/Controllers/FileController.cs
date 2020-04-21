using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using SystemDocument = DealEngine.Domain.Entities.Document;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using HtmlToOpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DealEngine.WebUI.Controllers
{
	[Authorize]
    public class FileController : BaseController
    {		
		IUnitOfWork _unitOfWork;
		IFileService _fileService;
        IProgrammeService _programmeService;
		IMapperSession<SystemDocument> _documentRepository;
		IMapperSession<Image> _imageRepository;
        IMapperSession<Product> _productRepository;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<FileController> _logger;

  //      string _appData = "~/App_Data/";
		//string _uploadFolder = "uploads";

		public FileController(
            ILogger<FileController> logger,
            IProgrammeService programmeService,
            IApplicationLoggingService applicationLoggingService,
            IUserService userRepository, 
            IUnitOfWork unitOfWork, 
            IFileService fileService,
            IMapperSession<SystemDocument> documentRepository, 
            IMapperSession<Image> imageRepository, 
            IMapperSession<Product> productRepository
            )
			: base (userRepository)
		{
            _programmeService = programmeService;
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _unitOfWork = unitOfWork;
			_fileService = fileService;
			_documentRepository = documentRepository;
			_imageRepository = imageRepository;
            _productRepository = productRepository;
        }

		[HttpGet]
		public async Task<IActionResult> GetDocument (Guid id, string format)
		{
            User user = null;
            try
            {
                SystemDocument doc = await _documentRepository.GetByIdAsync(id);
                string extension = "";
                if (doc.ContentType == MediaTypeNames.Text.Html)
                {
                    extension = ".html";

                    if (format == "docx")
                    {
                        // Testing HtmlToOpenXml
                        string html = _fileService.FromBytes(doc.Contents);
                        string oldhtml = html;



                        using (MemoryStream virtualFile = new MemoryStream())
                        {
                            //using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(virtualFile, false))
                            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(virtualFile, WordprocessingDocumentType.Document))
                            {
                                // Add a main document part. 
                                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();                 
                                new DocumentFormat.OpenXml.Wordprocessing.Document(new Body()).Save(mainPart);
                                string showBorder = "<figure class=\"table\"><table style=\"border-bottom:solid;border-left:solid;border-right:solid;border-top:solid;\"><tbody><tr>";
                                string noBorder = "<figure class=\"table\"><table><tbody><tr>";

                                // Create document with a "main part" to it. No data has been added yet.
                                if (html.Contains(showBorder)){
                                    html = html.Replace(showBorder, "<table border=\"1\"><tbody><tr>");
                                    // NEED TO DO CLOSING TAGS TOO      width=\"100%\" align=\"center\"     <tr style=\"font-weight:bold\">
                                }
                                if (html.Contains(noBorder)){
                                    html = html.Replace(noBorder, "<table border=\"0\"><tbody><tr>");
                                    // NEED TO DO CLOSING TAGS TOO      width=\"100%\" align=\"center\"     <tr style=\"font-weight:bold\">
                                }

                                // Create a new html convertor with input mainPart
                                HtmlConverter converter = new HtmlConverter(mainPart);
                                
                                // Need to figure out how to add classes to style the document... (adding to the top of HTML document doesn't work, also lots of the table styling css doesn't actually work. Just the old way works where style isn't specified e.g <table width=\"100%\" border=\"0\"><tr style=\"font-weight: bold\"><td>Studio</td><td colspan=\"2\")
                                // converter.HtmlStyles.DefaultStyle = converter.HtmlStyles.GetStyle("testClass");
                                // converter.RefreshStyles();
                                
                                converter.ImageProcessing = ImageProcessing.AutomaticDownload;
                                converter.ParseHtml(html);
                            }
                            return File(virtualFile.ToArray(), MediaTypeNames.Application.Octet, doc.Name + ".docx");
                        }
                    }
                }
                return File(doc.Contents, doc.ContentType, doc.Name + extension);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProductIndex(string Programme)
        {
            var productList = new List<Product>();
            var programme = await _programmeService.GetProgramme(Guid.Parse(Programme));
            productList = programme.Products.ToList();

            return View(productList);
        }

		[HttpGet]
		public async Task<IActionResult> CreateDocument (string id, string productId)
		{
			DocumentViewModel model = new DocumentViewModel ();
            User user = null;
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return View(model);

                Guid documentId = Guid.Empty;
                if (!Guid.TryParse(id, out documentId))
                    throw new Exception(id + " is not a valid document Id");

                if (documentId == Guid.Empty)
                    return View(model);

                SystemDocument document = await _documentRepository.GetByIdAsync(documentId);
                if (document == null)
                    throw new Exception("Unable to update document: Could not find document with id " + id);

                model.DocumentId = document.Id;
                model.Name = document.Name;
                model.Description = document.Description;
                model.DocumentType = document.DocumentType;
                model.Content = _fileService.FromBytes(document.Contents);
                model.ProductId = productId;

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpPost]
		public async Task<IActionResult> CreateDocument (DocumentViewModel model)
		{
            User user = null;
            SystemDocument document = null;
            Product product = null;
            try
            {                
                user = await CurrentUser();
                if (model.DocumentId != Guid.Empty)
                {
                    document = await _documentRepository.GetByIdAsync(model.DocumentId);
                    if (document != null)
                    {
                        document.DateDeleted = DateTime.Now;
                        await _documentRepository.AddAsync(document);
                    }

                }
                
                document = new SystemDocument(user, model.Name, MediaTypeNames.Text.Html, model.DocumentType);
                document.Description = model.Description;
                document.Contents = _fileService.ToBytes(System.Net.WebUtility.HtmlDecode(model.Content));
                document.OwnerOrganisation = user.PrimaryOrganisation;
                document.IsTemplate = true;
                await _documentRepository.AddAsync(document);
                //if (model.ProductId != null)
                //{
                //    product = await _productRepository.GetByIdAsync(Guid.Parse(model.ProductId));
                //    product.Documents.Add(document);
                //    await _productRepository.AddAsync(product);
                //}

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpGet]
		public async Task<IActionResult> ManageDocuments()
		{
			BaseListViewModel<DocumentInfoViewModel> models = new BaseListViewModel<DocumentInfoViewModel> ();
            User user = null;
            try
            {
                user = await CurrentUser();
                List<SystemDocument> docs = _documentRepository.FindAll().Where(d => d.DateDeleted == null && user.PrimaryOrganisation == d.OwnerOrganisation).ToList();

                if (user.PrimaryOrganisation.IsBroker || user.PrimaryOrganisation.IsTC || user.PrimaryOrganisation.IsInsurer)
                {
                    //docs = _documentRepository.FindAll().Where(d => !d.DateDeleted.HasValue && d.IsTemplate);
                    //if(productId != null)
                    //{
                    //    var products = await _productRepository.GetByIdAsync(Guid.Parse(productId));
                    //    docs = products.Documents.ToList();
                    //}

                }

                if(docs.Count != 0)
                {
                    foreach (SystemDocument doc in docs)
                    {
                        string documentType = "";
                        switch (doc.DocumentType)
                        {
                            case 0:
                                {
                                    documentType = "Wording";
                                    break;
                                }
                            case 1:
                                {
                                    documentType = "Certificate";
                                    break;
                                }
                            case 2:
                                {
                                    documentType = "Schedule";
                                    break;
                                }
                            case 3:
                                {
                                    documentType = "Payment Confirmation";
                                    break;
                                }
                            case 4:
                                {
                                    documentType = "Invoice";
                                    break;
                                }
                            case 5:
                                {
                                    documentType = "Advisory";
                                    break;
                                }
                            case 6:
                                {
                                    documentType = "Sub-Certificate";
                                    break;
                                }
                            default:
                                {
                                    throw new Exception(string.Format("Can not get Document Type for document", doc.Id));
                                }
                        }
                        //var product = _productRepository.FindAll().Where(prod => !prod.DateDeleted.HasValue && prod.Documents.Contains(doc)).First();
                        models.Add(new DocumentInfoViewModel
                        {
                            DisplayName = doc.Name,
                            //ProductId = productId,
                            Type = documentType,
                            Owner = doc.OwnerOrganisation.Name,
                            Id = doc.Id,
                        });
                    }
                }
                else
                {
                    return RedirectToAction("CreateDocument");                    
                }                

                return View(models);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

		[HttpGet]
		public async Task<IActionResult> Render (string id)
		{
            throw new Exception("Method will need to be re-written");
            //string serverFile = Path.Combine(_appData, _uploadFolder, id);
            //string filepath = Server.MapPath(serverFile);
            //if (System.IO.File.Exists(filepath))
            //    System.IO.File.Delete(filepath);

            //// Create a document by supplying the filepath. 

            //using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
            //{
            //    // Add a main document part. 
            //    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            //    // Create the document structure and add some text.
            //    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            //    Body body = mainPart.Document.AppendChild(new Body());
            //    Paragraph para = body.AppendChild(new Paragraph());
            //    Run run = para.AppendChild(new Run());
            //    run.AppendChild(new Text("Create text in body - CreateWordprocessingDocument. This is a test."));
            //    run.AppendChild(new Text("Second line?"));
            //    Run run2 = para.AppendChild(new Run());
            //    run2.AppendChild(new Text("Second run"));
            //    Paragraph par2 = body.AppendChild(new Paragraph());
            //    Run run3 = par2.AppendChild(new Run());
            //    run3.AppendChild(new Text("Second paragaph"));
            //}
            //return null;
        }
    }
}
