using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class LocationViewModel : BaseViewModel
    {
		public LocationViewModel(ClientInformationSheet ClientInformationSheet)
		{
			Locations = GetLocations(ClientInformationSheet);
			LocationTypes = GetLocationTypes();

		}

		public LocationViewModel()
		{
		}

		private IList<Location> GetLocations(ClientInformationSheet ClientInformationSheet)
		{
			Locations = new List<Location>();
			foreach (var Location in ClientInformationSheet.Locations)
			{
				Locations.Add(Location);
			}
			return Locations;
		}
		private IList<SelectListItem> GetLocationTypes()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem()
				{
					Value="Residential",
					Text="Residential"
				},
				new SelectListItem()
				{
					Value="Commercial",
					Text="Commercial"
				},
				new SelectListItem()
				{
					Value="Industrial",
					Text="Industrial"
				},
				new SelectListItem()
				{
					Value="Rural",
					Text="Rural"
				},
				new SelectListItem()
				{
					Value="Postal",
					Text="Postal Address"
				},
				new SelectListItem()
				{
					Value="Billing",
					Text="Billing Address"
				},
				new SelectListItem()
				{
					Value="Other",
					Text="Other"
				}																												
			};
		}

		public IList<Location> Locations { get; set; }
		public string StreetDetails { get; set; }
		public string CityDetails { get; set; }
		public string CountryDetails { get; set; }
		public string CommonNameDetails { get; set; }
        public IList<SelectListItem> LocationTypes { get; set; }
    }

}
