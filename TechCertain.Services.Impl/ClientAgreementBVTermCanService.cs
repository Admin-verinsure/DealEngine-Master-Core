using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementBVTermCanService : IClientAgreementBVTermCanService
    {
        IMapperSession<ClientAgreementBVTermCancel> _clientAgreementBVTermCanRepository;

        public ClientAgreementBVTermCanService(IMapperSession<ClientAgreementBVTermCancel> clientAgreementBVTermCanRepository)
        {
            _clientAgreementBVTermCanRepository = clientAgreementBVTermCanRepository;
        }

        public async Task AddAgreementBVTermCan(User createdBy, string boatNameCan, int yearOfManufactureCan, string boatMakeCan, string boatModelCan, int termLimitCan, decimal excessCan, 
            decimal premiumCan, decimal fSLCan, decimal brokerageRateCan, decimal brokerageCan, ClientAgreementTermCancel clientAgreementTermCan, Boat boatCan)
        {
            if (string.IsNullOrWhiteSpace(boatNameCan))
                throw new ArgumentNullException(nameof(boatNameCan));
            if (string.IsNullOrWhiteSpace(termLimitCan.ToString()))
                throw new ArgumentNullException(nameof(termLimitCan));
            if (string.IsNullOrWhiteSpace(excessCan.ToString()))
                throw new ArgumentNullException(nameof(excessCan));
            if (string.IsNullOrWhiteSpace(premiumCan.ToString()))
                throw new ArgumentNullException(nameof(premiumCan));
            if (string.IsNullOrWhiteSpace(fSLCan.ToString()))
                throw new ArgumentNullException(nameof(fSLCan));
            if (string.IsNullOrWhiteSpace(brokerageRateCan.ToString()))
                throw new ArgumentNullException(nameof(brokerageRateCan));
            if (string.IsNullOrWhiteSpace(brokerageCan.ToString()))
                throw new ArgumentNullException(nameof(brokerageCan));
            if (clientAgreementTermCan == null)
                throw new ArgumentNullException(nameof(clientAgreementTermCan));
            if (boatCan == null)
                throw new ArgumentNullException(nameof(boatCan));

            ClientAgreementBVTermCancel clientAgreementBVTermCan = new ClientAgreementBVTermCancel(createdBy, boatNameCan, yearOfManufactureCan, boatMakeCan, boatModelCan, 
                termLimitCan, excessCan, premiumCan, fSLCan, brokerageRateCan, brokerageCan, clientAgreementTermCan, boatCan);
            //clientAgreementTermCan.BoatTermsCan.Add(clientAgreementBVTermCan);
            await _clientAgreementBVTermCanRepository.AddAsync(clientAgreementBVTermCan);
            
        }

        public async Task<List<ClientAgreementBVTermCancel>> GetAllAgreementBVTermCanFor(ClientAgreementTermCancel clientAgreementTermCan)
        {
            return await _clientAgreementBVTermCanRepository.FindAll().Where(cagbvtCan => cagbvtCan.ClientAgreementTermCan == clientAgreementTermCan && cagbvtCan.DateDeleted == null &&
            cagbvtCan.TermCategoryCan == "active").ToListAsync();
        }

        public async Task UpdateAgreementBVTermCan(ClientAgreementBVTermCancel clientAgreementBVTermCan)
        {
            await _clientAgreementBVTermCanRepository.AddAsync(clientAgreementBVTermCan);
        }

        public async Task DeleteAgreementBVTermCan(User deletedBy, ClientAgreementBVTermCancel clientAgreementBVTermCan)
        {
            clientAgreementBVTermCan.Delete(deletedBy);
            await UpdateAgreementBVTermCan(clientAgreementBVTermCan);
        }

        public async Task<List<ClientAgreementBVTermCancel>> GetAgreementBVTermCanFor(ClientAgreementTermCancel clientAgreementTermCan, Boat boatCan)
        {
            return await _clientAgreementBVTermCanRepository.FindAll().Where(cagbvtCan => cagbvtCan.ClientAgreementTermCan == clientAgreementTermCan &&
                                                                                    cagbvtCan.BoatCan == boatCan).ToListAsync();
        }
    }
}
