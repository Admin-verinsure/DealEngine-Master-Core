﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechCertain.Domain.Entities;

namespace techcertain2019core.Models.ViewModels.Agreement
{
    public class EditAgreementVesselTerms : BaseViewModel
    {
        public IEnumerable<ClientAgreementBVTerm> BoatViewModels { get; set; }



    }
}