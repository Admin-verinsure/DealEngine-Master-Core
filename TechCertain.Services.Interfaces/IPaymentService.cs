using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IPaymentService
    {

        Payment AddNewPayment(User createdBy, ClientProgramme clientProgramme, Merchant merchant, PaymentGateway paymentGateway);

        IQueryable<Payment> GetAllPayment();

        [Obsolete]
        Payment GetPayment(Guid clientProgrammeID, Guid merchantID, Guid paymentGatewayID);
        void Update(Payment payment);
        Payment GetPayment(Guid clientProgrammeID);
    }
}
