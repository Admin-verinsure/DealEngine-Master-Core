using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models.ProductModels
{

	public class ProductViewModel : BaseViewModel
	{
        [Required]
		public ProductDescriptionVM Description { get; set; }

		public ProductRisksVM Risks { get; set; }

		//public RiskEntityViewModel [] Risks { get; set; }

		public ProductInformationSheetVM InformationSheet { get; set; }

        public TerritoryViewModel territoryBuilderVM { get; set; }

        public ProductSettingsVM Settings { get; set; }

		public ProductPartiesVM Parties { get; set; }

		public ProductViewModel ()
		{
			Description = new ProductDescriptionVM ();
			Risks = new ProductRisksVM ();
			InformationSheet = new ProductInformationSheetVM ();
			Settings = new ProductSettingsVM ();
			Parties = new ProductPartiesVM ();
		}
	}

	public class ProductDescriptionVM
	{
		public Guid OriginalProductId { get; set; }

		public bool Draft { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public IList<SelectListItem> Languages { get; set; }

        public MultiSelectList LanguagesList { get; set; }

        public string[] SelectedLanguages { get; set; }

		public Guid OwnerOrganisation { get; set; }

		public Guid CreatorOrganisation { get; set; }

		public bool Public { get; set; }

		public bool IsBaseProduct { get; set; }

		public IList<SelectListItem> BaseProducts { get; set; }

		public string SelectedBaseProduct { get; set; }

		public string DateCreated { get; set; }

		public ProductDescriptionVM ()
		{
			Languages = new List<SelectListItem> ();
			BaseProducts = new List<SelectListItem> ();
		}
	}

	public class ProductRisksVM : List<RiskEntityViewModel>
	{

	}

	public class ProductSettingsVM
	{
		public IList<SelectListItem> Documents { get; set; }

		public string [] SelectedDocuments { get; set; }

		public IList<SelectListItem> InformationSheets { get; set; }

		public string SelectedInformationSheet { get; set; }

		public IList<SelectListItem> PossibleOwnerOrganisations { get; set; }

		public string SelectedOwnerOrganisation { get; set; }

		public IList<SelectListItem> InsuranceProgrammes { get; set; }

		public string SelectedInsuranceProgramme { get; set; }

		public ProductSettingsVM ()
		{
			Documents = new List<SelectListItem> ();
			InformationSheets = new List<SelectListItem> ();
			PossibleOwnerOrganisations = new List<SelectListItem> ();
			InsuranceProgrammes = new List<SelectListItem> ();
		}
	}

	public class ProductInformationSheetVM
	{
		public IEnumerable<SelectListItem> InformationSheets { get; set; }

		public Guid SelectedInformationSheet { get; set; }

		public ProductInformationSheetVM ()
		{
			InformationSheets = new List<SelectListItem> ();
		}
	}

	public class ProductPartiesVM
	{
		public IEnumerable<SelectListItem> Insurers { get; set; }

		public Guid SelectedInsurer { get; set; }

		public IEnumerable<SelectListItem> Brokers { get; set; }

		public Guid SelectedBroker { get; set; }
	}
}

