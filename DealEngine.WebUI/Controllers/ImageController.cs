using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DealEngine.WebUI.Models.Image;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Microsoft.Extensions.Logging;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]

    public class ImageController : BaseController
    {
        IMapperSession<CKImage> _ckimageRepository;
        private readonly ICKImageService _ckimageService;
        private readonly IWebHostEnvironment _hostingEnv;
        //IMapperSession<User> _userRepository;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<ImageController> _logger;

        public ImageController(IUserService userRepository, 
            IMapperSession<CKImage> ckimageRepository, 
            ICKImageService ckimageService, 
            IWebHostEnvironment hostingEnv,
            IApplicationLoggingService applicationLoggingService,
            ILogger<ImageController> logger
            )
            : base(userRepository)
        {
            _applicationLoggingService=applicationLoggingService;
            _logger=logger;
            _ckimageRepository = ckimageRepository;
            _ckimageService = ckimageService;
            _hostingEnv = hostingEnv;
        }


        [HttpGet]
        public async Task<ViewResult> ManageImage()
        {
            var list = await _ckimageService.GetAllImages();
            ImageViewModel model = new ImageViewModel();
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
        public async Task<IActionResult> Upload(ImageViewModel model)
        {

            var user = await CurrentUser();
            if (model != null)
            {
                if (model.Image != null)
                {

                    var contentType = model.Image.ContentType;
                    var extension = "";
                    
                    if (contentType == "image/jpeg")
                    {
                        extension = ".jpg";
                    }
                    else if (contentType == "image/png")
                    {
                        extension = ".png";
                    }
                    else
                    {
                        throw new FileFormatException("Invalid File Type");
                    }

                    var filename = model.Name + extension;
                    var path = Path.Combine(_hostingEnv.WebRootPath, "images", filename);
                    var url = "";
                    
                    try
                    {
                        Stream stream = model.Image.OpenReadStream();
                        System.Drawing.Image thumbnail = GetReducedImage(100,100,stream);

                        if (thumbnail != null)
                        {
                               thumbnail.Save("wwwroot/images/_" + filename, thumbnail.RawFormat);
                        }
                        else
                        {
                            throw new ArgumentNullException("Image is null.");
                        }
                    }

                    catch(Exception ex)
                    {
                        await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                    }

                    try
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(fileStream);
                            url = "../../images/" + filename;
                        }

                        CKImage newCKImage = new CKImage
                        {
                            Name = filename,
                            Path = url,
                            ThumbPath = "../../images/_" + filename
                        };

                        await _ckimageRepository.AddAsync(newCKImage);

                    }

                    catch (Exception Ex)
                    {
                        Console.WriteLine(Ex.ToString());
                    }

                }
                return Redirect("~/Image/ManageImage");
            }
            return Redirect("~/Image/ManageImage");
        }


        public async Task<IActionResult> CKUpload()
        {
            IFormFileCollection files = HttpContext.Request.Form.Files;
            int fileCount = files.Count;
            IFormFile file = null;

            if (fileCount == 0)
            {
                return BadRequest();
            }

            else if (fileCount == 1)
            {
                foreach (var x in files)
                {
                    file = x;
                }
            }
            else
            {
                return BadRequest();
            }

            if (file != null && file.Length > 0)
            {
                var name = file.FileName;
                var path = Path.Combine(_hostingEnv.WebRootPath, "images", name);
                var json = Json(new { path });
                string url = "";

               try
                    {
                        Stream stream = file.OpenReadStream();
                        System.Drawing.Image thumbnail = GetReducedImage(100,100,stream);

                        if (thumbnail != null)
                        {
                               thumbnail.Save("wwwroot/images/_" + name, thumbnail.RawFormat);
                        }
                        else
                        {
                            throw new ArgumentNullException("Image is null.");
                        }
                    }
                    
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        url = "../../images/" + name;
                        json = Json(new { url });
                    }

                    CKImage newCKImage = new CKImage
                    {
                        Name = name,
                        Path = url,
                        ThumbPath =  "../../images/_" + name
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
            // Delete the file
            // Delete the db record
            //_ckimageRepository.AddAsync(newCKImage);           
            return View("~/Views/Image/ManageImage.cshtml");
        }


        public System.Drawing.Image GetReducedImage(int Width, int Height, Stream resourceImage)
        {
            try
            {
                System.Drawing.Image fullImage = System.Drawing.Image.FromStream(resourceImage);
                System.Drawing.Image thumbnail = fullImage.GetThumbnailImage(Width, Height, ()=> false, IntPtr.Zero);

                return thumbnail;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

}