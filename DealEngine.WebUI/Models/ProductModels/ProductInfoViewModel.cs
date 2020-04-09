using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.ProductModels
{
	public class ProductInfoViewModel : BaseViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string OwnerCompany { get; set; }

		public IList<string> SelectedLanguages { get; set; }

		public string DateCreated { get; set; }

		public ProductInfoViewModel ()
		{
		}
	}
}

