using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class BoatUseViewModel : BaseViewModel
    {
        public Guid AnswerSheetId { get; set; }
        public Guid BoatUseId { get; set; }
        public string BoatUseCategory { get; set; }
        public string BoatUseLiveOnBoard { get; set; }
        public string BoatUseRace { get; set; }
        public string BoatUseRaceCategory { get; set; }
        public string BoatUseRaceUseSpinnakers { get; set; }
        public string BoatUseLiveNotes { get; set; }
        public string BoatUseRaceNotes { get; set; }
        public string BoatUseAdditionalNotes { get; set; }
        public string BoatUseEffectiveDate { get; set; }
        public string BoatUseCeaseDate { get; set; }
        public int BoatUseCeaseReason { get; set; }

        public BoatUse ToEntity(User creatingUser)
        {
            BoatUse boatUse = new BoatUse(creatingUser, BoatUseCategory);
            UpdateEntity(boatUse);
            return boatUse;
        }

        public BoatUse UpdateEntity(BoatUse boatUse)
        {
            boatUse.BoatUseCategory = BoatUseCategory;
            boatUse.BoatUseLiveOnBoard = BoatUseLiveOnBoard;
            boatUse.BoatUseRace = BoatUseRace;
            boatUse.BoatUseRaceCategory = BoatUseRaceCategory;
            boatUse.BoatUseRaceUseSpinnakers = BoatUseRaceUseSpinnakers;
            boatUse.BoatUseLiveNotes = BoatUseLiveNotes;
            boatUse.BoatUseRaceNotes = BoatUseRaceNotes;

            boatUse.BoatUseAdditionalNotes = BoatUseAdditionalNotes;
            if (!string.IsNullOrEmpty(BoatUseEffectiveDate))
            {
                boatUse.BoatUseEffectiveDate = DateTime.Parse(BoatUseEffectiveDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
            }
            else
            {
                boatUse.BoatUseEffectiveDate = DateTime.MinValue;
            }
            if (!string.IsNullOrEmpty(BoatUseCeaseDate))
            {
                boatUse.BoatUseCeaseDate = DateTime.Parse(BoatUseCeaseDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ"));
            }
            else
            {
                boatUse.BoatUseCeaseDate = DateTime.MinValue;
            }
            boatUse.BoatUseCeaseReason = BoatUseCeaseReason;

            return boatUse;
        }

        public static BoatUseViewModel FromEntity(BoatUse boatUse)
        {
            BoatUseViewModel model = new BoatUseViewModel
            {
                BoatUseCategory = boatUse.BoatUseCategory,
                BoatUseId = boatUse.Id,
                BoatUseLiveOnBoard = boatUse.BoatUseLiveOnBoard,
                BoatUseRace = boatUse.BoatUseRace,
                BoatUseRaceCategory = boatUse.BoatUseRaceCategory,
                BoatUseRaceUseSpinnakers = boatUse.BoatUseRaceUseSpinnakers,
                BoatUseLiveNotes = boatUse.BoatUseLiveNotes,
                BoatUseRaceNotes = boatUse.BoatUseRaceNotes,

                BoatUseAdditionalNotes = boatUse.BoatUseAdditionalNotes,
                BoatUseEffectiveDate = (boatUse.BoatUseEffectiveDate > DateTime.MinValue) ? boatUse.BoatUseEffectiveDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                BoatUseCeaseDate = (boatUse.BoatUseCeaseDate > DateTime.MinValue) ? boatUse.BoatUseCeaseDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                BoatUseCeaseReason = boatUse.BoatUseCeaseReason,
            };
            //if (boatUse.BoatUseBoat != null)
            //    model.BoatUseBoat = boatUse.BoatUseBoat.Id;

            return model;
        }
    }
}

