using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IPaymentService
    {

        Task<Payment> AddNewPayment(User createdBy, ClientProgramme clientProgramme, Merchant merchant, PaymentGateway paymentGateway);

        Task<List<Payment>> GetAllPayment();

        [Obsolete]
        Task<Payment> GetPayment(Guid clientProgrammeID, Guid merchantID, Guid paymentGatewayID);
        Task Update(Payment payment);
        Task<Payment> GetPayment(Guid clientProgrammeID);
    }
}
