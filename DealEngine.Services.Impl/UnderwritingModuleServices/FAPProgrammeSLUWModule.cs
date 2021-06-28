﻿using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class FAPProgrammeSLUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public FAPProgrammeSLUWModule()
        {
            Name = "FAPProgramme_SL";
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

            if (agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "SL" && ct.DateDeleted == null) != null)
            {
                foreach (ClientAgreementTerm slterm in agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "SL" && ct.DateDeleted == null))
                {
                    slterm.Delete(underwritingUser);
                }
            }

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "sltermlimit", "sltermexcess", "sltermpremiumclass1", "sltermpremiumclass1noemp", "slamlextensionpremium");

            //Create default referral points based on the clientagreementrules
            if (agreement.ClientAgreementReferrals.Count == 0)
            {
                foreach (var clientagreementreferralrule in agreement.ClientAgreementRules.Where(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null))
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, clientagreementreferralrule.Name, clientagreementreferralrule.Description, "", clientagreementreferralrule.Value, clientagreementreferralrule.OrderNumber, clientagreementreferralrule.DoNotCheckForRenew));
            }
            else
            {
                foreach (var clientagreementreferral in agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null))
                    clientagreementreferral.Status = "";
            }

            int agreementperiodindays = 0;
            agreementperiodindays = (agreement.ExpiryDate - agreement.InceptionDate).Days;

            agreement.QuoteDate = DateTime.UtcNow;

            int coverperiodindays = 0;
            coverperiodindays = (agreement.ExpiryDate - agreement.ExpiryDate.AddYears(-1)).Days;

            int coverperiodindaysforchange = 0;
            coverperiodindaysforchange = (agreement.ExpiryDate - DateTime.UtcNow).Days;

            //Programme specific term

            //Default Professional Business, Retroactive Date, TerritoryLimit, Jurisdiction, AuditLog Detail
            string strProfessionalBusiness = "";
            string retrodate = "";
            string strTerritoryLimit = "";
            string strJurisdiction = "";
            string auditLogDetail = "";

            if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "TripleA Programme")
            {
                strProfessionalBusiness = "Life and general advisers of any insurance or assurance company and/or intermediaries, agents or consultants in the sale or negotiation of any financial product or the provision of any financial advice including mortgage advice and financial services educational workshops.";
                retrodate = "Unlimited excluding known claims or circumstances";
                strTerritoryLimit = "Australia and New Zealand";
                strJurisdiction = "Australia and New Zealand";
                auditLogDetail = "TripleA SL UW created/modified";

            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Apollo Programme")
            {
                strProfessionalBusiness = "General Insurance Brokers, Life Agents, Investment Advisers, Financial Planning and Mortgage Broking, Consultants and Advisers in the sale of any financial product including referrals to other financial product providers.";
                retrodate = agreement.InceptionDate.ToString("dd/MM/yyyy");
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "Apollo SL UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Financial Advice NZ Financial Advice Provider Liability Programme")
            {
                strProfessionalBusiness = "Financial Advice Provider – in the provision of Life & Health Insurance, Investment Advice, Mortgage Broking, Financial Planning and Fire & General Broking ";
                retrodate = "Unlimited excluding known claims or circumstances";
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "FANZ SL UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Financial Advice NZ Financial Advice Provider Liability ML Programme")
            {
                strProfessionalBusiness = "Provision of Life & Health Insurance, Investment Advice, Mortgage Broking, Financial Planning and Fire & General Broking ";
                retrodate = "Unlimited excluding known claims or circumstances";
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "FANZ SL UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Abbott Financial Advisor Liability Programme")
            {
                strProfessionalBusiness = "Sales & Promotion of Life, Investment & General Insurance products, Financial planning & Mortgage Brokering Services";
                retrodate = "N/A";
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "Abbott SL UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "NZFSG Programme" ||
                agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "NZFSG ML Programme")
            {
                strProfessionalBusiness = "Financial Advice Provider – in the provision of Life & Health Insurance, Mortgage Broking and Fire & General Broking.";
                retrodate = agreement.InceptionDate.ToString("dd/MM/yyyy");
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "NZFSG SL UW created/modified";

            }

            //renewal data (retro date and endorsements)
            string strretrodate = "";
            if (agreement.ClientInformationSheet.IsRenewawl && agreement.ClientInformationSheet.RenewFromInformationSheet != null)
            {
                var renewFromAgreement = agreement.ClientInformationSheet.RenewFromInformationSheet.Programme.Agreements.FirstOrDefault(p => p.ClientAgreementTerms.Any(i => i.SubTermType == "SL"));

                if (renewFromAgreement != null)
                {
                    strretrodate = renewFromAgreement.RetroactiveDate;

                    foreach (var renewendorsement in renewFromAgreement.ClientAgreementEndorsements)
                    {

                        if (renewendorsement.DateDeleted == null)
                        {
                            ClientAgreementEndorsement newclientendorsement =
                                new ClientAgreementEndorsement(underwritingUser, renewendorsement.Name, renewendorsement.Type, product, renewendorsement.Value, renewendorsement.OrderNumber, agreement);
                            agreement.ClientAgreementEndorsements.Add(newclientendorsement);
                        }
                    }
                }

            }

            int TermLimit = 0;
            decimal TermPremium = 0M;
            decimal TermBasePremium = 0M;
            decimal TermExtensionPremium = 0M;
            decimal TermBrokerage = 0M;
            decimal TermExcess = 0M;
            TermLimit = Convert.ToInt32(rates["sltermlimit"]);
            TermExcess = rates["sltermexcess"];

            //Check class information to calculate the premium
            bool bolclass2referral = false;
            bool bolclass3referral = false;
            if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasClassOfLicense").Any())
            {
                if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasClassOfLicense").First().Value == "1")
                {
                    TermPremium = rates["sltermpremiumclass1noemp"];
                    if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.HaveAnyEmployeeYN").First().Value == "1" &&
                        Convert.ToInt32(agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.TotalEmployees").First().Value) > 1)
                    {
                        TermPremium = rates["sltermpremiumclass1"];
                    }
                }
                else if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasClassOfLicense").First().Value == "2")
                {
                    bolclass2referral = true;
                }
                else if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "PIViewModel.HasClassOfLicense").First().Value == "3")
                {
                    bolclass3referral = true;
                }
            }

            //Check SL AML extension
            if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "SLViewModel.HasReportingEntityOptions").First().Value == "1" &&
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "SLViewModel.HasTrainingOptions").First().Value == "1" &&
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "SLViewModel.HasManageAMLOptions").First().Value == "1" &&
                        agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "SLViewModel.HasAMLCFTExtensionOptions").First().Value == "1")
            {
                TermExtensionPremium = rates["slamlextensionpremium"];

                if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "NZFSG Programme" ||
                agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "NZFSG ML Programme")
                {
                    //work out 5% of PI $1mil option premium whichever is the greater 
                    var PIAgreement = agreement.ClientInformationSheet.Programme.Agreements.FirstOrDefault(p => p.ClientAgreementTerms.Any(i => i.SubTermType == "PI"));
                    if (PIAgreement != null)
                    {
                        foreach (var term in PIAgreement.ClientAgreementTerms)
                        {
                            if (term.TermLimit == 1000000 && term.DateDeleted == null)
                            {
                                TermExtensionPremium = (TermExtensionPremium > (term.Premium * 0.05M)) ? TermExtensionPremium : (term.Premium * 0.05M);
                            }
                        }
                    }
                }
                
            }
            if (!bolclass2referral && !bolclass3referral)
            {
                TermPremium += TermExtensionPremium;
            }

            TermBasePremium = TermPremium;
            TermPremium = TermPremium * agreementperiodindays / coverperiodindays;
            TermBrokerage = TermPremium * agreement.Brokerage / 100;

            ClientAgreementTerm termsltermoption = GetAgreementTerm(underwritingUser, agreement, "SL", TermLimit, TermExcess);
            termsltermoption.TermLimit = TermLimit;
            termsltermoption.Premium = TermPremium;
            termsltermoption.BasePremium = TermBasePremium;
            termsltermoption.Excess = TermExcess;
            termsltermoption.BrokerageRate = agreement.Brokerage;
            termsltermoption.Brokerage = TermBrokerage;
            termsltermoption.DateDeleted = null;
            termsltermoption.DeletedBy = null;

            //Change policy premium claculation
            if (agreement.ClientInformationSheet.IsChange && agreement.ClientInformationSheet.PreviousInformationSheet != null)
            {
                var PreviousAgreement = agreement.ClientInformationSheet.PreviousInformationSheet.Programme.Agreements.FirstOrDefault(p => p.ClientAgreementTerms.Any(i => i.SubTermType == "SL"));
                foreach (var term in PreviousAgreement.ClientAgreementTerms)
                {
                    if (term.Bound)
                    {
                        var PreviousBoundPremium = term.Premium;
                        if (term.BasePremium > 0 && PreviousAgreement.ClientInformationSheet.IsChange)
                        {
                            PreviousBoundPremium = term.BasePremium;
                        }
                        termsltermoption.PremiumDiffer = (TermPremium - PreviousBoundPremium) * coverperiodindaysforchange / agreementperiodindays;
                        termsltermoption.PremiumPre = PreviousBoundPremium;
                        if (termsltermoption.TermLimit == term.TermLimit && termsltermoption.Excess == term.Excess)
                        {
                            termsltermoption.Bound = true;
                        }
                        if (termsltermoption.PremiumDiffer < 0)
                        {
                            termsltermoption.PremiumDiffer = 0;
                        }

                    }

                }
            }

            //Referral points per agreement
            //Class 2 referral
            uwrfclass2(underwritingUser, agreement, bolclass2referral);
            //Class 3 referral
            uwrfclass3(underwritingUser, agreement, bolclass3referral);

            //Update agreement Status
            if (agreement.ClientAgreementReferrals.Where(cref => cref.DateDeleted == null && cref.Status == "Pending").Count() > 0)
            {
                agreement.Status = "Referred";
            }
            else
            {
                agreement.Status = "Quoted";
            }

            //Update agreement Name of Insured
            agreement.InsuredName = informationSheet.Owner.Name;

            //Update agreement Professional Business, Retroactive Date, TerritoryLimit, Jurisdiction
            agreement.ProfessionalBusiness = strProfessionalBusiness;
            agreement.RetroactiveDate = retrodate;
            agreement.TerritoryLimit = strTerritoryLimit;
            agreement.Jurisdiction = strJurisdiction;
            if (!String.IsNullOrEmpty(strretrodate))
            {
                agreement.RetroactiveDate = strretrodate;
            }

            //Create agreement audit log
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

                //Inception date rule (turned on after implementing change, any remaining policy and new policy will use submission date as inception date)
                if (informationSheet.IsRenewawl)
                {
                    int renewalgraceperiodindays = 0;
                    renewalgraceperiodindays = programme.BaseProgramme.RenewGracePriodInDays;
                    if (DateTime.UtcNow > product.DefaultInceptionDate.AddDays(renewalgraceperiodindays))
                    {
                        inceptionDate = DateTime.UtcNow;
                    }
                }
                else
                {
                    int newalgraceperiodindays = 0;
                    newalgraceperiodindays = programme.BaseProgramme.NewGracePriodInDays;
                    if (DateTime.UtcNow > product.DefaultInceptionDate.AddDays(newalgraceperiodindays))
                    {
                        inceptionDate = DateTime.UtcNow;
                    }
                }

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

        void uwrfclass2(User underwritingUser, ClientAgreement agreement, bool bolclass2referral)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass2" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass2") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass2").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass2").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass2").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass2").OrderNumber,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass2").DoNotCheckForRenew));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass2" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolclass2referral) //Class 2 referral
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass2" && cref.DateDeleted == null).Status = "Pending";
                    }
                }

                if (agreement.ClientInformationSheet.IsRenewawl
                            && agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass2" && cref.DateDeleted == null).DoNotCheckForRenew)
                {
                    agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass2" && cref.DateDeleted == null).Status = "";
                }
            }
        }

        void uwrfclass3(User underwritingUser, ClientAgreement agreement, bool bolclass3referral)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass3" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass3") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass3").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass3").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass3").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass3").OrderNumber,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwrfclass3").DoNotCheckForRenew));
            }
            else
            {
                if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass3" && cref.DateDeleted == null).Status != "Pending")
                {
                    if (bolclass3referral) //Class 3 referral
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass3" && cref.DateDeleted == null).Status = "Pending";
                    }
                }

                if (agreement.ClientInformationSheet.IsRenewawl
                            && agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass3" && cref.DateDeleted == null).DoNotCheckForRenew)
                {
                    agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwrfclass3" && cref.DateDeleted == null).Status = "";
                }
            }
        }


    }
}




