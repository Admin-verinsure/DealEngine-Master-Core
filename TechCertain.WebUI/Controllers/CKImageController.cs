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
// using System.Drawing.Common;
//Install-Package System.Drawing.Common -Version 4.5.1 || https://stackoverflow.com/questions/54075745/how-to-create-thumbnail-image-in-net-core-using-the-help-of-iformfile




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
            var list = await _ckimageService.GetAllImages();
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
                        throw new System.IO.FileFormatException("Invalid File Type");
                    }
                    var filename = model.Name + extension;
                    var path = Path.Combine(_hostingEnv.WebRootPath, "images", filename);
                    var url = "";

                    try
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(fileStream);
                            url = "../images/" + filename;
                        }

/*// THUMBNAIL

Stream stream=ProductImage.OpenReadStream();

Image newImage=GetReducedImage(32,32,stream);
newImage.Save("path+filename");

//POTENTIAL ISSUE NOT USING FLUL PATH you should not use file.FileName from the user input and directly combine it with path.combine, as this file name could contain routing to subdirectories ("../../") you always need to recheck with e.g. Path.GetFullPath(generatedPath) if the return value is the same as your wanted upload directory. Also the filename from the request is not unique. – cyptus Mar 19 '18 at 14:22
using (var memstream = new MemoryStream())
{
    model.Image.CopyTo(memstream);
    // contents = memstream.ToArray();
    // contents = Convert.ToBase64String(ImageBytes); // if you want a string use this.
}*/

                        CKImage newCKImage = new CKImage
                        {
                            Name = filename,
                            Path = url,
                            FullPath = path
                            // Thumbnail = TODO;
                        };

                        await _ckimageRepository.AddAsync(newCKImage);

                    }

                    catch (Exception Ex)
                    {
                        Console.WriteLine(Ex.ToString());
                    }

                }
                var lol = "breakpoint";
                return Redirect("~/CKImage/CKImageHome");
            }
            var lol2 = "breakpoiiiint";
            return Redirect("~/CKImage/CKImageHome");
        }
        /*// THUMBNAIL

public Image GetReducedImage(int width, int height, Stream resourceImage)
{
try
{
Image image = Image.FromStream(resourceImage);
Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

return thumb;
}
catch (Exception e)
{
return null;
}
}
*/
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

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        url = "../images/" + name;
                        json = Json(new { url });
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

            // Delete the file
            // Delete the db record
            //_ckimageRepository.AddAsync(newCKImage);           
            return View("~/Views/CKImage/CKImageHome.cshtml");
        }
    }

}