using System;

namespace TechCertain.WebUI.Models.Policy
{
	public class PolicySectionVM : BaseViewModel
	{
		public bool Draft { get; set; }

		public DescriptionSectionVM Description { get; set; }

		public ContentSectionVM Content { get; set; }

		public OptionsSectionVM Options { get; set; }
	}
}

