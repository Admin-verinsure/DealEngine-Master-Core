using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace techcertain2019core.Models.ViewModels
{
    public class PrivateServerViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string ServerName { get; set; }

        public string ServerAddress { get; set; }
    }
}