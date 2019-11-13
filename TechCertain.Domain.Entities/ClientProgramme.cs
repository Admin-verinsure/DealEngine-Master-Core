using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class ClientProgramme : EntityBase, IAggregateRoot
    {
        public virtual Organisation Owner { get; protected set; }
        public virtual Programme BaseProgramme { get; set; }
        public virtual IDictionary<Product, bool> Products { get; protected set; }
        public virtual ClientInformationSheet InformationSheet { get; set; }
        public virtual IList<ClientAgreement> Agreements { get; protected set; }
        public virtual IList<EGlobalSubmission> ClientAgreementEGlobalSubmissions { get; set; }
        public virtual IList<EGlobalResponse> ClientAgreementEGlobalResponses { get; set; }
        public virtual string PaymentType { get; set; }
        public virtual User BrokerContactUser { get; set; }
        protected ClientProgramme() : this(null, null, null) { }
        public virtual string EGlobalBranchCode { get; set; }
        public virtual string EGlobalClientNumber { get; set; }
        public virtual ChangeReason changeReason { get; set; }
        public virtual string EGlobalClientStatus { get; set; }
        public virtual bool HasEGlobalCustomDescription { get; set; }
        public virtual string EGlobalCustomDescription { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual string ClientProgrammeMembershipNumber { get; set; }

        public ClientProgramme (User createdBy, Organisation createdFor, Programme baseProgramme)
			: base(createdBy)
		{
			Owner = createdFor;
			BaseProgramme = baseProgramme;
			Agreements = new List<ClientAgreement> ();
            ClientAgreementEGlobalSubmissions = new List<EGlobalSubmission>();
            ClientAgreementEGlobalResponses = new List<EGlobalResponse>();
        }

		public virtual IEnumerable<Product> GetSelectedProducts ()
		{
			return Products.Where((arg) => arg.Value).Select((arg) => arg.Key);
			// because I can't quite figure out the Linq for this, so  use this instead
			//return from product in Products where product.Value == true select product.Key;
		}
	}
}

