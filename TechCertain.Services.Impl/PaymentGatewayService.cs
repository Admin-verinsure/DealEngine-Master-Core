using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        IMapperSession<PaymentGateway> _paymentGatewayRepository;

        public PaymentGatewayService(IMapperSession<PaymentGateway> paymentGatewayRepository)
        {
            _paymentGatewayRepository = paymentGatewayRepository;
        }

        public void AddNewPaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType)
        {
            if (string.IsNullOrWhiteSpace(paymentGatewayName))
                throw new ArgumentNullException(nameof(paymentGatewayName));
            if (string.IsNullOrWhiteSpace(paymentGatewayWebServiceURL))
                throw new ArgumentNullException(nameof(paymentGatewayWebServiceURL));
           
            _paymentGatewayRepository.AddAsync(new PaymentGateway(createdBy, paymentGatewayName, paymentGatewayWebServiceURL, paymentGatewayResponsePageURL, paymentGatewayType));
        }

        public IQueryable<PaymentGateway> GetAllPaymentGateways()
        {
            // find all payment gateways that haven't been deleted.
            var paymentGateways = _paymentGatewayRepository.FindAll();
            return paymentGateways.Where(pgws => pgws.DateDeleted == null).OrderBy(pgws => pgws.PaymentGatewayName);
        }

        public void RemovePaymentGateway(User deletedBy, string paymentGatewayWebServiceURL)
        {
            // find payment gateway that matches the specified Web Service URL, and delete it
            PaymentGateway paymentGateway = GetAllPaymentGateways().FirstOrDefault(pgw => pgw.PaymentGatewayWebServiceURL == paymentGatewayWebServiceURL);
            if (paymentGateway != null)
            {
                paymentGateway.Delete(deletedBy);
                _paymentGatewayRepository.RemoveAsync(paymentGateway);
            }
        }

        public PaymentGateway GetPaymentGateway(Guid paymentGatewayId)
        {
            return GetAllPaymentGateways().FirstOrDefault(pgw => pgw.PaymentGatewayId == paymentGatewayId);
        }
    }
}
