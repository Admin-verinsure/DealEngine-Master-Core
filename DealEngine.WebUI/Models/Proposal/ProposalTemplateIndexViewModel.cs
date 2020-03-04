using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.Proposal
{
    public class ProposalTemplateIndexViewModel
    {
        public List<ProposalTemplateViewModel> ProposalTemplates { get; set; }

        public ProposalTemplateIndexViewModel()
        {
            ProposalTemplates = new List<ProposalTemplateViewModel>();
        }
    }
}