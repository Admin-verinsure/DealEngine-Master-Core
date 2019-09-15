using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using SystemDocument = TechCertain.Domain.Entities.Document;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using techcertain2015rebuildcore.Models.ViewModels;

namespace techcertain2015rebuildcore.Controllers
{
	[Authorize]
    public class FileController : BaseController
    {
		ILogger _logger;
		IUnitOfWorkFactory _unitOfWork;
		IFileService _fileService;
		IRepository<SystemDocument> _documentRepository;
		IRepository<Image> _imageRepository;

		string _appData = "~/App_Data/";
		string _uploadFolder = "uploads";

		public FileController(IUserService userRepository, ILogger logger, IUnitOfWorkFactory unitOfWork, IFileService fileService,
		                      IRepository<SystemDocument> documentRepository, IRepository<Image> imageRepository)
			: base (userRepository)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_fileService = fileService;
			_documentRepository = documentRepository;
			_imageRepository = imageRepository;
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Image(string id)
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
		public ActionResult GetDocument (Guid id, string format)
		{
            throw new Exception("This method needs to be re-written");
   //         if (id == Guid.Empty)
			//	return new HttpNotFoundResult ();

			//SystemDocument doc = _documentRepository.GetById (id);
			//string extension = "";
			//if (doc.ContentType == MediaTypeNames.Text.Html) {
			//	extension = ".html";

			//	if (format == "docx") {
			//		// Testing HtmlToOpenXml
			//		string html = _fileService.FromBytes (doc.Contents);
			//		using (MemoryStream virtualFile = new MemoryStream ()) {
			//			using (WordprocessingDocument wordDocument = WordprocessingDocument.Create (virtualFile, WordprocessingDocumentType.Document)) {
			//				// Add a main document part. 
			//				MainDocumentPart mainPart = wordDocument.AddMainDocumentPart ();
			//				new DocumentFormat.OpenXml.Wordprocessing.Document (new Body ()).Save (mainPart);

			//				HtmlConverter converter = new HtmlConverter (mainPart);
			//				converter.ImageProcessing = ImageProcessing.AutomaticDownload;
			//				converter.ParseHtml (html);
			//			}
			//			return File (virtualFile.ToArray (), MediaTypeNames.Application.Octet, doc.Name + ".docx");
			//		}
			//	}
			//}
			//return File (doc.Contents, doc.ContentType, doc.Name + extension);
		}

		//void UploadFile(string folder, HttpPostedFile file)
		//{
		//	var fileName = Path.GetFileName(file.FileName);
		//	var serverPath = Path.Combine ("~/App_Data/uploads", fileName);
		//	var absolutePath = Server.MapPath(serverPath);
		//	file.SaveAs(absolutePath);
		//}

		//ActionResult DownloadFile(string folder, string fileName, string mediaType)
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
		public ActionResult CreateDocument (string id)
		{
			DocumentViewModel model = new DocumentViewModel ();

			if (string.IsNullOrWhiteSpace(id))
				return View (model);

			Guid documentId = Guid.Empty;
			if (!Guid.TryParse (id, out documentId))
				throw new Exception (id + " is not a valid document Id");

			if (documentId == Guid.Empty)
				return View (model);

			SystemDocument document = _documentRepository.GetById (documentId);
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
		public ActionResult CreateDocument (DocumentViewModel model)
		{
            if (model.DocumentId != Guid.Empty)
            {
                SystemDocument document = _documentRepository.GetById(model.DocumentId);
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

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    _documentRepository.Add(document);
                    uow.Commit();
                }
            }

			return View (model);
		}

		[HttpGet]
		public ActionResult ManageDocuments ()
		{
			BaseListViewModel<DocumentInfoViewModel> models = new BaseListViewModel<DocumentInfoViewModel> ();

            var docs = _documentRepository.FindAll().Where(d => CurrentUser.Organisations.Contains(d.OwnerOrganisation) && !d.DateDeleted.HasValue);

            if (CurrentUser.PrimaryOrganisation.IsBroker || CurrentUser.PrimaryOrganisation.IsTC || CurrentUser.PrimaryOrganisation.IsInsurer)
            {
                docs = _documentRepository.FindAll().Where(d => !d.DateDeleted.HasValue);
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
                models.Add(new DocumentInfoViewModel {
                    DisplayName = doc.Name,
                    Type = documentType,
                    Owner = doc.OwnerOrganisation.Name,
					Id = doc.Id
				});
			}

			return View (models);
		}

		[HttpGet]
		public ActionResult Render (string id)
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
