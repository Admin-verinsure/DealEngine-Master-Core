﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class BuildingViewModel
    {
        public Guid AnswerSheetId { get; set; }

        public Guid BuildingId { get; set; }

        public string BuildingName { get; set; }

        public string ConstructionType { get; set; }

        public int ConstructionYear { get; set; }

        public int NumOfUnits { get; set; }

        public int NumberOfStoreys { get; set; }

        public string IsBuilding3OrMoreStoreysHigh { get; set; }

        public string HasInsulatedSandwichPanels { get; set; }

        public int PercentOfInsuSandwichPanels { get; set; }

        public string StructuralFraming { get; set; }

        public string HasSprinklers { get; set; }

        public string NZS4541Compliant { get; set; }

        public string IsHalfOrMoreUnOccupied { get; set; }

        public string IsTownWaterSupplied { get; set; }

        public string HasAlarm { get; set; }

        public string BuildingLastValuationDate { get; set; }

        public string BuildingBasisofSettlement { get; set; }

        public int BuildingRVValue { get; set; }

        public int BuildingRIVValue { get; set; }

        public int BuildingDValue { get; set; }

        public int BuildingIVValue { get; set; }

        public int BuildingIIVValue { get; set; }

        public string BuildingBasisofSettlementND { get; set; }

        public string ContentsBasisofSettlement { get; set; }

        public int ContentsRVValue { get; set; }

        public int ContentsIVValue { get; set; }

        public string ContentsBasisofSettlementND { get; set; }

        public string StockBasisofSettlement { get; set; }

        public int StockRVValue { get; set; }

        public int StockIVValue { get; set; }

        public string StockBasisofSettlementND { get; set; }

        public string HasMDND { get; set; }

        public string BuildingNotes { get; set; }

        public string ResidentialProportion { get; set; }

        public string OwnerOrTenant { get; set; }

        public string HasHoseReels { get; set; }

        public string HasFireExtinguishers { get; set; }

        public string HasSecurityGuard { get; set; }

        public string HasFlammableLiquidsOrGases { get; set; }

        public string FlammableLiquidsOrGasesDesc { get; set; }

        public string HasSafe { get; set; }

        public string HasSafeAlarm { get; set; }

        public string HasSafeBolted { get; set; }

        public int ConstructionLimit { get; set; }

        public int CapitalAdditions { get; set; }

        public int CreditConstruction { get; set; }

        public int CreditCapAdds { get; set; }

        public int DomesticUnits { get; set; }

        public int BuildingFRVValue { get; set; }

        public int BuildingFRIVValue { get; set; }

        public string HasNBSExceed { get; set; }

        public IList<SelectListItem> OrganisationalUnits { get; set; }

        public Guid[] InterestedParties { get; set; }

        public Guid BuildingLocation { get; set; }

        public decimal BuildingLatitude { get; set; }

        public decimal BuildingLongitude { get; set; }

        public string BuildingApproved { get; set; }

        public string BuildingCategory { get; set; }
        public string LocationStreet { get; set; }
        public Building ToEntity(User creatingUser)
        {
            Building building = new Building(creatingUser);
            UpdateEntity(building);
            return building;
        }

        public Building UpdateEntity(Building building)
        {
            building.BuildingName = BuildingName;
            building.OwnerOrTenant = OwnerOrTenant;
            if (!string.IsNullOrEmpty(BuildingLastValuationDate))
            {
                building.BuildingLastValuationDate = DateTime.Parse(BuildingLastValuationDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
            }
            else
            {
                building.BuildingLastValuationDate = DateTime.MinValue;
            }
            building.BuildingRVValue = BuildingRVValue;
            building.BuildingRIVValue = BuildingRIVValue;
            building.BuildingDValue = BuildingDValue;
            building.BuildingIVValue = BuildingIVValue;
            building.BuildingIIVValue = BuildingIIVValue;
            building.ConstructionLimit = ConstructionLimit;
            building.CapitalAdditions = CapitalAdditions;
            building.CreditConstruction = CreditConstruction;
            building.CreditCapAdds = CreditCapAdds;
            building.DomesticUnits = DomesticUnits;
            building.BuildingFRVValue = BuildingFRVValue;
            building.BuildingFRIVValue = BuildingFRIVValue;
            building.ConstructionType = ConstructionType;
            building.ConstructionYear = ConstructionYear;
            building.ResidentialProportion = ResidentialProportion;
            building.NumOfUnits = NumOfUnits;
            building.NumberOfStoreys = NumberOfStoreys;
            building.HasSprinklers = HasSprinklers;
            building.NZS4541Compliant = NZS4541Compliant;
            building.HasHoseReels = HasHoseReels;
            building.HasFireExtinguishers = HasFireExtinguishers;
            building.IsTownWaterSupplied = IsTownWaterSupplied;
            building.IsHalfOrMoreUnOccupied = IsHalfOrMoreUnOccupied;
            building.HasAlarm = HasAlarm;
            building.HasSecurityGuard = HasSecurityGuard;
            building.HasFlammableLiquidsOrGases = HasFlammableLiquidsOrGases;
            building.FlammableLiquidsOrGasesDesc = FlammableLiquidsOrGasesDesc;
            building.HasNBSExceed = HasNBSExceed;
            building.HasSafe = HasSafe;
            building.HasSafeAlarm = HasSafeAlarm;
            building.HasSafeBolted = HasSafeBolted;
            building.BuildingNotes = BuildingNotes;
            building.BuildingLatitude = BuildingLatitude;
            building.BuildingLongitude = BuildingLongitude;
            building.BuildingApproved = BuildingApproved;
            building.BuildingCategory = BuildingCategory;
            return building;
        }

        public static BuildingViewModel FromEntity(Building building)
        {
            BuildingViewModel model = new BuildingViewModel
            {
                BuildingId = building.Id,
                BuildingName = building.BuildingName,
                OwnerOrTenant = building.OwnerOrTenant,
                BuildingLastValuationDate = (building.BuildingLastValuationDate > DateTime.MinValue) ? building.BuildingLastValuationDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                BuildingRVValue = building.BuildingRVValue,
                BuildingRIVValue = building.BuildingRIVValue,
                BuildingDValue = building.BuildingDValue,
                BuildingIVValue = building.BuildingIVValue,
                BuildingIIVValue = building.BuildingIIVValue,
                ConstructionLimit = building.ConstructionLimit,
                CapitalAdditions = building.CapitalAdditions,
                CreditConstruction = building.CreditConstruction,
                CreditCapAdds = building.CreditCapAdds,
                DomesticUnits = building.DomesticUnits,
                BuildingFRVValue = building.BuildingFRVValue,
                BuildingFRIVValue = building.BuildingFRIVValue,
                ConstructionType = building.ConstructionType,
                ConstructionYear = building.ConstructionYear,
                ResidentialProportion = building.ResidentialProportion,
                NumOfUnits = building.NumOfUnits,
                NumberOfStoreys = building.NumberOfStoreys,
                HasSprinklers = building.HasSprinklers,
                NZS4541Compliant = building.NZS4541Compliant,
                HasHoseReels = building.HasHoseReels,
                HasFireExtinguishers = building.HasFireExtinguishers,
                IsTownWaterSupplied = building.IsTownWaterSupplied,
                IsHalfOrMoreUnOccupied = building.IsHalfOrMoreUnOccupied,
                HasAlarm = building.HasAlarm,
                HasSecurityGuard = building.HasSecurityGuard,
                HasFlammableLiquidsOrGases = building.HasFlammableLiquidsOrGases,
                FlammableLiquidsOrGasesDesc = building.FlammableLiquidsOrGasesDesc,
                HasNBSExceed = building.HasNBSExceed,
                HasSafe = building.HasSafe,
                HasSafeAlarm = building.HasSafeAlarm,
                HasSafeBolted = building.HasSafeBolted,
                BuildingNotes = building.BuildingNotes,
                BuildingLatitude = building.BuildingLatitude,
                BuildingLongitude = building.BuildingLongitude,
                BuildingApproved = building.BuildingApproved,
                BuildingCategory = building.BuildingCategory,
              //  Locationst = building.Location,

        };
            if (building.Location != null)
            {
                model.BuildingLocation = building.Location.Id;
                model.LocationStreet = building.Location.Street;
            }

            return model;
        }
    }

}