using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class PaymentService : IPaymentService
    {
        IMapperSession<Payment> _paymentRepository;

        public PaymentService(IMapperSession<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public void Update(Payment payment)
        {
             _paymentRepository.UpdateAsync(payment);            
        }

        public Payment AddNewPayment(User createdBy, ClientProgramme clientProgramme, Merchant merchant, PaymentGateway paymentGateway)
        {
            if (string.IsNullOrWhiteSpace(clientProgramme.ToString()))
                throw new ArgumentNullException(nameof(clientProgramme));
            if (string.IsNullOrWhiteSpace(merchant.ToString()))
                throw new ArgumentNullException(nameof(merchant));
            if (string.IsNullOrWhiteSpace(paymentGateway.ToString()))
                throw new ArgumentNullException(nameof(paymentGateway));

            Payment payment = new Payment(createdBy, clientProgramme, merchant, paymentGateway);
            _paymentRepository.AddAsync(payment);
            //check with craig best way to check
            return payment;//CheckExists(paymentGatewayWebServiceURL);
        }

        public IQueryable<Payment> GetAllPayment()
        {
            var payment = _paymentRepository.FindAll();
            return payment.Where(p => p.DateDeleted == null);
        }

        public Payment GetPayment(Guid clientProgrammeID, Guid merchantID, Guid paymentGatewayID)
        {
            Payment payment = GetAllPayment().FirstOrDefault(p => p.PaymentClientProgramme.Id == clientProgrammeID &&
                                                             p.PaymentMerchant.Id == merchantID && p.PaymentPaymentGateway.Id == paymentGatewayID);
            return payment;
        }

        public Payment GetPayment(Guid clientProgrammeID)
        {
            Payment payment = GetAllPayment().FirstOrDefault(p => p.PaymentClientProgramme.Id == clientProgrammeID);
            return payment;
        }

    }
}
