using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class ClientProgramme : EntityBase, IAggregateRoot
    {
        private Organisation organisation;
        private ClientInformationSheet clientInformationSheet;
        private Programme programme;
        private User user;
        private string v1;
        private string v2;
        private string v3;
        private string v4;
        private string v5;
        private string v6;

        public virtual Organisation Owner { get; protected set; }
        public virtual Programme BaseProgramme { get; set; }
        public virtual ClientInformationSheet InformationSheet { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual User BrokerContactUser { get; set; }
        public virtual ChangeReason ChangeReason { get; set; }
        public virtual IDictionary<Product, bool> Products { get; protected set; }
        public virtual IList<ClientAgreement> Agreements { get; protected set; }
        public virtual IList<EGlobalSubmission> ClientAgreementEGlobalSubmissions { get; set; }
        public virtual IList<EGlobalResponse> ClientAgreementEGlobalResponses { get; set; }
        public virtual IList<SubClientProgramme> SubClientProgrammes { get; set; }       
        public virtual bool HasEGlobalCustomDescription { get; set; }
        public virtual string PaymentType { get; set; }
        public virtual string EGlobalBranchCode { get; set; }
        public virtual string EGlobalClientNumber { get; set; }
        public virtual string EGlobalClientStatus { get; set; }
        public virtual string EGlobalCustomDescription { get; set; }
        public virtual string ClientProgrammeMembershipNumber { get; set; }
        protected ClientProgramme() : this(null, null, null) { }

        public ClientProgramme (User createdBy, Organisation createdFor, Programme baseProgramme)
			: base(createdBy)
		{
			Owner = createdFor;
			BaseProgramme = baseProgramme;
			Agreements = new List<ClientAgreement> ();
            ClientAgreementEGlobalSubmissions = new List<EGlobalSubmission>();
            ClientAgreementEGlobalResponses = new List<EGlobalResponse>();
            SubClientProgrammes = new List<SubClientProgramme>();
        }

        //public ClientProgramme(User creatingUser, 
        //    Organisation organisation, 
        //    ClientInformationSheet clientInformationSheet, 
        //    Programme programme, 
        //    User user, string v1, string v2, string v3, string v4, string v5, string v6)
        //    :base(creatingUser)
        //{
        //    Owner = organisation;
        //    InformationSheet = clientInformationSheet;
        //    BaseProgramme = programme;
        //    BrokerContactUser = user;
        //    PaymentType = v1;
        //    EGlobalBranchCode = v2;
        //    EGlobalClientNumber = v3;
        //    EGlobalClientStatus = v4;
        //    EGlobalCustomDescription = v5;
        //    ClientProgrammeMembershipNumber = v6;
        //}

        public virtual IEnumerable<Product> GetSelectedProducts ()
		{
			return Products.Where((arg) => arg.Value).Select((arg) => arg.Key);
			// because I can't quite figure out the Linq for this, so  use this instead
			//return from product in Products where product.Value == true select product.Key;
		}
	}

    public class SubClientProgramme : ClientProgramme
    {
        public virtual ClientProgramme BaseClientProgramme { get; set; }
        protected SubClientProgramme() : this(null) { }
        public SubClientProgramme(ClientProgramme baseClientProgramme)            
        {            
            BaseClientProgramme = baseClientProgramme;
        }
        
        public virtual void CopyClientProgramme(ClientProgramme clientProgramme, Organisation organisation)
        {
            Owner = organisation;            
            BaseProgramme = clientProgramme.BaseProgramme;
            BrokerContactUser = clientProgramme.BrokerContactUser;
            PaymentType = clientProgramme.PaymentType;
            EGlobalBranchCode = clientProgramme.EGlobalBranchCode;
            EGlobalClientNumber = clientProgramme.EGlobalClientNumber;
            EGlobalClientStatus = clientProgramme.EGlobalClientStatus;
            EGlobalCustomDescription = clientProgramme.EGlobalCustomDescription;
            ClientProgrammeMembershipNumber = clientProgramme.ClientProgrammeMembershipNumber;
        }
    }
}

