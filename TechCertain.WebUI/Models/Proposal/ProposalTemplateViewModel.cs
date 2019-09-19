using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Proposal
{
    public class ProposalTemplateViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Question> UnassignedQuestions {get; set;}
    }

    public class Question
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }
    }
}