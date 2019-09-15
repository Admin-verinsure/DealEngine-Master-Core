using System;

namespace techcertain2019core.Models.ViewModels.Policy
{
	public class DescriptionSectionVM
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Version { get; set; }

		public int Revision { get; set; }

		public Guid Creator { get; set; }

		public Guid Owner { get; set; }

		public string CreatorName { get; set; }

		public string OwnerName { get; set; }
	}
}

