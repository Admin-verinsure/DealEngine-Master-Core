using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TechCertain.Domain.Entities;

namespace techcertain2019core.Models.ViewModels.Programme
{


	public class InformationItems : BaseViewModel
	{
		public Guid Id { get; set; }
        public string Name { get; set; }

        public string Label { get; set; }
        public string Type { get; set; }


        public IList<DropdownList> option { get; set; }
    


    }
}

