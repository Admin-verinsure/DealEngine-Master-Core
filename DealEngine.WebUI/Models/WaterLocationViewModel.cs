using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class WaterLocationViewModel : BaseViewModel
    {
        public WaterLocationViewModel()
        {
            Locations = new List<Location>();
        }
        public Guid AnswerSheetId { get; set; }

        public Guid WaterLocationId { get; set; }

        public string WaterLocationName { get; set; }

        public decimal WaterLocationLatitude { get; set; }

        public decimal WaterLocationLongitude { get; set; }

        public Guid WaterLocationLocation { get; set; }

        public string MarinaLocationName { get; set; }
        public Guid WaterLocationMarinaLocation{ get; set; }
        public Guid OrganisationalUnit { get; set; }
        public OrganisationalUnit OrganisationalUnits { get; set; }
        public List<OrganisationalUnitViewModel> LOrganisationalUnits { get; set; }
        public IList<Location> Locations { get; set; }


        public IList<String> OUselectedVal { get; set; }
        public IList<Guid> OUselectedText { get; set; }


        public string WaterLocationApproved { get; set; }

        public string WaterLocationMooringType { get; set; }
        public IList<Domain.Entities.Organisation> LMarinalocation { get; set; }

        public WaterLocation ToEntity(User creatingUser)
        {
            WaterLocation waterLocation = new WaterLocation(creatingUser);
            UpdateEntity(waterLocation);
            return waterLocation;
        }

        public WaterLocation UpdateEntity(WaterLocation waterLocation)
        {
            waterLocation.MarinaName = WaterLocationName;
            waterLocation.Latitude = WaterLocationLatitude;
            waterLocation.Longitude = WaterLocationLongitude;
            waterLocation.Approved = WaterLocationApproved;


            waterLocation.MooringType = WaterLocationMooringType;
            return waterLocation;
        }

        public static WaterLocationViewModel FromEntity(WaterLocation waterLocation)
        {
            WaterLocationViewModel model = new WaterLocationViewModel
            {
                WaterLocationId = waterLocation.Id,
                WaterLocationLatitude = waterLocation.Latitude,
                WaterLocationLongitude = waterLocation.Longitude,
                WaterLocationApproved = waterLocation.Approved,
                WaterLocationMooringType = waterLocation.MooringType,
                MarinaLocationName = waterLocation.MarinaName,
        };
            return model;
        }
    }

}
