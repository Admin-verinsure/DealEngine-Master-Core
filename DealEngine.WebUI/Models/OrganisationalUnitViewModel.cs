using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models
{
    public class OrganisationalUnitViewModel : BaseViewModel
    {
		public Guid AnswerSheetId { get; set; }

        public string Name { get; set; }

        public Guid OrganisationalUnitId { get; set; }

        public Guid OrganisationId { get; set; }

        public IList<LocationViewModel> Locations { get; set; }
    }
}
