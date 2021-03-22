//using System;
//using System.Collections.Generic;
//using System.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace DealEngine.Services.Interfaces
{
    public interface IUpdateTypeService
    {
        Task<List<UpdateType>> GetAllUpdateTypes();
        //Task<List<PaymentGateway>> GetAllPaymentGateways();

        // Task AddAgreementTerm(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType);

        Task AddUpdateType(User createdBy,string typeName, string typeValue, bool typeIsTc, bool typeIsBroker, bool typeIsInsurer, bool typeIsClient);
        Task<UpdateType> GetUpdateType(Guid updateTypeId);
    }
}
