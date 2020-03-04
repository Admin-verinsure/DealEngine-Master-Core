using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.Product
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

