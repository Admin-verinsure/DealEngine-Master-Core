using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealEngine.WebUI.Models
{
    public class SystemEmailTemplateViewModel : BaseViewModel
    {

        public string SystemEmailName { get; set; }

        public string Subject { get; set; }

        public string SystemEmailType { get; set; }

        public string Body { get; set; }

        public string InternalNotes { get; set; }

    }


}