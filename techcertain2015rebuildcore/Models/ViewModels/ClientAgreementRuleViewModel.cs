using System;

namespace techcertain2019core.Models.ViewModels
{
    public class ClientAgreementRuleViewModel : BaseViewModel
    {
        public Guid ClientAgreementRuleID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public int OrderNumber { get; set; }

        public Guid ClientAgreementID { get; set; }

    }
}