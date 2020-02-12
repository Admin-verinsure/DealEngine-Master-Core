using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Programme : EntityBase, IAggregateRoot
    {
        public virtual string Name { get; set; }
        public virtual Organisation Owner { get; protected set; }
        public virtual IList<Product> Products { get; protected set; }
        public virtual IList<TerritoryTemplate> TerritoryTemplates { get; set; }
        public virtual IList<BusinessActivityTemplate> BusinessActivityTemplates { get; set; }
        public virtual IList<SharedDataRoleTemplate> SharedDataRoleTemplates { get; set; }
        public virtual string Declaration { get; set; }
        public virtual IList<EmailTemplate> EmailTemplates { get; protected set; }
        public virtual IList<ClientProgramme> ClientProgrammes { get; protected set; }
        public virtual IList<Merchant> Merchants { get; protected set; }
        public virtual User BrokerContactUser { get; set; }
        public virtual Boolean IsPublic { get; set; }
        public virtual IList<Organisation> Parties { get; set; }
        public virtual IList<User> UISIssueNotifyUsers { get; set; }
        public virtual IList<User> UISSubmissionNotifyUsers { get; set; }
        public virtual IList<User> AgreementReferNotifyUsers { get; set; }
        public virtual IList<User> AgreementIssueNotifyUsers { get; set; }
        public virtual IList<User> AgreementBoundNotifyUsers { get; set; }
        public virtual IList<User> PaymentConfigNotifyUsers { get; set; }
        public virtual IList<User> InvoiceConfigNotifyUsers { get; set; }
        public virtual Boolean StopAgreement { get; set; }
        public virtual DateTime StopAgreementDateTime { get; set; }
        public virtual string StopAgreementMessage { get; set; }
        public virtual string NoPaymentRequiredMessage { get; set; }
        public virtual Boolean AllowUsesChange { get; set; }
        public virtual Boolean CalculateCancelTerm { get; set; }
        public virtual bool HasCCPayment
        {
            get;
            set;
        }

        public virtual bool HasPFPayment
        {
            get;
            set;
        }

        public virtual bool HasInvoicePayment
        {
            get;
            set;
        }

        public virtual decimal TaxRate { get; set; }
        public virtual decimal SurchargeRate { get; set; }
        public virtual bool ProgrammeEmailCCToBroker { get; set; }

        public virtual IList<Package> Packages { get; protected set; }

        public virtual Boolean UsesEGlobal { get; set; }

        public virtual string PolicyNumberPrefixString { get; set; }

        protected Programme() : this(null) { }

        public Programme(User createdBy) : base(createdBy)
        {
            SharedDataRoleTemplates = new List<SharedDataRoleTemplate>();
            BusinessActivityTemplates = new List<BusinessActivityTemplate>();
            TerritoryTemplates = new List<TerritoryTemplate>();
            Products = new List<Product>();
            EmailTemplates = new List<EmailTemplate>();
            ClientProgrammes = new List<ClientProgramme>();
            Merchants = new List<Merchant>();
            Parties = new List<Organisation>();
            UISIssueNotifyUsers = new List<User>();
            UISSubmissionNotifyUsers = new List<User>();
            AgreementReferNotifyUsers = new List<User>();
            AgreementIssueNotifyUsers = new List<User>();
            AgreementBoundNotifyUsers = new List<User>();
            PaymentConfigNotifyUsers = new List<User>();
            InvoiceConfigNotifyUsers = new List<User>();
            Packages = new List<Package>();
        }

        public virtual ClientProgramme IssueFor(Organisation clientOrganisation)
        {
            return new ClientProgramme(CreatedBy, clientOrganisation, this);
        }

        public virtual IEnumerable<Document> GetProductDocuments()
        {
            return Products.SelectMany((arg) => arg.Documents);
        }
       
	}

}

