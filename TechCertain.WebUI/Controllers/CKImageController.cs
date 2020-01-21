using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models.Image;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;




namespace TechCertain.WebUI.Controllers
{
    public class CKImageController : BaseController
    {
        IMapperSession<CKImage> _ckimageRepository;
        private readonly ICKImageService _ckimageService;
        private readonly IWebHostEnvironment _hostingEnv;
        IMapperSession<User> _userRepository;

        public CKImageController(IUserService userRepository, IMapperSession<CKImage> ckimageRepository, ICKImageService ckimageService, IWebHostEnvironment hostingEnv)
            : base(userRepository)
        {
            _ckimageRepository = ckimageRepository;
            _ckimageService = ckimageService;
            _hostingEnv = hostingEnv;
        }


        [HttpGet]
        public async Task<ViewResult> CKImageHome()
        {
            var list = await  _ckimageService.GetAllImages();
            CKImageUploadViewModel model = new CKImageUploadViewModel();
            model.Item = list;

            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> GetAllImages()
        {
            var model = await _ckimageService.GetAllImages();          
            return View(model);
        }

        public async Task<IActionResult> CKGetAllImages()
        {
            var model = await _ckimageService.GetAllImages();
            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> Upload(CKImageUploadViewModel model)
        {

            if (model != null)
            {
                if (model.Image != null)
                {
                    using (var memstream = new MemoryStream())
                    {
                        model.Image.CopyTo(memstream);
                        // contents = memstream.ToArray();
                        // contents = Convert.ToBase64String(ImageBytes); // if you want a string use this.
                    }
                }

                CKImage newCKImage = new CKImage
                {
                    Name = model.Name,
                };

                await _ckimageRepository.AddAsync(newCKImage);

                return Redirect("~/CKImage/CKImageHome");
            }

            return Redirect("~/CKImage/CKImageHome");
        }

        public async Task<IActionResult> CKUpload()
        {
            IFormFileCollection files = HttpContext.Request.Form.Files;
            int fileCount = files.Count;
            IFormFile file = null;

            if (fileCount == 0){
                return BadRequest();
            }

            else if (fileCount == 1){
                foreach (var x in files)
                {
                    file = x;
                }
            }
            else {
                 return BadRequest();
            }           

            if (file != null && file.Length > 0){
                var name = file.FileName;
                var path = Path.Combine(_hostingEnv.WebRootPath, "images", name);
                var json = Json(new{path});
                string url = "";

                try
                {

                    using (var fileStream = new FileStream(path, FileMode.Create)){ 
                        await file.CopyToAsync(fileStream);
                        url = "../images/" + name;
                        json = Json(new {url });
                    }

                    CKImage newCKImage = new CKImage
                    {
                        Name = name,
                        Path = url,
                        FullPath = path
                    };

                    await _ckimageRepository.AddAsync(newCKImage);

                    return json;
                }

                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                }
            }
            else
            {
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpPost]
        public IActionResult Delete()
        {
            //_ckimageRepository.AddAsync(newCKImage);           
            return View("~/Views/CKImage/CKImageHome.cshtml");
        }



    }
		
}