using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        IUnitOfWorkFactory _unitOfWork;
        IRepository<PaymentGateway> _paymentGatewayRepository;

        public PaymentGatewayService(IUnitOfWorkFactory unitOfWork, IRepository<PaymentGateway> paymentGatewayRepository)
        {
            _unitOfWork = unitOfWork;
            _paymentGatewayRepository = paymentGatewayRepository;
        }

        public bool AddNewPaymentGateway(User createdBy, string paymentGatewayName, string paymentGatewayWebServiceURL, string paymentGatewayResponsePageURL, string paymentGatewayType)
        {
            if (string.IsNullOrWhiteSpace(paymentGatewayName))
                throw new ArgumentNullException(nameof(paymentGatewayName));
            if (string.IsNullOrWhiteSpace(paymentGatewayWebServiceURL))
                throw new ArgumentNullException(nameof(paymentGatewayWebServiceURL));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _paymentGatewayRepository.Add(new PaymentGateway(createdBy, paymentGatewayName, paymentGatewayWebServiceURL, paymentGatewayResponsePageURL, paymentGatewayType));
                work.Commit();
            }

            return CheckExists(paymentGatewayWebServiceURL);
        }

        /// <exception cref="System.ArgumentNullException">Thrown when paymentGatewayName and paymentGatewayWebServiceURL is null, empty or a white space.</exception>
        public bool CheckExists(string paymentGatewayWebServiceURL)
        {
            // have we specified an WebServiceURL?
            if (string.IsNullOrWhiteSpace(paymentGatewayWebServiceURL))
                throw new ArgumentNullException(nameof(paymentGatewayWebServiceURL));
            return _paymentGatewayRepository.FindAll().FirstOrDefault(pgw => pgw.PaymentGatewayWebServiceURL == paymentGatewayWebServiceURL) != null;
        }

        public IQueryable<PaymentGateway> GetAllPaymentGateways()
        {
            // find all payment gateways that haven't been deleted.
            var paymentGateways = _paymentGatewayRepository.FindAll();
            return paymentGateways.Where(pgws => pgws.DateDeleted == null).OrderBy(pgws => pgws.PaymentGatewayName);
        }

        public bool RemovePaymentGateway(User deletedBy, string paymentGatewayWebServiceURL)
        {
            // find payment gateway that matches the specified Web Service URL, and delete it
            PaymentGateway paymentGateway = GetAllPaymentGateways().FirstOrDefault(pgw => pgw.PaymentGatewayWebServiceURL == paymentGatewayWebServiceURL);
            if (paymentGateway != null)
            {
                using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
                {
                    paymentGateway.Delete(deletedBy);
                    _paymentGatewayRepository.Add(paymentGateway);
                    work.Commit();
                }
            }
            // check that it has been removed, and return the inverse result
            return !CheckExists(paymentGatewayWebServiceURL);
        }

        public PaymentGateway GetPaymentGateway(Guid paymentGatewayId)
        {
            PaymentGateway paymentGateway = GetAllPaymentGateways().FirstOrDefault(pgw => pgw.PaymentGatewayId == paymentGatewayId);
            return paymentGateway;
        }
    }
}
