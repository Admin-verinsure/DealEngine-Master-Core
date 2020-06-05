﻿using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class ClientAgreementEndorsementService : IClientAgreementEndorsementService
    {
        IMapperSession<ClientAgreementEndorsement> _clientAgreementEndorsementRepository;
        IMapperSession<ClientAgreement> _clientAgreementRepository;

        public ClientAgreementEndorsementService(IMapperSession<ClientAgreementEndorsement> clientAgreementEndorsementRepository, IMapperSession<ClientAgreement> clientAgreementRepository)
        {
            _clientAgreementEndorsementRepository = clientAgreementEndorsementRepository;
            _clientAgreementRepository = clientAgreementRepository;
        }

        public async Task AddClientAgreementEndorsement(User createdBy, string name, string type, Product product, string value, int orderNumber, ClientAgreement clientAgreement)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(orderNumber.ToString()))
                throw new ArgumentNullException(nameof(orderNumber));
            if (clientAgreement == null)
                throw new ArgumentNullException(nameof(clientAgreement));

            ClientAgreementEndorsement clientAgreementEndorsement = new ClientAgreementEndorsement(createdBy, name, type, product, value, orderNumber, clientAgreement);
            clientAgreement.ClientAgreementEndorsements.Add(clientAgreementEndorsement);
            await _clientAgreementEndorsementRepository.AddAsync(clientAgreementEndorsement);
            await _clientAgreementRepository.UpdateAsync(clientAgreement);
        }


        public async Task<List<ClientAgreementEndorsement>> GetAllClientAgreementEndorsementFor(ClientAgreement clientAgreement)
        {
            return await _clientAgreementEndorsementRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement).ToListAsync();            
        }

        public async Task<ClientAgreementEndorsement> GetClientAgreementEndorsementBy(Guid clientAgreementEndorsementId)
        {
            return await _clientAgreementEndorsementRepository.GetByIdAsync(clientAgreementEndorsementId);
        }

        public async Task<ClientAgreementEndorsement> GetClientAgreementEndorsementByName(ClientAgreement clientAgreement ,string clientAgreementEndorsementName)
        {
            return await _clientAgreementEndorsementRepository.FindAll().FirstOrDefaultAsync(c => c.Name == clientAgreementEndorsementName && c.ClientAgreement == clientAgreement); ;
        }

    }
}