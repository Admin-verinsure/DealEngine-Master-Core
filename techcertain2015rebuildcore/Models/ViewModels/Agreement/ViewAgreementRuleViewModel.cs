using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Agreement
{
    public class ViewAgreementRuleViewModel : BaseViewModel
    {
        public AgreementRulesViewModel ClientAgreementRules { get; set; }

        public bool HasRules { get; set; }

        public Guid ClientAgreementID { get; set; }

        public Guid ClientProgrammeID { get; set; }

    }

    public class AgreementRulesViewModel : List<ClientAgreementRuleViewModel>
    {      

    }

}