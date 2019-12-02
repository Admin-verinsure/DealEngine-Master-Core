﻿using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechCertain.Services.Impl.UnderwritingModuleServices
{
    public class NZACSSLUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public NZACSSLUWModule()
        {
            Name = "NZACS_SL";
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

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "sl250klimitminpremium", "sl500klimitminpremium", "sl1millimitminpremium",
                "sl250klimitunder6employeerate", "sl500klimitunder6employeerate", "sl1millimitunder6employeerate", "sl250klimit6to10employeerate", "sl500klimit6to10employeerate", 
                "sl1millimit6to10employeerate", "sl250klimitover10employeerate", "sl500klimitover10employeerate", "sl1millimitover10employeerate");

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
            int TermLimit500k = 500000;
            decimal TermPremium500k = 0m;
            decimal TermBrokerage500k = 0m;
            int TermLimit1mil = 1000000;
            decimal TermPremium1mil = 0m;
            decimal TermBrokerage1mil = 0m;

            int TermExcess = 0;
            int employeenumber = 5;
            //Calculation

            //Return terms based on the limit options

            TermExcess = 500;

            TermPremium250k = GetPremiumFor(rates, employeenumber, TermLimit250k);
            ClientAgreementTerm termsl250klimitoption = GetAgreementTerm(underwritingUser, agreement, "SL", TermLimit250k, TermExcess);
            termsl250klimitoption.TermLimit = TermLimit250k;
            termsl250klimitoption.Premium = TermPremium250k;
            termsl250klimitoption.Excess = TermExcess;
            termsl250klimitoption.BrokerageRate = agreement.Brokerage;
            termsl250klimitoption.Brokerage = TermBrokerage250k;
            termsl250klimitoption.DateDeleted = null;
            termsl250klimitoption.DeletedBy = null;

            TermPremium500k = GetPremiumFor(rates, employeenumber, TermLimit500k);
            ClientAgreementTerm termsl500klimitoption = GetAgreementTerm(underwritingUser, agreement, "SL", TermLimit500k, TermExcess);
            termsl500klimitoption.TermLimit = TermLimit500k;
            termsl500klimitoption.Premium = TermPremium500k;
            termsl500klimitoption.Excess = TermExcess;
            termsl500klimitoption.BrokerageRate = agreement.Brokerage;
            termsl500klimitoption.Brokerage = TermBrokerage500k;
            termsl500klimitoption.DateDeleted = null;
            termsl500klimitoption.DeletedBy = null;

            TermPremium1mil = GetPremiumFor(rates, employeenumber, TermLimit1mil);
            ClientAgreementTerm termsl1millimitoption = GetAgreementTerm(underwritingUser, agreement, "SL", TermLimit1mil, TermExcess);
            termsl1millimitoption.TermLimit = TermLimit1mil;
            termsl1millimitoption.Premium = TermPremium1mil;
            termsl1millimitoption.Excess = TermExcess;
            termsl1millimitoption.BrokerageRate = agreement.Brokerage;
            termsl1millimitoption.Brokerage = TermBrokerage1mil;
            termsl1millimitoption.DateDeleted = null;
            termsl1millimitoption.DeletedBy = null;

            //Referral points per agreement
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


            string auditLogDetail = "NZACS SL UW created/modified";
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

        decimal GetPremiumFor(IDictionary<string, decimal> rates, int employeenumber, int limitoption)
        {
            decimal premiumoption = 0M;
            decimal minpremiumoption = 0M;

            switch (limitoption)
            {
                case 250000:
                    {
                        minpremiumoption = rates["sl250klimitminpremium"];
                        if (employeenumber <= 5)
                        {
                            premiumoption = rates["sl250klimitunder6employeerate"] * employeenumber;
                        }
                        else if (employeenumber > 5 && employeenumber <= 10)
                        {
                            premiumoption = rates["sl250klimit6to10employeerate"] * employeenumber;
                        }
                        else if (employeenumber > 10)
                        {
                            premiumoption = rates["sl250klimitover10employeerate"] * employeenumber;
                                                    }
                        premiumoption = (premiumoption > minpremiumoption) ? premiumoption : minpremiumoption;
                        break;
                    }
                case 500000:
                    {
                        minpremiumoption = rates["sl500klimitminpremium"];
                        if (employeenumber <= 5)
                        {
                            premiumoption = rates["sl500klimitunder6employeerate"] * employeenumber;
                        }
                        else if (employeenumber > 5 && employeenumber <= 10)
                        {
                            premiumoption = rates["sl500klimit6to10employeerate"] * employeenumber;
                        }
                        else if (employeenumber > 10)
                        {
                            premiumoption = rates["sl500klimitover10employeerate"] * employeenumber;
                        }
                        premiumoption = (premiumoption > minpremiumoption) ? premiumoption : minpremiumoption;
                        break;
                    }
                case 1000000:
                    {
                        minpremiumoption = rates["sl1millimitminpremium"];
                        if (employeenumber <= 5)
                        {
                            premiumoption = rates["sl1millimitunder6employeerate"] * employeenumber;
                        }
                        else if (employeenumber > 5 && employeenumber <= 10)
                        {
                            premiumoption = rates["sl1millimit6to10employeerate"] * employeenumber;
                        }
                        else if (employeenumber > 10)
                        {
                            premiumoption = rates["sl1millimitover10employeerate"] * employeenumber;
                        }
                        premiumoption = (premiumoption > minpremiumoption) ? premiumoption : minpremiumoption;
                        break;
                    }
                default:
                    {
                        throw new Exception(string.Format("Can not calculate premium for SL"));
                    }
            }

            return premiumoption;
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




    }
}


