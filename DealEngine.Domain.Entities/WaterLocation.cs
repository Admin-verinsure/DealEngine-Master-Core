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

        public virtual IList<Location> Locations
        {
            get;
            set;
        }

        public virtual string MarinaName
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual bool IsPublic
        {
            get;
            set;
        }

        public virtual decimal Latitude
        {
            get;
            set;
        }

        public virtual decimal Longitude
        {
            get;
            set;
        }

        public virtual string Approved
        {
            get;
            set;
        }

        public virtual string Category
        {
            get;
            set;
        }

        public virtual string MooringType
        {
            get;
            set;
        }



        public virtual WaterLocation CloneForNewSheet(ClientInformationSheet newSheet)
        {
            WaterLocation newWaterLocation = new WaterLocation(newSheet.CreatedBy);
            newWaterLocation.Latitude = Latitude;
            newWaterLocation.Longitude = Longitude;
            newWaterLocation.Approved = Approved;
            newWaterLocation.Category = Category;
            newWaterLocation.MooringType = MooringType;
            return newWaterLocation;
        }

    }
}
