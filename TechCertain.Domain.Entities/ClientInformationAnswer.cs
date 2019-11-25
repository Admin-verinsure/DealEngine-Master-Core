using System;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
	public class ClientInformationAnswer : EntityBase, IAggregateRoot
	{
		public virtual string Value { get; set; }

		public virtual string ItemName { get; protected set; }

        public virtual string ClaimDetails { get; set; }
        public virtual ClientInformationAnswer OriginalAnswer { get; protected set; }

        public virtual ClientInformationSheet ClientInformationSheet { get; set; }

        protected ClientInformationAnswer() : base(null) { }

        public ClientInformationAnswer(User createdBy, string itemName, string value, string claimDetails, ClientInformationSheet InformationSheetId)
            : base(createdBy)
        {
            ItemName = itemName;
            Value = value;
            ClaimDetails = claimDetails;
            ClientInformationSheet = InformationSheetId;

        }
        public ClientInformationAnswer(User createdBy, string itemName, string value,  ClientInformationSheet InformationSheetId)
           : base(createdBy)
        {
            ItemName = itemName;
            Value = value;
            ClientInformationSheet = InformationSheetId;

        }
        public ClientInformationAnswer (User createdBy, string itemName, string value)
			: base (createdBy)
		{
			ItemName = itemName;
			Value = value;
		}

		public virtual ClientInformationAnswer CloneForNewSheet (ClientInformationSheet newSheet)
		{
			//if (ClientInformationSheet == newSheet)
			//	throw new Exception ("Cannot clone answer for original information");

			ClientInformationAnswer answer = new ClientInformationAnswer (newSheet.CreatedBy, ItemName, Value);
			answer.OriginalAnswer = this;
			return answer;
		}
	}
}

