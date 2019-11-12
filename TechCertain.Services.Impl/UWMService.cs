using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Services;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class UWMService : IUWMService
    {

        IClientAgreementService _clientAgreementService;
        IClientAgreementRuleService _clientAgreementRuleService;
        IClientAgreementTermService _clientAgreementTermService;
        IClientAgreementMVTermService _clientAgreementMVTermService;
        IClientAgreementEndorsementService _clientAgreementEndorsementService;
        IUnderwritingModule _underwritingModule;        

        public UWMService(
            IUnderwritingModule underwritingModule,
            IClientAgreementService clientAgreementService, 
            IClientAgreementRuleService clientAgreementRuleService,
            IClientAgreementTermService clientAgreementTermService,
            IClientAgreementMVTermService clientAgreementMVTermService,
            IClientAgreementEndorsementService clientAgreementEndorsementService)
        {
            _clientAgreementService = clientAgreementService;
            _clientAgreementRuleService = clientAgreementRuleService;
            _clientAgreementTermService = clientAgreementTermService;
            _clientAgreementMVTermService = clientAgreementMVTermService;
            _clientAgreementEndorsementService = clientAgreementEndorsementService;
            _underwritingModule = underwritingModule;            
        }

        
        public bool UWM_ICIBNZIMV(User createdBy, ClientInformationSheet sheet, string reference)
        {
            var _modules = new Dictionary<string, IUnderwritingModule>();
            var modules = RegisterModules();
            bool result = false;
			foreach (Product product in sheet.Programme.BaseProgramme.Products) {
				if (!product.UnderwritingEnabled)
					continue;
				
				string uwmCode = product.UnderwritingModuleCode;
				if (string.IsNullOrWhiteSpace (uwmCode))
					throw new Exception ("No underwriting module specificed for product '" + product.Id + "'");
				var uwm = Load(uwmCode, _modules);
				result &= uwm.Underwrite (createdBy, sheet, product, reference);
			}
			return result;
        }

        public void Register(string key, IUnderwritingModule module, Dictionary<string, IUnderwritingModule> _modules)
        {
            _modules[key] = module;
        }

        public IUnderwritingModule Load(string key, Dictionary<string, IUnderwritingModule> _modules)
        {           
            var modules = RegisterModules();
            foreach (var module in modules)
                Register(module.Name, module, _modules);

            if (!_modules.ContainsKey(key))
                throw new Exception("No underwriting module for \"" + key + "\" registered");

            return _modules[key];
        }

        protected IUnderwritingModule[] RegisterModules()
        {
            var modules = new IUnderwritingModule[] {
                new ICIBHIANZUWModule(),
                new ICIBARCCOUWModule(),
                new MarshCoastGuardUWModule(),
                new EmptyUWModule(),
            };
            return modules;
        }



    }
    public class EmptyUWModule : IUnderwritingModule
    {
        public EmptyUWModule()
        {
        }

        public string Name
        {
            get
            {
                return "NoUnderwrite";
            }
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet)
        {
            return true;
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
        {
            return true;
        }
    }

    public class ICIBARCCOUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public ICIBARCCOUWModule()
        {
            Name = "ICIB_ARCCO";
        }

        public bool Underwrite(User CurrentUser, ClientInformationSheet informationSheet)
        {
            throw new NotImplementedException();
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
        {
            ClientAgreement agreement = GetClientAgreement(underwritingUser, informationSheet, informationSheet.Programme, product, reference);
            Guid id = agreement.Id;

            ClientAgreementTerm term = GetAgreementTerm(underwritingUser, agreement, "MV");
            var mvTerms = term.MotorTerms;

            if (mvTerms != null)
            {
                foreach (ClientAgreementMVTerm mVTerm in mvTerms)
                {
                    mVTerm.Delete(underwritingUser);
                }
            }

            if (agreement.ClientAgreementRules.Count == 0)
                foreach (var rule in product.Rules.Where(r => !string.IsNullOrWhiteSpace(r.Name)))
                    agreement.ClientAgreementRules.Add(new ClientAgreementRule(underwritingUser, rule, agreement));

            if (agreement.ClientAgreementEndorsements.Count == 0)
                foreach (var endorsement in product.Endorsements.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
                    agreement.ClientAgreementEndorsements.Add(new ClientAgreementEndorsement(underwritingUser, endorsement, agreement));

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "FSLUNDERFee", "FSLOVER3Rate", "MVRate", "BurnerRate",
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

            if (informationSheet.Vehicles.Where(v => !v.Removed && v.DateDeleted == null && v.VehicleCeaseDate == DateTime.MinValue).Count() >= 10)
            {
                vehicleMinPremium = rates["MinPremium"];
            }
            else
            {
                vehicleMinPremium = rates["MinPremiumLess"];
            }


            foreach (var vehicle in informationSheet.Vehicles.Where(v => !v.Removed && v.DateDeleted == null))
            {
                string vehicleCategory = null;
                decimal vehicleRate = 0m;
                vehicleRate = rates["MVRate"];
                decimal vehicleFsl = 0m;
                decimal vehicleFslRate = 0m;
                decimal vehiclePremium = 0m;
                decimal vehicleBurnerRate = 0m;
                vehicleBurnerRate = rates["BurnerRate"];
                decimal vehicleBurnerPremium = 0m;
                decimal vehicleExcess = 0m;
                decimal vehicleBrokerage = 0m;
                decimal vehicleBrokerageRate = 0m;

                GetFslFor(rates, vehicle, ref vehicleFsl, ref vehicleFslRate);

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

                    if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue)
                    {
                        vexpirydate = vehicle.VehicleCeaseDate;
                    }
                    else if (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseDate == DateTime.MinValue)
                    {
                        vinceptiondate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate : vehicle.OriginalVehicle.VehicleInceptionDate;
                        vexpirydate = agreement.ExpiryDate;
                    }


                }
                else //new vehicles
                {
                    veffectivedate = (vehicle.VehicleEffectiveDate > DateTime.MinValue) ? vehicle.VehicleEffectiveDate : agreement.InceptionDate;
                    vexpirydate = (vehicle.VehicleCeaseDate > DateTime.MinValue) ? vehicle.VehicleCeaseDate : agreement.ExpiryDate;
                    vinceptiondate = veffectivedate;
                }


                numberofdaysofcover = (vexpirydate - veffectivedate).Days;
                defaultnumberofdaysofcover = (agreement.ExpiryDate - agreement.ExpiryDate.AddYears(-1)).Days;

                vehicle.VehicleInceptionDate = vinceptiondate;
                vehicle.VehicleExpireDate = vexpirydate;

                vehicleBurnerPremium = vehicle.GroupSumInsured * vehicleBurnerRate / 100 / defaultnumberofdaysofcover * numberofdaysofcover;
                vehiclePremium = (vehicle.GroupSumInsured * vehicleRate / 100) + vehicleFsl;
                if (vehicle.VehicleType == 8) //Trailer
                {
                    if (vehiclePremium < (100 + vehicleFsl))
                    {
                        vehiclePremium = 100 + vehicleFsl;
                    }
                }
                else if (vehicle.VehicleType == 9) //Motorcycle
                {
                    if (vehiclePremium < (350 + vehicleFsl))
                    {
                        vehiclePremium = 350 + vehicleFsl;
                    }
                }
                else
                {
                    if (vehiclePremium < (vehicleMinPremium + vehicleFsl))
                    {
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
                    mvTerm = term.MotorTerms.FirstOrDefault(mvt => mvt.Vehicle == vehicle && mvt.DateDeleted != null);
                else
                    term.MotorTerms = new List<ClientAgreementMVTerm>();

                if (mvTerm == null)
                {
                    mvTerm = new ClientAgreementMVTerm(underwritingUser, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured,
                                                       vehicleExcess, vehiclePremium, vehicleFsl, vehicleBrokerageRate, vehicleBrokerage, vehicleCategory, vehicle.FleetNumber, term, vehicle, vehicleBurnerPremium);
                    term.MotorTerms.Add(mvTerm);
                }
                else
                {
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

                if (agreement.ClientInformationSheet.PreviousInformationSheet != null && vehicle.OriginalVehicle != null)
                {
                    //ClientAgreementTerm termPre = agreement.ClientInformationSheet.PreviousInformationSheet.ClientAgreement.ClientAgreementTerms.FirstOrDefault (t => t.SubTermType == "MV" && t.DateDeleted == null);
                    ClientAgreementTerm termPre = agreement.ClientInformationSheet.PreviousInformationSheet.Programme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "MV" && t.DateDeleted == null);
                    if (termPre != null)
                    {
                        ClientAgreementMVTerm mvTermPre = termPre.MotorTerms.FirstOrDefault(mvt => mvt.Vehicle == vehicle.OriginalVehicle && mvt.DateDeleted == null);

                        if (mvTermPre != null)
                        {
                            if (vehicle.OriginalVehicle.VehicleEffectiveDate > DateTime.MinValue)
                            {
                                prenumberofdaysofcover = (vehicle.OriginalVehicle.VehicleExpireDate - vehicle.OriginalVehicle.VehicleEffectiveDate).Days;
                            }
                            else
                            {
                                prenumberofdaysofcover = (vehicle.OriginalVehicle.VehicleExpireDate - vehicle.OriginalVehicle.VehicleInceptionDate).Days;
                            }

                            if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue)
                            {
                                prenumberofdaysofcover = (vehicle.VehicleExpireDate - vehicle.VehicleInceptionDate).Days;
                            }
                            if (vehicle.OriginalVehicle.VehicleEffectiveDate > DateTime.MinValue && vehicle.VehicleEffectiveDate > DateTime.MinValue)
                            {
                                if (vehicle.OriginalVehicle.VehicleCeaseDate == DateTime.MinValue && vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.OriginalVehicle.VehicleEffectiveDate == vehicle.VehicleEffectiveDate)
                                {
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
                                (vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseReason == 3))
                            {
                                vehicleFslDiffer = 0m;
                                vehiclePremiumDiffer = 0m;
                                vehicleBurnerPremiumDiffer = 0m;
                                vehicleExcessDiffer = 0;
                                vehicleTermLimitDiffer = 0;

                                totalvehicleFslDiffer += 0m;
                                totalvehiclePremiumDiffer += 0m;
                                totalvehicleBurnerPremiumDiffer += 0m;
                                totalvehicleTermLimitDiffer += 0;

                            }
                            else
                            {
                                if (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseDate == DateTime.MinValue)
                                {
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
                                else if (prenumberofdaysofcover == 0)
                                {
                                    vehicleFslDiffer = vehicleFsl - mvTermPre.FSL;
                                    vehiclePremiumDiffer = vehiclePremium - mvTermPre.Premium;
                                    vehicleBurnerPremiumDiffer = vehicleBurnerPremium - mvTermPre.BurnerPremium;
                                    vehicleExcessDiffer = vehicleExcess - mvTermPre.Excess;
                                    vehicleTermLimitDiffer = vehicle.GroupSumInsured - mvTermPre.TermLimit;

                                    totalvehicleFslDiffer += vehicleFsl - mvTermPre.FSL;
                                    totalvehiclePremiumDiffer += vehiclePremium - mvTermPre.Premium;
                                    totalvehicleBurnerPremiumDiffer += vehicleBurnerPremium - mvTermPre.BurnerPremium;
                                    totalvehicleTermLimitDiffer += (vehicle.OriginalVehicle.VehicleCeaseDate > DateTime.MinValue) ? (vehicle.GroupSumInsured) : (vehicle.GroupSumInsured - mvTermPre.TermLimit);
                                }
                                else
                                {
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

                }
                else
                {
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

                if (vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseReason == 4)
                {
                    mvTerm.TermCategory = "transfered";
                }
                else if (vehicle.VehicleCeaseDate > DateTime.MinValue && vehicle.VehicleCeaseReason != 4)
                {
                    mvTerm.TermCategory = "ceased";
                }
                else
                {
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

        ClientAgreement GetClientAgreement(User CurrentUser, ClientInformationSheet informationSheet, ClientProgramme programme, Product product, string reference)
        {
            ClientAgreement clientAgreement = programme.Agreements.FirstOrDefault(a => a.Product != null && a.Product.Id == product.Id);
            if (clientAgreement == null)
            {

                DateTime inceptionDate = (product.DefaultInceptionDate > DateTime.MinValue) ? product.DefaultInceptionDate : DateTime.UtcNow;
                DateTime expiryDate = (product.DefaultExpiryDate > DateTime.MinValue) ? product.DefaultExpiryDate : DateTime.UtcNow;

                clientAgreement = new ClientAgreement(CurrentUser, informationSheet.Owner.Name, inceptionDate, expiryDate, product.DefaultBrokerage, product.DefaultBrokerFee, informationSheet, product, reference);
                programme.Agreements.Add(clientAgreement);

            }
            return clientAgreement;
        }

        ClientAgreementTerm GetAgreementTerm(User CurrentUser, ClientAgreement agreement, string subTerm)
        {
            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == subTerm && t.DateDeleted == null);
            if (term == null)
            {
                term = new ClientAgreementTerm(CurrentUser, 0, 0m, 0m, 0m, 0m, 0m, agreement, subTerm);
                agreement.ClientAgreementTerms.Add(term);

                //set $20 broker fee for change agreement
                if (agreement.ClientInformationSheet.PreviousInformationSheet != null && agreement.BrokerFee <= 0)
                {
                    agreement.BrokerFee = 20;
                }
            }

            return term;
        }

        IDictionary<string, decimal> BuildRulesTable(ClientAgreement agreement, params string[] names)
        {
            var dict = new Dictionary<string, decimal>();

            foreach (string name in names)
                dict[name] = Convert.ToDecimal(agreement.ClientAgreementRules.FirstOrDefault(r => r.Name == name).Value);

            return dict;
        }

        void GetFslFor(IDictionary<string, decimal> rates, Vehicle vehicle, ref decimal vehicleFsl, ref decimal vehicleFslRate)
        {
            decimal fslUnderFee = rates["FSLUNDERFee"];
            decimal fslOver3Fee = rates["FSLOVER3Rate"];

            if (vehicle.GrossVehicleMass >= 0 && vehicle.GrossVehicleMass < 3500)
            {
                vehicleFsl = fslUnderFee;
            }
            else if (vehicle.GrossVehicleMass >= 3500)
            {
                vehicleFslRate = fslOver3Fee;
                vehicleFsl = vehicle.GroupSumInsured * vehicleFslRate / 100;

                if (vehicleFsl < fslUnderFee)
                    vehicleFsl = fslUnderFee;
            }

        }

        decimal GetVehicleRate(int value, params decimal[] rates)
        {
            return rates[value - 1];
        }
    }

    public class ICIBHIANZUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public ICIBHIANZUWModule()
        {
            Name = "ICIB_HIANZ";
        }

        public bool Underwrite(User CurrentUser, ClientInformationSheet informationSheet)
        {
            throw new NotImplementedException();
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
        {
            ClientAgreement agreement = GetClientAgreement(underwritingUser, informationSheet, informationSheet.Programme, product, reference);
            Guid id = agreement.Id;

            ClientAgreementTerm term = GetAgreementTerm(underwritingUser, agreement, "MV");
            var mvTerms = term.MotorTerms;

            if (mvTerms != null)
            {
                foreach (ClientAgreementMVTerm mVTerm in mvTerms)
                {
                    mVTerm.Delete(underwritingUser);
                }
            }

            if (agreement.ClientAgreementRules.Count == 0)
                foreach (var rule in product.Rules.Where(r => !string.IsNullOrWhiteSpace(r.Name)))
                    agreement.ClientAgreementRules.Add(new ClientAgreementRule(underwritingUser, rule, agreement));

            if (agreement.ClientAgreementEndorsements.Count == 0)
                foreach (var endorsement in product.Endorsements.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
                    agreement.ClientAgreementEndorsements.Add(new ClientAgreementEndorsement(underwritingUser, endorsement, agreement));

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "NICITYSVRate", "NITOWNSVRate", "SICITYSVRate", "SITOWNSVRate",
                                                                  "NICITY1CRate", "NITOWN1CRate", "SICITY1CRate", "SITOWN1CRate", "NICITY1UAPRate", "NITOWN1UAPRate", "SICITY1UAPRate", "SITOWN1UAPRate",
                                                                  "NICITY1PRate", "NITOWN1PRate", "SICITY1PRate", "SITOWN1PRate", "NICITY1RRate", "NITOWN1RRate", "SICITY1RRate", "SITOWN1RRate",
                                                                  "NICITY2Rate", "NITOWN2Rate", "SICITY2Rate", "SITOWN2Rate", "NICITY3Rate", "NITOWN3Rate", "SICITY3Rate", "SITOWN3Rate",
                                                                  "NICITYSVRate", "NITOWNSVRate", "SICITYSVRate", "SITOWNSVRate", "FSLUNDERFee", "FSLOVER3Rate"
                                                                 );
            decimal totalVehicleFsl = 0m;
            int totalVehicleSumInsured = 0;
            decimal totalVehiclePremium = 0m;
            decimal totalVehicleBrokerage = 0m;
            foreach (var vehicle in informationSheet.Vehicles.Where(v => v.VehicleType > 0 && !v.Removed && v.DateDeleted == null))
            {
                string vehicleCategory = null;
                decimal vehicleRate = 0m;
                decimal vehicleFsl = 0m;
                decimal vehicleFslRate = 0m;
                decimal vehiclePremium = 0m;
                decimal vehicleExcess = 0m;
                decimal vehicleBrokerage = 0m;
                decimal vehicleBrokerageRate = 0m;

                GetRateAndCategoryFor(rates, vehicle, out vehicleCategory, out vehicleRate);
                vehicle.VehicleCategory = vehicleCategory;
                GetFslFor(rates, vehicle, ref vehicleFsl, ref vehicleFslRate);

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
                    mvTerm = term.MotorTerms.FirstOrDefault(mvt => mvt.Vehicle == vehicle && mvt.DateDeleted != null);
                else
                    term.MotorTerms = new List<ClientAgreementMVTerm>();

                if (mvTerm == null)
                {
                    mvTerm = new ClientAgreementMVTerm(underwritingUser, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured,
                                                       vehicleExcess, vehiclePremium, vehicleFsl, vehicleBrokerageRate, vehicleBrokerage, vehicleCategory, vehicle.FleetNumber, term, vehicle, 0M);
                    term.MotorTerms.Add(mvTerm);
                }
                else
                {
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

        ClientAgreement GetClientAgreement(User CurrentUser, ClientInformationSheet informationSheet, ClientProgramme programme, Product product, string reference)
        {
            ClientAgreement clientAgreement = programme.Agreements.FirstOrDefault(a => a.Product != null && a.Product.Id == product.Id);
            if (clientAgreement == null)
            {
                DateTime inceptionDate = (product.DefaultInceptionDate > DateTime.MinValue) ? product.DefaultInceptionDate : DateTime.UtcNow;
                DateTime expiryDate = (product.DefaultExpiryDate > DateTime.MinValue) ? product.DefaultExpiryDate : DateTime.UtcNow;

                clientAgreement = new ClientAgreement(CurrentUser, informationSheet.Owner.Name, inceptionDate, expiryDate, product.DefaultBrokerage, product.DefaultBrokerFee, informationSheet, product, reference);
                programme.Agreements.Add(clientAgreement);

            }
            return clientAgreement;
        }

        ClientAgreementTerm GetAgreementTerm(User CurrentUser, ClientAgreement agreement, string subTerm)
        {
            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == subTerm && t.DateDeleted == null);
            if (term == null)
            {
                term = new ClientAgreementTerm(CurrentUser, 0, 0m, 0m, 0m, 0m, 0m, agreement, subTerm);
                agreement.ClientAgreementTerms.Add(term);
            }

            return term;
        }

        IDictionary<string, decimal> BuildRulesTable(ClientAgreement agreement, params string[] names)
        {
            var dict = new Dictionary<string, decimal>();

            foreach (string name in names)
                dict[name] = Convert.ToDecimal(agreement.ClientAgreementRules.FirstOrDefault(r => r.Name == name).Value);

            return dict;
        }

        void GetRateAndCategoryFor(IDictionary<string, decimal> rates, Vehicle vehicle, out string vehicleCategory, out decimal vehicleRate)
        {
            switch (vehicle.VehicleType)
            {
                case 1:
                    {
                        switch (vehicle.SubUseType)
                        {
                            case 1:
                                {
                                    vehicleCategory = "1C";
                                    vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITY1CRate"], rates["NICITY1CRate"], rates["NITOWN1CRate"], rates["SICITY1CRate"], rates["SITOWN1CRate"]);
                                    break;
                                }
                            case 2:
                                {
                                    vehicleCategory = "1P";
                                    vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITY1PRate"], rates["NICITY1PRate"], rates["NITOWN1PRate"], rates["SICITY1PRate"], rates["SITOWN1PRate"]);
                                    break;
                                }
                            case 3:
                                {
                                    vehicleCategory = "1R";
                                    vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITY1RRate"], rates["NICITY1RRate"], rates["NITOWN1RRate"], rates["SICITY1RRate"], rates["SITOWN1RRate"]);
                                    break;
                                }
                            case 4:
                                {
                                    vehicleCategory = "1UAP";
                                    vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITY1UAPRate"], rates["NICITY1UAPRate"], rates["NITOWN1UAPRate"], rates["SICITY1UAPRate"], rates["SITOWN1UAPRate"]);
                                    break;
                                }
                            default:
                                {
                                    throw new Exception(string.Format("Invalid Vehicle SubUseType {0} for Vehicle {1}", vehicle.SubUseType, vehicle.Id));
                                }
                        }
                        break;
                    }
                case 2:
                    {
                        vehicleCategory = "2";
                        vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITY2Rate"], rates["NICITY2Rate"], rates["NITOWN2Rate"], rates["SICITY2Rate"], rates["SITOWN2Rate"]);
                        break;
                    }
                // 3 and 4 share the same rates
                case 3:
                case 4:
                    {
                        vehicleCategory = "3";
                        vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITY3Rate"], rates["NICITY3Rate"], rates["NITOWN3Rate"], rates["SICITY3Rate"], rates["SITOWN3Rate"]);
                        break;
                    }
                case 5:
                    {
                        vehicleCategory = "SV";
                        vehicleRate = GetVehicleRate(vehicle.AreaOfOperation, 0m, rates["NICITYSVRate"], rates["NICITYSVRate"], rates["NITOWNSVRate"], rates["SICITYSVRate"], rates["SITOWNSVRate"]);
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Invalid Vehicle VehicleType {0} for Vehicle {1}", vehicle.VehicleType, vehicle.Id));
                    }
            }
        }

        void GetFslFor(IDictionary<string, decimal> rates, Vehicle vehicle, ref decimal vehicleFsl, ref decimal vehicleFslRate)
        {
            decimal fslUnderFee = rates["FSLUNDERFee"];
            decimal fslOver3Fee = rates["FSLOVER3Rate"];

            if (vehicle.VehicleCategory != "3")
            {
                if (vehicle.GrossVehicleMass >= 0 && vehicle.GrossVehicleMass < 3500)
                {
                    vehicleFsl = fslUnderFee;
                }
                else if (vehicle.GrossVehicleMass >= 3500)
                {
                    vehicleFslRate = fslOver3Fee;
                    vehicleFsl = vehicle.GroupSumInsured * vehicleFslRate / 100;

                    if (vehicleFsl < fslUnderFee)
                        vehicleFsl = fslUnderFee;
                }
            }
            else
            {
                vehicleFslRate = fslOver3Fee;
                vehicleFsl = vehicle.GroupSumInsured * vehicleFslRate / 100;
            }
        }

        decimal GetVehicleRate(int value, decimal defaultRate, params decimal[] rates)
        {
            int index = value - 1;
            if (index < 0 || index >= rates.Length)
                return defaultRate;

            return rates[index];
        }
    }

    public class MarshCoastGuardUWModule: IUnderwritingModule
    {
        public string Name { get; protected set; }

        public MarshCoastGuardUWModule()
        {
            Name = "Marsh_CoastGuard";
        }

        public bool Underwrite(User CurrentUser, ClientInformationSheet informationSheet)
        {
            throw new NotImplementedException();
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
        {
            ClientAgreement agreement = GetClientAgreement(underwritingUser, informationSheet, informationSheet.Programme, product, reference);
            Guid id = agreement.Id;

            ClientAgreementTerm term = GetAgreementTerm(underwritingUser, agreement, "BV");
            var bvTerms = term.BoatTerms;
            var mvTerms = term.MotorTerms;

            if (bvTerms != null)
            {
                foreach (ClientAgreementBVTerm bVTerm in bvTerms)
                {
                    bVTerm.Delete(underwritingUser);
                }
            }

            if (mvTerms != null)
            {
                foreach (ClientAgreementMVTerm mVTerm in mvTerms)
                {
                    mVTerm.Delete(underwritingUser);
                }
            }

            if (agreement.ClientAgreementRules.Count == 0)
                foreach (var rule in product.Rules.Where(r => !string.IsNullOrWhiteSpace(r.Name)))
                    agreement.ClientAgreementRules.Add(new ClientAgreementRule(underwritingUser, rule, agreement));

            if (agreement.ClientAgreementEndorsements.Count == 0)
                foreach (var endorsement in product.Endorsements.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
                    agreement.ClientAgreementEndorsements.Add(new ClientAgreementEndorsement(underwritingUser, endorsement, agreement));

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "tcunder50kexcess250rate", "tcunder50kexcess500rate", "tcunder50kminpremium",
                "tc50k100kexcess250rate", "tc50k100kexcess500rate", "tc50k100kminpremium", "tc100k200kexcess500rate", "tc100k200kexcess1000rate", "tc100k200kminpremium",
                "tc200k250kexcess500rate", "tc200k250kexcess1000rate", "tc200k250kminpremium", "jbunder100kexcess500rate", "jbunder100kexcess1000rate", "jbunder100kminpremium",
                "jsunder100kexcess500rate", "jsunder100kexcess1000rate", "jsunder100kminpremium",
                "mcunder100kexcess250rate", "mcunder100kexcess500rate", "mcunder100kminpremium", "mc100k200kexcess500rate", "mc100k200kexcess1000rate", "mc100k200kminpremium",
                "mc200k350kexcess1000rate", "mc200k350kexcess2000rate", "mc200k350kminpremium", "mc350k500kexcess1000rate", "mc350k500kexcess2000rate", "mc350k500kminpremium",
                "fslfeefort", "fslratefortc", "loadingforcatandyc", "loadingforycraceusespinnakersuptp50nm", "loadingforycraceusespinnakersuptp200nm", "mvpremiumrate");

            //Create default referral points based on the clientagreementrules
            if (agreement.ClientAgreementReferrals.Count == 0)
            {
                foreach (var clientagreementreferralrule in agreement.ClientAgreementRules.Where(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null))
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, clientagreementreferralrule.Name, clientagreementreferralrule.Description, "", clientagreementreferralrule.Value, clientagreementreferralrule.OrderNumber));
            }
            else
            {
                foreach (var clientagreementreferral in agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null))
                    clientagreementreferral.Status = "";
            }

            int agreementperiodindays = 0;
            agreementperiodindays = (agreement.ExpiryDate - agreement.InceptionDate).Days;

            int totalTermLimit = 0;
            decimal totalTermFsl = 0m;
            decimal totalTermPremium = 0m;
            decimal totalTermBrokerage = 0m;
            decimal totalVehicleFsl = 0m;
            int totalVehicleSumInsured = 0;
            decimal totalVehiclePremium = 0m;
            decimal totalVehicleBrokerage = 0m;
            decimal totalBoatFsl = 0m;
            int totalBoatSumInsured = 0;
            decimal totalBoatPremium = 0m;
            decimal totalBoatBrokerage = 0m;

            decimal totalBoatFslPre = 0m;
            decimal totalBoatFslDiffer = 0m;
            decimal totalBoatPremiumPre = 0m;
            decimal totalBoatPremiumDiffer = 0m;
            int totalBoatTermLimitPre = 0;
            int totalBoatTermLimitDiffer = 0;

            bool bolNewBoatAdded = false;
            bool bolNewVehicleAdded = false;

            agreement.QuoteDate = DateTime.UtcNow;

            //calculate boat premium and FSL (BV Term)
            foreach (var boat in informationSheet.Boats.Where(v => !v.Removed && v.DateDeleted == null))
            {

                //Pre-rate premiums based on the vehicle effectiove date and cease date
                DateTime boatinceptiondate;
                DateTime boatexpirydate;
                DateTime boateffectivedate;
                int boatperiodindays = 0;

                if (!informationSheet.IsChange && (boat.BoatEffectiveDate < agreement.InceptionDate || boat.BoatEffectiveDate > agreement.InceptionDate.AddDays(30)))
                {
                    boat.BoatEffectiveDate = agreement.InceptionDate;
                }

                //default for new boat
                boateffectivedate = (boat.BoatEffectiveDate > DateTime.MinValue) ? boat.BoatEffectiveDate : agreement.InceptionDate;
                boatexpirydate = (boat.BoatCeaseDate > DateTime.MinValue) ? boat.BoatCeaseDate : agreement.ExpiryDate;
                boatinceptiondate = boateffectivedate;
                
                if (boat.OriginalBoat != null) //exsiting boat
                {
                    boatinceptiondate = boat.OriginalBoat.BoatInceptionDate;
                    boatexpirydate = boat.OriginalBoat.BoatExpireDate;
                    boateffectivedate = (boat.BoatEffectiveDate > DateTime.MinValue) ? boat.BoatEffectiveDate : boat.OriginalBoat.BoatInceptionDate;

                    if (boat.OriginalBoat.BoatCeaseDate == DateTime.MinValue && boat.BoatCeaseDate > DateTime.MinValue)
                    {
                        boatexpirydate = boat.BoatCeaseDate;
                    }
                    else if (boat.OriginalBoat.BoatCeaseDate > DateTime.MinValue && boat.BoatCeaseDate == DateTime.MinValue)
                    {
                        boatinceptiondate = (boat.BoatEffectiveDate > DateTime.MinValue) ? boat.BoatEffectiveDate : boat.OriginalBoat.BoatInceptionDate;
                        boatexpirydate = agreement.ExpiryDate;
                    }
                } else
                {
                    bolNewBoatAdded = true;
                }

                boat.BoatInceptionDate = boatinceptiondate;
                boat.BoatExpireDate = boatexpirydate;

                boatperiodindays = (boatexpirydate - boateffectivedate).Days;

                decimal boatRate = 0m;
                decimal boatMinPremium = 0m;
                decimal boatFsl = 0m;
                decimal boatFslRate = 0m;
                decimal boatPremium = 0m;
                decimal boatBrokerage = 0m;
                decimal boatBrokerageRate = 0m;
                decimal boatproratedFsl = 0m;
                decimal boatproratedPremium = 0m;
                decimal boatproratedBrokerage = 0m;

                GetFslRateForTraileredBoat(rates, boat, ref boatFslRate);

                GetBoatRateAndMinPremiumFor(rates, boat, out boatRate, out boatMinPremium);

                boatFsl = boat.MaxSumInsured * boatFslRate / 100;
                boatPremium = boat.MaxSumInsured * boatRate / 100;

                //Additional loading for Hull Configuration (Cat) + BoatType1 (YachtsandCatamarans)
                if (boat.HullConfiguration == "Catamaran" && boat.BoatType1 == "YachtsandCatamarans")
                {
                    boatPremium = boatPremium * (1 + rates["loadingforcatandyc"]);
                }

                //Additional loading for BoatType1 (YachtsandCatamarans) + BoatUseRaceUseSpinnakers (Yes) + BoatUseRaceCategory (YachtClubSocialRacingupto50nm, Yachtracingupto200nm)
                if (boat.BoatType1 == "YachtsandCatamarans" && boat.BoatUse.Any(ycbu => ycbu.BoatUseRaceUseSpinnakers == "True" && ycbu.BoatUseCategory == "Race" && ycbu.DateDeleted == null && !ycbu.Removed))
                {
                    if (boat.BoatUse.Any(ycbu => ycbu.BoatUseRaceCategory == "YachtClubSocialRacingupto50nm")) //BoatUseRaceCategory (YachtClubSocialRacingupto50nm)
                    {
                        boatPremium = boatPremium * (1 + rates["loadingforycraceusespinnakersuptp50nm"]);
                    }
                    else if (boat.BoatUse.Any(ycbu => ycbu.BoatUseRaceCategory == "Yachtracingupto200nm")) //BoatUseRaceCategory (Yachtracingupto200nm)
                    {
                        boatPremium = boatPremium * (1 + rates["loadingforycraceusespinnakersuptp200nm"]);
                    }

                }

                if (boatPremium < boatMinPremium)
                    boatPremium = boatMinPremium;
                boatPremium = boatPremium + boatFsl;
                boatBrokerageRate = agreement.Brokerage;
                boatBrokerage = boatPremium * boatBrokerageRate / 100;
                boatproratedPremium = boatPremium;
                boatproratedFsl = boatFsl;
                boatproratedBrokerage = boatBrokerage;
                //Pre-rate premium if the boat effective date is later than policy inception date
                if (boat.BoatEffectiveDate > agreement.InceptionDate)
                {
                    boatproratedPremium = boatPremium * boatperiodindays / agreementperiodindays;
                    boatproratedFsl = boatFsl * boatperiodindays / agreementperiodindays;
                    boatproratedBrokerage = boatBrokerage * boatperiodindays / agreementperiodindays;
                }

                totalBoatFsl += boatproratedFsl;
                totalBoatSumInsured += (boat.BoatCeaseDate > DateTime.MinValue) ? 0 : boat.MaxSumInsured;
                totalBoatPremium += boatproratedPremium;
                totalBoatBrokerage += boatproratedBrokerage;

                ClientAgreementBVTerm bvTerm = null;
                if (term.BoatTerms != null)
                    bvTerm = term.BoatTerms.FirstOrDefault(bvt => bvt.Boat == boat && bvt.DateDeleted != null);
                else
                    term.BoatTerms = new List<ClientAgreementBVTerm>();

                if (bvTerm == null)
                {
                    bvTerm = new ClientAgreementBVTerm(underwritingUser, boat.BoatName, boat.YearOfManufacture, boat.BoatName, boat.BoatModel, boat.MaxSumInsured, boat.BoatQuoteExcessOption, boatproratedPremium, boatproratedFsl,
                                                       boatBrokerageRate, boatproratedBrokerage, term, boat);
                    bvTerm.BoatMake = boat.BoatMake;
                    bvTerm.BoatModel = boat.BoatModel;
                    bvTerm.TermCategory = "active";
                    bvTerm.AnnualPremium = boatPremium;
                    bvTerm.AnnualFSL = boatFsl;
                    bvTerm.AnnualBrokerage = boatBrokerage;
                    term.BoatTerms.Add(bvTerm);
                }
                else
                {
                    bvTerm.BoatName = boat.BoatName;
                    bvTerm.YearOfManufacture = boat.YearOfManufacture;
                    bvTerm.TermLimit = boat.MaxSumInsured;
                    bvTerm.Premium = boatproratedPremium;
                    bvTerm.Excess = boat.BoatQuoteExcessOption;
                    bvTerm.FSL = boatproratedFsl;
                    bvTerm.BoatMake = boat.BoatMake;
                    bvTerm.BoatModel = boat.BoatModel;
                    bvTerm.DateDeleted = null;
                    bvTerm.BrokerageRate = boatBrokerageRate;
                    bvTerm.Brokerage = boatproratedBrokerage;
                    bvTerm.LastModifiedOn = DateTime.UtcNow;
                    bvTerm.LastModifiedBy = underwritingUser;
                    bvTerm.TermCategory = "active";
                    bvTerm.AnnualPremium = boatPremium;
                    bvTerm.AnnualFSL = boatFsl;
                    bvTerm.AnnualBrokerage = boatBrokerage;
                }

                //===========
                //For Change Agreement
                decimal boatFslPre = 0m;
                decimal boatFslDiffer = 0m;
                decimal boatPremiumPre = 0m;
                decimal boatPremiumDiffer = 0m;
                decimal boatExcessPre = 0m;
                decimal boatExcessDiffer = 0m;
                int boatTermLimitPre = 0;
                int boatTermLimitDiffer = 0;
                int preboatperiodindays = 0;

                if (agreement.ClientInformationSheet.IsChange && agreement.ClientInformationSheet.PreviousInformationSheet != null && boat.OriginalBoat != null)
                {
                    ClientAgreementTerm termPre = agreement.ClientInformationSheet.PreviousInformationSheet.Programme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);
                    if (termPre != null)
                    {
                        ClientAgreementBVTerm bvTermPre = termPre.BoatTerms.FirstOrDefault(bvpret => bvpret.Boat == boat.OriginalBoat && bvpret.DateDeleted == null);

                        if (bvTermPre != null)
                        {
                            if (boat.OriginalBoat.BoatEffectiveDate > DateTime.MinValue)
                            {
                                preboatperiodindays = (boat.OriginalBoat.BoatExpireDate - boat.OriginalBoat.BoatEffectiveDate).Days;
                            }
                            else
                            {
                                preboatperiodindays = (boat.OriginalBoat.BoatExpireDate - boat.OriginalBoat.BoatInceptionDate).Days;
                            }

                            if (boat.OriginalBoat.BoatCeaseDate == DateTime.MinValue && boat.BoatCeaseDate > DateTime.MinValue)
                            {
                                preboatperiodindays = (boat.BoatExpireDate - boat.BoatInceptionDate).Days;
                            }
                            if (boat.OriginalBoat.BoatEffectiveDate > DateTime.MinValue && boat.BoatEffectiveDate > DateTime.MinValue)
                            {
                                if (boat.OriginalBoat.BoatCeaseDate == DateTime.MinValue && boat.BoatCeaseDate > DateTime.MinValue && boat.OriginalBoat.BoatEffectiveDate == boat.BoatEffectiveDate)
                                {
                                    preboatperiodindays = (boat.BoatExpireDate - boat.BoatEffectiveDate).Days;
                                }
                            }

                            if (boat.OriginalBoat.BoatEffectiveDate > DateTime.MinValue && boat.BoatEffectiveDate > DateTime.MinValue)
                            {
                                if (boat.OriginalBoat.BoatCeaseDate == DateTime.MinValue && boat.BoatCeaseDate > DateTime.MinValue && boat.OriginalBoat.BoatEffectiveDate == boat.BoatEffectiveDate)
                                {
                                    preboatperiodindays = (boat.BoatExpireDate - boat.BoatEffectiveDate).Days;
                                }
                            }

                            boatFslPre = bvTermPre.FSL;
                            boatPremiumPre = bvTermPre.Premium;
                            boatExcessPre = bvTermPre.Excess;
                            boatTermLimitPre = bvTermPre.TermLimit;

                            totalBoatFslPre += bvTermPre.FSL;
                            totalBoatPremiumPre += bvTermPre.Premium;
                            totalBoatTermLimitPre += bvTermPre.TermLimit;

                            if ((boat.MaxSumInsured == boat.OriginalBoat.MaxSumInsured && boat.BoatEffectiveDate == boat.OriginalBoat.BoatEffectiveDate &&
                                boat.BoatCeaseDate == boat.OriginalBoat.BoatCeaseDate) ||
                                (boat.BoatCeaseDate > DateTime.MinValue && boat.BoatCeaseReason == 3))
                            {
                                boatFslDiffer = 0m;
                                boatPremiumDiffer = 0m;
                                boatExcessDiffer = 0;
                                boatTermLimitDiffer = 0;

                                totalBoatFslDiffer += 0m;
                                totalBoatPremiumDiffer += 0m;
                                totalBoatTermLimitDiffer += 0;

                            }
                            else
                            {
                                if (boat.OriginalBoat.BoatCeaseDate > DateTime.MinValue && boat.BoatCeaseDate == DateTime.MinValue)
                                {
                                    boatFslDiffer = boatproratedFsl;
                                    boatPremiumDiffer = boatproratedPremium;
                                    boatExcessDiffer = boat.BoatQuoteExcessOption;
                                    boatTermLimitDiffer = boat.MaxSumInsured;

                                    totalBoatFslDiffer += boatproratedFsl;
                                    totalBoatPremiumDiffer += boatproratedPremium;
                                    totalBoatTermLimitDiffer += boat.MaxSumInsured;
                                }
                                else if (preboatperiodindays == 0)
                                {
                                    boatFslDiffer = boatproratedFsl - bvTermPre.FSL;
                                    boatPremiumDiffer = boatproratedPremium - bvTermPre.Premium;
                                    boatExcessDiffer = boat.BoatQuoteExcessOption - bvTermPre.Excess;
                                    boatTermLimitDiffer = boat.MaxSumInsured - bvTermPre.TermLimit;

                                    totalBoatFslDiffer += boatproratedFsl - bvTermPre.FSL;
                                    totalBoatPremiumDiffer += boatproratedPremium - bvTermPre.Premium;
                                    totalBoatTermLimitDiffer += (boat.OriginalBoat.BoatCeaseDate > DateTime.MinValue) ? (boat.MaxSumInsured) : (boat.MaxSumInsured - bvTermPre.TermLimit);
                                }
                                else
                                {
                                    boatFslDiffer = boatproratedFsl - (bvTermPre.FSL * boatperiodindays / preboatperiodindays);
                                    boatPremiumDiffer = boatproratedPremium - (bvTermPre.Premium * boatperiodindays / preboatperiodindays);
                                    boatExcessDiffer = boat.BoatQuoteExcessOption - bvTermPre.Excess;
                                    boatTermLimitDiffer = boat.MaxSumInsured - bvTermPre.TermLimit;

                                    totalBoatFslDiffer += boatproratedFsl - (bvTermPre.FSL * boatperiodindays / preboatperiodindays);
                                    totalBoatPremiumDiffer += boatproratedPremium - (bvTermPre.Premium * boatperiodindays / preboatperiodindays);
                                    totalBoatTermLimitDiffer += (boat.OriginalBoat.BoatCeaseDate > DateTime.MinValue) ? (boat.MaxSumInsured) : (boat.MaxSumInsured - bvTermPre.TermLimit);
                                }
                            }


                        }
                    }

                }
                else
                {
                    boatFslDiffer = boatproratedFsl;
                    boatPremiumDiffer = boatproratedPremium;
                    boatExcessDiffer = boat.BoatQuoteExcessOption;
                    boatTermLimitDiffer = boat.MaxSumInsured;

                    totalBoatFslDiffer += boatproratedFsl;
                    totalBoatPremiumDiffer += boatproratedPremium;
                    totalBoatTermLimitDiffer += boat.MaxSumInsured;
                }

                bvTerm.FSLPre = boatFslPre;
                bvTerm.FSLDiffer = boatFslDiffer;
                bvTerm.PremiumPre = boatPremiumPre;
                bvTerm.PremiumDiffer = boatPremiumDiffer;
                bvTerm.ExcessPre = boatExcessPre;
                bvTerm.ExcessDiffer = boatExcessDiffer;
                bvTerm.TermLimitPre = boatTermLimitPre;
                bvTerm.TermLimitDiffer = boatTermLimitDiffer;

                if (boat.BoatCeaseDate > DateTime.MinValue && boat.BoatCeaseReason == 4)
                {
                    bvTerm.TermCategory = "transfered";
                }
                else if (boat.BoatCeaseDate > DateTime.MinValue && boat.BoatCeaseReason != 4)
                {
                    bvTerm.TermCategory = "ceased";
                }
                else
                {
                    bvTerm.TermCategory = "active";
                }
                //===========

                totalTermLimit += boat.MaxSumInsured;
                totalTermFsl += boatproratedFsl;
                totalTermPremium += boatproratedPremium;
                totalTermBrokerage += boatproratedBrokerage;

                //Referral points per vessel
                //Trailer Craft Sum Insured over $250k
                uwrftrailercraftsuminsuredover250k(underwritingUser, boat, agreement);
                //Trailer Craft (jetboats) Sum Insured over $100k
                uwrfjetboatsuminsuredover100k(underwritingUser, boat, agreement);
                //Trailer Craft (jetskis) Sum Insured over $100k
                uwrfjetskisuminsuredover100k(underwritingUser, boat, agreement);
                //Moored Craft Sum Insured over $500k
                uwrfmooredcraftsuminsuredover500k(underwritingUser, boat, agreement);
                //Year of vessel built pre 1985
                uwrfyearbuiltpre1985(underwritingUser, boat, agreement);
                //Not built professionally
                uwrfnotbuiltprofessionally(underwritingUser, boat, agreement);
                //Vessel type (boattype1) other
                uwrfboattype1other(underwritingUser, boat, agreement);
                //Hull construction carbon or other
                uwrfhullconstructioncarbonorother(underwritingUser, boat, agreement);
                //Hull configuration trimaran or other
                uwrfhullconfigurationtrimaranorother(underwritingUser, boat, agreement);
                //Motor type inboardpetrol or jet
                uwrfmotortypeinboardpetrolorjet(underwritingUser, boat, agreement);
                //Motor modified
                uwrfmotormodified(underwritingUser, boat, agreement);
                //Max speed over 60 knots
                uwrfmaxspeedover60knots(underwritingUser, boat, agreement);
                //Vessel use live on board
                uwrfboatuseliveonboard(underwritingUser, boat, agreement);
                //Vessel use race (Oceangoingracingover200nm, Category1Racing)
                uwrfboatuseraceotheroption(underwritingUser, boat, agreement);
                //Swing Moored Type
                uwrfswingmooredtype(underwritingUser, boat, agreement);
                //Other Marina Referred to TC
                uwrfothermarinatc(underwritingUser, boat, agreement);
                //Other Marina
                uwrfothermarina(underwritingUser, boat, agreement);

            }


            //calculate trailer premium and FSL (MV Term)
            foreach (var vehicle in informationSheet.Vehicles.Where(v => !v.Removed && v.DateDeleted == null))
            {
                if (vehicle.VehicleEffectiveDate < agreement.InceptionDate || vehicle.VehicleEffectiveDate > agreement.InceptionDate.AddDays(30))
                {
                    vehicle.VehicleEffectiveDate = agreement.InceptionDate;
                }

                vehicle.VehicleInceptionDate = vehicle.VehicleEffectiveDate;
                vehicle.VehicleExpireDate = agreement.ExpiryDate;

                int vehicleperiodindays = 0;
                vehicleperiodindays = (vehicle.VehicleExpireDate - vehicle.VehicleInceptionDate).Days;

                string vehicleCategory = null;
                decimal vehicleFsl = 0m;
                decimal vehiclePremium = 0m;
                decimal vehicleBurnerPremium = 0m;
                decimal vehicleExcess = 0m;
                decimal vehicleBrokerage = 0m;
                decimal vehicleBrokerageRate = 0m;
                decimal vehicleproratedFsl = 0m;
                decimal vehicleproratedPremium = 0m;
                decimal vehicleproratedBrokerage = 0m;

                GetFslFeeForTrailer(rates, vehicle, ref vehicleFsl);

                vehiclePremium = (vehicle.GroupSumInsured * rates["mvpremiumrate"] / 100) + vehicleFsl;
                vehicleBrokerageRate = agreement.Brokerage;
                vehicleBrokerage = vehiclePremium * vehicleBrokerageRate / 100;
                vehicleproratedPremium = vehiclePremium;
                vehicleproratedFsl = vehicleFsl;
                vehicleproratedBrokerage = vehicleBrokerage;
                //Pre-rate premium if the vehicle effective date is later than policy inception date
                if (vehicle.VehicleEffectiveDate > agreement.InceptionDate)
                {
                    vehicleproratedPremium = vehiclePremium * vehicleperiodindays / agreementperiodindays;
                    vehicleproratedFsl = vehicleFsl * vehicleperiodindays / agreementperiodindays;
                    vehicleproratedBrokerage = vehicleBrokerage * vehicleperiodindays / agreementperiodindays;
                }

                totalVehicleFsl += vehicleproratedFsl;
                totalVehicleSumInsured += (vehicle.VehicleCeaseDate > DateTime.MinValue) ? 0 : vehicle.GroupSumInsured;
                totalVehiclePremium += vehicleproratedPremium;
                totalVehicleBrokerage += vehicleproratedBrokerage;

                ClientAgreementMVTerm mvTerm = null;
                if (term.MotorTerms != null)
                    mvTerm = term.MotorTerms.FirstOrDefault(mvt => mvt.Vehicle == vehicle && mvt.DateDeleted != null);
                else
                    term.MotorTerms = new List<ClientAgreementMVTerm>();

                if (mvTerm == null)
                {
                    mvTerm = new ClientAgreementMVTerm(underwritingUser, vehicle.Registration, vehicle.Year, vehicle.Make, vehicle.Model, vehicle.GroupSumInsured,
                                                       vehicleExcess, vehicleproratedPremium, vehicleproratedFsl, vehicleBrokerageRate, vehicleproratedBrokerage, vehicleCategory, vehicle.FleetNumber, term, vehicle, vehicleBurnerPremium);
                    mvTerm.TermCategory = "active";
                    mvTerm.AnnualPremium = vehiclePremium;
                    mvTerm.AnnualFSL = vehicleFsl;
                    mvTerm.AnnualBrokerage = vehicleBrokerage;
                    term.MotorTerms.Add(mvTerm);
                }
                else
                {
                    mvTerm.Registration = vehicle.Registration;
                    mvTerm.Year = vehicle.Year;
                    mvTerm.Make = vehicle.Make;
                    mvTerm.Model = vehicle.Model;
                    mvTerm.TermLimit = vehicle.GroupSumInsured;
                    mvTerm.Premium = vehicleproratedPremium;
                    mvTerm.BurnerPremium = vehicleBurnerPremium;
                    mvTerm.FSL = vehicleproratedFsl;
                    mvTerm.DateDeleted = null;
                    mvTerm.BrokerageRate = vehicleBrokerageRate;
                    mvTerm.Brokerage = vehicleproratedBrokerage;
                    mvTerm.VehicleCategory = vehicleCategory;
                    mvTerm.FleetNumber = vehicle.FleetNumber;
                    mvTerm.LastModifiedOn = DateTime.UtcNow;
                    mvTerm.LastModifiedBy = underwritingUser;
                    mvTerm.TermCategory = "active";
                    mvTerm.AnnualPremium = vehiclePremium;
                    mvTerm.AnnualFSL = vehicleFsl;
                    mvTerm.AnnualBrokerage = vehicleBrokerage;
                }

                totalTermLimit += vehicle.GroupSumInsured;
                totalTermFsl += vehicleproratedFsl;
                totalTermPremium += vehicleproratedPremium;
                totalTermBrokerage += vehicleproratedBrokerage;
            }

            term.TermLimit = totalTermLimit;
            term.Premium = totalTermPremium;
            term.FSL = totalTermFsl;
            term.BrokerageRate = agreement.Brokerage;
            term.Brokerage = totalTermBrokerage;

            //Referral points per agreement
            //Claim over $5k of losses
            uwrfclaimover5koflosses(underwritingUser, agreement);
            //Prior insurance
            uwrfpriorinsurance(underwritingUser, agreement);

            //Update agreement status
            if (agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null && cref.Status == "Pending").Count() > 0)
            {
                agreement.Status = "Referred";
            }
            else
            {
                agreement.Status = "Quoted";
            }

            //Set broker fee for change agreement
            if (agreement.ClientInformationSheet.IsChange && (bolNewBoatAdded || bolNewVehicleAdded))
            {
                agreement.BrokerFee = 15;
            } else if (agreement.ClientInformationSheet.IsChange && !bolNewBoatAdded && !bolNewVehicleAdded)
            {
                agreement.BrokerFee = 0;
            }

            string auditLogDetail = "Marsh Coastguard UW created/modified";
            AuditLog auditLog = new AuditLog(underwritingUser, informationSheet, agreement, auditLogDetail);
            agreement.ClientAgreementAuditLogs.Add(auditLog);

            return true;

        }

        ClientAgreement GetClientAgreement(User currentUser, ClientInformationSheet informationSheet, ClientProgramme programme, Product product, string reference)
        {
            ClientAgreement clientAgreement = programme.Agreements.FirstOrDefault(a => a.Product != null && a.Product.Id == product.Id);
            ClientAgreement previousClientAgreement = null;
            if (clientAgreement == null)
            {
                DateTime inceptionDate = DateTime.UtcNow;
                DateTime expiryDate = DateTime.UtcNow.AddYears(1);

                if (informationSheet.IsChange) //change agreement to keep the original inception date and expiry date
                {
                    if (informationSheet.PreviousInformationSheet != null)
                    {
                        previousClientAgreement = informationSheet.PreviousInformationSheet.Programme.Agreements.FirstOrDefault(prea => prea.Product != null && prea.Product.Id == product.Id);
                        if (previousClientAgreement != null)
                        {
                            inceptionDate = previousClientAgreement.InceptionDate;
                            expiryDate = previousClientAgreement.ExpiryDate;
                        }
                    }
                }
                clientAgreement = new ClientAgreement(currentUser, informationSheet.Owner.Name, inceptionDate, expiryDate, product.DefaultBrokerage, product.DefaultBrokerFee, informationSheet, product, reference);

                clientAgreement.MasterAgreement = true;
                clientAgreement.PreviousAgreement = previousClientAgreement;
                programme.Agreements.Add(clientAgreement);
                clientAgreement.Status = "Quoted";

            }
            return clientAgreement;
        }

        ClientAgreementTerm GetAgreementTerm(User CurrentUser, ClientAgreement agreement, string subTerm)
        {
            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == subTerm && t.DateDeleted == null);
            if (term == null)
            {
                term = new ClientAgreementTerm(CurrentUser, 0, 0m, 0m, 0m, 0m, 0m, agreement, subTerm);
                agreement.ClientAgreementTerms.Add(term);
            }

            return term;
        }

        IDictionary<string, decimal> BuildRulesTable(ClientAgreement agreement, params string[] names)
        {
            var dict = new Dictionary<string, decimal>();

            foreach (string name in names)
                dict[name] = Convert.ToDecimal(agreement.ClientAgreementRules.FirstOrDefault(r => r.Name == name).Value);

            return dict;
        }

        void GetFslFeeForTrailer(IDictionary<string, decimal> rates, Vehicle vehicle, ref decimal vehicleFsl)
        {
            vehicleFsl = rates["fslfeefort"];
        }

        void GetFslRateForTraileredBoat(IDictionary<string, decimal> rates, Boat boat, ref decimal boatFslRate)
        {
            if (boat.BoatType2 != "Moored" && boat.BoatType2 != "Berthed") //Moored Craft
            {
                boatFslRate = rates["fslratefortc"];
            }
        }

        void GetBoatRateAndMinPremiumFor(IDictionary<string, decimal> rates, Boat boat, out decimal boatRate, out decimal boatMinPremium)
        {
            int boatUWCategory = 0;

            boatRate = 0M;
            boatMinPremium = 0M;

            if (boat.BoatType2 != null || boat.BoatType1 != null)
            {
                if (boat.BoatType2 == "Roadtrailer" || boat.BoatType2 == "Stored") //Trailer Craft
                {
                    if (boat.BoatType1 == "Jetboat")
                    {
                        boatUWCategory = 2; //Trailer Craft (jetboats)
                    }
                    else if (boat.BoatType1 == "Jetski")
                    {
                        boatUWCategory = 3; //Trailer Craft (jetskis)
                    }
                    else
                    {
                        boatUWCategory = 1; //Trailer Craft (excluding jetboats, jetskis)
                    }
                }
                else if (boat.BoatType2 == "Berthed" || boat.BoatType2 == "Moored") //Moored Craft
                {
                    boatUWCategory = 4;
                }
            }
            else
            {
                throw new Exception(string.Format("Can not get BoatType for boat", boat.Id));
            }

            switch (boatUWCategory)
            {
                case 1:
                    {
                        if (boat.MaxSumInsured > 0 && boat.MaxSumInsured <= 50000)
                        {
                            boatMinPremium = rates["tcunder50kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 250:
                                    {
                                        boatRate = rates["tcunder50kexcess250rate"];
                                        break;
                                    }
                                case 500:
                                    {
                                        boatRate = rates["tcunder50kexcess500rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        else if (boat.MaxSumInsured > 50000 && boat.MaxSumInsured <= 100000)
                        {
                            boatMinPremium = rates["tc50k100kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 250:
                                    {
                                        boatRate = rates["tc50k100kexcess250rate"];
                                        break;
                                    }
                                case 500:
                                    {
                                        boatRate = rates["tc50k100kexcess500rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        else if (boat.MaxSumInsured > 100000 && boat.MaxSumInsured <= 200000)
                        {
                            boatMinPremium = rates["tc100k200kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 500:
                                    {
                                        boatRate = rates["tc100k200kexcess500rate"];
                                        break;
                                    }
                                case 1000:
                                    {
                                        boatRate = rates["tc100k200kexcess1000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        else if (boat.MaxSumInsured > 200000 && boat.MaxSumInsured <= 250000)
                        {
                            boatMinPremium = rates["tc200k250kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 500:
                                    {
                                        boatRate = rates["tc200k250kexcess500rate"];
                                        break;
                                    }
                                case 1000:
                                    {
                                        boatRate = rates["tc200k250kexcess1000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (boat.MaxSumInsured > 0 && boat.MaxSumInsured <= 100000)
                        {
                            boatMinPremium = rates["jbunder100kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 500:
                                    {
                                        boatRate = rates["jbunder100kexcess500rate"];
                                        break;
                                    }
                                case 1000:
                                    {
                                        boatRate = rates["jbunder100kexcess1000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (boat.MaxSumInsured > 0 && boat.MaxSumInsured <= 100000)
                        {
                            boatMinPremium = rates["jsunder100kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 500:
                                    {
                                        boatRate = rates["jsunder100kexcess500rate"];
                                        break;
                                    }
                                case 1000:
                                    {
                                        boatRate = rates["jsunder100kexcess1000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        break; ;
                    }
                case 4:
                    {
                        if (boat.MaxSumInsured > 0 && boat.MaxSumInsured <= 100000)
                        {
                            boatMinPremium = rates["mcunder100kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 250:
                                    {
                                        boatRate = rates["mcunder100kexcess250rate"];
                                        break;
                                    }
                                case 500:
                                    {
                                        boatRate = rates["mcunder100kexcess500rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        else if (boat.MaxSumInsured > 100000 && boat.MaxSumInsured <= 200000)
                        {
                            boatMinPremium = rates["mc100k200kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 500:
                                    {
                                        boatRate = rates["mc100k200kexcess500rate"];
                                        break;
                                    }
                                case 1000:
                                    {
                                        boatRate = rates["mc100k200kexcess1000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        else if (boat.MaxSumInsured > 200000 && boat.MaxSumInsured <= 350000)
                        {
                            boatMinPremium = rates["mc200k350kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 1000:
                                    {
                                        boatRate = rates["mc200k350kexcess1000rate"];
                                        break;
                                    }
                                case 2000:
                                    {
                                        boatRate = rates["mc200k350kexcess2000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        else if (boat.MaxSumInsured > 350000 && boat.MaxSumInsured <= 500000)
                        {
                            boatMinPremium = rates["mc350k500kminpremium"];
                            switch (boat.BoatQuoteExcessOption)
                            {
                                case 1000:
                                    {
                                        boatRate = rates["mc350k500kexcess1000rate"];
                                        break;
                                    }
                                case 2000:
                                    {
                                        boatRate = rates["mc350k500kexcess2000rate"];
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                                    }
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Can not get rate or min premium for boat", boat.Id));
                    }
            }
        }

        void uwrftrailercraftsuminsuredover250k(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrftrailercraftsuminsuredover250k" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrftrailercraftsuminsuredover250k") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrftrailercraftsuminsuredover250k").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrftrailercraftsuminsuredover250k").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrftrailercraftsuminsuredover250k").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrftrailercraftsuminsuredover250k").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrftrailercraftsuminsuredover250k" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType2 == "Roadtrailer" || boat.BoatType2 == "Stored") && (boat.BoatType1 != "Jetboat" && boat.BoatType1 != "Jetski")) //Trailer Craft (excluding jetboats, jetskis)
                    {
                        if (boat.MaxSumInsured > 250000)
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrftrailercraftsuminsuredover250k" && cref.DateDeleted == null).Status = "Pending";
                        }
                    }
                }
            }
        }

        void uwrfjetboatsuminsuredover100k(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfjetboatsuminsuredover100k" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetboatsuminsuredover100k") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetboatsuminsuredover100k").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetboatsuminsuredover100k").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetboatsuminsuredover100k").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetboatsuminsuredover100k").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfjetboatsuminsuredover100k" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType2 == "Roadtrailer" || boat.BoatType2 == "Stored") && boat.BoatType1 == "Jetboat") //Trailer Craft (jetboats)
                    {
                        if (boat.MaxSumInsured > 100000)
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfjetboatsuminsuredover100k" && cref.DateDeleted == null).Status = "Pending";
                        }
                    }
                }
            }
        }

        void uwrfjetskisuminsuredover100k(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfjetskisuminsuredover100k" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetskisuminsuredover100k") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetskisuminsuredover100k").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetskisuminsuredover100k").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetskisuminsuredover100k").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfjetskisuminsuredover100k").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfjetskisuminsuredover100k" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType2 == "Roadtrailer" || boat.BoatType2 == "Stored") && boat.BoatType1 == "Jetski") //Trailer Craft (jetskis)
                    {
                        if (boat.MaxSumInsured > 100000)
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfjetskisuminsuredover100k" && cref.DateDeleted == null).Status = "Pending";
                        }
                    }
                }
            }
        }

        void uwrfmooredcraftsuminsuredover500k(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmooredcraftsuminsuredover500k" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmooredcraftsuminsuredover500k") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmooredcraftsuminsuredover500k").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmooredcraftsuminsuredover500k").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmooredcraftsuminsuredover500k").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmooredcraftsuminsuredover500k").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmooredcraftsuminsuredover500k" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.BoatType2 == "Moored" || boat.BoatType2 == "Berthed") //Moored Craft
                    {
                        if (boat.MaxSumInsured > 500000)
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmooredcraftsuminsuredover500k" && cref.DateDeleted == null).Status = "Pending";
                        }
                    }
                }
            }
        }

        void uwrfyearbuiltpre1985(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfyearbuiltpre1985" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfyearbuiltpre1985") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfyearbuiltpre1985").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfyearbuiltpre1985").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfyearbuiltpre1985").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfyearbuiltpre1985").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfyearbuiltpre1985" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.YearOfManufacture < 1985)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfyearbuiltpre1985" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfnotbuiltprofessionally(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotbuiltprofessionally" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotbuiltprofessionally") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotbuiltprofessionally").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotbuiltprofessionally").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotbuiltprofessionally").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotbuiltprofessionally").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotbuiltprofessionally" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.BuiltProfessionally == "False")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotbuiltprofessionally" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfboattype1other(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboattype1other" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboattype1other") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboattype1other").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboattype1other").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboattype1other").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboattype1other").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboattype1other" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.BoatType1 == "Other")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboattype1other" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfhullconstructioncarbonorother(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhullconstructioncarbonorother" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconstructioncarbonorother") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconstructioncarbonorother").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconstructioncarbonorother").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconstructioncarbonorother").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconstructioncarbonorother").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhullconstructioncarbonorother" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.HullConstruction == "Carbon" || boat.HullConstruction == "Other")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhullconstructioncarbonorother" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfhullconfigurationtrimaranorother(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhullconfigurationtrimaranorother" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconfigurationtrimaranorother") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconfigurationtrimaranorother").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconfigurationtrimaranorother").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconfigurationtrimaranorother").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhullconfigurationtrimaranorother").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhullconfigurationtrimaranorother" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.HullConfiguration == "Trimaran" || boat.HullConfiguration == "Other")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhullconfigurationtrimaranorother" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfmotortypeinboardpetrolorjet(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmotortypeinboardpetrolorjet" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotortypeinboardpetrolorjet") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotortypeinboardpetrolorjet").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotortypeinboardpetrolorjet").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotortypeinboardpetrolorjet").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotortypeinboardpetrolorjet").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmotortypeinboardpetrolorjet" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.MotorType == "InboardPetrol" || boat.MotorType == "Jet")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmotortypeinboardpetrolorjet" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfmotormodified(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmotormodified" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotormodified") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotormodified").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotormodified").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotormodified").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmotormodified").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmotormodified" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.ModifiedMotor == "True")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmotormodified" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfmaxspeedover60knots(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmaxspeedover60knots" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmaxspeedover60knots") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmaxspeedover60knots").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmaxspeedover60knots").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmaxspeedover60knots").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfmaxspeedover60knots").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmaxspeedover60knots" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.MaxRatedSpeed == "Over60Knots")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfmaxspeedover60knots" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfclaimover5koflosses(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclaimover5koflosses" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclaimover5koflosses") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclaimover5koflosses").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclaimover5koflosses").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclaimover5koflosses").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclaimover5koflosses").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclaimover5koflosses" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Claims.Any(clm => clm.ClaimEstimateInsuredLiability > 5000))
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclaimover5koflosses" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfpriorinsurance(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfpriorinsurance" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfpriorinsurance") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfpriorinsurance").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfpriorinsurance").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfpriorinsurance").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfpriorinsurance").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfpriorinsurance" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "Claimexp1").First().Value == "true" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "Claimexp2").First().Value == "true" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "Claimexp3").First().Value == "true" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "Claimexp4").First().Value == "true" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "Claimexp5").First().Value == "true")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfpriorinsurance" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfboatuseliveonboard(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboatuseliveonboard" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseliveonboard") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseliveonboard").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseliveonboard").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseliveonboard").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseliveonboard").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboatuseliveonboard" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType1 == "CruisersandLaunches" || boat.BoatType1 == "YachtsandCatamarans")
                        && boat.BoatUse.Any(ycbu => ycbu.BoatUseCategory == "LiveOnBoard" && ycbu.DateDeleted == null && !ycbu.Removed))
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboatuseliveonboard" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfboatuseraceotheroption(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboatuseraceotheroption" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseraceotheroption") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseraceotheroption").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseraceotheroption").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseraceotheroption").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfboatuseraceotheroption").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboatuseraceotheroption" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (boat.BoatType1 == "YachtsandCatamarans" && boat.BoatUse.Any(ycbu => ycbu.BoatUseCategory == "Race" && ycbu.BoatUseRaceUseSpinnakers == "True" && ycbu.DateDeleted == null && !ycbu.Removed))
                    {
                        if (boat.BoatUse.Any(ycbu => ycbu.BoatUseRaceCategory == "Oceangoingracingover200nm") ||
                            boat.BoatUse.Any(ycbu => ycbu.BoatUseRaceCategory == "Category1Racing")) //BoatUseRaceCategory (Oceangoingracingover200nm, Category1Racing)
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfboatuseraceotheroption" && cref.DateDeleted == null).Status = "Pending";
                        }
                    }
                }
            }
        }

        void uwrfswingmooredtype(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfswingmooredtype" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfswingmooredtype") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfswingmooredtype").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfswingmooredtype").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfswingmooredtype").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfswingmooredtype").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfswingmooredtype" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType2 == "Berthed" || boat.BoatType2 == "Moored") && boat.WaterLocationMooringType == "Swing")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfswingmooredtype" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfothermarinatc(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            agreement.ReferToTC = false;

            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfothermarinatc" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarinatc") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarinatc").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarinatc").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarinatc").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarinatc").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfothermarinatc" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType2 == "Berthed" || boat.BoatType2 == "Moored") && boat.BoatWaterLocation == null && boat.OtherMarina && boat.OtherMarinaName != "")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfothermarinatc" && cref.DateDeleted == null).Status = "Pending";
                        agreement.ReferToTC = true;
                    }
                }
            }
        }

        void uwrfothermarina(User underwritingUser, Boat boat, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfothermarina" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarina") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarina").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarina").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarina").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfothermarina").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfothermarina" && cref.DateDeleted == null).Status != "Pending")
                {
                    if ((boat.BoatType2 == "Berthed" || boat.BoatType2 == "Moored") && boat.BoatWaterLocation != null && !boat.BoatWaterLocation.IsApproved &&
                        boat.BoatWaterLocation.InsuranceAttributes.FirstOrDefault(bwlocia => bwlocia.InsuranceAttributeName == "Other Marina" && bwlocia.DateDeleted == null) != null)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfothermarina" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }


    }

}
