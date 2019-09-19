using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Programme
{


	public class Information : BaseViewModel
	{
		public string Id { get; set; }
       

        public IList<InformationItems> informationitem { get; set; }

        public IList<InformationSection> Sections { get; set; }

    }
}

