using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Boat : EntityBase, IAggregateRoot
    {
        public Boat() : base(null) { }

        public Boat(User createdBy)
            : base(createdBy)
        {
        }

        public virtual Boat OriginalBoat
        {
            get;
            protected set;
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }


        public virtual string BoatName
        {
            get;
            set;
        }

        public virtual string BoatType1
        {
            get;
            set;
        }

        public virtual string BoatType2
        {
            get;
            set;
        }

        public virtual string HullConstruction
        {
            get;
            set;
        }

        public virtual string HullConfiguration
        {
            get;
            set;
        }

        public virtual Organisation BoatWaterLocation
        {
            get;
            set;
        }

        public virtual Building BoatLandLocation
        {
            get;
            set;
        }

        public virtual int YearOfManufacture
        {
            get;
            set;
        }

        public virtual string BoatMake
        {
            get;
            set;
        }


        public virtual string BoatModel
        {
            get;
            set;
        }

        public virtual string OtherHullConstruction
        {
            get;
            set;
        }

        public virtual string OtherHullConfiguration
        {
            get;
            set;
        }

        public virtual int MaxSumInsured
        {
            get;
            set;
        }

        public virtual decimal Sum
        {
            get;
            set;
        }
        public virtual string BuiltProfessionally
        {
            get;
            set;
        }

        public virtual string MotorType
        {
            get;
            set;
        }

        public virtual string ModifiedMotor
        {
            get;
            set;
        }

        public virtual string MaxRatedSpeed
        {
            get;
            set;
        }

        public virtual string RiggingType
        {
            get;
            set;
        }


        public virtual string MastType
        {
            get;
            set;
        }

        public virtual string AucklandRegistration
        {
            get;
            set;
        }

        public virtual string NationalRegistration
        {
            get;
            set;
        }


        public virtual IList<Organisation> InterestedParties
        {
            get;
            set;
        }

        public virtual string BoatNotes
        {
            get;
            set;
        }
        public virtual string WaterLocationMooringType
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual DateTime BoatEffectiveDate
        {
            get;
            set;
        }
        public virtual DateTime BoatIPEffectiveDate
        {
            get;
            set;
        }
        public virtual DateTime BoatCeaseDate
        {
            get;
            set;
        }

        public virtual int BoatCeaseReason
        {
            get;
            set;
        }

        public virtual DateTime BoatInceptionDate
        {
            get;
            set;
        }

        public virtual DateTime BoatExpireDate
        {
            get;
            set;
        }

        public virtual string BoatIsTrailered
        {
            get;
            set;
        }

        public virtual Vehicle BoatTrailer
        {
            get;
            set;
        }

        public virtual Organisation BoatOperator
        {
            get;
            set;
        }

        public virtual IList<BoatUse> BoatUse
        {
            get;
            set;
        }

        public virtual decimal BoatQuickQuotePremium
        {
            get;
            set;
        }

        public virtual int BoatQuoteExcessOption
        {
            get;
            set;
        }
        public virtual string VesselArea
        {
            get;
            set;
        }

        public virtual string OtherMarinaName
        {
            get;
            set;
        }

        public virtual Boolean OtherMarina
        {
            get;
            set;
        }

        // public virtual string SelectedBoatUse { get; set; }
        // public virtual string[] Boatselected { get; set; }


        public virtual Boat CloneForNewSheet(ClientInformationSheet newSheet)
        {
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone boat for original information");

            Boat newBoat = new Boat(newSheet.CreatedBy);
            newBoat.BoatName = BoatName;
            newBoat.BoatType1 = BoatType1;
            newBoat.BoatType2 = BoatType2;
            newBoat.HullConstruction = HullConstruction;
            newBoat.OtherHullConstruction = OtherHullConstruction;
            newBoat.HullConfiguration = HullConfiguration;
            newBoat.OtherHullConfiguration = OtherHullConfiguration;
            newBoat.YearOfManufacture = YearOfManufacture;
            newBoat.BoatMake = BoatMake;
            newBoat.BoatModel = BoatModel;
            newBoat.MaxSumInsured = MaxSumInsured;
            newBoat.BuiltProfessionally = BuiltProfessionally;
            newBoat.MotorType = MotorType;
            newBoat.ModifiedMotor = ModifiedMotor;
            newBoat.MaxRatedSpeed = MaxRatedSpeed;
            newBoat.RiggingType = RiggingType;
            newBoat.MastType = MastType;
            newBoat.AucklandRegistration = AucklandRegistration;
            newBoat.NationalRegistration = NationalRegistration;

            newBoat.InterestedParties = new List<Organisation>(InterestedParties);
            newBoat.BoatUse = new List<BoatUse>(BoatUse);
            //newBoat.BoatUse = BoatUse;

            newBoat.BoatNotes = BoatNotes;
            //newBoat.BoatWaterLocation = newSheet.Organisation.FirstOrDefault(wl => wl.Id == Id);
            newBoat.BoatWaterLocation = BoatWaterLocation;
            newBoat.BoatLandLocation = newSheet.Buildings.FirstOrDefault(ll => ll.OriginalBuilding.Id == BoatLandLocation.Id);
            newBoat.BoatTrailer = newSheet.Vehicles.FirstOrDefault(bv => bv.OriginalVehicle.Id == BoatTrailer.Id);
            //newBoat.BoatOperator = newSheet.Organisation.FirstOrDefault(boper => boper.Id == Id);
            newBoat.BoatOperator = BoatOperator;
            if (BoatEffectiveDate > DateTime.MinValue)
                newBoat.BoatEffectiveDate = BoatEffectiveDate;
            if (BoatCeaseDate > DateTime.MinValue)
                newBoat.BoatCeaseDate = BoatCeaseDate;
            newBoat.BoatCeaseReason = BoatCeaseReason;
            if (BoatInceptionDate > DateTime.MinValue)
                newBoat.BoatInceptionDate = BoatInceptionDate;
            if (BoatExpireDate > DateTime.MinValue)
                newBoat.BoatExpireDate = BoatExpireDate;
            newBoat.BoatIsTrailered = BoatIsTrailered;
            newBoat.BoatQuickQuotePremium = BoatQuickQuotePremium;
            newBoat.BoatQuoteExcessOption = BoatQuoteExcessOption;
            newBoat.OtherMarinaName = OtherMarinaName;
            newBoat.OtherMarina = OtherMarina;
            newBoat.VesselArea = VesselArea;
            newBoat.WaterLocationMooringType = WaterLocationMooringType;
            newBoat.OriginalBoat = this;
            return newBoat;
        }
    }
}

