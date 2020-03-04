using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class WaterLocation : EntityBase, IAggregateRoot
    {
        protected WaterLocation() : base(null) { }

        public WaterLocation(User createdBy)
            : base(createdBy)
        {

        }

        public virtual Location WaterLocationLocation
        {
            get;
            set;
        }

        public virtual Organisation WaterLocationMarinaLocation
        {
            get;
            set;
        }
        public virtual OrganisationalUnit OrganisationalUnit
        {
            get;
            set;
        }

        public virtual string WaterLocationName
        {
            get;
            set;
        }

        public virtual string MarinaLocationName
        {
            get;
            set;
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual decimal WaterLocationLatitude
        {
            get;
            set;
        }

        public virtual decimal WaterLocationLongitude
        {
            get;
            set;
        }

        public virtual string WaterLocationApproved
        {
            get;
            set;
        }

        public virtual string WaterLocationCategory
        {
            get;
            set;
        }

        public virtual string WaterLocationMooringType
        {
            get;
            set;
        }

    public virtual WaterLocation OriginalWaterLocation
        {
            get;
            protected set;
        }

        public virtual WaterLocation CloneForNewSheet(ClientInformationSheet newSheet)
        {
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone water location for original information");

            WaterLocation newWaterLocation = new WaterLocation(newSheet.CreatedBy);
            newWaterLocation.WaterLocationName = WaterLocationName;
            newWaterLocation.WaterLocationLatitude = WaterLocationLatitude;
            newWaterLocation.WaterLocationLongitude = WaterLocationLongitude;
            newWaterLocation.WaterLocationLocation = newSheet.Locations.FirstOrDefault(l => l.OriginalLocation.Id == Id);
            newWaterLocation.WaterLocationMarinaLocation = WaterLocationMarinaLocation;
            newWaterLocation.WaterLocationApproved = WaterLocationApproved;
            newWaterLocation.WaterLocationCategory = WaterLocationCategory;
            newWaterLocation.WaterLocationMooringType = WaterLocationMooringType;
            newWaterLocation.OriginalWaterLocation = this;
            return newWaterLocation;
        }

    }
}
