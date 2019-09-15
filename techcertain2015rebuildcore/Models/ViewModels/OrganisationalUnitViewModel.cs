using System;
using System.Collections.Generic;

namespace techcertain2015rebuildcore.Models.ViewModels
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