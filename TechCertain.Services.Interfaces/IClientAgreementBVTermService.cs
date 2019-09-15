using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementBVTermService
    {
        bool AddAgreementBVTerm(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTerm clientAgreementTerm, Boat boat);

        IQueryable<ClientAgreementBVTerm> GetAllAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm);

        bool UpdateAgreementBVTerm(ClientAgreementBVTerm clientAgreementBVTerm);

        bool DeleteAgreementBVTerm(User deletedBy, ClientAgreementBVTerm clientAgreementBVTerm);

        IQueryable<ClientAgreementBVTerm> GetAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm, Boat boat);
    }
}
