using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechCertain.WebUI.Models.Agreement
{
	public class AgreementTemplateViewModel : BaseViewModel
	{
		public Guid OriginalAgreementId { get; set; }

		public Guid OwnerOrganisation { get; set; }

		public Guid CreatorOrganisation { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public IList<SelectListItem> Languages { get; set; }

		public string[] SelectedLanguages { get; set; }

		public IList<SelectListItem> InformationSheets { get; set; }

		public string SelectedInformationSheet { get; set; }

		public IList<SelectListItem> Products { get; set; }

		public string[] SelectedProducts { get; set; }
		
	}
}

