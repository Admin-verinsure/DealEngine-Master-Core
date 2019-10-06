using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Building : EntityBase, IAggregateRoot
    {
        protected Building() : base(null) { }

        public Building(User createdBy)
            : base(createdBy)
        {
            
        }

        public virtual Location Location    
        {
            get;
            set;
        }

        public virtual string BuildingName
        {
            get;
            set;
        }

        public virtual ClientInformationSheet ClientInformationSheet
        {
            get;
            set;
        }

        public virtual string ConstructionType
        {
            get;
            set;
        }

        public virtual int ConstructionYear
        {
            get;
            set;
        }

        public virtual int NumOfUnits
        {
            get;
            set;
        }

        public virtual int NumberOfStoreys
        {
            get;
            set;
        }

        public virtual string IsBuilding3OrMoreStoreysHigh
        {
            get;
            set;
        }


        public virtual string HasInsulatedSandwichPanels
        {
            get;
            set;
        }

        public virtual int PercentOfInsuSandwichPanels
        {
            get;
            set;
        }

        public virtual string StructuralFraming
        {
            get;
            set;
        }

        public virtual string HasSprinklers
        {
            get;
            set;
        }

        public virtual string NZS4541Compliant
        {
            get;
            set;
        }

        public virtual string IsHalfOrMoreUnOccupied
        {
            get;
            set;
        }

        public virtual string IsTownWaterSupplied
        {
            get;
            set;
        }

        public virtual string HasAlarm
        {
            get;
            set;
        }

        public virtual DateTime BuildingLastValuationDate
        {
            get;
            set;
        }

        public virtual string BuildingBasisofSettlement
        {
            get;
            set;
        }

        public virtual int BuildingRVValue
        {
            get;
            set;
        }

        public virtual int BuildingRIVValue
        {
            get;
            set;
        }

        public virtual int BuildingDValue
        {
            get;
            set;
        }

        public virtual int BuildingIVValue
        {
            get;
            set;
        }

        public virtual int BuildingIIVValue
        {
            get;
            set;
        }

        public virtual string BuildingBasisofSettlementND
        {
            get;
            set;
        }

        public virtual string ContentsBasisofSettlement
        {
            get;
            set;
        }

        public virtual int ContentsRVValue
        {
            get;
            set;
        }

        public virtual int ContentsIVValue
        {
            get;
            set;
        }

        public virtual string ContentsBasisofSettlementND
        {
            get;
            set;
        }

        public virtual string StockBasisofSettlement
        {
            get;
            set;
        }

        public virtual int StockRVValue
        {
            get;
            set;
        }

        public virtual int StockIVValue
        {
            get;
            set;
        }

        public virtual string StockBasisofSettlementND
        {
            get;
            set;
        }

        public virtual string HasMDND
        {
            get;
            set;
        }

        public virtual IList<Organisation> InterestedParties
        {
            get;
            set;
        }

        public virtual string BuildingNotes
        {
            get;
            set;
        }

        public virtual string ResidentialProportion
        {
            get;
            set;
        }

        public virtual string OwnerOrTenant
        {
            get;
            set;
        }

        public virtual string HasHoseReels
        {
            get;
            set;
        }

        public virtual string HasFireExtinguishers
        {
            get;
            set;
        }

        public virtual string HasSecurityGuard
        {
            get;
            set;
        }

        public virtual string HasFlammableLiquidsOrGases
        {
            get;
            set;
        }

        public virtual string FlammableLiquidsOrGasesDesc
        {
            get;
            set;
        }

        public virtual string HasSafe
        {
            get;
            set;
        }

        public virtual string HasSafeAlarm
        {
            get;
            set;
        }

        public virtual string HasSafeBolted
        {
            get;
            set;
        }

        public virtual int ConstructionLimit
        {
            get;
            set;
        }

        public virtual int CapitalAdditions
        {
            get;
            set;
        }

        public virtual int CreditConstruction
        {
            get;
            set;
        }

        public virtual int CreditCapAdds
        {
            get;
            set;
        }

        public virtual int DomesticUnits
        {
            get;
            set;
        }

        public virtual int BuildingFRVValue
        {
            get;
            set;
        }

        public virtual int BuildingFRIVValue
        {
            get;
            set;
        }

        public virtual string HasNBSExceed
        {
            get;
            set;
        }

        public virtual bool Removed
        {
            get;
            set;
        }

        public virtual decimal BuildingLatitude
        {
            get;
            set;
        }

        public virtual decimal BuildingLongitude
        {
            get;
            set;
        }

        public virtual string BuildingApproved
        {
            get;
            set;
        }

        public virtual string BuildingCategory
        {
            get;
            set;
        }

        public virtual Building OriginalBuilding
        {
            get;
            protected set;
        }

        public virtual Building CloneForNewSheet(ClientInformationSheet newSheet)
        {
            if (ClientInformationSheet == newSheet)
                throw new Exception("Cannot clone building for original information");

            Building newBuilding = new Building(newSheet.CreatedBy);
            newBuilding.BuildingName = BuildingName;
            if (BuildingLastValuationDate > DateTime.MinValue)
                newBuilding.BuildingLastValuationDate = BuildingLastValuationDate;
            newBuilding.BuildingBasisofSettlement = BuildingBasisofSettlement;
            newBuilding.OwnerOrTenant = OwnerOrTenant;
            newBuilding.BuildingRVValue = BuildingRVValue;
            newBuilding.BuildingRIVValue = BuildingRIVValue;
            newBuilding.BuildingDValue = BuildingDValue;
            newBuilding.BuildingIVValue = BuildingIVValue;
            newBuilding.BuildingIIVValue = BuildingIIVValue;
            newBuilding.ConstructionLimit = ConstructionLimit;
            newBuilding.CapitalAdditions = CapitalAdditions;
            newBuilding.CreditConstruction = CreditConstruction;
            newBuilding.CreditCapAdds = CreditCapAdds;
            newBuilding.DomesticUnits = DomesticUnits;
            newBuilding.BuildingFRVValue = BuildingFRVValue;
            newBuilding.BuildingFRIVValue = BuildingFRIVValue;
            newBuilding.ConstructionType = ConstructionType;
            newBuilding.ConstructionYear = ConstructionYear;
            newBuilding.NumOfUnits = NumOfUnits;
            newBuilding.NumberOfStoreys = NumberOfStoreys;
            newBuilding.HasHoseReels = HasHoseReels;
            newBuilding.HasFireExtinguishers = HasFireExtinguishers;
            newBuilding.StructuralFraming = StructuralFraming;
            newBuilding.HasSprinklers = HasSprinklers;
            newBuilding.NZS4541Compliant = NZS4541Compliant;
            newBuilding.IsHalfOrMoreUnOccupied = IsHalfOrMoreUnOccupied;
            newBuilding.IsTownWaterSupplied = IsTownWaterSupplied;
            newBuilding.ResidentialProportion = ResidentialProportion;
            newBuilding.HasAlarm = HasAlarm;
            newBuilding.HasSecurityGuard = HasSecurityGuard;
            newBuilding.HasFlammableLiquidsOrGases = HasFlammableLiquidsOrGases;
            newBuilding.FlammableLiquidsOrGasesDesc = FlammableLiquidsOrGasesDesc;
            newBuilding.HasNBSExceed = HasNBSExceed;
            newBuilding.HasSafe = HasSafe;
            newBuilding.HasSafeAlarm = HasSafeAlarm;
            newBuilding.HasSafeBolted = HasSafeBolted;
            newBuilding.BuildingNotes = BuildingNotes;
            newBuilding.BuildingLatitude = BuildingLatitude;
            newBuilding.BuildingLongitude = BuildingLongitude;
            newBuilding.Location = newSheet.Locations.FirstOrDefault(l => l.OriginalLocation.Id == Location.Id);
            newBuilding.BuildingApproved = BuildingApproved;
            newBuilding.BuildingCategory = BuildingCategory;

            newBuilding.OriginalBuilding = this;
            return newBuilding;
        }

    }
}
