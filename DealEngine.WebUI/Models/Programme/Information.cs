using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Programme
{


	public class Information : BaseViewModel
	{
		public string Id { get; set; }
       

        public IList<InformationItems> informationitem { get; set; }

        public IList<InformationSection> Sections { get; set; }

    }
}

