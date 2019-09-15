using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IPaymentGatewayService
    {

        bool AddNewPaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType);

        bool RemovePaymentGateway(User deletedBy, string paymentGatewayWebServiceURL);

        bool CheckExists(string paymentGatewayWebServiceURL);

        IQueryable<PaymentGateway> GetAllPaymentGateways();

        PaymentGateway GetPaymentGateway(Guid paymentGatewayId);
    }
}
