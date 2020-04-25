using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class DANZPIUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public DANZPIUWModule()
        {
            Name = "DANZ_PI";
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

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "piBSSTopUpPremium", "piIRTopUpPremium", "piQSTopUpPremium", "piSETopUpPremium",
                "piTPTopUpPremium", "piValTopUpPremium", "piUPTopUpPremium", "piEPTopUpPremium", "piRMTopUpPremium",
                "piProjMTopUpPremium", "pitermexcess5000discount", "pitermexcess10kdiscount", 
                "pl500klimitincomeunder100kpremium", "pl500klimitincome100kto200kpremium", "pl500klimitincome200kto500kpremium",
                "pl1millimitincomeunder100kpremium", "pl1millimitincome100kto200kpremium", "pl1millimitincome200kto500kpremium", 
                "pl2millimitincomeunder100kpremium", "pl2millimitincome100kto200kpremium", "pl2millimitincome200kto500kpremium");

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

            decimal feeincome = 0;

            decimal decBSS= 0M;
            decimal decArchRSD = 0M;
            decimal decArchRMD = 0M;
            decimal decArchCB = 0M;
            decimal decArchI = 0M;
            decimal decIR = 0M;
            decimal decQS = 0M;
            decimal decSE = 0M;
            decimal decTP = 0M;
            decimal decVal = 0M;
            decimal decUP = 0M;
            decimal decProjM = 0M;
            decimal decEP = 0M;
            decimal decRM = 0M;
            decimal decOther = 0M;

            decimal decBSSTopUpPre = 0M;
            decimal decIRTopUpPre = 0M;
            decimal decQSTopUpPre = 0M;
            decimal decSETopUpPre = 0M;
            decimal decTPTopUpPre = 0M;
            decimal decValTopUpPre = 0M;
            decimal decUPTopUpPre = 0M;
            decimal decProjMTopUpPre = 0M;
            decimal decEPTopUpPre = 0M;
            decimal decRMTopUpPre = 0M;

            decimal decPIPremiumTopUp = 0M;
            decimal totalfeeincome = 0M;
            int numberoffeeincome = 1;
            bool bolworkoutsidenz = false;
            

            if (agreement.ClientInformationSheet.Organisation.Count > 0)
            {
                foreach (var uisorg in agreement.ClientInformationSheet.Organisation)
                {

                }

                
            }


            if (agreement.ClientInformationSheet.RevenueData != null)
            {
                totalfeeincome = agreement.ClientInformationSheet.RevenueData.LastFinancialYearTotal;

                if (agreement.ClientInformationSheet.RevenueData.CurrentYearTotal > 0 && 
                    (agreement.ClientInformationSheet.RevenueData.CurrentYearTotal > agreement.ClientInformationSheet.RevenueData.LastFinancialYearTotal) )
                {
                    totalfeeincome += agreement.ClientInformationSheet.RevenueData.CurrentYearTotal;
                    numberoffeeincome += 1;
                }
                
                feeincome = totalfeeincome / numberoffeeincome;

                foreach (var uISTerritory in agreement.ClientInformationSheet.RevenueData.Territories)
                {
                    if (!bolworkoutsidenz && uISTerritory.Location != "New Zealand" && uISTerritory.Percentage > 0) //Work outside New Zealand Check
                    {
                        bolworkoutsidenz = true;
                    }
                }

                foreach (var uISActivity in agreement.ClientInformationSheet.RevenueData.Activities)
                {
                    if (uISActivity.AnzsciCode == "E322") //Building Structure Services
                    {
                        decBSS = uISActivity.Percentage;
                        decBSSTopUpPre = rates["piBSSTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692121") //Architecture - residential single dwellings
                    {
                        decArchRSD = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "M692122") //Architecture - residential multi dwellings
                    {
                        decArchRMD = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "M692123") //Architecture - commercial building
                    {
                        decArchCB = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "M692124") //Architecture - interior
                    {
                        decArchI = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "M692160") //Inspection Reports 
                    {
                        decIR = uISActivity.Percentage;
                        decIRTopUpPre = rates["piIRTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692170") //Quantity Surveying 
                    {
                        decQS = uISActivity.Percentage;
                        decQSTopUpPre = rates["piQSTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692180") //Structural Engineering 
                    {
                        decSE = uISActivity.Percentage;
                        decSETopUpPre = rates["piSETopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692190") //Town Planning 
                    {
                        decTP = uISActivity.Percentage;
                        decTPTopUpPre = rates["piTPTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692210") //Valuations 
                    {
                        decVal = uISActivity.Percentage;
                        decValTopUpPre = rates["piValTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692211") //Urban planning 
                    {
                        decUP = uISActivity.Percentage;
                        decUPTopUpPre = rates["piUPTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692212") //Environmental planning 
                    {
                        decEP = uISActivity.Percentage;
                        decEPTopUpPre = rates["piEPTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692213") //Resource management 
                    {
                        decRM = uISActivity.Percentage;
                        decRMTopUpPre = rates["piRMTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "M692214") //Project management 
                    {
                        decProjM = uISActivity.Percentage;
                        decProjMTopUpPre = rates["piProjMTopUpPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "S") //Other Services
                    {
                        decOther = uISActivity.Percentage;
                    }

                }

                decPIPremiumTopUp = Math.Max(decBSSTopUpPre, Math.Max(decIRTopUpPre, Math.Max(decQSTopUpPre, Math.Max(decSETopUpPre, Math.Max(decTPTopUpPre, Math.Max(decValTopUpPre, Math.Max(decUPTopUpPre, Math.Max(decProjMTopUpPre, Math.Max(decEPTopUpPre, decRMTopUpPre)))))))));
            }

            //ClientAgreementEndorsement cAEConstruction = agreement.ClientAgreementEndorsements.FirstOrDefault(cae => cae.Name == "Project Managers (Construction)");
            //ClientAgreementEndorsement cAENonConstruction = agreement.ClientAgreementEndorsements.FirstOrDefault(cae => cae.Name == "Project Managers (Non-Construction)");

            //if (cAEConstruction != null)
            //{
            //    cAEConstruction.DateDeleted = DateTime.UtcNow;
            //    cAEConstruction.DeletedBy = underwritingUser;
            //}
            //if (cAENonConstruction != null)
            //{
            //    cAENonConstruction.DateDeleted = DateTime.UtcNow;
            //    cAENonConstruction.DeletedBy = underwritingUser;
            //}
            //if (decCon > 0)
            //{
            //    if (cAEConstruction != null)
            //    {
            //        cAEConstruction.DateDeleted = null;
            //        cAEConstruction.DeletedBy = null;
            //    }
            //}
            //else
            //{
            //    if (cAENonConstruction != null)
            //    {
            //        cAENonConstruction.DateDeleted = null;
            //        cAENonConstruction.DeletedBy = null;
            //    }
            //}


            string strretrodate = "";

            if (agreement.ClientInformationSheet.PreRenewOrRefDatas.Count() > 0)
            {
                foreach (var preRenewOrRefData in agreement.ClientInformationSheet.PreRenewOrRefDatas)
                {
                    if (preRenewOrRefData.DataType == "preterm")
                    {
                        if (!string.IsNullOrEmpty(preRenewOrRefData.PIRetro))
                        {
                            strretrodate = preRenewOrRefData.PIRetro;
                        }

                    }
                    if (preRenewOrRefData.DataType == "preendorsement" && preRenewOrRefData.EndorsementProduct == "PI")
                    {
                        if (agreement.ClientAgreementEndorsements.FirstOrDefault(cae => cae.Name == preRenewOrRefData.EndorsementTitle) == null)
                        {
                            ClientAgreementEndorsement clientAgreementEndorsement = new ClientAgreementEndorsement(underwritingUser, preRenewOrRefData.EndorsementTitle, "Exclusion", product, preRenewOrRefData.EndorsementText, 130, agreement);
                            agreement.ClientAgreementEndorsements.Add(clientAgreementEndorsement);
                        }
                    }
                }
            }

            int TermLimit500k = 500000;
            int TermLimit1mil = 1000000;
            int TermLimit2mil = 2000000;

            int TermExcess2500 = 2500;
            int TermExcess5000 = 5000;
            decimal TermExcess5000Discount = rates["pitermexcess5000discount"];
            int TermExcess10k = 10000;
            decimal TermExcess10kDiscount = rates["pitermexcess10kdiscount"];

            decimal TermPremium500k2500Excess = 0m;
            decimal TermPremium500k5000Excess = 0m;
            decimal TermPremium500k10kExcess = 0m;
            decimal TermBrokerage500k2500Excess = 0m;
            decimal TermBrokerage500k5000Excess = 0m;
            decimal TermBrokerage500k10kExcess = 0m;

            TermPremium500k2500Excess = GetPremiumFor(rates, feeincome, TermLimit500k);
            TermPremium500k5000Excess = TermPremium500k2500Excess - TermExcess5000Discount;
            TermPremium500k10kExcess = TermPremium500k2500Excess - TermExcess10kDiscount;
            TermBrokerage500k2500Excess = TermPremium500k2500Excess * agreement.Brokerage / 100;
            TermBrokerage500k5000Excess = TermPremium500k5000Excess * agreement.Brokerage / 100;
            TermBrokerage500k10kExcess = TermPremium500k10kExcess * agreement.Brokerage / 100;

            ClientAgreementTerm term500klimit2500excessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit500k, TermExcess2500);
            term500klimit2500excessoption.TermLimit = TermLimit500k;
            term500klimit2500excessoption.Premium = TermPremium500k2500Excess;
            term500klimit2500excessoption.Excess = TermExcess2500;
            term500klimit2500excessoption.BrokerageRate = agreement.Brokerage;
            term500klimit2500excessoption.Brokerage = TermBrokerage500k2500Excess;
            term500klimit2500excessoption.DateDeleted = null;
            term500klimit2500excessoption.DeletedBy = null;

            ClientAgreementTerm term500klimit5000excessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit500k, TermExcess5000);
            term500klimit5000excessoption.TermLimit = TermLimit500k;
            term500klimit5000excessoption.Premium = TermPremium500k5000Excess;
            term500klimit5000excessoption.Excess = TermExcess5000;
            term500klimit5000excessoption.BrokerageRate = agreement.Brokerage;
            term500klimit5000excessoption.Brokerage = TermBrokerage500k5000Excess;
            term500klimit5000excessoption.DateDeleted = null;
            term500klimit5000excessoption.DeletedBy = null;

            ClientAgreementTerm term500klimit10kexcessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit500k, TermExcess10k);
            term500klimit10kexcessoption.TermLimit = TermLimit500k;
            term500klimit10kexcessoption.Premium = TermPremium500k10kExcess;
            term500klimit10kexcessoption.Excess = TermExcess10k;
            term500klimit10kexcessoption.BrokerageRate = agreement.Brokerage;
            term500klimit10kexcessoption.Brokerage = TermBrokerage500k10kExcess;
            term500klimit10kexcessoption.DateDeleted = null;
            term500klimit10kexcessoption.DeletedBy = null;

            decimal TermPremium1mil2500Excess = 0m;
            decimal TermPremium1mil5000Excess = 0m;
            decimal TermPremium1mil10kExcess = 0m;
            decimal TermBrokerage1mil2500Excess = 0m;
            decimal TermBrokerage1mil5000Excess = 0m;
            decimal TermBrokerage1mil10kExcess = 0m;

            TermPremium1mil2500Excess = GetPremiumFor(rates, feeincome, TermLimit1mil);
            TermPremium1mil5000Excess = TermPremium1mil2500Excess - TermExcess5000Discount;
            TermPremium1mil10kExcess = TermPremium1mil2500Excess - TermExcess10kDiscount;
            TermBrokerage1mil2500Excess = TermPremium1mil2500Excess * agreement.Brokerage / 100;
            TermBrokerage1mil5000Excess = TermPremium1mil5000Excess * agreement.Brokerage / 100;
            TermBrokerage1mil10kExcess = TermPremium1mil10kExcess * agreement.Brokerage / 100;

            ClientAgreementTerm term1millimit2500excessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1mil, TermExcess2500);
            term1millimit2500excessoption.TermLimit = TermLimit1mil;
            term1millimit2500excessoption.Premium = TermPremium1mil2500Excess;
            term1millimit2500excessoption.Excess = TermExcess2500;
            term1millimit2500excessoption.BrokerageRate = agreement.Brokerage;
            term1millimit2500excessoption.Brokerage = TermBrokerage1mil2500Excess;
            term1millimit2500excessoption.DateDeleted = null;
            term1millimit2500excessoption.DeletedBy = null;

            ClientAgreementTerm term1millimit5000excessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1mil, TermExcess5000);
            term1millimit5000excessoption.TermLimit = TermLimit1mil;
            term1millimit5000excessoption.Premium = TermPremium1mil5000Excess;
            term1millimit5000excessoption.Excess = TermExcess5000;
            term1millimit5000excessoption.BrokerageRate = agreement.Brokerage;
            term1millimit5000excessoption.Brokerage = TermBrokerage1mil5000Excess;
            term1millimit5000excessoption.DateDeleted = null;
            term1millimit5000excessoption.DeletedBy = null;

            ClientAgreementTerm term1millimit10kexcessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1mil, TermExcess10k);
            term1millimit10kexcessoption.TermLimit = TermLimit1mil;
            term1millimit10kexcessoption.Premium = TermPremium1mil10kExcess;
            term1millimit10kexcessoption.Excess = TermExcess10k;
            term1millimit10kexcessoption.BrokerageRate = agreement.Brokerage;
            term1millimit10kexcessoption.Brokerage = TermBrokerage1mil10kExcess;
            term1millimit10kexcessoption.DateDeleted = null;
            term1millimit10kexcessoption.DeletedBy = null;

            decimal TermPremium2mil2500Excess = 0m;
            decimal TermPremium2mil5000Excess = 0m;
            decimal TermPremium2mil10kExcess = 0m;
            decimal TermBrokerage2mil2500Excess = 0m;
            decimal TermBrokerage2mil5000Excess = 0m;
            decimal TermBrokerage2mil10kExcess = 0m;

            TermPremium2mil2500Excess = GetPremiumFor(rates, feeincome, TermLimit2mil);
            TermPremium2mil5000Excess = TermPremium2mil2500Excess - TermExcess5000Discount;
            TermPremium2mil10kExcess = TermPremium2mil2500Excess - TermExcess10kDiscount;
            TermBrokerage2mil2500Excess = TermPremium2mil2500Excess * agreement.Brokerage / 100;
            TermBrokerage2mil5000Excess = TermPremium2mil5000Excess * agreement.Brokerage / 100;
            TermBrokerage2mil10kExcess = TermPremium2mil10kExcess * agreement.Brokerage / 100;

            ClientAgreementTerm term2millimit2500excessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2mil, TermExcess2500);
            term2millimit2500excessoption.TermLimit = TermLimit2mil;
            term2millimit2500excessoption.Premium = TermPremium2mil2500Excess;
            term2millimit2500excessoption.Excess = TermExcess2500;
            term2millimit2500excessoption.BrokerageRate = agreement.Brokerage;
            term2millimit2500excessoption.Brokerage = TermBrokerage2mil2500Excess;
            term2millimit2500excessoption.DateDeleted = null;
            term2millimit2500excessoption.DeletedBy = null;

            ClientAgreementTerm term2millimit5000excessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2mil, TermExcess5000);
            term2millimit5000excessoption.TermLimit = TermLimit2mil;
            term2millimit5000excessoption.Premium = TermPremium2mil5000Excess;
            term2millimit5000excessoption.Excess = TermExcess5000;
            term2millimit5000excessoption.BrokerageRate = agreement.Brokerage;
            term2millimit5000excessoption.Brokerage = TermBrokerage2mil5000Excess;
            term2millimit5000excessoption.DateDeleted = null;
            term2millimit5000excessoption.DeletedBy = null;

            ClientAgreementTerm term2millimit10kexcessoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2mil, TermExcess10k);
            term2millimit10kexcessoption.TermLimit = TermLimit2mil;
            term2millimit10kexcessoption.Premium = TermPremium2mil10kExcess;
            term2millimit10kexcessoption.Excess = TermExcess10k;
            term2millimit10kexcessoption.BrokerageRate = agreement.Brokerage;
            term2millimit10kexcessoption.Brokerage = TermBrokerage2mil10kExcess;
            term2millimit10kexcessoption.DateDeleted = null;
            term2millimit10kexcessoption.DeletedBy = null;


            ////Referral points per agreement
            ////Number of Personnel
            //uwrfnumberofpersonnel(underwritingUser, agreement, totalnumberofpersonnel, rates);
            ////Non PMINZ Members
            //uwrfnonpminzmembers(underwritingUser, agreement, bolnonpmimember);
            ////Other or Non PM Activities
            //uwrfotherornonpmactivities(underwritingUser, agreement, decOPMA, decNPMA);
            ////Operates Outside of NZ
            //uwrfoperatesoutsideofnz(underwritingUser, agreement, bolworkoutsidenz);
            ////High Fee Income
            //uwrfhighfeeincome(underwritingUser, agreement, feeincome, rates);
            ////Contracting Services
            //uwrfcontractingservices(underwritingUser, agreement);
            ////Claims / Insurance History
            //uwrfpriorinsurance(underwritingUser, agreement);
            ////No Projects Managed
            //uwrfnoprojectsmanaged(underwritingUser, agreement);
            ////Renewal Premiums Lower than Expiring
            //uwrfrenewalpremiumslowerthanexpiring(underwritingUser, agreement, bolrenewalpremiumslowerthanexpiring);
            ////Renewal Premiums Higher than Expiring
            //uwrfrenewalpremiumshigherthanexpiring(underwritingUser, agreement, bolrenewalpremiumshigherthanexpiring);
            ////Capacity of an Engineer to Contract
            //uwrfcapacityofanengineertocontract(underwritingUser, agreement);
            ////Construction revenue as role as Engineer to the Contract
            //uwrfconstructionrevenue(underwritingUser, agreement, constructionEngineerDetails);
            ////Component Specification Activities
            //uwrfcomponentspecificationactivities(underwritingUser, agreement);

            //Update agreement status
            if (agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null && cref.Status == "Pending").Count() > 0)
            {
                agreement.Status = "Referred";
            }
            else
            {
                agreement.Status = "Quoted";
            }

            string retrodate = "Inception";
            agreement.TerritoryLimit = "Worldwide excluding USA/Canada";
            agreement.Jurisdiction = "Worldwide excluding USA/Canada";
            agreement.RetroactiveDate = retrodate;
            if (!String.IsNullOrEmpty(strretrodate))
            {
                agreement.RetroactiveDate = strretrodate;
            }

            string auditLogDetail = "DANZ PI UW created/modified";
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
                case 500000:
                    {
                        if (feeincome >= 0 && feeincome <= 100000)
                        {
                            premiumoption = rates["pl500klimitincomeunder100kpremium"];
                        }
                        else if (feeincome > 100000 && feeincome <= 200000)
                        {
                            premiumoption = rates["pl500klimitincome100kto200kpremium"];
                        }
                        else if (feeincome > 200000 && feeincome <= 500000)
                        {
                            premiumoption = rates["pl500klimitincome200kto500kpremium"];
                        }
                        break;
                    }
                case 1000000:
                    {
                        if (feeincome >= 0 && feeincome <= 100000)
                        {
                            premiumoption = rates["pl1millimitincomeunder100kpremium"];
                        }
                        else if (feeincome > 100000 && feeincome <= 200000)
                        {
                            premiumoption = rates["pl1millimitincome100kto200kpremium"];
                        }
                        else if (feeincome > 200000 && feeincome <= 500000)
                        {
                            premiumoption = rates["pl1millimitincome200kto500kpremium"];
                        }
                        break;
                    }
                case 2000000:
                    {
                        if (feeincome >= 0 && feeincome <= 100000)
                        {
                            premiumoption = rates["pl2millimitincomeunder100kpremium"];
                        }
                        else if (feeincome > 100000 && feeincome <= 200000)
                        {
                            premiumoption = rates["pl2millimitincome100kto200kpremium"];
                        }
                        else if (feeincome > 200000 && feeincome <= 500000)
                        {
                            premiumoption = rates["pl2millimitincome200kto500kpremium"];
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Can not calculate premium for PI"));
                    }
            }

            return premiumoption;
        }


        void uwrfnumberofpersonnel(User underwritingUser, ClientAgreement agreement, int totalnumberofpersonnel, IDictionary<string, decimal> rates)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnumberofpersonnel" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnumberofpersonnel") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnumberofpersonnel").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnumberofpersonnel").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnumberofpersonnel").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnumberofpersonnel").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnumberofpersonnel" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (totalnumberofpersonnel > rates["maximumnumberofpersonnel"])
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnumberofpersonnel" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfnonpminzmembers(User underwritingUser, ClientAgreement agreement, bool bolnonpmimember)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnonpminzmembers" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnonpminzmembers") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnonpminzmembers").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnonpminzmembers").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnonpminzmembers").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnonpminzmembers" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (!bolnonpmimember)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnonpminzmembers" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfotherornonpmactivities(User underwritingUser, ClientAgreement agreement, decimal decOPMA, decimal decNPMA)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfotherornonpmactivities" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfotherornonpmactivities") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfotherornonpmactivities").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfotherornonpmactivities").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfotherornonpmactivities").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfotherornonpmactivities").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfotherornonpmactivities" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (decOPMA > 0 || decNPMA > 0)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfotherornonpmactivities" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfoperatesoutsideofnz(User underwritingUser, ClientAgreement agreement, bool bolworkoutsidenz)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfoperatesoutsideofnz" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfoperatesoutsideofnz" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolworkoutsidenz) //Work outside New Zealand
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfoperatesoutsideofnz" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfhighfeeincome(User underwritingUser, ClientAgreement agreement, decimal feeincome, IDictionary<string, decimal> rates)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhighfeeincome" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhighfeeincome") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhighfeeincome").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhighfeeincome").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhighfeeincome").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfhighfeeincome").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhighfeeincome" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (feeincome > rates["maximumfeeincome"])
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfhighfeeincome" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfcontractingservices(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcontractingservices" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcontractingservices") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcontractingservices").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcontractingservices").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcontractingservices").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcontractingservices").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcontractingservices" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PMINZPIViewModel.ContractingServicesOptions").First().Value != null)
                    {
                        var result = agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PMINZPIViewModel.ContractingServicesOptions").First().Value.Substring(agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PMINZPIViewModel.ContractingServicesOptions").First().Value.Length - 2);
                        if (result != "10")
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcontractingservices" && cref.DateDeleted == null).Status = "Pending";
                        }

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
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "ClaimsHistoryViewModel.HasDamageLossOptions").First().Value == "1" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "ClaimsHistoryViewModel.HasWithdrawnOptions").First().Value == "1" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "ClaimsHistoryViewModel.HasRefusedOptions").First().Value == "1" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "ClaimsHistoryViewModel.HasStatutoryOffenceOptions").First().Value == "1" ||
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "ClaimsHistoryViewModel.HasLiquidationOptions").First().Value == "1" ||
                        agreement.ClientInformationSheet.ClaimNotifications.Where(acscn => acscn.DateDeleted == null).Count() > 0)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfpriorinsurance" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfnoprojectsmanaged(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnoprojectsmanaged" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnoprojectsmanaged") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnoprojectsmanaged").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnoprojectsmanaged").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnoprojectsmanaged").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnoprojectsmanaged").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnoprojectsmanaged" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PMINZPIViewModel.HasManagedProjectOptions").First().Value == "2")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnoprojectsmanaged" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfrenewalpremiumslowerthanexpiring(User underwritingUser, ClientAgreement agreement, bool bolrenewalpremiumslowerthanexpiring)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfrenewalpremiumslowerthanexpiring" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumslowerthanexpiring") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumslowerthanexpiring").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumslowerthanexpiring").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumslowerthanexpiring").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumslowerthanexpiring").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfrenewalpremiumslowerthanexpiring" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolrenewalpremiumslowerthanexpiring)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfrenewalpremiumslowerthanexpiring" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfrenewalpremiumshigherthanexpiring(User underwritingUser, ClientAgreement agreement, bool bolrenewalpremiumshigherthanexpiring)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfrenewalpremiumshigherthanexpiring" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumshigherthanexpiring") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumshigherthanexpiring").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumshigherthanexpiring").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumshigherthanexpiring").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfrenewalpremiumshigherthanexpiring").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfrenewalpremiumshigherthanexpiring" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolrenewalpremiumshigherthanexpiring)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfrenewalpremiumshigherthanexpiring" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfcapacityofanengineertocontract(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcapacityofanengineertocontract" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcapacityofanengineertocontract") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcapacityofanengineertocontract").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcapacityofanengineertocontract").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcapacityofanengineertocontract").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcapacityofanengineertocontract").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcapacityofanengineertocontract" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PMINZPIViewModel.HasEngineerOptions").First().Value == "1")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcapacityofanengineertocontract" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfconstructionrevenue(User underwritingUser, ClientAgreement agreement, bool constructionEngineerDetails)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfconstructionrevenue" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfconstructionrevenue") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfconstructionrevenue").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfconstructionrevenue").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfconstructionrevenue").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfconstructionrevenue").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfconstructionrevenue" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (constructionEngineerDetails)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfconstructionrevenue" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfcomponentspecificationactivities(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcomponentspecificationactivities" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcomponentspecificationactivities") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcomponentspecificationactivities").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcomponentspecificationactivities").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcomponentspecificationactivities").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcomponentspecificationactivities").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcomponentspecificationactivities" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PMINZPIViewModel.HasIncludedDesignOptions").First().Value == "1")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcomponentspecificationactivities" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

    }
}
