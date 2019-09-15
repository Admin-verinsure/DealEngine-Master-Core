using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class PrivateServerViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string ServerName { get; set; }

        public string ServerAddress { get; set; }
    }
}