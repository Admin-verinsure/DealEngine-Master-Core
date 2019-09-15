using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class PaymentService : IPaymentService
    {
        IUnitOfWorkFactory _unitOfWork;
        IRepository<Payment> _paymentRepository;

        public PaymentService(IUnitOfWorkFactory unitOfWork, IRepository<Payment> paymentRepository)
        {
            _unitOfWork = unitOfWork;
            _paymentRepository = paymentRepository;
        }

        public void Update(Payment payment)
        {
            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _paymentRepository.Update(payment);
                work.Commit();
            }
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

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _paymentRepository.Add(payment);
                work.Commit();
            }

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
