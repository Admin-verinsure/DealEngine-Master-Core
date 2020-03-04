using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DealEngine.WebUI.Models
{
    public class MerchantViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string MerchantUserName { get; set; }

        public string MerchantKey { get; set; }

        public string MerchantReference { get; set; }

        public string MerchantPassword { get; set; }

        public Guid PaymentGateway { get; set; }

        public IEnumerable<PaymentGatewayViewModel> AllPaymentGateways { get; set; }

    }
}