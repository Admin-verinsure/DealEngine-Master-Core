using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace DealEngine.WebUI.Models.Image
{
    public class ImageViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public IFormFile Image { get; set; }

        public string Path { get; set; }

        public IFormFile Thumbnail { get; set; }

        public IList<CKImage> Item { get;set; }

        public IList<Product> Products {get; set;}

        public IFormFile File { get; set; }

        public string Product { get; set; }

        public string OS { get; set; }
        public string BaseURL { get; set; }

    }
}
