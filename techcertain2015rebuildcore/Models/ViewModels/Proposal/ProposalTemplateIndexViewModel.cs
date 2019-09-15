using System;
using System.Collections.Generic;

namespace techcertain2015rebuildcore.Models.ViewModels.Proposal
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