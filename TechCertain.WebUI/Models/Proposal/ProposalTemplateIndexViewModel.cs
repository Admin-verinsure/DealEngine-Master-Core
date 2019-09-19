using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Proposal
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