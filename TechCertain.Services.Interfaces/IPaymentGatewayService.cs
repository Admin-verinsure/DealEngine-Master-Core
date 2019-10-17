using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IPaymentGatewayService
    {

        void AddNewPaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType);
        void RemovePaymentGateway(User deletedBy, string paymentGatewayWebServiceURL);
        IQueryable<PaymentGateway> GetAllPaymentGateways();
        PaymentGateway GetPaymentGateway(Guid paymentGatewayId);
    }
}
