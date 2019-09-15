using System;
using System.Collections.Generic;

namespace techcertain2015rebuildcore.Models.ViewModels
{
	public class ExperimentalInfoBuilderViewModel : BaseViewModel
	{
		public string Title { get; set; }

		public string Description { get; set; }

		public IEnumerable<ExperimentalInfoSectionViewModel> Pages { get; set; }

		public IEnumerable<InformationQuestionConditional> Conditionals { get; set; }

		public ExperimentalInfoBuilderViewModel ()
		{
		}
	}

	public class ExperimentalInfoSectionViewModel
	{
		public string Title { get; set; }

		public IEnumerable<ExperimentalInfoQuestionViewModel> Questions { get; set; }

		public ExperimentalInfoSectionViewModel ()
		{
		}
	}

	public class ExperimentalInfoQuestionViewModel
	{
		public bool NeedsReview { get; set; }

		public bool ReferUnderWriting { get; set; }

        public bool NeedsMilestone { get; set; }
        public bool Required { get; set; }

		public string EditorId { get; set; }

		public string QuestionType { get; set; }

		public string QuestionTitle { get; set; }

		public string Options { get; set; }

		public string[] OptionsArray {
			get { return Options?.Split (new string [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); }
		}

		public bool HorizontalLayout { get; set; }

		public ExperimentalInfoQuestionViewModel ()
		{
			Options = string.Empty;
		}
	}

	public class InformationQuestionConditional
	{
		public string QuestionId { get; set; }

		public string [] Controls { get; set; }

		public string TriggerValue { get; set; }

		public int Visibility { get; set; }
	}
}

