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

            //IDictionary<string, decimal> rates = BuildRulesTable(agreement, "pl1millimitincomeunder1milpremium", "pl2millimitincomeunder1milpremium", "pl3millimitincomeunder1milpremium", "pl4millimitincomeunder1milpremium", "pl5millimitincomeunder1milpremium",
            //    "pl1millimitincome1milto3milpremium", "pl2millimitincome1milto3milpremium", "pl3millimitincome1milto3milpremium", "pl4millimitincome1milto3milpremium", "pl5millimitincome1milto3milpremium",
            //    "pl1millimitincome3milto5milpremium", "pl2millimitincome3milto5milpremium", "pl3millimitincome3milto5milpremium", "pl4millimitincome3milto5milpremium", "pl5millimitincome3milto5milpremium");

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
            }

            //Return terms based on the limit options

            TermExcess = 500;

            //TermPremium250k = GetPremiumFor(rates, feeincome, TermLimit250k);
            ClientAgreementTerm termsl250klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit250k, TermExcess);
            termsl250klimitoption.TermLimit = TermLimit250k;
            termsl250klimitoption.Premium = TermPremium250k;
            termsl250klimitoption.Excess = TermExcess;
            termsl250klimitoption.BrokerageRate = agreement.Brokerage;
            termsl250klimitoption.Brokerage = TermBrokerage250k;
            termsl250klimitoption.DateDeleted = null;
            termsl250klimitoption.DeletedBy = null;

            //TermPremium350k = GetPremiumFor(rates, feeincome, TermLimit350k);
            ClientAgreementTerm termsl350klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit350k, TermExcess);
            termsl350klimitoption.TermLimit = TermLimit350k;
            termsl350klimitoption.Premium = TermPremium350k;
            termsl350klimitoption.Excess = TermExcess;
            termsl350klimitoption.BrokerageRate = agreement.Brokerage;
            termsl350klimitoption.Brokerage = TermBrokerage350k;
            termsl350klimitoption.DateDeleted = null;
            termsl350klimitoption.DeletedBy = null;

            //TermPremium400k = GetPremiumFor(rates, feeincome, TermLimit400k);
            ClientAgreementTerm termsl400klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit400k, TermExcess);
            termsl400klimitoption.TermLimit = TermLimit400k;
            termsl400klimitoption.Premium = TermPremium400k;
            termsl400klimitoption.Excess = TermExcess;
            termsl400klimitoption.BrokerageRate = agreement.Brokerage;
            termsl400klimitoption.Brokerage = TermBrokerage400k;
            termsl400klimitoption.DateDeleted = null;
            termsl400klimitoption.DeletedBy = null;

            //TermPremium500k = GetPremiumFor(rates, feeincome, TermLimit500k);
            ClientAgreementTerm termsl500klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit500k, TermExcess);
            termsl500klimitoption.TermLimit = TermLimit500k;
            termsl500klimitoption.Premium = TermPremium500k;
            termsl500klimitoption.Excess = TermExcess;
            termsl500klimitoption.BrokerageRate = agreement.Brokerage;
            termsl500klimitoption.Brokerage = TermBrokerage500k;
            termsl500klimitoption.DateDeleted = null;
            termsl500klimitoption.DeletedBy = null;

            //TermPremium600k = GetPremiumFor(rates, feeincome, TermLimit600k);
            ClientAgreementTerm termsl600klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit600k, TermExcess);
            termsl600klimitoption.TermLimit = TermLimit600k;
            termsl600klimitoption.Premium = TermPremium600k;
            termsl600klimitoption.Excess = TermExcess;
            termsl600klimitoption.BrokerageRate = agreement.Brokerage;
            termsl600klimitoption.Brokerage = TermBrokerage600k;
            termsl600klimitoption.DateDeleted = null;
            termsl600klimitoption.DeletedBy = null;

            //TermPremium750k = GetPremiumFor(rates, feeincome, TermLimit750k);
            ClientAgreementTerm termsl750klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit750k, TermExcess);
            termsl750klimitoption.TermLimit = TermLimit750k;
            termsl750klimitoption.Premium = TermPremium750k;
            termsl750klimitoption.Excess = TermExcess;
            termsl750klimitoption.BrokerageRate = agreement.Brokerage;
            termsl750klimitoption.Brokerage = TermBrokerage750k;
            termsl750klimitoption.DateDeleted = null;
            termsl750klimitoption.DeletedBy = null;

            //TermPremium1mil = GetPremiumFor(rates, feeincome, TermLimit1mil);
            ClientAgreementTerm termsl1millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1mil, TermExcess);
            termsl1millimitoption.TermLimit = TermLimit1mil;
            termsl1millimitoption.Premium = TermPremium1mil;
            termsl1millimitoption.Excess = TermExcess;
            termsl1millimitoption.BrokerageRate = agreement.Brokerage;
            termsl1millimitoption.Brokerage = TermBrokerage1mil;
            termsl1millimitoption.DateDeleted = null;
            termsl1millimitoption.DeletedBy = null;

            //TermPremium1andhalfmil = GetPremiumFor(rates, feeincome, TermLimit1andhalfmil);
            ClientAgreementTerm termsl1andhalfmillimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1andhalfmil, TermExcess);
            termsl1andhalfmillimitoption.TermLimit = TermLimit1andhalfmil;
            termsl1andhalfmillimitoption.Premium = TermPremium1andhalfmil;
            termsl1andhalfmillimitoption.Excess = TermExcess;
            termsl1andhalfmillimitoption.BrokerageRate = agreement.Brokerage;
            termsl1andhalfmillimitoption.Brokerage = TermBrokerage1andhalfmil;
            termsl1andhalfmillimitoption.DateDeleted = null;
            termsl1andhalfmillimitoption.DeletedBy = null;

            //TermPremium2mil = GetPremiumFor(rates, feeincome, TermLimit2mil);
            ClientAgreementTerm termsl2millimitoption = GetAgreementTerm(underwritingUser, agreement, "PL", TermLimit2mil, TermExcess);
            termsl2millimitoption.TermLimit = TermLimit2mil;
            termsl2millimitoption.Premium = TermPremium2mil;
            termsl2millimitoption.Excess = TermExcess;
            termsl2millimitoption.BrokerageRate = agreement.Brokerage;
            termsl2millimitoption.Brokerage = TermBrokerage2mil;
            termsl2millimitoption.DateDeleted = null;
            termsl2millimitoption.DeletedBy = null;

            //TermPremium2andhalfmil = GetPremiumFor(rates, feeincome, TermLimit2andhalfmil);
            ClientAgreementTerm termsl2andhalfmillimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2andhalfmil, TermExcess);
            termsl2andhalfmillimitoption.TermLimit = TermLimit2andhalfmil;
            termsl2andhalfmillimitoption.Premium = TermPremium2andhalfmil;
            termsl2andhalfmillimitoption.Excess = TermExcess;
            termsl2andhalfmillimitoption.BrokerageRate = agreement.Brokerage;
            termsl2andhalfmillimitoption.Brokerage = TermBrokerage2andhalfmil;
            termsl2andhalfmillimitoption.DateDeleted = null;
            termsl2andhalfmillimitoption.DeletedBy = null;

            //TermPremium3mil = GetPremiumFor(rates, feeincome, TermLimit3mil);
            ClientAgreementTerm termsl3millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit3mil, TermExcess);
            termsl3millimitoption.TermLimit = TermLimit3mil;
            termsl3millimitoption.Premium = TermPremium3mil;
            termsl3millimitoption.Excess = TermExcess;
            termsl3millimitoption.BrokerageRate = agreement.Brokerage;
            termsl3millimitoption.Brokerage = TermBrokerage3mil;
            termsl3millimitoption.DateDeleted = null;
            termsl3millimitoption.DeletedBy = null;

            //TermPremium4mil = GetPremiumFor(rates, feeincome, TermLimit4mil);
            ClientAgreementTerm termsl4millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit4mil, TermExcess);
            termsl4millimitoption.TermLimit = TermLimit4mil;
            termsl4millimitoption.Premium = TermPremium4mil;
            termsl4millimitoption.Excess = TermExcess;
            termsl4millimitoption.BrokerageRate = agreement.Brokerage;
            termsl4millimitoption.Brokerage = TermBrokerage4mil;
            termsl4millimitoption.DateDeleted = null;
            termsl4millimitoption.DeletedBy = null;

            //TermPremium5mil = GetPremiumFor(rates, feeincome, TermLimit5mil);
            ClientAgreementTerm termsl5millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit5mil, TermExcess);
            termsl5millimitoption.TermLimit = TermLimit5mil;
            termsl5millimitoption.Premium = TermPremium5mil;
            termsl5millimitoption.Excess = TermExcess;
            termsl5millimitoption.BrokerageRate = agreement.Brokerage;
            termsl5millimitoption.Brokerage = TermBrokerage5mil;
            termsl5millimitoption.DateDeleted = null;
            termsl5millimitoption.DeletedBy = null;

            //TermPremium6mil = GetPremiumFor(rates, feeincome, TermLimit6mil);
            ClientAgreementTerm termsl6millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit6mil, TermExcess);
            termsl6millimitoption.TermLimit = TermLimit6mil;
            termsl6millimitoption.Premium = TermPremium6mil;
            termsl6millimitoption.Excess = TermExcess;
            termsl6millimitoption.BrokerageRate = agreement.Brokerage;
            termsl6millimitoption.Brokerage = TermBrokerage6mil;
            termsl6millimitoption.DateDeleted = null;
            termsl6millimitoption.DeletedBy = null;

            //TermPremium8mil = GetPremiumFor(rates, feeincome, TermLimit8mil);
            ClientAgreementTerm termsl8millimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit8mil, TermExcess);
            termsl8millimitoption.TermLimit = TermLimit8mil;
            termsl8millimitoption.Premium = TermPremium8mil;
            termsl8millimitoption.Excess = TermExcess;
            termsl8millimitoption.BrokerageRate = agreement.Brokerage;
            termsl8millimitoption.Brokerage = TermBrokerage8mil;
            termsl8millimitoption.DateDeleted = null;
            termsl8millimitoption.DeletedBy = null;

            //TermPremium10mil = GetPremiumFor(rates, feeincome, TermLimit10mil);
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

        decimal GetPremiumFor(IDictionary<string, decimal> rates, decimal feeincome, int limitoption)
        {
            decimal premiumoption = 0M;

            switch (limitoption)
            {
                case 1000000:
                    {
                        if (feeincome >= 0 && feeincome < 1000000)
                        {
                            premiumoption = rates["pl1millimitincomeunder1milpremium"];
                        }
                        else if (feeincome >= 1000000 && feeincome < 3000000)
                        {
                            premiumoption = rates["pl1millimitincome1milto3milpremium"];
                        }
                        else if (feeincome >= 3000000 && feeincome < 5000000)
                        {
                            premiumoption = rates["pl1millimitincome3milto5milpremium"];
                        }
                        break;
                    }
                case 2000000:
                    {
                        if (feeincome >= 0 && feeincome < 1000000)
                        {
                            premiumoption = rates["pl2millimitincomeunder1milpremium"];
                        }
                        else if (feeincome >= 1000000 && feeincome < 3000000)
                        {
                            premiumoption = rates["pl2millimitincome1milto3milpremium"];
                        }
                        else if (feeincome >= 3000000 && feeincome < 5000000)
                        {
                            premiumoption = rates["pl2millimitincome3milto5milpremium"];
                        }
                        break;
                    }
                case 3000000:
                    {
                        if (feeincome >= 0 && feeincome < 1000000)
                        {
                            premiumoption = rates["pl3millimitincomeunder1milpremium"];
                        }
                        else if (feeincome >= 1000000 && feeincome < 3000000)
                        {
                            premiumoption = rates["pl3millimitincome1milto3milpremium"];
                        }
                        else if (feeincome >= 3000000 && feeincome < 5000000)
                        {
                            premiumoption = rates["pl3millimitincome3milto5milpremium"];
                        }
                        break;
                    }
                case 4000000:
                    {
                        if (feeincome >= 0 && feeincome < 1000000)
                        {
                            premiumoption = rates["pl4millimitincomeunder1milpremium"];
                        }
                        else if (feeincome >= 1000000 && feeincome < 3000000)
                        {
                            premiumoption = rates["pl4millimitincome1milto3milpremium"];
                        }
                        else if (feeincome >= 3000000 && feeincome < 5000000)
                        {
                            premiumoption = rates["pl4millimitincome3milto5milpremium"];
                        }
                        break;
                    }
                case 5000000:
                    {
                        if (feeincome >= 0 && feeincome < 1000000)
                        {
                            premiumoption = rates["pl5millimitincomeunder1milpremium"];
                        }
                        else if (feeincome >= 1000000 && feeincome < 3000000)
                        {
                            premiumoption = rates["pl5millimitincome1milto3milpremium"];
                        }
                        else if (feeincome >= 3000000 && feeincome < 5000000)
                        {
                            premiumoption = rates["pl5millimitincome3milto5milpremium"];
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Can not calculate premium for PL"));
                    }
            }

            return premiumoption;
        }



    }
}