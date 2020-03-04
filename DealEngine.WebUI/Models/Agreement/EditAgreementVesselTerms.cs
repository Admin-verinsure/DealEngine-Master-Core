using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Agreement
{
    public class EditAgreementVesselTerms : BaseViewModel
    {
        public IEnumerable<ClientAgreementBVTerm> BoatViewModels { get; set; }



    }
}