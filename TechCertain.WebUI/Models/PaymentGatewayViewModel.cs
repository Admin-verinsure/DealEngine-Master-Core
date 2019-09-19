using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechCertain.Domain.Entities;


namespace TechCertain.WebUI.Models
{
    public class PaymentGatewayViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string PaymentGatewayName { get; set; }

        public string PaymentGatewayWebServiceURL { get; set; }

        public string PaymentGatewayResponsePageURL { get; set; }

        public string PaymentGatewayType { get; set; }

        
        public static PaymentGatewayViewModel FromEntity(PaymentGateway paymentGateway)
        {
            PaymentGatewayViewModel model = new PaymentGatewayViewModel
            {
                Id = paymentGateway.Id,
                PaymentGatewayName = paymentGateway.PaymentGatewayName,
                PaymentGatewayWebServiceURL = paymentGateway.PaymentGatewayWebServiceURL,
                PaymentGatewayResponsePageURL = paymentGateway.PaymentGatewayResponsePageURL,
                PaymentGatewayType = paymentGateway.PaymentGatewayType,
            };
            return model;
        }

    }
}