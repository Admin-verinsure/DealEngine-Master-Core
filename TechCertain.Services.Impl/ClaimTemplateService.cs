using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ClaimTemplateService : IClaimTemplateService
    {
        IMapperSession<Claim> _claimsRepository;

        public ClaimTemplateService(IMapperSession<Claim> claimsRepository)
        {
            _claimsRepository = claimsRepository;
        }

        public async Task CreateClaimsForOrganisation(Organisation primaryOrganisation)
        {
            //claim(type, value) in this case the type is type of rule and value is the rule

            //view rules
            Claim v1 = new Claim("View", "SchemeProject", primaryOrganisation);
            Claim v2 = new Claim("View", "ManageProposalDropDown", primaryOrganisation);
            Claim v3 = new Claim("View", "SchemeProjectHistory", primaryOrganisation);
            Claim v4 = new Claim("View", "MembersContactDetails", primaryOrganisation);
            Claim v5 = new Claim("View", "InvoiceInformation", primaryOrganisation);
            Claim v6 = new Claim("View", "PremiumSummary", primaryOrganisation);
            await _claimsRepository.AddAsync(v1);
            await _claimsRepository.AddAsync(v2);
            await _claimsRepository.AddAsync(v3);
            await _claimsRepository.AddAsync(v4);
            await _claimsRepository.AddAsync(v5);
            await _claimsRepository.AddAsync(v6);

            //Send rules
            Claim s1 = new Claim("Send", "SendSchemeProjectEmails", primaryOrganisation);
            Claim s2 = new Claim("Send", "Proposals", primaryOrganisation);
            Claim s3 = new Claim("Send", "ProposalReminders", primaryOrganisation);
            Claim s4 = new Claim("Send", "Policy", primaryOrganisation);
            await _claimsRepository.AddAsync(s1);
            await _claimsRepository.AddAsync(s2);
            await _claimsRepository.AddAsync(s3);
            await _claimsRepository.AddAsync(s4);

            //edit rules
            Claim e1 = new Claim("Edit", "SchemeProjectSettings", primaryOrganisation);
            Claim e2 = new Claim("Edit", "SchemeProjectUserPermissions", primaryOrganisation);
            Claim e3 = new Claim("Edit", "SchemeProjectNotification", primaryOrganisation);
            Claim e4 = new Claim("Edit", "ScreenMessages", primaryOrganisation);
            Claim e5 = new Claim("Edit", "ProposalSettings", primaryOrganisation);
            Claim e6 = new Claim("Edit", "BrokerUsers", primaryOrganisation);
            Claim e7 = new Claim("Edit", "SchemeProjectQuote", primaryOrganisation);
            Claim e8 = new Claim("Edit", "SchemeProjectPolicy", primaryOrganisation);
            Claim e9 = new Claim("Edit", "NamesOfInsured", primaryOrganisation);
            Claim e10 = new Claim("Edit", "Quotes", primaryOrganisation);
            Claim e11 = new Claim("Edit", "QuoteInceptionDates", primaryOrganisation);
            Claim e12 = new Claim("Edit", "QuoteExpiryDates", primaryOrganisation);
            Claim e13 = new Claim("Edit", "NameOfInsurered", primaryOrganisation);
            Claim e14 = new Claim("Edit", "QuoteTerms", primaryOrganisation);
            Claim e15 = new Claim("Edit", "DefaultTerms", primaryOrganisation);
            Claim e16 = new Claim("Edit", "BoundTerms", primaryOrganisation);
            Claim e17 = new Claim("Edit", "QuoteDates", primaryOrganisation);
            Claim e18 = new Claim("Edit", "PeriodOfInsurance", primaryOrganisation);
            Claim e19 = new Claim("Edit", "TerritoryJurisdiction", primaryOrganisation);
            Claim e20 = new Claim("Edit", "RetrospectiveDates", primaryOrganisation);
            Claim e21 = new Claim("Edit", "Endorsements", primaryOrganisation);
            Claim e22 = new Claim("Edit", "BreachOfProfessionalDuty", primaryOrganisation);
            Claim e23 = new Claim("Edit", "QuoteProposal", primaryOrganisation);
            Claim e24 = new Claim("Edit", "QuoteBroker", primaryOrganisation);
            Claim e25 = new Claim("Edit", "QuoteBrokerage", primaryOrganisation);
            Claim e26 = new Claim("Edit", "ReferenceID", primaryOrganisation);
            await _claimsRepository.AddAsync(e1);
            await _claimsRepository.AddAsync(e2);
            await _claimsRepository.AddAsync(e3);
            await _claimsRepository.AddAsync(e4);
            await _claimsRepository.AddAsync(e5);
            await _claimsRepository.AddAsync(e6);
            await _claimsRepository.AddAsync(e7);
            await _claimsRepository.AddAsync(e8);
            await _claimsRepository.AddAsync(e9);
            await _claimsRepository.AddAsync(e10);
            await _claimsRepository.AddAsync(e11);
            await _claimsRepository.AddAsync(e12);
            await _claimsRepository.AddAsync(e13);
            await _claimsRepository.AddAsync(e14);
            await _claimsRepository.AddAsync(e15);
            await _claimsRepository.AddAsync(e16);
            await _claimsRepository.AddAsync(e17);
            await _claimsRepository.AddAsync(e18);
            await _claimsRepository.AddAsync(e19);
            await _claimsRepository.AddAsync(e20);
            await _claimsRepository.AddAsync(e21);
            await _claimsRepository.AddAsync(e22);
            await _claimsRepository.AddAsync(e23);
            await _claimsRepository.AddAsync(e24);
            await _claimsRepository.AddAsync(e25);
            await _claimsRepository.AddAsync(e26);

            //cancel rules
            Claim cn1 = new Claim("Cancel", "Policies", primaryOrganisation);
            await _claimsRepository.AddAsync(cn1);

            //template rules
            Claim t1 = new Claim("Template", "ConfigureMailTemplates", primaryOrganisation);
            Claim t2 = new Claim("Template", "RenderQuoteToTemplate", primaryOrganisation);
            await _claimsRepository.AddAsync(t1);
            await _claimsRepository.AddAsync(t2);

            //add rules
            Claim a1 = new Claim("Add", "ProjectMembers", primaryOrganisation);
            Claim a2 = new Claim("Add", "Quotes", primaryOrganisation);
            await _claimsRepository.AddAsync(a1);
            await _claimsRepository.AddAsync(a2);

            //manage rules
            Claim m1 = new Claim("Manage", "QuoteFiles", primaryOrganisation);
            Claim m2 = new Claim("Manage", "SendQuoteDocsEmail", primaryOrganisation);
            Claim m3 = new Claim("Manage", "SendQuoteDocsEmail", primaryOrganisation);
            Claim m4 = new Claim("Manage", "ResendPolicyDocuments", primaryOrganisation);
            Claim m5 = new Claim("Manage", "AccessUnderwritingWizard", primaryOrganisation);
            Claim m6 = new Claim("Manage", "IssueQuotesAcceptance", primaryOrganisation);
            Claim m7 = new Claim("Manage", "AnalyseProposals", primaryOrganisation);
            Claim m8 = new Claim("Manage", "ReportCompile", primaryOrganisation);
            Claim m9 = new Claim("Manage", "ReportProjectPolicyDocuments", primaryOrganisation);
            Claim m10 = new Claim("Manage", "ReportProjectQuoteTemplate", primaryOrganisation);
            Claim m11 = new Claim("Manage", "ReportProjectPolicyTemplate", primaryOrganisation);
            Claim m12 = new Claim("Manage", "ReceiveQuoteDocsEmail", primaryOrganisation);
            Claim m13 = new Claim("Manage", "ReferQuoteToBroker", primaryOrganisation);
            Claim m14 = new Claim("Manage", "QuoteSendRenewalReminder", primaryOrganisation);
            Claim m15 = new Claim("Manage", "IssueQuoteToBroker", primaryOrganisation);
            Claim m16 = new Claim("Manage", "SetQuoteAsIssued", primaryOrganisation);
            Claim m17 = new Claim("Manage", "DeclineToQuote", primaryOrganisation);
            Claim m18 = new Claim("Manage", "BindQuote", primaryOrganisation);
            Claim m19 = new Claim("Manage", "RevertBrokerQuoteAdvice", primaryOrganisation);
            Claim m20 = new Claim("Manage", "ReinsurerConfirmReferral", primaryOrganisation);
            Claim m21 = new Claim("Manage", "ReinsurerIssueQuoteToBroker", primaryOrganisation);
            Claim m22 = new Claim("Manage", "ReinsurerDeclineReferral", primaryOrganisation);
            Claim m23 = new Claim("Manage", "InsurerIssueQuoteToBroker", primaryOrganisation);
            Claim m24 = new Claim("Manage", "DeleteSchemeQuotePolicy", primaryOrganisation);
            Claim m25 = new Claim("Manage", "QuoteViewAdvancedSettings", primaryOrganisation);
            Claim m26 = new Claim("Manage", "ArchiveQuotePolicy", primaryOrganisation);
            Claim m27 = new Claim("Manage", "UnarchiveQuotePolicy", primaryOrganisation);
            Claim m28 = new Claim("Manage", "QuoteSetCOBAuth", primaryOrganisation);
            Claim m29 = new Claim("Manage", "RenewPolicy", primaryOrganisation);
            Claim m30 = new Claim("Manage", "QuoteRemoveRenewalsDue", primaryOrganisation);
            Claim m31 = new Claim("Manage", "ProcessQuotePayments", primaryOrganisation);
            Claim m32 = new Claim("Manage", "EGlobalBilling", primaryOrganisation);
            Claim m33 = new Claim("Manage", "UserLockOut90Days", primaryOrganisation);
            Claim m34 = new Claim("Manage", "MFAChallengeRequiredAccess", primaryOrganisation);
            await _claimsRepository.AddAsync(m1);
            await _claimsRepository.AddAsync(m2);
            await _claimsRepository.AddAsync(m3);
            await _claimsRepository.AddAsync(m4);
            await _claimsRepository.AddAsync(m5);
            await _claimsRepository.AddAsync(m6);
            await _claimsRepository.AddAsync(m7);
            await _claimsRepository.AddAsync(m8);
            await _claimsRepository.AddAsync(m9);
            await _claimsRepository.AddAsync(m10);
            await _claimsRepository.AddAsync(m11);
            await _claimsRepository.AddAsync(m12);
            await _claimsRepository.AddAsync(m13);
            await _claimsRepository.AddAsync(m14);
            await _claimsRepository.AddAsync(m15);
            await _claimsRepository.AddAsync(m16);
            await _claimsRepository.AddAsync(m17);
            await _claimsRepository.AddAsync(m18);
            await _claimsRepository.AddAsync(m19);
            await _claimsRepository.AddAsync(m20);
            await _claimsRepository.AddAsync(m21);
            await _claimsRepository.AddAsync(m22);
            await _claimsRepository.AddAsync(m23);
            await _claimsRepository.AddAsync(m24);
            await _claimsRepository.AddAsync(m25);
            await _claimsRepository.AddAsync(m26);
            await _claimsRepository.AddAsync(m27);
            await _claimsRepository.AddAsync(m28);
            await _claimsRepository.AddAsync(m29);
            await _claimsRepository.AddAsync(m30);
            await _claimsRepository.AddAsync(m31);
            await _claimsRepository.AddAsync(m32);
            await _claimsRepository.AddAsync(m33);
            await _claimsRepository.AddAsync(m34);
        }
    }
}
