using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using SystemDocument = TechCertain.Domain.Entities.Document;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using HtmlToOpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace TechCertain.WebUI.Controllers
{
	//[Authorize]
    public class FileController : BaseController
    {		
		IUnitOfWork _unitOfWork;
		IFileService _fileService;
		IMapperSession<SystemDocument> _documentRepository;
		IMapperSession<Image> _imageRepository;
        IMapperSession<Product> _productRepository;
        IHttpContextAccessor _httpContextAccessor;

        string _appData = "~/App_Data/";
		string _uploadFolder = "uploads";

		public FileController(IUserService userRepository, SignInManager<DealEngineUser> signInManager, DealEngineDBContext dealEngineDBContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IFileService fileService,
		                      IMapperSession<SystemDocument> documentRepository, IMapperSession<Image> imageRepository, IMapperSession<Product> productRepository)
			: base (userRepository)
		{			
			_unitOfWork = unitOfWork;
			_fileService = fileService;
			_documentRepository = documentRepository;
			_imageRepository = imageRepository;
            _productRepository = productRepository;
        }

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Image(string id)
		{

            //string uploadFolder = "~/App_Data/uploads/";
            //var serverPath = System.IO.Path.Combine(uploadFolder, fileName);
            //var file = File(Server.MapPath(fileName),
            //    System.Net.Mime.MediaTypeNames.Application.Octet,
            //    System.IO.Path.GetFileName(fileName));
            //return file;

            //if (string.IsNullOrWhiteSpace(id))
            //    return new HttpNotFoundResult();

            //string extension = Path.GetExtension(fileName).ToLower();
            //string mediaType = (extension == "jpeg" || extension == "jpg") ? MediaTypeNames.Image.Jpeg :
            //    (extension == "gif") ? MediaTypeNames.Image.Gif :
            //    (extension == "tiff") ? MediaTypeNames.Image.Tiff : MediaTypeNames.Application.Octet;
            throw new Exception("This method needs to be re-written");
            Image img = _fileService.GetImage(id);
            return File(img.Contents, img.ContentType, img.Name);

        }

		[HttpGet]
		public async Task<IActionResult> GetDocument (Guid id, string format)
		{
            //throw new Exception("This method needs to be re-written");
            //if (id == Guid.Empty)
            //    return new HttpNotFoundResult();

            SystemDocument doc = _documentRepository.GetByIdAsync(id).Result;
            string extension = "";
            if (doc.ContentType == MediaTypeNames.Text.Html)
            {
                extension = ".html";

                if (format == "docx")
                {
                    // Testing HtmlToOpenXml
                    string html = _fileService.FromBytes(doc.Contents);
                    using (MemoryStream virtualFile = new MemoryStream())
                    {
                        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(virtualFile, WordprocessingDocumentType.Document))
                        {
                            // Add a main document part. 
                            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                            new DocumentFormat.OpenXml.Wordprocessing.Document(new Body()).Save(mainPart);

                            HtmlConverter converter = new HtmlConverter(mainPart);
                            converter.ImageProcessing = ImageProcessing.AutomaticDownload;
                            converter.ParseHtml(html);
                        }
                        return File(virtualFile.ToArray(), MediaTypeNames.Application.Octet, doc.Name + ".docx");
                    }
                }
            }
            return File(doc.Contents, doc.ContentType, doc.Name + extension);
        }

		//void UploadFile(string folder, HttpPostedFile file)
		//{
		//	var fileName = Path.GetFileName(file.FileName);
		//	var serverPath = Path.Combine ("~/App_Data/uploads", fileName);
		//	var absolutePath = Server.MapPath(serverPath);
		//	file.SaveAs(absolutePath);
		//}

		//async Task<IActionResult> DownloadFile(string folder, string fileName, string mediaType)
		//{
		//	try
		//	{
		//		var serverFile = Path.Combine (_appData, folder, fileName);
		//		string absolutePath = Server.MapPath(serverFile);
		//		if (!System.IO.File.Exists(absolutePath))
		//			return HttpNotFound();
		//		return File (Server.MapPath (serverFile), mediaType, Path.GetFileName (fileName));
		//	}
		//	catch (Exception ex) {
		//		ErrorSignal.FromCurrentContext().Raise(ex);
		//		return HttpNotFound();
		//	}
		//}

		[HttpGet]
		public async Task<IActionResult> CreateDocument (string id)
		{
			DocumentViewModel model = new DocumentViewModel ();

			if (string.IsNullOrWhiteSpace(id))
				return View (model);

			Guid documentId = Guid.Empty;
			if (!Guid.TryParse (id, out documentId))
				throw new Exception (id + " is not a valid document Id");

			if (documentId == Guid.Empty)
				return View (model);

			SystemDocument document = _documentRepository.GetByIdAsync(documentId).Result;
			if (document == null)
				throw new Exception ("Unable to update document: Could not find document with id " + id);

            model.DocumentId = document.Id;
			model.Name = document.Name;
			model.Description = document.Description;
			model.DocumentType = document.DocumentType;
			model.Content = _fileService.FromBytes (document.Contents);

			return View (model);
		}

		[HttpPost]
		public async Task<IActionResult> CreateDocument (DocumentViewModel model)
		{
            if (model.DocumentId != Guid.Empty)
            {
                SystemDocument document = _documentRepository.GetByIdAsync(model.DocumentId).Result;
                if (document != null)
                {
                    using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                    {
                        document.Name = model.Name;
                        document.DocumentType = model.DocumentType;
                        document.Description = model.Description;
                        document.Contents = _fileService.ToBytes(System.Net.WebUtility.HtmlDecode(model.Content));
                        document.LastModifiedBy = CurrentUser;
                        document.LastModifiedOn = DateTime.UtcNow;
                        uow.Commit();
                    }
                }

            } else
            {
                SystemDocument document = new SystemDocument(CurrentUser, model.Name, MediaTypeNames.Text.Html, model.DocumentType);
                document.Description = model.Description;
                document.Contents = _fileService.ToBytes(System.Net.WebUtility.HtmlDecode(model.Content));
                document.OwnerOrganisation = CurrentUser.PrimaryOrganisation;
                document.IsTemplate = true;
                _documentRepository.AddAsync(document);
            }

			return View (model);
		}

		[HttpGet]
		public async Task<IActionResult> ManageDocuments ()
		{
			BaseListViewModel<DocumentInfoViewModel> models = new BaseListViewModel<DocumentInfoViewModel> ();

            var docs = _documentRepository.FindAll().Where(d => CurrentUser.Organisations.Contains(d.OwnerOrganisation) && !d.DateDeleted.HasValue);

            if (CurrentUser.PrimaryOrganisation.IsBroker || CurrentUser.PrimaryOrganisation.IsTC || CurrentUser.PrimaryOrganisation.IsInsurer)
            {
                docs = _documentRepository.FindAll().Where(d => !d.DateDeleted.HasValue && d.IsTemplate);
            }
            
			foreach (SystemDocument doc in docs) {
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
                    default:
                        {
                            throw new Exception(string.Format("Can not get Document Type for document", doc.Id));
                        }
                }
                //var product = _productRepository.FindAll().Where(prod => !prod.DateDeleted.HasValue && prod.Documents.Contains(doc)).First();
                models.Add(new DocumentInfoViewModel {
                    DisplayName = doc.Name,
                    //ProductName = product.Name,
                    Type = documentType,
                    Owner = doc.OwnerOrganisation.Name,
					Id = doc.Id
				});
			}

			return View (models);
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
