using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task AddNewPaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType)
        {
            if (string.IsNullOrWhiteSpace(paymentGatewayName))
                throw new ArgumentNullException(nameof(paymentGatewayName));
            if (string.IsNullOrWhiteSpace(paymentGatewayWebServiceURL))
                throw new ArgumentNullException(nameof(paymentGatewayWebServiceURL));
           
            await _paymentGatewayRepository.AddAsync(new PaymentGateway(createdBy, paymentGatewayName, paymentGatewayWebServiceURL, paymentGatewayResponsePageURL, paymentGatewayType));
        }

        public async Task<List<PaymentGateway>> GetAllPaymentGateways()
        {
            // find all payment gateways that haven't been deleted.
            return await _paymentGatewayRepository.FindAll().Where(pgws => pgws.DateDeleted == null).OrderBy(pgws => pgws.PaymentGatewayName).ToListAsync();
        }

        public async Task RemovePaymentGateway(User deletedBy, string paymentGatewayWebServiceURL)
        {
            // find payment gateway that matches the specified Web Service URL, and delete it
            PaymentGateway paymentGateway = GetAllPaymentGateways().Result.FirstOrDefault(pgw => pgw.PaymentGatewayWebServiceURL == paymentGatewayWebServiceURL);
            if (paymentGateway != null)
            {
                paymentGateway.Delete(deletedBy);
                await _paymentGatewayRepository.RemoveAsync(paymentGateway);
            }
        }

        public async Task<PaymentGateway> GetPaymentGateway(Guid paymentGatewayId)
        {
            return await _paymentGatewayRepository.GetByIdAsync(paymentGatewayId);
        }
    }
}
