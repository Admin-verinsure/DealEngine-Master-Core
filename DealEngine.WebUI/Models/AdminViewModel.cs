using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DealEngine.WebUI.Models
{
    public class AdminViewModel : BaseViewModel
    {
        public IList<PrivateServerViewModel> PrivateServers { get; set; }
        public IList<PaymentGatewayViewModel> PaymentGateways { get; set; }
        public IList<MerchantViewModel> Merchants { get; set; }
        public IList<SelectListItem> LockedUsers { get; set; }
    }
}