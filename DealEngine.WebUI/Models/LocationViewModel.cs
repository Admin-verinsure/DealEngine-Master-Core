using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class LocationViewModel : BaseViewModel
    {
		public Guid AnswerSheetId { get; set; }

		public Guid LocationId { get; set; }

        public string Street { get; set; }

        public string Suburb { get; set; }

        public string Postcode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string CommonName { get; set; }

        public string RiskZone { get; set; }

        public IList<SelectListItem> OrganisationalUnits { get; set; }

        public Guid[] SelectedOrganisationalUnits { get; set; }

        public Guid OrganisationId { get; set; }

        public IList<BuildingViewModel> Buildings { get; set; }

        // public IList<Building> Buildings { get; set; }

        public IList<WaterLocationViewModel> WaterLocations { get; set; }

        public string LocationType { get; set; }

        public Location ToEntity (User creatingUser)
		{
			Location location = new Location (creatingUser);
			UpdateEntity (location);
			return location;
		}

		public Location UpdateEntity (Location location)
		{
			location.Street = Street;
			location.Suburb = Suburb;
			location.Postcode = Postcode;
			location.City = City;
			location.State = State;
			location.Country = Country;
			location.CommonName = CommonName;
            location.RiskZone = RiskZone;
            location.LocationType = LocationType;
            return location;
		}

		public static LocationViewModel FromEntity (Location location)
		{
			LocationViewModel model = new LocationViewModel {
				LocationId = location.Id,
				Street = location.Street,
				Suburb = location.Suburb,
				Postcode = location.Postcode,
				City = location.City,
				State = location.State,
				Country = location.Country,
				CommonName = location.CommonName,
                RiskZone = location.RiskZone,
                LocationType = location.LocationType,
            };
			return model;
		}
    }

}
