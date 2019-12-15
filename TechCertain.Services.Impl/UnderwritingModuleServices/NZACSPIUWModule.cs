using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechCertain.Services.Impl.UnderwritingModuleServices
{
    public class NZACSPIUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public NZACSPIUWModule()
        {
            Name = "NZACS_PI";
        }

        public bool Underwrite(User CurrentUser, ClientInformationSheet informationSheet)
        {
            throw new NotImplementedException();
        }

        public bool Underwrite(User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
        {
            ClientAgreement agreement = GetClientAgreement(underwritingUser, informationSheet, informationSheet.Programme, product, reference);
            Guid id = agreement.Id;

            if (agreement.ClientAgreementRules.Count == 0)
                foreach (var rule in product.Rules.Where(r => !string.IsNullOrWhiteSpace(r.Name)))
                    agreement.ClientAgreementRules.Add(new ClientAgreementRule(underwritingUser, rule, agreement));

            if (agreement.ClientAgreementEndorsements.Count == 0)
                foreach (var endorsement in product.Endorsements.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
                    agreement.ClientAgreementEndorsements.Add(new ClientAgreementEndorsement(underwritingUser, endorsement, agreement));

            if (agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "PI" && ct.DateDeleted == null) != null)
            {
                foreach (ClientAgreementTerm piterm in agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "PI" && ct.DateDeleted == null))
                {
                    piterm.Delete(underwritingUser);
                }
            }

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "piexcessrate", "piminexcess", "pimaxexcess", "piexcessdiscountrate4kto5k", "piexcessdiscountrate5kto6k",
                            "piexcessdiscountrate6kto7k", "piexcessdiscountrate7kto8k", "piexcessdiscountrate8kto9k", "piexcessdiscountrate9kto10k", "pischoolactivityloadingunder5percentage",
                            "pischoolactivityloading5to20percentage", "pischoolactivityloading20to65percentage", "pischoolactivityloading65to100percentage", "pimarkupfeerate",
                            "pimaxmarkupfee", "pi250klimitbasepremium", "pi350klimitbasepremium", "pi400klimitbasepremium", "pi500klimitbasepremium", "pi600klimitbasepremium", "pi750klimitbasepremium",
                            "pi1millimitbasepremium", "pi1andhalfmillimitbasepremium", "pi2millimitbasepremium", "pi2andhalfmillimitbasepremium", "pi3millimitbasepremium", "pi4millimitbasepremium",
                            "pi5millimitbasepremium", "pi6millimitbasepremium", "pi8millimitbasepremium", "pi10millimitbasepremium", "pi250klimitminmarkupfee", "pi350klimitminmarkupfee",
                            "pi400klimitminmarkupfee", "pi500klimitminmarkupfee", "pi600klimitminmarkupfee", "pi750klimitminmarkupfee", "pi1millimitminmarkupfee", "pi1andhalfmillimitminmarkupfee", 
                            "pi2millimitminmarkupfee", "pi2andhalfmillimitminmarkupfee", "pi3millimitminmarkupfee", "pi4millimitminmarkupfee", "pi5millimitminmarkupfee", "pi6millimitminmarkupfee",
                            "pi8millimitminmarkupfee", "pi10millimitminmarkupfee", 
                            "pi250klimitunder1milrate", "pi250klimit1milto2milrate", "pi250klimitover2milrate", "pi350klimitunder1milrate", "pi350klimit1milto2milrate", "pi350klimitover2milrate", 
                            "pi400klimitunder1milrate", "pi400klimit1milto2milrate", "pi400klimitover2milrate", "pi500klimitunder1milrate", "pi500klimit1milto2milrate", "pi500klimitover2milrate",
                            "pi600klimitunder1milrate", "pi600klimit1milto2milrate", "pi600klimitover2milrate", "pi750klimitunder1milrate", "pi750klimit1milto2milrate", "pi750klimitover2milrate");

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

            agreement.QuoteDate = DateTime.UtcNow;

            int TermLimit250k = 250000;
            decimal TermPremium250k = 0m;
            decimal TermBrokerage250k = 0m;
            int TermLimit350k = 350000;
            decimal TermPremium350k = 0m;
            decimal TermBrokerage350k = 0m;
            int TermLimit400k = 400000;
            decimal TermPremium400k = 0m;
            decimal TermBrokerage400k = 0m;
            int TermLimit500k = 500000;
            decimal TermPremium500k = 0m;
            decimal TermBrokerage500k = 0m;
            int TermLimit600k = 600000;
            decimal TermPremium600k = 0m;
            decimal TermBrokerage600k = 0m;
            int TermLimit750k = 750000;
            decimal TermPremium750k = 0m;
            decimal TermBrokerage750k = 0m;
            int TermLimit1mil = 1000000;
            decimal TermPremium1mil = 0m;
            decimal TermBrokerage1mil = 0m;
            int TermLimit1andhalfmil = 1500000;
            decimal TermPremium1andhalfmil = 0m;
            decimal TermBrokerage1andhalfmil = 0m;
            int TermLimit2mil = 2000000;
            decimal TermPremium2mil = 0m;
            decimal TermBrokerage2mil = 0m;
            int TermLimit2andhalfmil = 2500000;
            decimal TermPremium2andhalfmil = 0m;
            decimal TermBrokerage2andhalfmil = 0m;
            int TermLimit3mil = 3000000;
            decimal TermPremium3mil = 0m;
            decimal TermBrokerage3mil = 0m;
            int TermLimit4mil = 4000000;
            decimal TermPremium4mil = 0m;
            decimal TermBrokerage4mil = 0m;
            int TermLimit5mil = 5000000;
            decimal TermPremium5mil = 0m;
            decimal TermBrokerage5mil = 0m;
            int TermLimit6mil = 6000000;
            decimal TermPremium6mil = 0m;
            decimal TermBrokerage6mil = 0m;
            int TermLimit8mil = 8000000;
            decimal TermPremium8mil = 0m;
            decimal TermBrokerage8mil = 0m;
            int TermLimit10mil = 10000000;
            decimal TermPremium10mil = 0m;
            decimal TermBrokerage10mil = 0m;

            int TermExcess = 0;
            decimal feeincome = 0;

            decimal schoolsactivitymoepercentage = 0M;
            decimal schoolsactivitynonmoepercentage = 0M;

            //Calculation
            if (agreement.ClientInformationSheet.RevenueData != null)
            {
                foreach (var uISTerritory in agreement.ClientInformationSheet.RevenueData.Territories)
                {
                    if (uISTerritory.Location == "NZ") //NZ income only
                    {
                        feeincome = Convert.ToDecimal(agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "totalRevenue").First().Value) * uISTerritory.Pecentage / 100;
                    }
                }

                foreach (var uISActivity in agreement.ClientInformationSheet.RevenueData.Activities)
                {
                    if (uISActivity.AnzsciCode == "M692130") //Architecture - Schools Activity MOE
                    {
                        schoolsactivitymoepercentage = uISActivity.Pecentage;
                    } else if (uISActivity.AnzsciCode == "M692140") //Architecture - Schools Activity Non-MOE
                    {
                        schoolsactivitynonmoepercentage = uISActivity.Pecentage;
                    }
                }
            }

            //Return terms based on the limit options

            TermExcess = Convert.ToInt32(Math.Round((feeincome * rates["piexcessrate"]), 0, MidpointRounding.AwayFromZero));
            if (TermExcess >= 0 && TermExcess <= Convert.ToInt32(rates["piminexcess"]))
            {
                TermExcess = Convert.ToInt32(rates["piminexcess"]);
            } else if (TermExcess >= Convert.ToInt32(rates["pimaxexcess"]))
            {
                TermExcess = Convert.ToInt32(rates["pimaxexcess"]);
            }

            TermPremium250k = GetPremiumFor(rates, feeincome, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl250klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit250k, TermExcess);
            termsl250klimitoption.TermLimit = TermLimit250k;
            termsl250klimitoption.Premium = TermPremium250k;
            termsl250klimitoption.Excess = TermExcess;
            termsl250klimitoption.BrokerageRate = agreement.Brokerage;
            termsl250klimitoption.Brokerage = TermBrokerage250k;
            termsl250klimitoption.DateDeleted = null;
            termsl250klimitoption.DeletedBy = null;

            TermPremium350k = GetPremiumFor(rates, feeincome, TermLimit350k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl350klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit350k, TermExcess);
            termsl350klimitoption.TermLimit = TermLimit350k;
            termsl350klimitoption.Premium = TermPremium350k;
            termsl350klimitoption.Excess = TermExcess;
            termsl350klimitoption.BrokerageRate = agreement.Brokerage;
            termsl350klimitoption.Brokerage = TermBrokerage350k;
            termsl350klimitoption.DateDeleted = null;
            termsl350klimitoption.DeletedBy = null;

            //TermPremium400k = GetPremiumFor(rates, feeincome, TermLimit400k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl400klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit400k, TermExcess);
            termsl400klimitoption.TermLimit = TermLimit400k;
            termsl400klimitoption.Premium = TermPremium400k;
            termsl400klimitoption.Excess = TermExcess;
            termsl400klimitoption.BrokerageRate = agreement.Brokerage;
            termsl400klimitoption.Brokerage = TermBrokerage400k;
            termsl400klimitoption.DateDeleted = null;
            termsl400klimitoption.DeletedBy = null;

            //TermPremium500k = GetPremiumFor(rates, feeincome, TermLimit500k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl500klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit500k, TermExcess);
            termsl500klimitoption.TermLimit = TermLimit500k;
            termsl500klimitoption.Premium = TermPremium500k;
            termsl500klimitoption.Excess = TermExcess;
            termsl500klimitoption.BrokerageRate = agreement.Brokerage;
            termsl500klimitoption.Brokerage = TermBrokerage500k;
            termsl500klimitoption.DateDeleted = null;
            termsl500klimitoption.DeletedBy = null;

            //TermPremium600k = GetPremiumFor(rates, feeincome, TermLimit600k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl600klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit600k, TermExcess);
            termsl600klimitoption.TermLimit = TermLimit600k;
            termsl600klimitoption.Premium = TermPremium600k;
            termsl600klimitoption.Excess = TermExcess;
            termsl600klimitoption.BrokerageRate = agreement.Brokerage;
            termsl600klimitoption.Brokerage = TermBrokerage600k;
            termsl600klimitoption.DateDeleted = null;
            termsl600klimitoption.DeletedBy = null;

            //TermPremium750k = GetPremiumFor(rates, feeincome, TermLimit750k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl750klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit750k, TermExcess);
            termsl750klimitoption.TermLimit = TermLimit750k;
            termsl750klimitoption.Premium = TermPremium750k;
            termsl750klimitoption.Excess = TermExcess;
            termsl750klimitoption.BrokerageRate = agreement.Brokerage;
            termsl750klimitoption.Brokerage = TermBrokerage750k;
            termsl750klimitoption.DateDeleted = null;
            termsl750klimitoption.DeletedBy = null;

            //TermPremium1mil = GetPremiumFor(rates, feeincome, TermLimit1mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl1millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1mil, TermExcess);
            termsl1millimitoption.TermLimit = TermLimit1mil;
            termsl1millimitoption.Premium = TermPremium1mil;
            termsl1millimitoption.Excess = TermExcess;
            termsl1millimitoption.BrokerageRate = agreement.Brokerage;
            termsl1millimitoption.Brokerage = TermBrokerage1mil;
            termsl1millimitoption.DateDeleted = null;
            termsl1millimitoption.DeletedBy = null;

            //TermPremium1andhalfmil = GetPremiumFor(rates, feeincome, TermLimit1andhalfmil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl1andhalfmillimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1andhalfmil, TermExcess);
            termsl1andhalfmillimitoption.TermLimit = TermLimit1andhalfmil;
            termsl1andhalfmillimitoption.Premium = TermPremium1andhalfmil;
            termsl1andhalfmillimitoption.Excess = TermExcess;
            termsl1andhalfmillimitoption.BrokerageRate = agreement.Brokerage;
            termsl1andhalfmillimitoption.Brokerage = TermBrokerage1andhalfmil;
            termsl1andhalfmillimitoption.DateDeleted = null;
            termsl1andhalfmillimitoption.DeletedBy = null;

            //TermPremium2mil = GetPremiumFor(rates, feeincome, TermLimit2mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl2millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2mil, TermExcess);
            termsl2millimitoption.TermLimit = TermLimit2mil;
            termsl2millimitoption.Premium = TermPremium2mil;
            termsl2millimitoption.Excess = TermExcess;
            termsl2millimitoption.BrokerageRate = agreement.Brokerage;
            termsl2millimitoption.Brokerage = TermBrokerage2mil;
            termsl2millimitoption.DateDeleted = null;
            termsl2millimitoption.DeletedBy = null;

            //TermPremium2andhalfmil = GetPremiumFor(rates, feeincome, TermLimit2andhalfmil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl2andhalfmillimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2andhalfmil, TermExcess);
            termsl2andhalfmillimitoption.TermLimit = TermLimit2andhalfmil;
            termsl2andhalfmillimitoption.Premium = TermPremium2andhalfmil;
            termsl2andhalfmillimitoption.Excess = TermExcess;
            termsl2andhalfmillimitoption.BrokerageRate = agreement.Brokerage;
            termsl2andhalfmillimitoption.Brokerage = TermBrokerage2andhalfmil;
            termsl2andhalfmillimitoption.DateDeleted = null;
            termsl2andhalfmillimitoption.DeletedBy = null;

            //TermPremium3mil = GetPremiumFor(rates, feeincome, TermLimit3mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl3millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit3mil, TermExcess);
            termsl3millimitoption.TermLimit = TermLimit3mil;
            termsl3millimitoption.Premium = TermPremium3mil;
            termsl3millimitoption.Excess = TermExcess;
            termsl3millimitoption.BrokerageRate = agreement.Brokerage;
            termsl3millimitoption.Brokerage = TermBrokerage3mil;
            termsl3millimitoption.DateDeleted = null;
            termsl3millimitoption.DeletedBy = null;

            //TermPremium4mil = GetPremiumFor(rates, feeincome, TermLimit4mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl4millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit4mil, TermExcess);
            termsl4millimitoption.TermLimit = TermLimit4mil;
            termsl4millimitoption.Premium = TermPremium4mil;
            termsl4millimitoption.Excess = TermExcess;
            termsl4millimitoption.BrokerageRate = agreement.Brokerage;
            termsl4millimitoption.Brokerage = TermBrokerage4mil;
            termsl4millimitoption.DateDeleted = null;
            termsl4millimitoption.DeletedBy = null;

            //TermPremium5mil = GetPremiumFor(rates, feeincome, TermLimit5mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl5millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit5mil, TermExcess);
            termsl5millimitoption.TermLimit = TermLimit5mil;
            termsl5millimitoption.Premium = TermPremium5mil;
            termsl5millimitoption.Excess = TermExcess;
            termsl5millimitoption.BrokerageRate = agreement.Brokerage;
            termsl5millimitoption.Brokerage = TermBrokerage5mil;
            termsl5millimitoption.DateDeleted = null;
            termsl5millimitoption.DeletedBy = null;

            //TermPremium6mil = GetPremiumFor(rates, feeincome, TermLimit6mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl6millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit6mil, TermExcess);
            termsl6millimitoption.TermLimit = TermLimit6mil;
            termsl6millimitoption.Premium = TermPremium6mil;
            termsl6millimitoption.Excess = TermExcess;
            termsl6millimitoption.BrokerageRate = agreement.Brokerage;
            termsl6millimitoption.Brokerage = TermBrokerage6mil;
            termsl6millimitoption.DateDeleted = null;
            termsl6millimitoption.DeletedBy = null;

            //TermPremium8mil = GetPremiumFor(rates, feeincome, TermLimit8mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl8millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit8mil, TermExcess);
            termsl8millimitoption.TermLimit = TermLimit8mil;
            termsl8millimitoption.Premium = TermPremium8mil;
            termsl8millimitoption.Excess = TermExcess;
            termsl8millimitoption.BrokerageRate = agreement.Brokerage;
            termsl8millimitoption.Brokerage = TermBrokerage8mil;
            termsl8millimitoption.DateDeleted = null;
            termsl8millimitoption.DeletedBy = null;

            //TermPremium10mil = GetPremiumFor(rates, feeincome, TermLimit10mil, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl10millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit10mil, TermExcess);
            termsl10millimitoption.TermLimit = TermLimit10mil;
            termsl10millimitoption.Premium = TermPremium10mil;
            termsl10millimitoption.Excess = TermExcess;
            termsl10millimitoption.BrokerageRate = agreement.Brokerage;
            termsl10millimitoption.Brokerage = TermBrokerage10mil;
            termsl10millimitoption.DateDeleted = null;
            termsl10millimitoption.DeletedBy = null;

            //Referral points per agreement


            //Update agreement status
            if (agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null && cref.Status == "Pending").Count() > 0)
            {
                agreement.Status = "Referred";
            }
            else
            {
                agreement.Status = "Quoted";
            }


            string auditLogDetail = "NZACS PI UW created/modified";
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
                DateTime inceptionDate = (product.DefaultInceptionDate > DateTime.MinValue) ? product.DefaultInceptionDate : DateTime.UtcNow;
                DateTime expiryDate = (product.DefaultExpiryDate > DateTime.MinValue) ? product.DefaultExpiryDate : DateTime.UtcNow.AddYears(1);

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
                if (product.IsMasterProduct)
                {
                    clientAgreement.MasterAgreement = true;
                }
                else
                {
                    clientAgreement.MasterAgreement = false;
                }
                clientAgreement.PreviousAgreement = previousClientAgreement;
                programme.Agreements.Add(clientAgreement);
                clientAgreement.Status = "Quoted";
            }
            else
            {
                clientAgreement.DeletedBy = null;
                clientAgreement.DateDeleted = null;
            }
            return clientAgreement;
        }

        ClientAgreementTerm GetAgreementTerm(User CurrentUser, ClientAgreement agreement, string subTerm, int limitoption, decimal excessoption)
        {
            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == subTerm && t.DateDeleted != null && t.TermLimit == limitoption && t.Excess == excessoption);

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

        decimal GetPremiumFor(IDictionary<string, decimal> rates, decimal feeincome, int limitoption, int termexcess, decimal schoolsactivitymoepercentage, decimal schoolsactivitynonmoepercentage)
        {
            decimal premiumoption = 0M;
            decimal basepremium = 0M;
            decimal excessdiscountrate = 0M;
            decimal schoolloading = 0M;
            decimal schoolloadingpremium = 0M;
            decimal markupfee = 0M;
            decimal minmarkupfee = 0M;

            //Get the excess discount
            if (termexcess >= 4000 && termexcess <= 5000)
            {
                excessdiscountrate = rates["piexcessdiscountrate4kto5k"];
            } else if(termexcess > 5000 && termexcess <= 6000)
            {
                excessdiscountrate = rates["piexcessdiscountrate5kto6k"];
            } else if (termexcess > 6000 && termexcess <= 7000)
            {
                excessdiscountrate = rates["piexcessdiscountrate6kto7k"];
            } else if (termexcess > 7000 && termexcess <= 8000)
            {
                excessdiscountrate = rates["piexcessdiscountrate7kto8k"];
            } else if (termexcess > 8000 && termexcess <= 9000)
            {
                excessdiscountrate = rates["piexcessdiscountrate8kto9k"];
            } else if (termexcess > 9000 && termexcess <= 10000)
            {
                excessdiscountrate = rates["piexcessdiscountrate9kto10k"];
            }

            //Get the school activity loading
            if ((schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) >= 0 && (schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) <= 5)
            {
                schoolloading = rates["pischoolactivityloadingunder5percentage"];
            }
            else if ((schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) > 5 && (schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) <= 20)
            {
                schoolloading = rates["pischoolactivityloading5to20percentage"];
            }
            else if ((schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) > 20 && (schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) <= 65)
            {
                schoolloading = rates["pischoolactivityloading20to65percentage"];
            }
            else if ((schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) > 65 && (schoolsactivitymoepercentage + schoolsactivitynonmoepercentage) <= 100)
            {
                schoolloading = rates["pischoolactivityloading65to100percentage"];
            }
            
            switch (limitoption)
            {
                case 250000:
                    {
                        basepremium = rates["pi250klimitbasepremium"];
                        minmarkupfee = rates["pi250klimitminmarkupfee"];
                        if (feeincome >= 0 && feeincome <= 1000000)
                        {
                            premiumoption = rates["pi250klimitunder1milrate"] * feeincome;
                        }
                        else if (feeincome > 1000000 && feeincome <= 2000000)
                        {
                            premiumoption = (rates["pi250klimitunder1milrate"] * 1000000) + (rates["pi250klimit1milto2milrate"] * (feeincome - 1000000));
                        }
                        else if (feeincome > 2000000)
                        {
                            premiumoption = (rates["pi250klimitunder1milrate"] * 1000000) + (rates["pi250klimit1milto2milrate"] * 1000000) + rates["pi250klimitover2milrate"] * (feeincome - 2000000);
                        }
                        premiumoption = (premiumoption > basepremium) ? premiumoption : basepremium;
                        schoolloadingpremium = premiumoption * schoolloading / 100;
                        markupfee = ((premiumoption+ schoolloadingpremium)* rates["pimarkupfeerate"]/100 > minmarkupfee) ? (premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 : minmarkupfee;
                        markupfee = (markupfee < rates["pimaxmarkupfee"]) ? markupfee : rates["pimaxmarkupfee"];
                        premiumoption = premiumoption + schoolloadingpremium + markupfee;
                        break;
                    }
                case 350000:
                    {
                        basepremium = rates["pi350klimitbasepremium"];
                        minmarkupfee = rates["pi350klimitminmarkupfee"];
                        if (feeincome >= 0 && feeincome <= 1000000)
                        {
                            premiumoption = rates["pi350klimitunder1milrate"] * feeincome;
                        }
                        else if (feeincome > 1000000 && feeincome <= 2000000)
                        {
                            premiumoption = (rates["pi350klimitunder1milrate"] * 1000000) + (rates["pi350klimit1milto2milrate"] * (feeincome - 1000000));
                        }
                        else if (feeincome > 2000000)
                        {
                            premiumoption = (rates["pi350klimitunder1milrate"] * 1000000) + (rates["pi350klimit1milto2milrate"] * 1000000) + rates["pi350klimitover2milrate"] * (feeincome - 2000000);
                        }
                        premiumoption = (premiumoption > basepremium) ? premiumoption : basepremium;
                        schoolloadingpremium = premiumoption * schoolloading / 100;
                        markupfee = ((premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 > minmarkupfee) ? (premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 : minmarkupfee;
                        markupfee = (markupfee < rates["pimaxmarkupfee"]) ? markupfee : rates["pimaxmarkupfee"];
                        premiumoption = premiumoption + schoolloadingpremium + markupfee;
                        break;
                    }
                case 400000:
                    {
                        basepremium = rates["pi400klimitbasepremium"];
                        minmarkupfee = rates["pi400klimitminmarkupfee"];
                        if (feeincome >= 0 && feeincome <= 1000000)
                        {
                            premiumoption = rates["pi400klimitunder1milrate"] * feeincome;
                        }
                        else if (feeincome > 1000000 && feeincome <= 2000000)
                        {
                            premiumoption = (rates["pi400klimitunder1milrate"] * 1000000) + (rates["pi400klimit1milto2milrate"] * (feeincome - 1000000));
                        }
                        else if (feeincome > 2000000)
                        {
                            premiumoption = (rates["pi400klimitunder1milrate"] * 1000000) + (rates["pi400klimit1milto2milrate"] * 1000000) + rates["pi400klimitover2milrate"] * (feeincome - 2000000);
                        }
                        premiumoption = (premiumoption > basepremium) ? premiumoption : basepremium;
                        schoolloadingpremium = premiumoption * schoolloading / 100;
                        markupfee = ((premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 > minmarkupfee) ? (premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 : minmarkupfee;
                        markupfee = (markupfee < rates["pimaxmarkupfee"]) ? markupfee : rates["pimaxmarkupfee"];
                        premiumoption = premiumoption + schoolloadingpremium + markupfee;
                        break;
                    }
                case 500000:
                    {
                        basepremium = rates["pi500klimitbasepremium"];
                        minmarkupfee = rates["pi500klimitminmarkupfee"];
                        if (feeincome >= 0 && feeincome <= 1000000)
                        {
                            premiumoption = rates["pi500klimitunder1milrate"] * feeincome;
                        }
                        else if (feeincome > 1000000 && feeincome <= 2000000)
                        {
                            premiumoption = (rates["pi500klimitunder1milrate"] * 1000000) + (rates["pi500klimit1milto2milrate"] * (feeincome - 1000000));
                        }
                        else if (feeincome > 2000000)
                        {
                            premiumoption = (rates["pi500klimitunder1milrate"] * 1000000) + (rates["pi500klimit1milto2milrate"] * 1000000) + rates["pi500klimitover2milrate"] * (feeincome - 2000000);
                        }
                        premiumoption = (premiumoption > basepremium) ? premiumoption : basepremium;
                        schoolloadingpremium = premiumoption * schoolloading / 100;
                        markupfee = ((premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 > minmarkupfee) ? (premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 : minmarkupfee;
                        markupfee = (markupfee < rates["pimaxmarkupfee"]) ? markupfee : rates["pimaxmarkupfee"];
                        premiumoption = premiumoption + schoolloadingpremium + markupfee;
                        break;
                    }
                case 600000:
                    {
                        basepremium = rates["pi600klimitbasepremium"];
                        minmarkupfee = rates["pi600klimitminmarkupfee"];
                        if (feeincome >= 0 && feeincome <= 1000000)
                        {
                            premiumoption = rates["pi600klimitunder1milrate"] * feeincome;
                        }
                        else if (feeincome > 1000000 && feeincome <= 2000000)
                        {
                            premiumoption = (rates["pi600klimitunder1milrate"] * 1000000) + (rates["pi600klimit1milto2milrate"] * (feeincome - 1000000));
                        }
                        else if (feeincome > 2000000)
                        {
                            premiumoption = (rates["pi600klimitunder1milrate"] * 1000000) + (rates["pi600klimit1milto2milrate"] * 1000000) + rates["pi600klimitover2milrate"] * (feeincome - 2000000);
                        }
                        premiumoption = (premiumoption > basepremium) ? premiumoption : basepremium;
                        schoolloadingpremium = premiumoption * schoolloading / 100;
                        markupfee = ((premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 > minmarkupfee) ? (premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 : minmarkupfee;
                        markupfee = (markupfee < rates["pimaxmarkupfee"]) ? markupfee : rates["pimaxmarkupfee"];
                        premiumoption = premiumoption + schoolloadingpremium + markupfee;
                        break;
                    }
                case 750000:
                    {
                        basepremium = rates["pi750klimitbasepremium"];
                        minmarkupfee = rates["pi750klimitminmarkupfee"];
                        if (feeincome >= 0 && feeincome <= 1000000)
                        {
                            premiumoption = rates["pi750klimitunder1milrate"] * feeincome;
                        }
                        else if (feeincome > 1000000 && feeincome <= 2000000)
                        {
                            premiumoption = (rates["pi750klimitunder1milrate"] * 1000000) + (rates["pi750klimit1milto2milrate"] * (feeincome - 1000000));
                        }
                        else if (feeincome > 2000000)
                        {
                            premiumoption = (rates["pi750klimitunder1milrate"] * 1000000) + (rates["pi750klimit1milto2milrate"] * 1000000) + rates["pi750klimitover2milrate"] * (feeincome - 2000000);
                        }
                        premiumoption = (premiumoption > basepremium) ? premiumoption : basepremium;
                        schoolloadingpremium = premiumoption * schoolloading / 100;
                        markupfee = ((premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 > minmarkupfee) ? (premiumoption + schoolloadingpremium) * rates["pimarkupfeerate"] / 100 : minmarkupfee;
                        markupfee = (markupfee < rates["pimaxmarkupfee"]) ? markupfee : rates["pimaxmarkupfee"];
                        premiumoption = premiumoption + schoolloadingpremium + markupfee;
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Can not calculate premium for PI"));
                    }
            }

            return premiumoption;
        }



    }
}