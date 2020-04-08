using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class PMINZPIUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public PMINZPIUWModule()
        {
            Name = "PMINZ_PI";
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

            //IDictionary<string, decimal> rates = BuildRulesTable(agreement, "piexcessrate", "piminexcess", "pimaxexcess", "piexcessdiscountrate4kto5k", "piexcessdiscountrate5kto6k",
            //                "piexcessdiscountrate6kto7k", "piexcessdiscountrate7kto8k", "piexcessdiscountrate8kto9k", "piexcessdiscountrate9kto10k", "pischoolactivityloadingunder5percentage",
            //                "pischoolactivityloading5to20percentage", "pischoolactivityloading20to65percentage", "pischoolactivityloading65to100percentage", "pimarkupfeerate",
            //                "pimaxmarkupfee", "pi250klimitbasepremium", "pi350klimitbasepremium", "pi400klimitbasepremium", "pi500klimitbasepremium", "pi600klimitbasepremium", "pi750klimitbasepremium",
            //                "pi1millimitbasepremium", "pi1andhalfmillimitbasepremium", "pi2millimitbasepremium", "pi2andhalfmillimitbasepremium", "pi3millimitbasepremium", "pi4millimitbasepremium",
            //                "pi5millimitbasepremium", "pi6millimitbasepremium", "pi8millimitbasepremium", "pi10millimitbasepremium", "pi250klimitminmarkupfee", "pi350klimitminmarkupfee",
            //                "pi400klimitminmarkupfee", "pi500klimitminmarkupfee", "pi600klimitminmarkupfee", "pi750klimitminmarkupfee", "pi1millimitminmarkupfee", "pi1andhalfmillimitminmarkupfee",
            //                "pi2millimitminmarkupfee", "pi2andhalfmillimitminmarkupfee", "pi3millimitminmarkupfee", "pi4millimitminmarkupfee", "pi5millimitminmarkupfee", "pi6millimitminmarkupfee",
            //                "pi8millimitminmarkupfee", "pi10millimitminmarkupfee",
            //                "pi250klimitunder1milrate", "pi250klimit1milto2milrate", "pi250klimitover2milrate", "pi350klimitunder1milrate", "pi350klimit1milto2milrate", "pi350klimitover2milrate",
            //                "pi400klimitunder1milrate", "pi400klimit1milto2milrate", "pi400klimitover2milrate", "pi500klimitunder1milrate", "pi500klimit1milto2milrate", "pi500klimitover2milrate",
            //                "pi600klimitunder1milrate", "pi600klimit1milto2milrate", "pi600klimitover2milrate", "pi750klimitunder1milrate", "pi750klimit1milto2milrate", "pi750klimitover2milrate",
            //                "pi1millimitunder1milrate", "pi1millimit1milto2milrate", "pi1millimitover2milrate", "pi1andhalfmillimitunder1milrate", "pi1andhalfmillimit1milto2milrate", "pi1andhalfmillimitover2milrate",
            //                "pi2millimitunder1milrate", "pi2millimit1milto2milrate", "pi2millimitover2milrate", "pi2andhalfmillimitunder1milrate", "pi2andhalfmillimit1milto2milrate", "pi2andhalfmillimitover2milrate",
            //                "pi3millimitunder1milrate", "pi3millimit1milto2milrate", "pi3millimitover2milrate", "pi4millimitunder1milrate", "pi4millimit1milto2milrate", "pi4millimitover2milrate",
            //                "pi5millimitunder1milrate", "pi5millimit1milto2milrate", "pi5millimitover2milrate", "pi6millimitunder1milrate", "pi6millimit1milto2milrate", "pi6millimitover2milrate",
            //                "pi8millimitunder1milrate", "pi8millimit1milto2milrate", "pi8millimitover2milrate", "pi10millimitunder1milrate", "pi10millimit1milto2milrate", "pi10millimitover2milrate",
            //                "anyadnzmemberrate");

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

            int TermExcess = 2000;
            decimal feeincome = 0;

            int intordnumber = 0;
            int intcapmnumber = 0;
            int intpmpnumber = 0;
            int intpdnumber = 0;

            decimal decBDSP = 0M;
            decimal decCon = 0M;
            decimal decDM = 0M;
            decimal decFASA = 0M;
            decimal decIT = 0M;
            decimal decMOP = 0M;
            decimal decPMTC = 0M;
            decimal decRCIM = 0M;
            decimal decTM = 0M;
            decimal decOPMA = 0M;
            decimal decNPMA = 0M;
            decimal decOther = 0M;
            decimal decSumActivity = 0M;
            decimal decPRBDSP = 0M;
            decimal decPRCon = 0M;
            decimal decPRFASA = 0M;
            decimal decPRIT = 0M;
            decimal decPRMOP = 0M;
            decimal decPROther = 0M;

            decimal decPIBasePremium = 0M;

            if (agreement.ClientInformationSheet.Organisation.Count > 0)
            {
                foreach (var uisorg in agreement.ClientInformationSheet.Organisation)
                {
                    if (uisorg.CertType == "Ordinary")
                    {
                        intordnumber += 1;
                    } 
                    else if (uisorg.CertType == "PMP")
                    {
                        intpmpnumber += 1;
                    }
                    else if (uisorg.CertType == "CAPM")
                    {
                        intcapmnumber += 1;
                    }
                    else if (uisorg.CertType == "ProjectDirector")
                    {
                        intpdnumber += 1;
                    }
                }
            }


            if (agreement.ClientInformationSheet.RevenueData != null)
            {
                foreach (var uISTerritory in agreement.ClientInformationSheet.RevenueData.Territories)
                {
                    if (uISTerritory.Location == "New Zealand") //New Zealand income only
                    {
                        feeincome = Convert.ToDecimal(agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "totalRevenue").First().Value) * uISTerritory.Pecentage / 100;
                    }
                }

                foreach (var uISActivity in agreement.ClientInformationSheet.RevenueData.Activities)
                {
                    if (uISActivity.AnzsciCode == "M696210") //Business Development & Strategic Planning
                    {
                        decBDSP = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "E") //Construction
                    {
                        decCon = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696230") //Design Management
                    {
                        decDM = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696240") //Financial and Accounting Systems Analysis
                    {
                        decFASA = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696250") //Information Technology
                    {
                        decIT = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696260") //Manufacturing & Operational Processes
                    {
                        decMOP = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696270") //Project Management Teaching and Coaching
                    {
                        decPMTC = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696300") //Resource Consent and Implementation Management
                    {
                        decRCIM = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696400") //Telecommunications Management
                    {
                        decTM = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696500") //Other Project Management Activities
                    {
                        decOPMA = uISActivity.Pecentage;
                    }
                    else if (uISActivity.AnzsciCode == "M696600") //Non Project Management Activities
                    {
                        decNPMA = uISActivity.Pecentage;
                    }

                }

                decOther = decPMTC + decOPMA + decDM + decTM + decRCIM;
                decSumActivity = decBDSP + decCon + decIT + decMOP + decNPMA + decOther;

                decPRBDSP = decBDSP / (decSumActivity - decNPMA) * decSumActivity;
                decPRCon = decCon / (decSumActivity - decNPMA) * decSumActivity;
                decPRFASA = decFASA / (decSumActivity - decNPMA) * decSumActivity;
                decPRIT = decIT / (decSumActivity - decNPMA) * decSumActivity;
                decPRMOP = decMOP / (decSumActivity - decNPMA) * decSumActivity;
                decPROther = decOther / (decSumActivity - decNPMA) * decSumActivity;

            }

            //decPIBasePremium = GetPIBasePremiumFor(rates, feeincome, decPRBDSP, decPRCon, decPRFASA, decPRIT, decPRMOP, decPROther);



            ClientAgreementTerm termsl250klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit250k, TermExcess);
            termsl250klimitoption.TermLimit = TermLimit250k;
            termsl250klimitoption.Premium = TermPremium250k;
            termsl250klimitoption.Excess = TermExcess;
            termsl250klimitoption.BrokerageRate = agreement.Brokerage;
            termsl250klimitoption.Brokerage = TermBrokerage250k;
            termsl250klimitoption.DateDeleted = null;
            termsl250klimitoption.DeletedBy = null;


            ////Referral points per agreement


            //Update agreement status
            if (agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null && cref.Status == "Pending").Count() > 0)
            {
                agreement.Status = "Referred";
            }
            else
            {
                agreement.Status = "Quoted";
            }


            string auditLogDetail = "PMINZ PI UW created/modified";
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


        decimal GetPIBasePremiumFor(IDictionary<string, decimal> rates, decimal feeincome, decimal decPRBDSP, decimal decPRCon, decimal decPRFASA, decimal decPRIT, decimal decPRMOP, decimal decPROther)
        {
            decimal pibasepremium = 0M;
            decimal basepremiumOrd = 0M;
            decimal basepremiumOrdCAPM = 0M;
            decimal basepremiumOrdPMP = 0M;
            decimal basepremiumOrdPD = 0M;
            decimal minpremiumOrd = 0M;
            decimal minpremiumOrdCAPM = 0M;
            decimal minpremiumOrdPMP = 0M;
            decimal minpremiumOrdPD = 0M;

            basepremiumOrd = (feeincome * decPRIT / 100 * rates["piitcomponentrateord"] / 100) + (feeincome * decPRCon / 100 * rates["piconstructioncomponentrateord"] / 100) +
                            (feeincome * decPRBDSP / 100 * rates["pibusinessdevpmtcomponentrateord"] / 100) + (feeincome * decPRMOP / 100 * rates["pimanufacturingcomponentrateord"] / 100) +
                            (feeincome * decPRFASA / 100 * rates["pifinancialcomponentrateord"] / 100) + (feeincome * decPROther / 100 * rates["piothercomponentrateord"] / 100);
            minpremiumOrd = (decPRIT / 100 * rates["piitcomponentminpremiumord"]) + (decPRCon / 100 * rates["piconstructioncomponentminpremiumord"] / 100) +
                            (decPRBDSP / 100 * rates["pibusinessdevpmtcomponentminpremiumord"] / 100) + (decPRMOP / 100 * rates["pimanufacturingcomponentminpremiumord"] / 100) +
                            (decPRFASA / 100 * rates["pifinancialcomponentminpremiumord"] / 100) + (decPROther / 100 * rates["piothercomponentminpremiumord"] / 100);
            basepremiumOrd = (basepremiumOrd > minpremiumOrd) ? basepremiumOrd : minpremiumOrd;






            return pibasepremium;
        }



    }
}
