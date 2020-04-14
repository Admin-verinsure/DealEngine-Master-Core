using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class PMINZPLUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public PMINZPLUWModule()
        {
            Name = "PMINZ_PL";
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

            if (agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "PL" && ct.DateDeleted == null) != null)
            {
                foreach (ClientAgreementTerm plterm in agreement.ClientAgreementTerms.Where(ct => ct.SubTermType == "PL" && ct.DateDeleted == null))
                {
                    plterm.Delete(underwritingUser);
                }
            }

            IDictionary<string, decimal> rates = BuildRulesTable(agreement, "pl2millimitpremium", "pl5millimitpremium", "pl10millimitpremium");

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

            int TermLimit2mil = 2000000;
            decimal TermPremium2mil = rates["pl2millimitpremium"];
            decimal TermBrokerage2mil = 0m;
            int TermLimit5mil = 5000000;
            decimal TermPremium5mil = rates["pl5millimitpremium"];
            decimal TermBrokerage5mil = 0m;
            int TermLimit10mil = 10000000;
            decimal TermPremium10mil = rates["pl10millimitpremium"];
            decimal TermBrokerage10mil = 0m;
            int TermExcess = 0;

            TermBrokerage2mil = TermPremium2mil * agreement.Brokerage;
            TermBrokerage5mil = TermPremium5mil * agreement.Brokerage;
            TermBrokerage10mil = TermPremium10mil * agreement.Brokerage;

            ClientAgreementTerm termpl2millimitoption = GetAgreementTerm(underwritingUser, agreement, "PL", TermLimit2mil, TermExcess);
            termpl2millimitoption.TermLimit = TermLimit2mil;
            termpl2millimitoption.Premium = TermPremium2mil;
            termpl2millimitoption.Excess = TermExcess;
            termpl2millimitoption.BrokerageRate = agreement.Brokerage;
            termpl2millimitoption.Brokerage = TermBrokerage2mil;
            termpl2millimitoption.DateDeleted = null;
            termpl2millimitoption.DeletedBy = null;

            ClientAgreementTerm termpl5millimitoption = GetAgreementTerm(underwritingUser, agreement, "PL", TermLimit5mil, TermExcess);
            termpl5millimitoption.TermLimit = TermLimit5mil;
            termpl5millimitoption.Premium = TermPremium5mil;
            termpl5millimitoption.Excess = TermExcess;
            termpl5millimitoption.BrokerageRate = agreement.Brokerage;
            termpl5millimitoption.Brokerage = TermBrokerage5mil;
            termpl5millimitoption.DateDeleted = null;
            termpl5millimitoption.DeletedBy = null;

            ClientAgreementTerm termpl10millimitoption = GetAgreementTerm(underwritingUser, agreement, "PL", TermLimit10mil, TermExcess);
            termpl10millimitoption.TermLimit = TermLimit10mil;
            termpl10millimitoption.Premium = TermPremium10mil;
            termpl10millimitoption.Excess = TermExcess;
            termpl10millimitoption.BrokerageRate = agreement.Brokerage;
            termpl10millimitoption.Brokerage = TermBrokerage10mil;
            termpl10millimitoption.DateDeleted = null;
            termpl10millimitoption.DeletedBy = null;


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

            
            agreement.TerritoryLimit = "Worldwide excluding USA/Canada";
            agreement.Jurisdiction = "Worldwide excluding USA/Canada";

            string auditLogDetail = "PMINZ PL UW created/modified";
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




    }
}
