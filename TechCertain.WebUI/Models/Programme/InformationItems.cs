using System;
using System.Collections.Generic;

namespace TechCertain.WebUI.Models.Programme
{


	public class InformationItems : BaseViewModel
	{
		public Guid Id { get; set; }
        public string Name { get; set; }

        public string Label { get; set; }
        public string Type { get; set; }
        public string ControlId { get; set; }


        public IList<DropdownList> option { get; set; }
    


    }
}

