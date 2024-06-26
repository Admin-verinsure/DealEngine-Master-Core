﻿using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
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
            var PaymentGatewayList = await GetAllPaymentGateways();
            PaymentGateway paymentGateway = PaymentGatewayList.FirstOrDefault(pgw => pgw.PaymentGatewayWebServiceURL == paymentGatewayWebServiceURL);
            if (paymentGateway != null)
            {
                paymentGateway.Delete(deletedBy);
                await _paymentGatewayRepository.UpdateAsync(paymentGateway);
            }
        }

        public async Task<PaymentGateway> GetPaymentGateway(Guid paymentGatewayId)
        {
            return await _paymentGatewayRepository.GetByIdAsync(paymentGatewayId);
        }
    }
}
