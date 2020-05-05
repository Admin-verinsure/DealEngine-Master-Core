﻿using System;
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
			LocationType = GetLocationTypes();

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
		public string Street { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string CommonName { get; set; }
        public IList<SelectListItem> LocationType { get; set; }
    }

}
