﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.Agreement
{
    public class ViewAgreementEndorsementViewModel : BaseViewModel
    {
        public AgreementEndorsementsViewModel ClientAgreementEndorsements { get; set; }

        public bool HasEndorsements { get; set; }

        public Guid ClientAgreementID { get; set; }

        public Guid ClientProgrammeID { get; set; }
        public Guid ClientAgreementEndorsementID { get; set; }
        public string EndorsementNameToAdd { get; set; }

        public IEnumerable<SelectListItem> AvailableEndorsementTitles { get; set; }
        public string Content { get; set; }

    }

    public class AgreementEndorsementsViewModel : List<ClientAgreementEndorsementViewModel>
    {

    }

}