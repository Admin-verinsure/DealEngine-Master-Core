using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models.Image;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;


namespace TechCertain.WebUI.Controllers
{
    public class CKImageController : BaseController
    {
        IMapperSession<CKImage> _ckimageRepository;
        private readonly ICKImageService _ckimageService;

        IMapperSession<User> _userRepository;

        public CKImageController(IUserService userRepository, IMapperSession<CKImage> ckimageRepository, ICKImageService ckimageService)
            : base(userRepository)
        {
            _ckimageRepository = ckimageRepository;
            _ckimageService = ckimageService;
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


        [HttpPost]
        public async Task<IActionResult> Upload(CKImageUploadViewModel model)
        {
            var contents = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            
   
            if (model != null)
            {
                if (model.Image != null)
                {
                    // Convert to Byte Array 
                     using (var memstream = new MemoryStream())
                    {
                        model.Image.CopyTo(memstream);
                        contents = memstream.ToArray();
                        // contents = Convert.ToBase64String(ImageBytes); // if you want a string use this.
                    }
                }

                CKImage newCKImage = new CKImage
                {
                    Name = model.Name,
                    Contents = contents,
                };

                await _ckimageRepository.AddAsync(newCKImage);

                return Redirect("~/CKImage/CKImageHome");
            }
            return Redirect("~/CKImage/CKImageHome");
        }

        [HttpPost]
        public IActionResult Delete(CKImageUploadViewModel model)
        {
            //_ckimageRepository.AddAsync(newCKImage);           
            return View("~/Views/CKImage/CKImageHome.cshtml");
        }



    }
		
}