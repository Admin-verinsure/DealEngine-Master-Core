using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Domain.Services.UnderwritingModules
{
	public class ICIBARCCOUWModule : IUnderwritingModule
	{
		public string Name { get; protected set; }

        public ICIBARCCOUWModule ()
		{
			Name = "ICIB_ARCCO";
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

			if (mvTerms != null) {
				foreach (ClientAgreementMVTerm mVTerm in mvTerms) {
					mVTerm.Delete (underwritingUser);
				}
			}

			if (agreement.ClientAgreementRules.Count == 0)
				foreach (var rule in product.Rules.Where (r => !string.IsNullOrWhiteSpace (r.Name)))
					agreement.ClientAgreementRules.Add (new ClientAgreementRule (underwritingUser, rule, agreement));

			if (agreement.ClientAgreementEndorsements.Count == 0)
				foreach (var endorsement in product.Endorsements.Where (e => !string.IsNullOrWhiteSpace (e.Name)))
					agreement.ClientAgreementEndorsements.Add (new ClientAgreementEndorsement (underwritingUser, endorsement, agreement));

			IDictionary<string, decimal> rates = BuildRulesTable (agreement, "FSLUNDERFee", "FSLOVER3Rate", "MVRate", "BurnerRate",
																  "MinPremium", "MinPremiumLess"
																 );
			decimal totalVehicleFsl = 0m;
			int totalVehicleSumInsured = 0;
			decimal totalVehiclePremium = 0m;
			decimal totalVehicleBurnerPremium = 0m;
			decimal totalVehicleBrokerage = 0m;
			decimal vehicleMinPremium = 0m;

			decimal totalvehicleFslPre = 0m;
			decimal totalvehicleFslDiffer = 0m;
			decimal totalvehiclePremiumPre = 0m;
			decimal totalvehiclePremiumDiffer = 0m;
			decimal totalvehicleBurnerPremiumPre = 0m;
			decimal totalvehicleBurnerPremiumDiffer = 0m;
			int totalvehicleTermLimitPre = 0;
			int totalvehicleTermLimitDiffer = 0;

			if (informationSheet.Vehicles.Where (v => !v.Removed && v.DateDeleted == null && v.VehicleCeaseDate == DateTime.MinValue).Count () >= 10) {
				vehicleMinPremium = rates ["MinPremium"];
			} else {
				vehicleMinPremium = rates ["MinPremiumLess"];
			}


			foreach (var vehicle in informationSheet.Vehicles.Where (v => !v.Removed && v.DateDeleted == null)) {
				string vehicleCategory = null;
				decimal vehicleRate = 0m;
				vehicleRate = rates ["MVRate"];
				decimal vehicleFsl = 0m;
				decimal vehicleFslRate = 0m;
				decimal vehiclePremium = 0m;
				decimal vehicleBurnerRate = 0m;
				vehicleBurnerRate = rates ["BurnerRate"];
				decimal vehicleBurnerPremium = 0m;
				decimal vehicleExcess = 0m;
				decimal vehicleBrokerage = 0m;
				decimal vehicleBrokerageRate = 0m;

				GetFslFor (rates, vehicle, ref vehicleFsl, ref vehicleFslRate);

				vehicleExcess = 2000;

				//Pre-rate premiums based on the vehicle effectiove date and cease date
				DateTime vinceptiondate;
				DateTime vexpirydate;
				DateTime veffectivedate;
				int numberofdaysofcover = 0;
				int defaultnumberofdaysofcover = 0;

				veffectivedate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate : agreement.InceptionDate;
				vexpirydate = (vehicle.VehicleCeaseDate > DateTime.MinValue) ? vehicle.VehicleCeaseDate : agreement.ExpiryDate;
				vinceptiondate = veffectivedate;

				if (vehicle.OriginalVehicle != null) //exsiting vehicles
				{
					vinceptiondate = vehicle.OriginalVehicle.VehicleInceptionDate;
					vexpirydate = vehicle.OriginalVehicle.VehicleExpireDate;
					veffectivedate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate : vehicle.OriginalVehicle.VehicleInceptionDate;

					if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue) {
						vexpirydate = vehicle.VehicleCeaseDate;
					} else if (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseDate == DateTime.MinValue) {
						vinceptiondate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate : vehicle.OriginalVehicle.VehicleInceptionDate;
						vexpirydate = agreement.ExpiryDate;
					}


				} else //new vehicles
				{
					veffectivedate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate : agreement.InceptionDate;
					vexpirydate = (vehicle.VehicleCeaseDate > DateTime.MinValue) ? vehicle.VehicleCeaseDate : agreement.ExpiryDate;
					vinceptiondate = veffectivedate;
				}


				numberofdaysofcover = (vexpirydate - veffectivedate).Days;
				defaultnumberofdaysofcover = (agreement.ExpiryDate - agreement.ExpiryDate.AddYears (-1)).Days;

				vehicle.VehicleInceptionDate = vinceptiondate;
				vehicle.VehicleExpireDate = vexpirydate;

				vehicleBurnerPremium = vehicle.GroupSumInsured * vehicleBurnerRate / 100 / defaultnumberofdaysofcover * numberofdaysofcover;
				vehiclePremium = (vehicle.GroupSumInsured * vehicleRate / 100) + vehicleFsl;
				if (vehicle.VehicleType == 8) //Trailer
				{
					if (vehiclePremium < (100 + vehicleFsl)) {
						vehiclePremium = 100 + vehicleFsl;
					}
				} else if (vehicle.VehicleType == 9) //Motorcycle
				  {
					if (vehiclePremium < (350 + vehicleFsl)) {
						vehiclePremium = 350 + vehicleFsl;
					}
				} else {
					if (vehiclePremium < (vehicleMinPremium + vehicleFsl)) {
						vehiclePremium = vehicleMinPremium + vehicleFsl;
					}
				}

				vehiclePremium = vehiclePremium / defaultnumberofdaysofcover * numberofdaysofcover;
				vehicleFsl = vehicleFsl / defaultnumberofdaysofcover * numberofdaysofcover;
				vehicleBrokerageRate = agreement.Brokerage;
				vehicleBrokerage = vehiclePremium * vehicleBrokerageRate / 100;
				totalVehicleFsl += vehicleFsl;
				totalVehicleSumInsured += (vehicle.VehicleCeaseDate > DateTime.MinValue) ? 0 : vehicle.GroupSumInsured;
				totalVehiclePremium += vehiclePremium;
				totalVehicleBurnerPremium += vehicleBurnerPremium;
				totalVehicleBrokerage += vehicleBrokerage;

				ClientAgreementMVTerm mvTerm = null;
				if (term.MotorTerms != null)
					mvTerm = term.MotorTerms.FirstOrDefault (mvt => mvt.Vehicle == vehicle && mvt.DateDeleted != null);
				else
					term.MotorTerms = new List<ClientAgreementMVTerm> ();

				if (mvTerm == null) {
					mvTerm = new ClientAgreementMVTerm (underwritingUser, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured,
													   vehicleExcess, vehiclePremium, vehicleFsl, vehicleBrokerageRate, vehicleBrokerage, vehicleCategory, vehicle.FleetNumber, term, vehicle, vehicleBurnerPremium);
					term.MotorTerms.Add (mvTerm);
				} else {
					mvTerm.Registration = vehicle.Registration;
					mvTerm.Year = vehicle.Year;
					mvTerm.Make = vehicle.Make;
					mvTerm.Model = vehicle.Model;
					mvTerm.TermLimit = vehicle.GroupSumInsured;
					mvTerm.Premium = vehiclePremium;
					mvTerm.BurnerPremium = vehicleBurnerPremium;
					mvTerm.FSL = vehicleFsl;
					mvTerm.DateDeleted = null;
					mvTerm.BrokerageRate = vehicleBrokerageRate;
					mvTerm.Brokerage = vehicleBrokerage;
					mvTerm.VehicleCategory = vehicleCategory;
					mvTerm.FleetNumber = vehicle.FleetNumber;
					mvTerm.LastModifiedOn = DateTime.UtcNow;
					mvTerm.LastModifiedBy = underwritingUser;
				}

				//For Change Agreement
				decimal vehicleFslPre = 0m;
				decimal vehicleFslDiffer = 0m;
				decimal vehiclePremiumPre = 0m;
				decimal vehiclePremiumDiffer = 0m;
				decimal vehicleBurnerPremiumPre = 0m;
				decimal vehicleBurnerPremiumDiffer = 0m;
				decimal vehicleExcessPre = 0m;
				decimal vehicleExcessDiffer = 0m;
				int vehicleTermLimitPre = 0;
				int vehicleTermLimitDiffer = 0;
				int prenumberofdaysofcover = 0;

				if (agreement.ClientInformationSheet.PreviousInformationSheet != null && vehicle.OriginalVehicle != null) {
					//ClientAgreementTerm termPre = agreement.ClientInformationSheet.PreviousInformationSheet.ClientAgreement.ClientAgreementTerms.FirstOrDefault (t => t.SubTermType == "MV" && t.DateDeleted == null);
					ClientAgreementTerm termPre = agreement.ClientInformationSheet.PreviousInformationSheet.Programme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault (t => t.SubTermType == "MV" && t.DateDeleted == null);
					if (termPre != null) {
						ClientAgreementMVTerm mvTermPre = termPre.MotorTerms.FirstOrDefault (mvt => mvt.Vehicle == vehicle.OriginalVehicle && mvt.DateDeleted == null);

						if (mvTermPre != null) {
							if (vehicle.OriginalVehicle.VehicleEffectiveDate > DateTime.MinValue) {
								prenumberofdaysofcover = (vehicle.OriginalVehicle.VehicleExpireDate - vehicle.OriginalVehicle.VehicleEffectiveDate).Days;
							} else {
								prenumberofdaysofcover = (vehicle.OriginalVehicle.VehicleExpireDate - vehicle.OriginalVehicle.VehicleInceptionDate).Days;
							}

							if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue) {
								prenumberofdaysofcover = (vehicle.VehicleExpireDate - vehicle.VehicleInceptionDate).Days;
							}
							if (vehicle.OriginalVehicle.VehicleEffectiveDate > DateTime.MinValue && vehicle.VehicleEffectiveDate > DateTime.MinValue) {
								if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.OriginalVehicle.VehicleEffectiveDate == vehicle.VehicleEffectiveDate) {
									prenumberofdaysofcover = (vehicle.VehicleExpireDate - vehicle.VehicleEffectiveDate).Days;
								}
							}

                            if (vehicle.OriginalVehicle.VehicleEffectiveDate > DateTime.MinValue && vehicle.VehicleEffectiveDate > DateTime.MinValue)
                            {
                                if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.OriginalVehicle.VehicleEffectiveDate == vehicle.VehicleEffectiveDate)
                                {
                                    prenumberofdaysofcover = (vehicle.VehicleExpireDate - vehicle.VehicleEffectiveDate).Days;
                                }
                            }
                     
                            vehicleFslPre = mvTermPre.FSL;
							vehiclePremiumPre = mvTermPre.Premium;
							vehicleBurnerPremiumPre = mvTermPre.BurnerPremium;
							vehicleExcessPre = mvTermPre.Excess;
							vehicleTermLimitPre = mvTermPre.TermLimit;

							totalvehicleFslPre += mvTermPre.FSL;
							totalvehiclePremiumPre += mvTermPre.Premium;
							totalvehicleBurnerPremiumPre += mvTermPre.BurnerPremium;
							totalvehicleTermLimitPre += mvTermPre.TermLimit;

							if ((vehicle.GroupSumInsured == vehicle.OriginalVehicle.GroupSumInsured && vehicle.VehicleEffectiveDate == vehicle.OriginalVehicle.VehicleEffectiveDate &&
								vehicle.VehicleCeaseDate == vehicle.OriginalVehicle.VehicleCeaseDate && vehicle.VehicleType == vehicle.OriginalVehicle.VehicleType) ||
								(vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseReason == 3)) {
								vehicleFslDiffer = 0m;
								vehiclePremiumDiffer = 0m;
								vehicleBurnerPremiumDiffer = 0m;
								vehicleExcessDiffer = 0;
								vehicleTermLimitDiffer = 0;

								totalvehicleFslDiffer += 0m;
								totalvehiclePremiumDiffer += 0m;
								totalvehicleBurnerPremiumDiffer += 0m;
								totalvehicleTermLimitDiffer += 0;

							} else {
								if (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseDate == DateTime.MinValue) {
									vehicleFslDiffer = vehicleFsl;
									vehiclePremiumDiffer = vehiclePremium;
									vehicleBurnerPremiumDiffer = vehicleBurnerPremium;
									vehicleExcessDiffer = vehicleExcess;
									vehicleTermLimitDiffer = vehicle.GroupSumInsured;

									totalvehicleFslDiffer += vehicleFsl;
									totalvehiclePremiumDiffer += vehiclePremium;
									totalvehicleBurnerPremiumDiffer += vehicleBurnerPremium;
									totalvehicleTermLimitDiffer += vehicle.GroupSumInsured;
								} else if (prenumberofdaysofcover == 0) {
									vehicleFslDiffer = vehicleFsl - mvTermPre.FSL;
									vehiclePremiumDiffer = vehiclePremium - mvTermPre.Premium;
									vehicleBurnerPremiumDiffer = vehicleBurnerPremium - mvTermPre.BurnerPremium;
									vehicleExcessDiffer = vehicleExcess - mvTermPre.Excess;
									vehicleTermLimitDiffer = vehicle.GroupSumInsured - mvTermPre.TermLimit;

									totalvehicleFslDiffer += vehicleFsl - mvTermPre.FSL;
									totalvehiclePremiumDiffer += vehiclePremium - mvTermPre.Premium;
									totalvehicleBurnerPremiumDiffer += vehicleBurnerPremium - mvTermPre.BurnerPremium;
									totalvehicleTermLimitDiffer += (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue) ? (vehicle.GroupSumInsured) : (vehicle.GroupSumInsured - mvTermPre.TermLimit);
								} else {
									vehicleFslDiffer = vehicleFsl - (mvTermPre.FSL * numberofdaysofcover / prenumberofdaysofcover);
									vehiclePremiumDiffer = vehiclePremium - (mvTermPre.Premium * numberofdaysofcover / prenumberofdaysofcover);
									vehicleBurnerPremiumDiffer = vehicleBurnerPremium - (mvTermPre.BurnerPremium * numberofdaysofcover / prenumberofdaysofcover);
									vehicleExcessDiffer = vehicleExcess - mvTermPre.Excess;
									vehicleTermLimitDiffer = vehicle.GroupSumInsured - mvTermPre.TermLimit;

									totalvehicleFslDiffer += vehicleFsl - (mvTermPre.FSL * numberofdaysofcover / prenumberofdaysofcover);
									totalvehiclePremiumDiffer += vehiclePremium - (mvTermPre.Premium * numberofdaysofcover / prenumberofdaysofcover);
									totalvehicleBurnerPremiumDiffer += vehicleBurnerPremium - (mvTermPre.BurnerPremium * numberofdaysofcover / prenumberofdaysofcover);
									totalvehicleTermLimitDiffer += (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue) ? (vehicle.GroupSumInsured) : (vehicle.GroupSumInsured - mvTermPre.TermLimit);
								}
							}


						}
					}

				} else {
					vehicleFslDiffer = vehicleFsl;
					vehiclePremiumDiffer = vehiclePremium;
					vehicleBurnerPremiumDiffer = vehicleBurnerPremium;
					vehicleExcessDiffer = vehicleExcess;
					vehicleTermLimitDiffer = vehicle.GroupSumInsured;

					totalvehicleFslDiffer += vehicleFsl;
					totalvehiclePremiumDiffer += vehiclePremium;
					totalvehicleBurnerPremiumDiffer += vehicleBurnerPremium;
					totalvehicleTermLimitDiffer += vehicle.GroupSumInsured;
				}

				mvTerm.FSLPre = vehicleFslPre;
				mvTerm.FSLDiffer = vehicleFslDiffer;
				mvTerm.PremiumPre = vehiclePremiumPre;
				mvTerm.PremiumDiffer = vehiclePremiumDiffer;
				mvTerm.BurnerPremiumPre = vehicleBurnerPremiumPre;
				mvTerm.BurnerPremiumDiffer = vehicleBurnerPremiumDiffer;
				mvTerm.ExcessPre = vehicleExcessPre;
				mvTerm.ExcessDiffer = vehicleExcessDiffer;
				mvTerm.TermLimitPre = vehicleTermLimitPre;
				mvTerm.TermLimitDiffer = vehicleTermLimitDiffer;

				if (vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseReason == 4) {
					mvTerm.TermCategory = "transfered";
				} else if (vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseReason != 4) {
					mvTerm.TermCategory = "ceased";
				} else {
					mvTerm.TermCategory = "active";
				}

			}

			term.TermLimit = totalVehicleSumInsured;
			term.Premium = totalVehiclePremium;
			term.BurnerPremium = totalVehicleBurnerPremium;
			term.FSL = totalVehicleFsl;
			term.BrokerageRate = agreement.Brokerage;
			term.Brokerage = totalVehicleBrokerage;

			term.TermLimitPre = totalvehicleTermLimitPre;
			term.TermLimitDiffer = totalvehicleTermLimitDiffer;
			term.PremiumPre = totalvehiclePremiumPre;
			term.PremiumDiffer = totalvehiclePremiumDiffer;
			term.BurnerPremiumPre = totalvehicleBurnerPremiumPre;
			term.BurnerPremiumDiffer = totalvehicleBurnerPremiumDiffer;
			term.FSLPre = totalvehicleFslPre;
			term.FSLDiffer = totalvehicleFslDiffer;

			agreement.QuoteDate = DateTime.UtcNow;

			return true;

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

				//set $20 broker fee for change agreement
				if (agreement.ClientInformationSheet.PreviousInformationSheet != null && agreement.BrokerFee <= 0) {
					agreement.BrokerFee = 20;
				}
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

		void GetFslFor (IDictionary<string, decimal> rates, Vehicle vehicle, ref decimal vehicleFsl, ref decimal vehicleFslRate)
		{
			decimal fslUnderFee = rates ["FSLUNDERFee"];
			decimal fslOver3Fee = rates ["FSLOVER3Rate"];

			if (vehicle.GrossVehicleMass >= 0 && vehicle.GrossVehicleMass < 3500) {
				vehicleFsl = fslUnderFee;
			} else if (vehicle.GrossVehicleMass >= 3500) {
				vehicleFslRate = fslOver3Fee;
				vehicleFsl = vehicle.GroupSumInsured * vehicleFslRate / 100;

				if (vehicleFsl < fslUnderFee)
					vehicleFsl = fslUnderFee;
			}

		}

		decimal GetVehicleRate (int value, params decimal [] rates)
		{
			return rates [value - 1];
		}
	}
}

