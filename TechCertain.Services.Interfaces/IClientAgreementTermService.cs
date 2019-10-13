using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementTermService
    {

        void AddAgreementTerm(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType);

        IQueryable<ClientAgreementTerm> GetAllAgreementTermFor(ClientAgreement clientAgreement);

        IList<ClientAgreementTerm> GetListAgreementTermFor(ClientAgreement clientAgreement);

        void UpdateAgreementTerm(ClientAgreementTerm clientAgreementTerm);

        void DeleteAgreementTerm(User deletedBy, ClientAgreementTerm clientAgreementTerm);
    }
}
