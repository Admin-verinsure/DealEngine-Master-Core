using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels
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