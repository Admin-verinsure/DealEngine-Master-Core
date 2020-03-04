using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class ClientAgreementReview : EntityBase, IAggregateRoot
    {
        protected ClientAgreementReview() : base(null) { }

        public ClientAgreementReview(User createdBy, ClientAgreement clientAgreement, string name, string description, string status, string type)
            : base(createdBy)
        {
            if (clientAgreement == null)
                throw new ArgumentNullException(nameof(clientAgreement));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentNullException(nameof(status));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));

            ClientAgreement = clientAgreement;
            Name = name;
            Description = description;
            Status = status;
            Type = type;
        }

        public virtual ClientAgreement ClientAgreement
        {
            get;
            protected set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual string Status
        {
            get;
            set;
        }

        public virtual int OrderNumber
        {
            get;
            set;
        }

        public virtual string Type
        {
            get;
            set;
        }

        public virtual DateTime Raised
        {
            get;
            set;
        }

        public virtual User RaisedBy
        {
            get;
            set;
        }

        public virtual DateTime Reviewed
        {
            get;
            set;
        }

        public virtual User ReviewedBy
        {
            get;
            set;
        }

        public virtual string ReviewedComment
        {
            get;
            set;
        }


    }
}
