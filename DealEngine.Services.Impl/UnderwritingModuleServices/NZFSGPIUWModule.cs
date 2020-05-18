using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class NZFSGPIUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public NZFSGPIUWModule()
        {
            Name = "NZFSG_PI";
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
                "pi500klimitincomeunder100kpremium", "pi500klimitincome100kto200kpremium", "pi500klimitincome200kto500kpremium",
                "pi1millimitincomeunder100kpremium", "pi1millimitincome100kto200kpremium", "pi1millimitincome200kto500kpremium",
                "pi2millimitincomeunder100kpremium", "pi2millimitincome100kto200kpremium", "pi2millimitincome200kto500kpremium", "piwwextpremium", "maximumfeeincome");

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

            decimal decMBR = 0M;
            decimal decMBC = 0M;
            decimal decIns = 0M;
            decimal decFP = 0M;
            decimal decKS = 0M;
            decimal decBA = 0M;
            decimal decAF = 0M;
            decimal decRFG = 0M;
            decimal decBFG = 0M;
            decimal decOther = 0M;

            decimal decBFG1To10PerExtraPre = 0M;
            decimal decFP1To10PerExtraPre = 0M;
            decimal decPEF0To70PerExtraPre = 0M;
            decimal decPEF71To100PerExtraPre = 0M;
            decimal decRFG10To20PerExtraPre = 0M;

            decimal decPIPremiumTopUp = 0M;

            if (agreement.ClientInformationSheet.RevenueData != null)
            {
                if (agreement.ClientInformationSheet.RevenueData.LastFinancialYearTotal > 0)
                {
                    feeincome = agreement.ClientInformationSheet.RevenueData.LastFinancialYearTotal;
                } else if (agreement.ClientInformationSheet.RevenueData.NextFinancialYearTotal > 0) 
                {
                    feeincome = agreement.ClientInformationSheet.RevenueData.NextFinancialYearTotal;
                }


                foreach (var uISActivity in agreement.ClientInformationSheet.RevenueData.Activities)
                {
                    if (uISActivity.AnzsciCode == "CUS0020") //Mortgage Broking (Residential)
                    {
                        decMBR = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "CUS0021") //Mortgage Broking (Commercial)
                    {
                        decMBC = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "CUS0022") //Insurance (Life, medical, disability only)
                    {
                        decIns = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "CUS0023") //Financial Planning
                    {
                        decFP = uISActivity.Percentage;
                        if (uISActivity.Percentage > 0 && uISActivity.Percentage <= 10)
                            decFP1To10PerExtraPre = rates["piFP1To10PerExtraPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "CUS0024") //Kiwisaver
                    {
                        decKS = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "CUS0025") //Budget Advice
                    {
                        decBA = uISActivity.Percentage;
                    }
                    else if (uISActivity.AnzsciCode == "CUS0026") //Asset Finance 
                    {
                        decAF = uISActivity.Percentage;
                        if (uISActivity.Percentage > 0 && uISActivity.Percentage <= 70)
                            decPEF0To70PerExtraPre = rates["piPEF0To70PerExtraPremium"];
                        if (uISActivity.Percentage > 70 && uISActivity.Percentage <= 100)
                            decPEF71To100PerExtraPre = rates["piPEF71To100PerExtraPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "CUS0027") //Referred Fire and General (i.e. Tower, Aon) 
                    {
                        decRFG = uISActivity.Percentage;
                        if (uISActivity.Percentage >= 10 && uISActivity.Percentage <= 20)
                            decRFG10To20PerExtraPre = rates["piRFG10To20PerExtraPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "CUS0028") //Broking Fire and General (i.e. NZI)
                    {
                        decBFG = uISActivity.Percentage;
                        if (uISActivity.Percentage > 0 && uISActivity.Percentage <= 10)
                            decBFG1To10PerExtraPre = rates["piBFG1To10PerExtraPremium"];
                    }
                    else if (uISActivity.AnzsciCode == "CUS0029") //Other 
                    {
                        decOther = uISActivity.Percentage;
                    }


                }

                decPIPremiumTopUp = decBFG1To10PerExtraPre + decFP1To10PerExtraPre + decPEF0To70PerExtraPre + decPEF71To100PerExtraPre + decRFG10To20PerExtraPre;
            }

            //ClientAgreementEndorsement cAEProjM = agreement.ClientAgreementEndorsements.FirstOrDefault(cae => cae.Name == "Project Managers Endorsement");
            //if (cAEProjM != null)
            //{
            //    cAEProjM.DateDeleted = DateTime.UtcNow;
            //    cAEProjM.DeletedBy = underwritingUser;
            //}
            //if (decProjM > 0)
            //{
            //    if (cAEProjM != null)
            //    {
            //        cAEProjM.DateDeleted = null;
            //        cAEProjM.DeletedBy = null;
            //    }
            //}


            bool bolcustomendorsementrenew = false;
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
                            bolcustomendorsementrenew = true;
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

            TermPremium500k2500Excess = GetPremiumFor(rates, feeincome, TermLimit500k) + decPIPremiumTopUp;
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

            TermPremium1mil2500Excess = GetPremiumFor(rates, feeincome, TermLimit1mil) + decPIPremiumTopUp;
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

            TermPremium2mil2500Excess = GetPremiumFor(rates, feeincome, TermLimit2mil) + decPIPremiumTopUp;
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
            ////Operates Outside of NZ
            //uwrfoperatesoutsideofnz(underwritingUser, agreement, bolworkoutsidenz);
            ////Claims / Insurance History
            //uwrfpriorinsurance(underwritingUser, agreement);
            ////High Fee Income
            //uwrfhighfeeincome(underwritingUser, agreement, feeincome, rates);
            ////Negative Turnover
            //uwrfnegativefeeincome(underwritingUser, agreement, feeincome, rates);
            ////Substancial Business Changes
            //uwrfsubstancialbusinesschanges(underwritingUser, agreement);
            ////Staff Dishonesty
            //uwrfstaffdishonesty(underwritingUser, agreement);
            ////Site License Category3
            //uwrfsitelicensecategory3(underwritingUser, agreement, bolsitelicensecategory3);
            ////DANZ Member
            //uwrfnotdanzmember(underwritingUser, agreement);
            ////Excluded Activities (I.e. Inspection reports, Valuation, Other)
            //uwrfexcludedactivities(underwritingUser, agreement, decIR, decVal, decOther);
            ////Over 10% Allied Professions Activities (Structural Engineer, Quantity Surveying, Building Services Engineer, Architect and Town Planning, Urban Planning , 
            ////Environmental planning, Inspection reports, Project Management, Resource Management, Local government Policy advice and Valuations)
            //uwrfover10perap(underwritingUser, agreement, decSE, decQS, decBSS, decTP, decUP, decEP, decIR, decProjM, decRM, decVal);
            ////Custom Endorsement renew
            //uwrfcustomendorsementrenew(underwritingUser, agreement, bolcustomendorsementrenew);
            ////Not a renewal of an existing policy
            //uwrfnotrenewal(underwritingUser, agreement);

            //Update agreement status
            if (agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null && cref.Status == "Pending").Count() > 0)
            {
                agreement.Status = "Referred";
            }
            else
            {
                agreement.Status = "Quoted";
            }

            agreement.ProfessionalBusiness = "Building Design Practitioner, Architectural Design, Mechanical Design, Electrical Design, Structural Design, Civil Design, Draughting and associated ancillary activities";
            string retrodate = "Policy Inception";
            agreement.TerritoryLimit = "Worldwide excluding USA/Canada";
            agreement.Jurisdiction = "Worldwide excluding USA/Canada";
            agreement.RetroactiveDate = retrodate;
            if (!String.IsNullOrEmpty(strretrodate))
            {
                agreement.RetroactiveDate = strretrodate;
            }

            string auditLogDetail = "NZFSG PI UW created/modified";
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
                            premiumoption = rates["pi500klimitincomeunder100kpremium"];
                        }
                        else if (feeincome > 100000 && feeincome <= 200000)
                        {
                            premiumoption = rates["pi500klimitincome100kto200kpremium"];
                        }
                        else if (feeincome > 200000 && feeincome <= 500000)
                        {
                            premiumoption = rates["pi500klimitincome200kto500kpremium"];
                        }
                        break;
                    }
                case 1000000:
                    {
                        if (feeincome >= 0 && feeincome <= 100000)
                        {
                            premiumoption = rates["pi1millimitincomeunder100kpremium"];
                        }
                        else if (feeincome > 100000 && feeincome <= 200000)
                        {
                            premiumoption = rates["pi1millimitincome100kto200kpremium"];
                        }
                        else if (feeincome > 200000 && feeincome <= 500000)
                        {
                            premiumoption = rates["pi1millimitincome200kto500kpremium"];
                        }
                        break;
                    }
                case 2000000:
                    {
                        if (feeincome >= 0 && feeincome <= 100000)
                        {
                            premiumoption = rates["pi2millimitincomeunder100kpremium"];
                        }
                        else if (feeincome > 100000 && feeincome <= 200000)
                        {
                            premiumoption = rates["pi2millimitincome100kto200kpremium"];
                        }
                        else if (feeincome > 200000 && feeincome <= 500000)
                        {
                            premiumoption = rates["pi2millimitincome200kto500kpremium"];
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

        void uwrfsitelicensecategory3(User underwritingUser, ClientAgreement agreement, bool bolsitelicensecategory3)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfsitelicensecategory3" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsitelicensecategory3") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsitelicensecategory3v").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsitelicensecategory3").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsitelicensecategory3").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsitelicensecategory3").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfsitelicensecategory3" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolsitelicensecategory3)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfsitelicensecategory3" && cref.DateDeleted == null).Status = "Pending";
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
                        agreement.ClientInformationSheet.ClaimNotifications.Where(acscn => acscn.DateDeleted == null && (acscn.ClaimStatus == "Settled" || acscn.ClaimStatus == "Precautionary notification only" || acscn.ClaimStatus == "Part Settled")).Count() > 0)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfpriorinsurance" && cref.DateDeleted == null).Status = "Pending";
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

        void uwrfnegativefeeincome(User underwritingUser, ClientAgreement agreement, decimal feeincome, IDictionary<string, decimal> rates)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnegativefeeincome" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnegativefeeincome") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnegativefeeincome").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnegativefeeincome").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnegativefeeincome").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnegativefeeincome").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnegativefeeincome" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (feeincome < 0)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnegativefeeincome" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfsubstancialbusinesschanges(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfsubstancialbusinesschanges" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsubstancialbusinesschanges") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsubstancialbusinesschanges").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsubstancialbusinesschanges").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsubstancialbusinesschanges").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfsubstancialbusinesschanges").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfsubstancialbusinesschanges" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasSubstantialChangeOptions").First().Value == "1")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfsubstancialbusinesschanges" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfstaffdishonesty(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfstaffdishonesty" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfstaffdishonesty") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfstaffdishonesty").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfstaffdishonesty").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfstaffdishonesty").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfstaffdishonesty").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfstaffdishonesty" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasPersonnelDismissedOptions").First().Value == "1")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfstaffdishonesty" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfnotdanzmember(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotdanzmember" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotdanzmember") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotdanzmember").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotdanzmember").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotdanzmember").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotdanzmember").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotdanzmember" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasDANZOptions").First().Value == "2")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotdanzmember" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfexcludedactivities(User underwritingUser, ClientAgreement agreement, decimal decIR, decimal decVal, decimal decOther)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfexcludedactivities" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfexcludedactivities") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfexcludedactivities").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfexcludedactivities").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfexcludedactivities").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfexcludedactivities").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfexcludedactivities" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (decIR > 0 || decVal > 0 || decOther > 0)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfexcludedactivities" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfover10perap(User underwritingUser, ClientAgreement agreement, decimal decSE, decimal decQS, decimal decBSS, decimal decTP, decimal decUP, decimal decEP, decimal decIR, decimal decProjM, decimal decRM, decimal decVal)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfover10perap" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfover10perap") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfover10perap").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfover10perap").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfover10perap").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfover10perap").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfover10perap" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (decSE > 10 || decQS > 10 || decBSS > 10 || decTP > 10 || decUP > 10 || decEP > 10 || decIR > 10 || decProjM > 10 || decRM > 10 || decVal > 10)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfover10perap" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfcustomendorsementrenew(User underwritingUser, ClientAgreement agreement, bool bolcustomendorsementrenew)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcustomendorsementrenew" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcustomendorsementrenew") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfoperatesoutsideofnz").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcustomendorsementrenew").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcustomendorsementrenew").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfcustomendorsementrenew").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcustomendorsementrenew" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolcustomendorsementrenew) //Custom Endorsement Renew
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfcustomendorsementrenew" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

        void uwrfnotrenewal(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotrenewal" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotrenewal") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotrenewal").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotrenewal").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotrenewal").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfnotrenewal").OrderNumber));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotrenewal" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasExistingPolicyOptions").First().Value == "2")
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfnotrenewal" && cref.DateDeleted == null).Status = "Pending";
                    }
                }
            }
        }

    }
}
