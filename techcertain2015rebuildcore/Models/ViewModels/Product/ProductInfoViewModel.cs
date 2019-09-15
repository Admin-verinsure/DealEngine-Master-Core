using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Product
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

