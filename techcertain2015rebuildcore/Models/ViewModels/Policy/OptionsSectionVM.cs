using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Policy
{
	public class OptionsSectionVM
	{
		public IEnumerable<SelectListItem> Territories { get; set; }

		public IEnumerable<SelectListItem> Jurisdictions { get; set; }

		public IEnumerable<SelectListItem> Clauses { get; set; }

		public string Territory { get; set; }

		public string Jurisdiction { get; set; }

		public string CustomTerritory { get; set; }

		public string CustomJurisdiction { get; set; }

		public string Clause { get; set; }

		public bool IsPrivate { get; set; }
	}
}

