using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Merchant : EntityBase, IAggregateRoot
    {
        protected Merchant() : base(null) { }

        public Merchant(User createdBy, string merchantUserName, string merchantPassword, string merchantKey, string merchantReference)
            : base(createdBy)
        {
            MerchantUserName = merchantUserName;
            MerchantKey = merchantKey;
            MerchantReference = merchantReference;
            MerchantPassword = merchantPassword;
        }

        public virtual Organisation MerchantOwner
        {
            get;
            set;
        }

        public virtual PaymentGateway MerchantPaymentGateway
        {
            get;
            set;
        }

        public virtual string MerchantUserName
        {
            get;
            set;
        }

        public virtual string MerchantKey
        {
            get;
            set;
        }

        public virtual string MerchantReference
        {
            get;
            set;
        }

        public virtual string MerchantPassword
        {
            get;
            set;
        }

        public virtual string MerchantBillerCode
        {
            get;
            set;
        }

        public virtual int MerchantPaymentFee
        {
            get;
            set;
        }

        public virtual bool MerchantUseQuoteReference
        {
            get;
            set;
        }

        public virtual Guid MerchantProgrammeId
        {
            get;
            set;
        }

        public virtual Guid Programme_id
        {
            get;
            set;
        }

        public virtual Guid MerchantPaymentGatewayId
        {
            get;
            set;
        }
    }
}

