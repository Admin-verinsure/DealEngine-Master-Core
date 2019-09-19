using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels
{
	public class PolicyDocumentViewModel : BaseViewModel
	{
		public string Title { get; set; }

		public string Creator { get; set; }

		public string Owner { get; set; }

		public string Description { get; set; }

		public string DocumentType { get; set; }

		//public DateTime DateCreated { get; set; }

		public string Version { get; set; }

		public int Revision { get; set; }

		//public PolicyDocumentSectionViewModel Sections { get; set; }

		public string Text { get; set; }

		public int Territory { get; set; }

		public int Jurisdiction { get; set; }

		public string CustomTerritory { get; set; }

		public string CustomJurisdiction { get; set; }

		public bool IsWording
		{
			get { return DocumentType == "Wording"; }
		}
	}

	public class PolicyDocumentListViewModel
	{
		public List<PolicyDocumentViewModel> Documents { get; set; }

		public PolicyDocumentListViewModel()
		{
			Documents = new List<PolicyDocumentViewModel> ();
		}
	}
}

