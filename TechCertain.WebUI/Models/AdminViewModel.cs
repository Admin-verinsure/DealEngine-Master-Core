using System;
using System.Collections.Generic;


namespace TechCertain.WebUI.Models
{
    public class AdminViewModel : BaseViewModel
    {
        public IList<PrivateServerViewModel> PrivateServers { get; set; }

        public IList<PaymentGatewayViewModel> PaymentGateways { get; set; }

        public IList<MerchantViewModel> Merchants { get; set; }
    }
}