using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechCertain.Domain.Entities;

namespace techcertain2015rebuildcore.Models.ViewModels.Agreement
{
    public class EditTermsViewModel : BaseViewModel
    {

        public string BoatName { get; set; }
        public int TermLimit { get; set; }

        public decimal Premium { get; set; }

		public decimal FSL { get; set; }

		public string Model { get; set; }

        public decimal Excess { get; set; }

		public string BoatMake { get; set; }
        public string BoatModel { get; set; }
        public string Make { get; set; }
        public string Registration { get; set; }

        public IEnumerable<ClientAgreementBVTerm> BVTerms { get; set; }
        public IEnumerable<ClientAgreementMVTerm> MVTerms { get; set; }
        


    }
}