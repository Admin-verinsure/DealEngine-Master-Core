using System;
using System.Collections.Generic;

namespace TechCertain.WebUI.Models
{
	public class ProductSetupViewModel : BaseViewModel
	{
		public List<RiskEntityViewModel> Risks { get; set; }


		public ProductSetupViewModel ()
		{
		}
	}

	public class RiskEntityViewModel
	{
		public Guid Id { get; set; }

		public string Insured { get; set; }

		public bool CoverAll { get; set; }

		public bool CoverLoss { get; set; }

		public bool CoverInterruption { get; set; }

		public bool CoverThirdParty { get; set; }
	}
}

