﻿using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class FAPProgrammeEDUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public FAPProgrammeEDUWModule()
        {
            Name = "FAPProgramme_ED";
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

            if (agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "ED" && ct.DateDeleted == null) != null)
            {
                foreach (ClientAgreementTerm edterm in agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "ED" && ct.DateDeleted == null))
                {
                    edterm.Delete(underwritingUser);
                }
            }

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "edtermlimit", "edtermexcess", "edtermpremium", "edtermaddionalpremiumperempover4", "edtermexcesshigher");

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
                auditLogDetail = "TripleA ED UW created/modified";

            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Apollo Programme")
            {
                strProfessionalBusiness = "General Insurance Brokers, Life Agents, Investment Advisers, Financial Planning and Mortgage Broking, Consultants and Advisers in the sale of any financial product including referrals to other financial product providers.";
                retrodate = agreement.InceptionDate.ToString("dd/MM/yyyy");
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "Apollo ED UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Financial Advice New Zealand Inc Programme")
            {
                strProfessionalBusiness = "";
                retrodate = agreement.InceptionDate.ToString("dd/MM/yyyy");
                strTerritoryLimit = "";
                strJurisdiction = "";
                auditLogDetail = "FANZ ED UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "Abbott Financial Advisor Liability Programme")
            {
                strProfessionalBusiness = "Sales & Promotion of Life, Investment & General Insurance products, Financial planning & Mortgage Brokering Services";
                retrodate = "N/A";
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "Abbott ED UW created/modified";
            }
            else if (agreement.ClientInformationSheet.Programme.BaseProgramme.NamedPartyUnitName == "NZFSG Programme")
            {
                //Additional professional business added based on selected business activities
                strProfessionalBusiness = "Mortgage broking and life, risk, health and medical insurance broking services. Fire and General referrals, including AON domestic placement services only. Advice in respect of ACC reporting status. Advice in relation to Kiwisaver.  Asset Finance.";
                retrodate = agreement.InceptionDate.ToString("dd/MM/yyyy");
                strTerritoryLimit = "New Zealand";
                strJurisdiction = "New Zealand";
                auditLogDetail = "NZFSG ED UW created/modified";

                if (agreement.ClientInformationSheet.RevenueData != null)
                {
                    foreach (var uISActivity in agreement.ClientInformationSheet.RevenueData.Activities)
                    {
                        if (uISActivity.AnzsciCode == "CUS0023") //Financial Planning
                        {
                            if (uISActivity.Percentage > 0)
                                strProfessionalBusiness += "  Advice in relation to Financial Planning.";

                        }
                        else if (uISActivity.AnzsciCode == "CUS0028") //Broking Fire and General (i.e. NZI)
                        {
                            if (uISActivity.Percentage > 0)
                                strProfessionalBusiness += "  Advice in relation to Fire and General Broking.";
                        }
                    }
                }
            }

            //renewal data (retro date and endorsements)
            string strretrodate = "";
            if (agreement.ClientInformationSheet.IsRenewawl && agreement.ClientInformationSheet.RenewFromInformationSheet != null)
            {
                var renewFromAgreement = agreement.ClientInformationSheet.RenewFromInformationSheet.Programme.Agreements.FirstOrDefault(p => p.ClientAgreementTerms.Any(i => i.SubTermType == "ED"));

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
            decimal TermBrokerage = 0M;
            decimal TermExcess = 0M;
            TermLimit = Convert.ToInt32(rates["edtermlimit"]);
            TermExcess = rates["edtermexcess"];
            TermPremium = rates["edtermpremium"];
            //Check employee number to calculate the additional premium
            if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.HaveAnyEmployeeYN").First().Value == "1" &&
                agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.HasEPLIOptions").First().Value == "1" &&
                agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.CoveredOptions").First().Value == "2")
            {
                TermExcess = rates["edtermexcesshigher"];
            }
            if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.HaveAnyEmployeeYN").First().Value == "1" &&
                agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.HasEPLIOptions").First().Value == "1" &&
                Convert.ToInt32(agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.TotalEmployees").First().Value) > 4)
            {
                TermPremium += (Convert.ToInt32(agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.TotalEmployees").First().Value) - 4) * rates["edtermaddionalpremiumperempover4"];
            }

            TermPremium = TermPremium * agreementperiodindays / coverperiodindays;
            TermBrokerage = TermPremium * agreement.Brokerage / 100;

            ClientAgreementTerm termedtermoption = GetAgreementTerm(underwritingUser, agreement, "ED", TermLimit, TermExcess);
            termedtermoption.TermLimit = TermLimit;
            termedtermoption.Premium = TermPremium;
            termedtermoption.BasePremium = TermPremium;
            termedtermoption.Excess = TermExcess;
            termedtermoption.BrokerageRate = agreement.Brokerage;
            termedtermoption.Brokerage = TermBrokerage;
            termedtermoption.DateDeleted = null;
            termedtermoption.DeletedBy = null;

            //Change policy premium calculation
            if (agreement.ClientInformationSheet.IsChange && agreement.ClientInformationSheet.PreviousInformationSheet != null)
            {
                var PreviousAgreement = agreement.ClientInformationSheet.PreviousInformationSheet.Programme.Agreements.Where(a => a.DateDeleted == null).
                    FirstOrDefault(p => p.ClientAgreementTerms.Any(i => i.SubTermType == "ED"));
                foreach (var term in PreviousAgreement.ClientAgreementTerms)
                {
                    if (term.Bound)
                    {
                        var PreviousBoundPremium = term.Premium;
                        if (term.BasePremium > 0 && PreviousAgreement.ClientInformationSheet.IsChange)
                        {
                            PreviousBoundPremium = term.BasePremium;
                        }
                        termedtermoption.PremiumDiffer = (TermPremium - PreviousBoundPremium) * coverperiodindaysforchange / agreementperiodindays;
                        termedtermoption.PremiumPre = PreviousBoundPremium;
                        if (termedtermoption.TermLimit == term.TermLimit && termedtermoption.Excess == term.Excess)
                        {
                            termedtermoption.Bound = true;
                        }
                        if (termedtermoption.PremiumDiffer < 0)
                        {
                            termedtermoption.PremiumDiffer = 0;
                        }
                    }
                }
            }


            //Referral points per agreement
            //ED Issues
            uwredissue(underwritingUser, agreement);

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


        void uwredissue(User underwritingUser, ClientAgreement agreement)
        {
            if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwredissue" && cref.DateDeleted == null) == null)
            {
                if (agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwredissue") != null)
                    agreement.ClientAgreementReferrals.Add(new ClientAgreementReferral(underwritingUser, agreement, agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwredissue").Name,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwredissue").Description,
                        "",
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwredissue").Value,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwredissue").OrderNumber,
                        agreement.ClientAgreementRules.FirstOrDefault(cr => cr.RuleCategory == "uwreferral" && cr.DateDeleted == null && cr.Value == "uwredissue").DoNotCheckForRenew));
            }
            else
            {

                if (agreement.Product.IsOptionalProduct && agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == agreement.Product.OptionalProductRequiredAnswer).First().Value == "1")
                {
                    if (agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwredissue" && cref.DateDeleted == null).Status != "Pending")
                    {
                        if (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.CoveredOptions").First().Value == "2" ||
                             (agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.CoveredOptions").First().Value == "1" &&
                             agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.LegalAdvisorOptions").First().Value == "2") ||
                             agreement.ClientInformationSheet.Answers.Where(sa => sa.ItemName == "EPLViewModel.IsInsuredClaimOptions").First().Value == "1")
                        {
                            agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwredissue" && cref.DateDeleted == null).Status = "Pending";
                        }
                    }

                    if (agreement.ClientInformationSheet.IsRenewawl
                                && agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwredissue" && cref.DateDeleted == null).DoNotCheckForRenew)
                    {
                        agreement.ClientAgreementReferrals.FirstOrDefault(cref => cref.ActionName == "uwredissue" && cref.DateDeleted == null).Status = "";
                    }
                }

            }
        }


    }
}
