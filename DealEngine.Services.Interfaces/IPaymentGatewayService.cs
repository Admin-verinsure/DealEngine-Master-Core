using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IPaymentGatewayService
    {

        Task AddNewPaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType);
        Task RemovePaymentGateway(User deletedBy, string paymentGatewayWebServiceURL);
        Task<List<PaymentGateway>> GetAllPaymentGateways();
        Task<PaymentGateway> GetPaymentGateway(Guid paymentGatewayId);
    }
}
