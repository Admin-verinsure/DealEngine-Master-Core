﻿using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementMVTermService
    {
        bool AddAgreementMVTerm(User createdBy, string registration, string year, string make, string model, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, string fleetNumber, ClientAgreementTerm clientAgreementTerm, Vehicle vehicle, decimal burnerpremium);

        IQueryable<ClientAgreementMVTerm> GetAllAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm);

        bool UpdateAgreementMVTerm(ClientAgreementMVTerm clientAgreementMVTerm);

        bool DeleteAgreementMVTerm(User deletedBy, ClientAgreementMVTerm clientAgreementMVTerm);

        IQueryable<ClientAgreementMVTerm> GetAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm, Vehicle vehicle);
    }
}
