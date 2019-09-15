using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Payment : EntityBase, IAggregateRoot
    {
        protected Payment() : base(null) { }

        public Payment(User createdBy, ClientProgramme paymentClientProgramme, Merchant paymentMerchant, PaymentGateway paymentPaymentGateway)
            : base(createdBy)
        {
            PaymentClientProgramme = paymentClientProgramme;
            PaymentMerchant = paymentMerchant;
            PaymentPaymentGateway = paymentPaymentGateway;
        }

        public virtual ClientProgramme PaymentClientProgramme
        {
            get;
            set;
        }

        public virtual Merchant PaymentMerchant
        {
            get;
            set;
        }

        public virtual PaymentGateway PaymentPaymentGateway
        {
            get;
            set;
        }

        public virtual string PaymentCurrency
        {
            get;
            set;
        }

        public virtual decimal PaymentAmount
        {
            get;
            set;
        }

        public virtual DateTime DatePaid
        {
            get;
            set;
        }

        public virtual bool IsPaid
        {
            get;
            set;
        }

        public virtual string PaymentEmail
        {
            get;
            set;
        }

        public virtual string PaymentGatewayTransactionReference
        {
            get;
            set;
        }

        public virtual string PaymentGatewayTransactionReference2
        {
            get;
            set;
        }

        public virtual string CreditCardNumber
        {
            get;
            set;
        }

        public virtual string CreditCardType
        {
            get;
            set;
        }

        public virtual string CardHolderName
        {
            get;
            set;
        }

        public virtual string PaymentFailureMessage
        {
            get;
            set;
        }

        public virtual string PaypayUrlSuccess
        {
            get;
            set;
        }

        public virtual string PaypayUrlFail
        {
            get;
            set;
        }


        public virtual int PaymentAttempts
        {
            get;
            set;
        }

        public virtual string UrlResultReference
        {
            get;
            set;
        }

    }
}

