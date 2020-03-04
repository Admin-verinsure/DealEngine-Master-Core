using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.WebUI.Helpers;

namespace DealEngine.WebUI.Models
{
	public class VehicleViewModel : BaseViewModel
	{
        public Guid AnswerSheetId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid OriginalVehicleId { get; set; }

        public bool Validated { get; set; }

        public string Registration { get; set; }

        public string Year { get; set; }

        public string Make { get; set; }

        public string VehicleModel { get; set; }

        public string VIN { get; set; }

        public string ChassisNumber { get; set; }

        public string EngineNumber { get; set; }

        public double SumInsured { get; set; }

        public string FleetNumber { get; set; }

        public string SerialNumber { get; set; }

        public string GrossVehicleMass { get; set; }

        public int AreaOfOperation { get; set; }

        public int VehicleType { get; set; }

        public int Use { get; set; }

        public int SubUse { get; set; }

        public string EffectiveDate { get; set; }

        public string CeaseDate { get; set; }

        public int CeaseReason { get; set; }

        public Guid[] InterestedParties { get; set; }

        public Guid[] InterestedParties2
        {
            get
            {
                return InterestedParties;
            }
            set
            {
                InterestedParties = value;
            }
        }

        public string Notes { get; set; }

        public Guid VehicleLocation { get; set; }

        public string VehicleCategory { get; set; }

        public VehicleViewModel()
        {
            Make = "Not Found";
            VehicleModel = "Not Found";
            SumInsured = 0;
        }

        public Vehicle ToEntity(User creatingUser)
        {
            Vehicle vehicle = new Vehicle(creatingUser, Registration, Make, VehicleModel);
            UpdateEntity(vehicle);
            return vehicle;
        }

        public Vehicle UpdateEntity(Vehicle vehicle)
        {
            // update basic feilds that don't require repository access
            try
            {
                vehicle.Make = Make;
                vehicle.Model = VehicleModel;
                vehicle.Year = Year;
                vehicle.GroupSumInsured = Convert.ToInt32(SumInsured);
                vehicle.FleetNumber = FleetNumber;
                vehicle.SerialNumber = SerialNumber;
                vehicle.AreaOfOperation = AreaOfOperation;
                vehicle.VehicleType = VehicleType;
                vehicle.UseType = Use;
                vehicle.SubUseType = SubUse;
                vehicle.VIN = VIN;
                vehicle.ChassisNumber = ChassisNumber;
                vehicle.EngineNumber = EngineNumber;
                vehicle.GrossVehicleMass = Convert.ToInt32(GrossVehicleMass);
                vehicle.Validated = Validated;
                vehicle.Notes = Notes;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
                if (!string.IsNullOrEmpty(EffectiveDate))
                {
                    vehicle.VehicleEffectiveDate = DateTime.Parse(EffectiveDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")).ToUniversalTime(tzi);
                }
                else
                {
                    vehicle.VehicleEffectiveDate = DateTime.MinValue;
                }
                if (!string.IsNullOrEmpty(CeaseDate))
                {
                    vehicle.VehicleCeaseDate = DateTime.Parse(CeaseDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")).ToUniversalTime(tzi);
                }
                else
                {
                    vehicle.VehicleCeaseDate = DateTime.MinValue;
                }
                vehicle.VehicleCeaseReason = CeaseReason;

                return vehicle;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return vehicle;
            }
        }

        public static VehicleViewModel FromEntity(Vehicle vehicle)
        {
            VehicleViewModel model = new VehicleViewModel
            {
                Validated = vehicle.Validated,
                VehicleId = vehicle.Id,
                Registration = vehicle.Registration,
                Year = vehicle.Year,
                Make = vehicle.Make,
                VehicleModel = vehicle.Model,
                VIN = vehicle.VIN,
                ChassisNumber = vehicle.ChassisNumber,
                EngineNumber = vehicle.EngineNumber,
                GrossVehicleMass = vehicle.GrossVehicleMass.ToString(),
                SumInsured = vehicle.GroupSumInsured,
                FleetNumber = vehicle.FleetNumber,
                SerialNumber = vehicle.SerialNumber,
                AreaOfOperation = vehicle.AreaOfOperation,
                VehicleType = vehicle.VehicleType,
                Use = vehicle.UseType,
                SubUse = vehicle.SubUseType,
                InterestedParties = vehicle.InterestedParties.Select(v => v.Id).ToArray(),
                Notes = vehicle.Notes,
                EffectiveDate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                CeaseDate = (vehicle.VehicleCeaseDate > DateTime.MinValue) ? vehicle.VehicleCeaseDate.ToTimeZoneTime(UserTimeZone).ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("en-NZ")) : "",
                CeaseReason = vehicle.VehicleCeaseReason,
            };
            if (vehicle.GarageLocation != null)
                model.VehicleLocation = vehicle.GarageLocation.Id;

            return model;
        }
	}
}

