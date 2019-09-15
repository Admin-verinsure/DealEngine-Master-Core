using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class WaterLocationViewModel : BaseViewModel
    {
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
        public List<LocationViewModel> lLocation { get; set; }


        public IList<String> OUselectedVal { get; set; }
        public IList<Guid> OUselectedText { get; set; }


        public string WaterLocationApproved { get; set; }

        public string WaterLocationMooringType { get; set; }
        public IList<Organisation> LMarinalocation { get; set; }

        public WaterLocation ToEntity(User creatingUser)
        {
            WaterLocation waterLocation = new WaterLocation(creatingUser);
            UpdateEntity(waterLocation);
            return waterLocation;
        }

        public WaterLocation UpdateEntity(WaterLocation waterLocation)
        {
            waterLocation.WaterLocationName = WaterLocationName;
            waterLocation.WaterLocationLatitude = WaterLocationLatitude;
            waterLocation.WaterLocationLongitude = WaterLocationLongitude;
            waterLocation.WaterLocationApproved = WaterLocationApproved;


            waterLocation.WaterLocationMooringType = WaterLocationMooringType;
            return waterLocation;
        }

        public static WaterLocationViewModel FromEntity(WaterLocation waterLocation)
        {
            WaterLocationViewModel model = new WaterLocationViewModel
            {
                WaterLocationId = waterLocation.Id,
                WaterLocationName = waterLocation.WaterLocationName,
                WaterLocationLatitude = waterLocation.WaterLocationLatitude,
                WaterLocationLongitude = waterLocation.WaterLocationLongitude,
                WaterLocationApproved = waterLocation.WaterLocationApproved,
                WaterLocationMooringType = waterLocation.WaterLocationMooringType,
                MarinaLocationName = waterLocation.MarinaLocationName,
        };
            return model;
        }
    }

}