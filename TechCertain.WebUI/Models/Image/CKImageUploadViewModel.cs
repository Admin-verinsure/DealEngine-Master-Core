using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace TechCertain.WebUI.Models.Image
{
    public class CKImageUploadViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public IFormFile Image { get; set; }

        public byte[] Contents { get; set; }

        public string ImagePath { get; set; }

        public IFormFile Thumbnail { get; set; }

        public IList<CKImage> Item { get;set; }


    }
}
