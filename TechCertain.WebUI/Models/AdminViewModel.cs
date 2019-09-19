using System;
using System.Collections.Generic;


namespace techcertain2019core.Models.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        public IList<PrivateServerViewModel> PrivateServers { get; set; }

        public IList<PaymentGatewayViewModel> PaymentGateways { get; set; }

        public IList<MerchantViewModel> Merchants { get; set; }
    }
}