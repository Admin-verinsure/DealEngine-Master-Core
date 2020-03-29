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

            int TermExcess = 0;
            decimal feeincome = 0;

            //TermPremium250k = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
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



    }
}
