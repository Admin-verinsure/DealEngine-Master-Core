using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels
{
	public class InsuranceRiskCategory : BaseViewModel
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public InsuranceRiskCategory () { }
	}

	//public class InsuranceRiskCategories : List<InsuranceRiskCategory>
	//{

	//}

	public class InsuranceRiskCategories
	{
		public IList<InsuranceRiskCategory> CategoriesList { get; set; }

		public string NewRiskName { get; }
		public string NewRiskDescription { get; }

		public IList<SelectListItem> CategoryItems { get; set; }

		public string SelectedCategory { get; set; }
	}
}

