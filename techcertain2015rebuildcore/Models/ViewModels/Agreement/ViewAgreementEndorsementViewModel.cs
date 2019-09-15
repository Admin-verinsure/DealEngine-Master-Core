﻿using System;
using System.Collections.Generic;

namespace techcertain2015rebuildcore.Models.ViewModels.Agreement
{
    public class ViewAgreementEndorsementViewModel : BaseViewModel
    {
        public AgreementEndorsementsViewModel ClientAgreementEndorsements { get; set; }

        public bool HasEndorsements { get; set; }

        public Guid ClientAgreementID { get; set; }

        public Guid ClientProgrammeID { get; set; }

        public string EndorsementNameToAdd { get; set; }

        public string EndorsementTextToAdd { get; set; }

    }

    public class AgreementEndorsementsViewModel : List<ClientAgreementEndorsementViewModel>
    {

    }

}