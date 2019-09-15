using System;
using System.Collections.Generic;


namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        public IList<PrivateServerViewModel> PrivateServers { get; set; }

        public IList<PaymentGatewayViewModel> PaymentGateways { get; set; }

        public IList<MerchantViewModel> Merchants { get; set; }
    }
}