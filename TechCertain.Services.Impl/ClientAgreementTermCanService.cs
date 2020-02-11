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
    public class ClientAgreementTermCanService : IClientAgreementTermCanService
    {
        IMapperSession<ClientAgreementTermCancel> _clientAgreementTermCanRepository;
        IMapperSession<ClientAgreement> _clientAgreementRepository;

        public ClientAgreementTermCanService(IMapperSession<ClientAgreementTermCancel> clientAgreementTermCanRepository, IMapperSession<ClientAgreement> clientAgreementRepository)
        {
            _clientAgreementTermCanRepository = clientAgreementTermCanRepository;
            _clientAgreementRepository = clientAgreementRepository;
        }

        public async Task AddAgreementTermCan(User createdBy, int termLimitCan, decimal excessCan, decimal premiumCan, decimal fSLCan, decimal brokerageRateCan, decimal brokerageCan, ClientAgreement clientAgreementCan, string subTermTypeCan)
        {
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
            if (clientAgreementCan == null)
                throw new ArgumentNullException(nameof(clientAgreementCan));

            ClientAgreementTermCancel clientAgreementTermCan = new ClientAgreementTermCancel(createdBy, termLimitCan, excessCan, premiumCan, fSLCan, brokerageRateCan, brokerageCan, clientAgreementCan, subTermTypeCan);
            clientAgreementCan.ClientAgreementTermsCancel.Add(clientAgreementTermCan);
            await _clientAgreementTermCanRepository.AddAsync(clientAgreementTermCan);
            await _clientAgreementRepository.UpdateAsync(clientAgreementCan);

        }


        public async Task<List<ClientAgreementTermCancel>> GetAllAgreementTermCanFor(ClientAgreement clientAgreementCan)
        {
            return await _clientAgreementTermCanRepository.FindAll().Where(cagtCan => cagtCan.ClientAgreementCan == clientAgreementCan &&
                                                                              cagtCan.DateDeleted == null).ToListAsync();
        }

        public async Task UpdateAgreementTermCan(ClientAgreementTermCancel clientAgreementTermCan)
        {
            await _clientAgreementTermCanRepository.AddAsync(clientAgreementTermCan);
        }

        public async Task DeleteAgreementTermCan(User deletedBy, ClientAgreementTermCancel clientAgreementTermCan)
        {
            clientAgreementTermCan.Delete(deletedBy);
            await UpdateAgreementTermCan(clientAgreementTermCan);
        }

        public async Task<List<ClientAgreementTermCancel>> GetListAgreementTermCanFor(ClientAgreement clientAgreementCan)
        {
            return await _clientAgreementTermCanRepository.FindAll().Where(cagtCan => cagtCan.ClientAgreementCan == clientAgreementCan &&
                                                                              cagtCan.DateDeleted == null).ToListAsync();
        }

        public async Task<List<ClientAgreementTermCancel>> GetAllClientAgreementTermCan()
        {
            return await _clientAgreementTermCanRepository.FindAll().ToListAsync();
        }
    }
}
