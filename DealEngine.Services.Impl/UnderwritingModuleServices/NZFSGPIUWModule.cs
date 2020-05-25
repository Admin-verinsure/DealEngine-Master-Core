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

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "piFP1To10PerExtraPremium", "piPEF0To70PerExtraPremium", "piPEF71To100PerExtraPremium", "piRFG10To20PerExtraPremium",
                "piBFG1To10PerExtraPremium", "pi2millimitincomeunder500kdiscountpremium", "pi2millimitincome500kto600kdiscountpremium", "pi2millimitincome600kto800kdiscountpremium",
                "pi2millimitincome800kto1mildiscountpremium", "pi2millimitincomeunder500kpremium", "pi2millimitincome500kto600kpremium",
                "pi2millimitincome600kto800kpremium", "pi2millimitincome800kto1milpremium", "pi3millimitextrapremium", "pi5millimitextrapremium");

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

            string strProfessionalBusiness = "Mortgage broking and life, risk, health and medical insurance broking services. Fire & general referrals, including AON domestic placement services only. Advice in respect of ACC reporting status. Advice in relation to Kiwisaver.  Asset Finance.";

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
                        if (uISActivity.Percentage > 0)
                        {
                            strProfessionalBusiness += "  Advice in relation to Financial Planning.";
                        }

                        if (uISActivity.Percentage > 0 && uISActivity.Percentage <= 10)
                        {
                            decFP1To10PerExtraPre = rates["piFP1To10PerExtraPremium"];
                        }
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
                        if (uISActivity.Percentage > 0)
                        {
                            strProfessionalBusiness += "  Advice in relation to Fire and General Broking.";
                        }
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

            int intnumberofadvisors = 0;
            if (agreement.ClientInformationSheet.Organisation.Count > 0)
            {
                foreach (var uisorg in agreement.ClientInformationSheet.Organisation)
                {
                    if (uisorg.DateDeleted == null && uisorg.InsuranceAttributes.FirstOrDefault(uisorgia => uisorgia.InsuranceAttributeName == "Advisor" && uisorgia.DateDeleted == null) != null)
                    {
                        intnumberofadvisors += 1;
                    }
                }
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

            int TermLimit2mil = 2000000;
            int TermLimit3mil = 3000000;
            int TermLimit5mil = 5000000;

            decimal TermPremium2mil = 0M;
            decimal TermPremium3mil = 0M;
            decimal TermPremium5mil = 0M;

            decimal TermBrokerage2mil = 0M;
            decimal TermBrokerage3mil = 0M;
            decimal TermBrokerage5mil = 0M;

            int TermExcess = 0;
            TermExcess = 1000;

            TermPremium2mil = GetPremiumFor(rates, feeincome, TermLimit2mil, intnumberofadvisors);
            ClientAgreementTerm term2millimitpremiumoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2mil, TermExcess);
            term2millimitpremiumoption.TermLimit = TermLimit2mil;
            term2millimitpremiumoption.Premium = TermPremium2mil;
            term2millimitpremiumoption.Excess = TermExcess;
            term2millimitpremiumoption.BrokerageRate = agreement.Brokerage;
            term2millimitpremiumoption.Brokerage = TermBrokerage2mil;
            term2millimitpremiumoption.DateDeleted = null;
            term2millimitpremiumoption.DeletedBy = null;

            TermPremium3mil = GetPremiumFor(rates, feeincome, TermLimit3mil, intnumberofadvisors);
            ClientAgreementTerm term3millimitpremiumoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit3mil, TermExcess);
            term3millimitpremiumoption.TermLimit = TermLimit3mil;
            term3millimitpremiumoption.Premium = TermPremium3mil;
            term3millimitpremiumoption.Excess = TermExcess;
            term3millimitpremiumoption.BrokerageRate = agreement.Brokerage;
            term3millimitpremiumoption.Brokerage = TermBrokerage3mil;
            term3millimitpremiumoption.DateDeleted = null;
            term3millimitpremiumoption.DeletedBy = null;

            TermPremium5mil = GetPremiumFor(rates, feeincome, TermLimit5mil, intnumberofadvisors);
            ClientAgreementTerm term5millimitpremiumoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit5mil, TermExcess);
            term5millimitpremiumoption.TermLimit = TermLimit5mil;
            term5millimitpremiumoption.Premium = TermPremium5mil;
            term5millimitpremiumoption.Excess = TermExcess;
            term5millimitpremiumoption.BrokerageRate = agreement.Brokerage;
            term5millimitpremiumoption.Brokerage = TermBrokerage5mil;
            term5millimitpremiumoption.DateDeleted = null;
            term5millimitpremiumoption.DeletedBy = null;


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

            agreement.ProfessionalBusiness = strProfessionalBusiness;
            string retrodate = agreement.InceptionDate.ToString("dd/MM/yyyy");
            agreement.TerritoryLimit = "Worldwide";
            agreement.Jurisdiction = "New Zealand";
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


        decimal GetPremiumFor(IDictionary<string, decimal> rates, decimal feeincome, int limitoption, int intnumberofadvisors)
        {
            decimal indadvisorpremiumoption = 0M;
            decimal premiumoption = 0M;

            if (intnumberofadvisors >= 4)
            {
                if (feeincome >= 0 && feeincome <= 500000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincomeunder500kdiscountpremium"];
                }
                else if (feeincome > 500000 && feeincome <= 600000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincome500kto600kdiscountpremium"];
                }
                else if (feeincome > 600000 && feeincome <= 800000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincome600kto800kdiscountpremium"];
                }
                else if (feeincome > 800000 && feeincome <= 1000000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincome800kto1mildiscountpremium"];
                }
            } else
            {
                if (feeincome >= 0 && feeincome <= 500000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincomeunder500kpremium"];
                }
                else if (feeincome > 500000 && feeincome <= 600000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincome500kto600kpremium"];
                }
                else if (feeincome > 600000 && feeincome <= 800000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincome600kto800kpremium"];
                }
                else if (feeincome > 800000 && feeincome <= 1000000)
                {
                    indadvisorpremiumoption = rates["pi2millimitincome800kto1milpremium"];
                }
            }

            switch (limitoption)
            {
                case 2000000:
                    {
                        indadvisorpremiumoption += 0;
                        break;
                    }
                case 3000000:
                    {
                        indadvisorpremiumoption += rates["pi3millimitextrapremium"];
                        break;
                    }
                case 5000000:
                    {
                        indadvisorpremiumoption += rates["pi5millimitextrapremium"];
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Can not calculate premium for PI"));
                    }
            }

            premiumoption = indadvisorpremiumoption * intnumberofadvisors;

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
