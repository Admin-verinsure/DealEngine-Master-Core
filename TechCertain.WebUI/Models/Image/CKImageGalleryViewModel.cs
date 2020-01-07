using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace TechCertain.WebUI.Models.Image
{
    public class CKImageGalleryViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public IFormFile Thumbnail { get; set; }

        public string ImagePath { get; set; }


    }
}
