using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class ProposalBuilderViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Logo_Option LogoOption { get; set; }

        public List<ProposalQuestion> ProposalQuestions { get; set; }
    }

    public class ProposalQuestion {

        public string Label { get; set; }

        public InputType InputType { get; set; }

        public string PrependIcon { get; set; }

        public string AppendIcon { get; set; }
    }

    public enum InputType {
        TEXTBOX, TEXTAREA
    }

    public enum Logo_Option
    {
        Organisation, Custom, None
    }  
}