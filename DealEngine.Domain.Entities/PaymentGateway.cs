using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class PaymentGateway : EntityBase, IAggregateRoot
    {
        
        protected PaymentGateway() : base(null) { }        

        public PaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType)
            : base(createdBy)
        {
            PaymentGatewayName = paymentGatewayName;
            PaymentGatewayWebServiceURL = paymentGatewayWebServiceURL;
            PaymentGatewayResponsePageURL = paymentGatewayResponsePageURL;
            PaymentGatewayType = paymentGatewayType;
        }

        public virtual string PaymentGatewayName
        {
            get;
            set;
        }

        public virtual string PaymentGatewayWebServiceURL
        {
            get;
            set;
        }

        public virtual string PaymentGatewayResponsePageURL
        {
            get;
            set;
        }

        public virtual string PaymentGatewayType
        {
            get;
            set;
        }

        public virtual bool PaymentGatewayTestAccount
        {
            get;
            set;
        }

        public virtual Guid PaymentGatewayId
        {
            get;
            set;
        }

        public virtual string PxpayUserId
        {
            get;
            set;
        }

        public virtual string PxpayKey
        {
            get;
            set;
        }

        public virtual string PxpayUrlSuccess
        {
            get;
            set;
        }

        public virtual string PxpayUrlFail
        {
            get;
            set;
        }
    }
}

