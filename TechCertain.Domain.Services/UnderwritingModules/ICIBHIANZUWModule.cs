using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Domain.Services.UnderwritingModules
{
	public class ICIBHIANZUWModule : IUnderwritingModule
	{
		public string Name { get; protected set; }

        public ICIBHIANZUWModule ()
		{
			Name = "ICIB_HIANZ";
        }

		public bool Underwrite (User currentUser, ClientInformationSheet informationSheet)
		{
			throw new NotImplementedException ();
		}

		public bool Underwrite (User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
		{
			ClientAgreement agreement = GetClientAgreement (underwritingUser, informationSheet, informationSheet.Programme, product, reference);
			Guid id = agreement.Id;

            ClientAgreementTerm term = GetAgreementTerm (underwritingUser, agreement, "MV");
			var mvTerms = term.MotorTerms;

            if (mvTerms != null)
            {
                foreach (ClientAgreementMVTerm mVTerm in mvTerms)
                {
                    mVTerm.Delete(underwritingUser);
                }
            }

            if (agreement.ClientAgreementRules.Count == 0)
				foreach (var rule in product.Rules.Where (r => !string.IsNullOrWhiteSpace (r.Name)))
					agreement.ClientAgreementRules.Add (new ClientAgreementRule (underwritingUser, rule, agreement));

			if (agreement.ClientAgreementEndorsements.Count == 0)
				foreach (var endorsement in product.Endorsements.Where (e => !string.IsNullOrWhiteSpace (e.Name)))
					agreement.ClientAgreementEndorsements.Add (new ClientAgreementEndorsement (underwritingUser, endorsement, agreement));

			IDictionary<string, decimal> rates = BuildRulesTable (agreement, "NICITYSVRate", "NITOWNSVRate", "SICITYSVRate", "SITOWNSVRate",
			                                                      "NICITY1CRate", "NITOWN1CRate", "SICITY1CRate", "SITOWN1CRate", "NICITY1UAPRate", "NITOWN1UAPRate", "SICITY1UAPRate", "SITOWN1UAPRate", 
			                                                      "NICITY1PRate", "NITOWN1PRate", "SICITY1PRate", "SITOWN1PRate", "NICITY1RRate", "NITOWN1RRate", "SICITY1RRate", "SITOWN1RRate", 
			                                                      "NICITY2Rate", "NITOWN2Rate", "SICITY2Rate", "SITOWN2Rate", "NICITY3Rate", "NITOWN3Rate", "SICITY3Rate", "SITOWN3Rate",
																  "NICITYSVRate", "NITOWNSVRate", "SICITYSVRate", "SITOWNSVRate", "FSLUNDERFee", "FSLOVER3Rate"
																 );
			decimal totalVehicleFsl = 0m;
			int totalVehicleSumInsured = 0;
			decimal totalVehiclePremium = 0m;
			decimal totalVehicleBrokerage = 0m;
			foreach (var vehicle in informationSheet.Vehicles.Where (v => v.VehicleType > 0 && !v.Removed && v.DateDeleted == null)) {
				string vehicleCategory = null;
				decimal vehicleRate = 0m;
				decimal vehicleFsl = 0m;
				decimal vehicleFslRate = 0m;
				decimal vehiclePremium = 0m;
				decimal vehicleExcess = 0m;
				decimal vehicleBrokerage = 0m;
				decimal vehicleBrokerageRate = 0m;

				GetRateAndCategoryFor (rates, vehicle, out vehicleCategory, out vehicleRate);
				vehicle.VehicleCategory = vehicleCategory;
				GetFslFor (rates, vehicle, ref vehicleFsl, ref vehicleFslRate);

				vehicleExcess = 500;
				if (vehicle.GroupSumInsured * 1 / 100 > 500)
					vehicleExcess = (vehicle.GroupSumInsured * 1 / 100);

				vehiclePremium = (vehicle.GroupSumInsured * vehicleRate / 100) + vehicleFsl;
				vehicleBrokerageRate = agreement.Brokerage;
				vehicleBrokerage = vehiclePremium * vehicleBrokerageRate / 100;
				totalVehicleFsl += vehicleFsl;
				totalVehicleSumInsured += vehicle.GroupSumInsured;
				totalVehiclePremium += vehiclePremium;
				totalVehicleBrokerage += vehicleBrokerage;

				ClientAgreementMVTerm mvTerm = null;
				if (term.MotorTerms != null)
					mvTerm = term.MotorTerms.FirstOrDefault (mvt => mvt.Vehicle == vehicle && mvt.DateDeleted != null);
				else
					term.MotorTerms = new List<ClientAgreementMVTerm> ();
				
				if (mvTerm == null) {
					mvTerm = new ClientAgreementMVTerm (underwritingUser, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured,
													   vehicleExcess, vehiclePremium, vehicleFsl, vehicleBrokerageRate, vehicleBrokerage, vehicleCategory, vehicle.FleetNumber, term, vehicle, 0M);
					term.MotorTerms.Add (mvTerm);
				}
				else {
					mvTerm.Registration = vehicle.Registration;
					mvTerm.Year = vehicle.Year;
					mvTerm.Make = vehicle.Make;
					mvTerm.Model = vehicle.Model;
					mvTerm.TermLimit = vehicle.GroupSumInsured;
					mvTerm.Premium = vehiclePremium;
					mvTerm.FSL = vehicleFsl;
					mvTerm.DateDeleted = null;
					mvTerm.BrokerageRate = vehicleBrokerageRate;
					mvTerm.Brokerage = vehicleBrokerage;
					mvTerm.VehicleCategory = vehicleCategory;
					mvTerm.FleetNumber = vehicle.FleetNumber;
					mvTerm.LastModifiedOn = DateTime.UtcNow;
					mvTerm.LastModifiedBy = underwritingUser;
				}

			}

			term.TermLimit = totalVehicleSumInsured;
			term.Premium = totalVehiclePremium;
			term.FSL = totalVehicleFsl;
			term.BrokerageRate = agreement.Brokerage;
			term.Brokerage = totalVehicleBrokerage;

			agreement.QuoteDate = DateTime.UtcNow;

			return true;
			//throw new NotImplementedException ();
		}

		ClientAgreement GetClientAgreement (User currentUser, ClientInformationSheet informationSheet, ClientProgramme programme, Product product, string reference)
		{
			ClientAgreement clientAgreement = programme.Agreements.FirstOrDefault (a => a.Product != null && a.Product.Id == product.Id);
			if (clientAgreement == null) {
				DateTime inceptionDate = (product.DefaultInceptionDate > DateTime.MinValue) ? product.DefaultInceptionDate : DateTime.UtcNow;
				DateTime expiryDate = (product.DefaultExpiryDate > DateTime.MinValue) ? product.DefaultExpiryDate : DateTime.UtcNow;

				clientAgreement = new ClientAgreement (currentUser, informationSheet.Owner.Name, inceptionDate, expiryDate, product.DefaultBrokerage, product.DefaultBrokerFee, informationSheet, product, reference);
				programme.Agreements.Add (clientAgreement);

			}
			return clientAgreement;
		}

		ClientAgreementTerm GetAgreementTerm (User currentUser, ClientAgreement agreement, string subTerm)
		{
			ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault (t => t.SubTermType == subTerm && t.DateDeleted == null);
			if (term == null) {
				term = new ClientAgreementTerm (currentUser, 0, 0m, 0m, 0m, 0m, 0m, agreement, subTerm);
				agreement.ClientAgreementTerms.Add (term);
			}

			return term;
		}

		IDictionary<string, decimal> BuildRulesTable (ClientAgreement agreement, params string [] names)
		{
			var dict = new Dictionary<string, decimal> ();

			foreach (string name in names)
				dict [name] = Convert.ToDecimal (agreement.ClientAgreementRules.FirstOrDefault (r => r.Name == name).Value);

			return dict;
		}

		void GetRateAndCategoryFor (IDictionary<string, decimal> rates, Vehicle vehicle, out string vehicleCategory, out decimal vehicleRate)
		{
			switch (vehicle.VehicleType) {
			case 1: {
					switch (vehicle.SubUseType) {
					case 1: {
							vehicleCategory = "1C";
							vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITY1CRate"], rates ["NICITY1CRate"], rates ["NITOWN1CRate"], rates ["SICITY1CRate"], rates ["SITOWN1CRate"]);
							break;
						}
					case 2: {
							vehicleCategory = "1P";
							vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITY1PRate"], rates ["NICITY1PRate"], rates ["NITOWN1PRate"], rates ["SICITY1PRate"], rates ["SITOWN1PRate"]);
							break;
						}
					case 3: {
							vehicleCategory = "1R";
							vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITY1RRate"], rates ["NICITY1RRate"], rates ["NITOWN1RRate"], rates ["SICITY1RRate"], rates ["SITOWN1RRate"]);
							break;
						}
					case 4: {
							vehicleCategory = "1UAP";
							vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITY1UAPRate"], rates ["NICITY1UAPRate"], rates ["NITOWN1UAPRate"], rates ["SICITY1UAPRate"], rates ["SITOWN1UAPRate"]);
							break;
						}
					default: {
							throw new Exception (string.Format ("Invalid Vehicle SubUseType {0} for Vehicle {1}", vehicle.SubUseType, vehicle.Id));
						}
					}
					break;
				}
			case 2: {
					vehicleCategory = "2";
					vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITY2Rate"], rates ["NICITY2Rate"], rates ["NITOWN2Rate"], rates ["SICITY2Rate"], rates ["SITOWN2Rate"]);
					break;
				}
			// 3 and 4 share the same rates
			case 3:
			case 4: {
					vehicleCategory = "3";
					vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITY3Rate"], rates ["NICITY3Rate"], rates ["NITOWN3Rate"], rates ["SICITY3Rate"], rates ["SITOWN3Rate"]);
					break;
				}
			case 5: {
					vehicleCategory = "SV";
					vehicleRate = GetVehicleRate (vehicle.AreaOfOperation, 0m, rates ["NICITYSVRate"], rates ["NICITYSVRate"], rates ["NITOWNSVRate"], rates ["SICITYSVRate"], rates ["SITOWNSVRate"]);
					break;
				}
			default: {
					throw new Exception (string.Format ("Invalid Vehicle VehicleType {0} for Vehicle {1}", vehicle.VehicleType, vehicle.Id));
				}
			}
		}

		void GetFslFor (IDictionary<string, decimal> rates, Vehicle vehicle, ref decimal vehicleFsl, ref decimal vehicleFslRate)
		{
			decimal fslUnderFee = rates ["FSLUNDERFee"];
			decimal fslOver3Fee = rates ["FSLOVER3Rate"];

			if (vehicle.VehicleCategory != "3") {
				if (vehicle.GrossVehicleMass >= 0 && vehicle.GrossVehicleMass < 3500) {
					vehicleFsl = fslUnderFee;
				}
				else if (vehicle.GrossVehicleMass >= 3500) {
					vehicleFslRate = fslOver3Fee;
					vehicleFsl = vehicle.GroupSumInsured * vehicleFslRate / 100;

					if (vehicleFsl < fslUnderFee)
						vehicleFsl = fslUnderFee;
				}
			}
			else {
				vehicleFslRate = fslOver3Fee;
				vehicleFsl = vehicle.GroupSumInsured * vehicleFslRate / 100;
			}
		}

		decimal GetVehicleRate (int value, decimal defaultRate, params decimal[] rates)
		{
			int index = value - 1;
			if (index < 0 || index >= rates.Length)
				return defaultRate;

			return rates [index];
		}
	}
}

