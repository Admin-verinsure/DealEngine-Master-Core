using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementTermService
    {

        bool AddAgreementTerm(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType);

        IQueryable<ClientAgreementTerm> GetAllAgreementTermFor(ClientAgreement clientAgreement);

        IList<ClientAgreementTerm> GetListAgreementTermFor(ClientAgreement clientAgreement);

        bool UpdateAgreementTerm(ClientAgreementTerm clientAgreementTerm);

        bool DeleteAgreementTerm(User deletedBy, ClientAgreementTerm clientAgreementTerm);
    }
}
