using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl.UnderwritingModuleServices
{
    public class CEASPIUWModule : IUnderwritingModule
    {
        public string Name { get; protected set; }

        public CEASPIUWModule()
        {
            Name = "CEAS_PI";
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


            //terms hardcoded 
            #region terms
            int TermLimit300k = 300000;
            decimal TermPremiumDEFAULT = 0m;
            decimal TermBrokerageDEFAULT = 0m;
            int TermExcess = 0;
            decimal feeincome = 0;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl300klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit300k, TermExcess);
            termsl300klimitoption.TermLimit = TermLimit300k;
            termsl300klimitoption.Premium = TermPremiumDEFAULT;
            termsl300klimitoption.Excess = TermExcess;
            termsl300klimitoption.BrokerageRate = agreement.Brokerage;
            termsl300klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl300klimitoption.DateDeleted = null;
            termsl300klimitoption.DeletedBy = null;

            int TermLimit500k = 500000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl500klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit500k, TermExcess);
            termsl500klimitoption.TermLimit = TermLimit500k;
            termsl500klimitoption.Premium = TermPremiumDEFAULT;
            termsl500klimitoption.Excess = TermExcess;
            termsl500klimitoption.BrokerageRate = agreement.Brokerage;
            termsl500klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl500klimitoption.DateDeleted = null;
            termsl500klimitoption.DeletedBy = null;
            
            int TermLimit750k = 750000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl750klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit750k, TermExcess);
            termsl750klimitoption.TermLimit = TermLimit750k;
            termsl750klimitoption.Premium = TermPremiumDEFAULT;
            termsl750klimitoption.Excess = TermExcess;
            termsl750klimitoption.BrokerageRate = agreement.Brokerage;
            termsl750klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl750klimitoption.DateDeleted = null;
            termsl750klimitoption.DeletedBy = null;
            
            int TermLimit1000k = 1000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl1000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1000k, TermExcess);
            termsl1000klimitoption.TermLimit = TermLimit1000k;
            termsl1000klimitoption.Premium = TermPremiumDEFAULT;
            termsl1000klimitoption.Excess = TermExcess;
            termsl1000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl1000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl1000klimitoption.DateDeleted = null;
            termsl1000klimitoption.DeletedBy = null;
            
            int TermLimit1500k = 1500000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl1500klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit1500k, TermExcess);
            termsl1500klimitoption.TermLimit = TermLimit1500k;
            termsl1500klimitoption.Premium = TermPremiumDEFAULT;
            termsl1500klimitoption.Excess = TermExcess;
            termsl1500klimitoption.BrokerageRate = agreement.Brokerage;
            termsl1500klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl1500klimitoption.DateDeleted = null;
            termsl1500klimitoption.DeletedBy = null;
            
            int TermLimit2000k = 2000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl2000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2000k, TermExcess);
            termsl2000klimitoption.TermLimit = TermLimit2000k;
            termsl2000klimitoption.Premium = TermPremiumDEFAULT;
            termsl2000klimitoption.Excess = TermExcess;
            termsl2000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl2000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl2000klimitoption.DateDeleted = null;
            termsl2000klimitoption.DeletedBy = null;

            int TermLimit2500k = 2500000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl2500klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit2500k, TermExcess);
            termsl2500klimitoption.TermLimit = TermLimit2500k;
            termsl2500klimitoption.Premium = TermPremiumDEFAULT;
            termsl2500klimitoption.Excess = TermExcess;
            termsl2500klimitoption.BrokerageRate = agreement.Brokerage;
            termsl2500klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl2500klimitoption.DateDeleted = null;
            termsl2500klimitoption.DeletedBy = null;

            int TermLimit3000k = 3000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl3000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit300k, TermExcess);
            termsl3000klimitoption.TermLimit = TermLimit3000k;
            termsl3000klimitoption.Premium = TermPremiumDEFAULT;
            termsl3000klimitoption.Excess = TermExcess;
            termsl3000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl3000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl3000klimitoption.DateDeleted = null;
            termsl3000klimitoption.DeletedBy = null;
            
            int TermLimit4000k = 4000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl4000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit4000k, TermExcess);
            termsl4000klimitoption.TermLimit = TermLimit4000k;
            termsl4000klimitoption.Premium = TermPremiumDEFAULT;
            termsl4000klimitoption.Excess = TermExcess;
            termsl4000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl4000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl4000klimitoption.DateDeleted = null;
            termsl4000klimitoption.DeletedBy = null;
            
            int TermLimit5000k = 5000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl5000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit5000k, TermExcess);
            termsl5000klimitoption.TermLimit = TermLimit5000k;
            termsl5000klimitoption.Premium = TermPremiumDEFAULT;
            termsl5000klimitoption.Excess = TermExcess;
            termsl5000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl5000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl5000klimitoption.DateDeleted = null;
            termsl5000klimitoption.DeletedBy = null;
            
            int TermLimit6000k = 6000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl6000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit6000k, TermExcess);
            termsl6000klimitoption.TermLimit = TermLimit6000k;
            termsl6000klimitoption.Premium = TermPremiumDEFAULT;
            termsl6000klimitoption.Excess = TermExcess;
            termsl6000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl6000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl6000klimitoption.DateDeleted = null;
            termsl6000klimitoption.DeletedBy = null;
            
            int TermLimit8000k = 8000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl8000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit8000k, TermExcess);
            termsl8000klimitoption.TermLimit = TermLimit8000k;
            termsl8000klimitoption.Premium = TermPremiumDEFAULT;
            termsl8000klimitoption.Excess = TermExcess;
            termsl8000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl8000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl8000klimitoption.DateDeleted = null;
            termsl8000klimitoption.DeletedBy = null;
            
            int TermLimit10000k = 10000000;

            //TermPremiumDEFAULT = GetPremiumFor(rates, feeincome, bolAnyADNZMember, TermLimit250k, TermExcess, schoolsactivitymoepercentage, schoolsactivitynonmoepercentage);
            ClientAgreementTerm termsl10000klimitoption = GetAgreementTerm(underwritingUser, agreement, "PI", TermLimit10000k, TermExcess);
            termsl10000klimitoption.TermLimit = TermLimit10000k;
            termsl10000klimitoption.Premium = TermPremiumDEFAULT;
            termsl10000klimitoption.Excess = TermExcess;
            termsl10000klimitoption.BrokerageRate = agreement.Brokerage;
            termsl10000klimitoption.Brokerage = TermBrokerageDEFAULT;
            termsl10000klimitoption.DateDeleted = null;
            termsl10000klimitoption.DeletedBy = null;           
            #endregion

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


            string auditLogDetail = "CEAS PI UW created/modified";
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
