using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using DealEngine.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models
{
    public class BoatViewModel : BaseViewModel
    {
        public BoatViewModel()
        {
            MooredTypeOptions = GetMooredTypeOptions();
        }

        private List<SelectListItem> GetMooredTypeOptions()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem{ Text = "-- Select --", Value = "0" },
                new SelectListItem{ Text = "Berthed", Value = "Berthed"},
                new SelectListItem{ Text = "Pile", Value = "Pile" },
                new SelectListItem{ Text = "Swing", Value = "Swing" }
            };
        }

        public Guid AnswerSheetId { get; set; }
        public Guid BoatId { get; set; }
        public Guid OriginalBoatId { get; set; }
        public string BoatName { get; set; }
        public string BoatType1 { get; set; }
        public string BoatType2 { get; set; }
        public string HullConstruction { get; set; }
        public string HullConfiguration { get; set; }
        public string OtherHullConstruction { get; set; }
        public string OtherHullConfiguration { get; set; }
        public int YearOfManufacture { get; set; }
        public string BoatMake { get; set; }
        public string BoatModel { get; set; }
        public int MaxSumInsured { get; set; }
        public string BuiltProfessionally { get; set; }
        public string MotorType { get; set; }
        public string ModifiedMotor { get; set; }
        public string MaxRatedSpeed { get; set; }
        public double Sum { get; set; }
        public string RiggingType { get; set; }
        public string MastType { get; set; }
        public string AucklandRegistration { get; set; }
        public string NationalRegistration { get; set; }
        public string BoatEffectiveDate { get; set; }
        public string BoatIPEffectiveDate { get; set; }
        public string BoatCeaseDate { get; set; }
        public int BoatCeaseReason { get; set; }
        public List<SelectListItem> MooredTypeOptions { get; set; }
        public IList<Domain.Entities.Organisation> InterestedParties { get; set; }
        public IList<String> BoatpartyVal { get; set; }
        public IList<Guid> BoatpartyText { get; set; }
        public IList<BoatUse> BoatUse { get; set; }
        public IList<String> BoatselectedVal { get; set; }
        public IList<Guid> BoatselectedText { get; set; }
        public Guid SelectedBoatUse{ get; set; }
        public string SelectedInterestedParty { get; set; }
        public string BoatNotes { get; set; }
        public string WaterLocationMooringType { get; set; }
        public Guid BoatWaterLocation { get; set; }
        public Guid BoatLandLocation { get; set; }
        public Guid BoatTrailer { get; set; }
        public Guid BoatOperator { get; set; }
        public  bool Removed { get; set; }       
        public string BoatIsTrailered { get; set; }
        public decimal BoatQuickQuotePremium { get; set; }
        public int BoatQuoteExcessOption { get; set; }
        public string VesselArea { get; set; }
        public string OtherMarinaName { get; set; }



        public Boat ToEntity(User creatingUser)
        {
            Boat boat = new Boat(creatingUser);
            UpdateEntity(boat);
            return boat;
        }

        public Boat UpdateEntity(Boat boat)
        {
            boat.BoatName = BoatName;
            boat.BoatType1 = BoatType1;
            boat.BoatType2 = BoatType2;
            boat.HullConstruction = HullConstruction;
            boat.HullConfiguration = HullConfiguration;
            boat.YearOfManufacture = YearOfManufacture;
            boat.OtherHullConstruction = OtherHullConstruction;
            boat.OtherHullConfiguration = OtherHullConfiguration;
            boat.BoatMake = BoatMake;
            boat.BoatModel = BoatModel;
            boat.MaxSumInsured = Convert.ToInt32(Sum);
            boat.BuiltProfessionally = BuiltProfessionally;
            boat.MotorType = MotorType;
            boat.ModifiedMotor = ModifiedMotor;
            boat.MaxRatedSpeed = MaxRatedSpeed;
            boat.RiggingType = RiggingType;
            boat.MastType = MastType;
            boat.AucklandRegistration = AucklandRegistration;
            boat.NationalRegistration = NationalRegistration;
            boat.BoatNotes = BoatNotes;
            boat.WaterLocationMooringType = WaterLocationMooringType;
            // boat.BoatUse = BoatUse;
            boat.BoatIsTrailered = BoatIsTrailered;
            boat.BoatQuickQuotePremium = BoatQuickQuotePremium;
            boat.BoatQuoteExcessOption = BoatQuoteExcessOption;
            boat.VesselArea = VesselArea;
            //  boat.BoatOperator = BoatOperator;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
            if (!string.IsNullOrEmpty(BoatEffectiveDate))
            {
                boat.BoatEffectiveDate = DateTime.Parse(BoatEffectiveDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")).ToUniversalTime(tzi);
            }
            else
            {
                boat.BoatEffectiveDate = DateTime.MinValue;
            }
            if (!string.IsNullOrEmpty(BoatIPEffectiveDate))
            {
                boat.BoatIPEffectiveDate = DateTime.Parse(BoatIPEffectiveDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")).ToUniversalTime(tzi);
            }
            else
            {
                boat.BoatIPEffectiveDate = DateTime.MinValue;
            }
            if (!string.IsNullOrEmpty(BoatCeaseDate))
            {
                boat.BoatCeaseDate = DateTime.Parse(BoatCeaseDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")).ToUniversalTime(tzi);
            }
            else
            {
                boat.BoatCeaseDate = DateTime.MinValue;
            }
            boat.BoatCeaseReason = BoatCeaseReason;
            return boat;
        }

        public static BoatViewModel FromEntity(Boat boat)
        {
            BoatViewModel model = new BoatViewModel
            {
                BoatName = boat.BoatName,
                BoatId = boat.Id,
                BoatType1 = boat.BoatType1,
                BoatType2 = boat.BoatType2,
                HullConstruction = boat.HullConstruction,
                HullConfiguration = boat.HullConfiguration,
                YearOfManufacture = boat.YearOfManufacture,
                OtherHullConstruction = boat.OtherHullConstruction,
                OtherHullConfiguration = boat.OtherHullConfiguration,
                BoatMake = boat.BoatMake,
                BoatModel = boat.BoatModel,
                MaxSumInsured = boat.MaxSumInsured,
                BuiltProfessionally = boat.BuiltProfessionally,
                MotorType = boat.MotorType,
                ModifiedMotor = boat.ModifiedMotor,
                MaxRatedSpeed = boat.MaxRatedSpeed,
                RiggingType = boat.RiggingType,
                MastType = boat.MastType,
                AucklandRegistration = boat.AucklandRegistration,
                BoatNotes = boat.BoatNotes,
                BoatIsTrailered = boat.BoatIsTrailered,
                //InterestedParties = boat.InterestedParties.Select(v => v.Id).ToArray(),
                NationalRegistration = boat.NationalRegistration,
                //BoatUse.Add(boat.BoatUse.Select(v => v.Id)),
                BoatIPEffectiveDate = (boat.BoatIPEffectiveDate > DateTime.MinValue) ? boat.BoatIPEffectiveDate.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                WaterLocationMooringType = boat.WaterLocationMooringType,
                BoatEffectiveDate = (boat.BoatEffectiveDate > DateTime.MinValue) ? boat.BoatEffectiveDate.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                BoatCeaseDate = (boat.BoatCeaseDate > DateTime.MinValue) ? boat.BoatCeaseDate.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                BoatCeaseReason = boat.BoatCeaseReason,
                BoatQuickQuotePremium = boat.BoatQuickQuotePremium,
                BoatQuoteExcessOption = boat.BoatQuoteExcessOption,
                VesselArea = boat.VesselArea,


            };
            if (boat.BoatLandLocation != null)
                model.BoatLandLocation = boat.BoatLandLocation.Id;
            if (boat.BoatWaterLocation != null)
                model.BoatWaterLocation = boat.BoatWaterLocation.Id;
            if (boat.BoatOperator != null)
                model.BoatOperator = boat.BoatOperator.Id;
            //model.BoatUse = new List<BoatUse>();
            //foreach (var boatuse in boat.BoatUse)
            //{
            //    model.BoatUse.Add(boatuse);
            //}
            return model;
        }
    }
}

