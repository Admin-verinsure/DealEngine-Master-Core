using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Agreement
{
    public class EditTermsCancelViewModel : BaseViewModel
    {
        public Guid clientAgreementCanId { get; set; }
        public Guid VesselCanId { get; set; }
        public Guid TermCanId { get; set; }
        public string TermTypeCan { get; set; }
        public string BoatNameCan { get; set; }
        public int TermLimitCan { get; set; }

        public decimal PremiumCan { get; set; }

        public decimal FSLCan { get; set; }

        public string ModelCan { get; set; }

        public decimal ExcessCan { get; set; }

        public string BoatMakeCan { get; set; }
        public string BoatModelCan { get; set; }
        public string MakeCan { get; set; }
        public string RegistrationCan { get; set; }

        public IEnumerable<ClientAgreementBVTermCancel> BVTermsCan { get; set; }
        public IEnumerable<ClientAgreementMVTermCancel> MVTermsCan { get; set; }



    }
}
