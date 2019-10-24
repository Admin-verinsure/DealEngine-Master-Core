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
    public class ClientAgreementMVTermService : IClientAgreementMVTermService
    {
        IMapperSession<ClientAgreementMVTerm> _clientAgreementMVTermRepository;

        public ClientAgreementMVTermService(IMapperSession<ClientAgreementMVTerm> clientAgreementMVTermRepository)
        {
            _clientAgreementMVTermRepository = clientAgreementMVTermRepository;
        }

        public async Task AddAgreementMVTerm(User createdBy, string registration, string year, string make, string model, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, string fleetNumber, ClientAgreementTerm clientAgreementTerm, Vehicle vehicle, decimal burnerpremium)
        {
            if (string.IsNullOrWhiteSpace(year))
                throw new ArgumentNullException(nameof(year));
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentNullException(nameof(make));
            if (string.IsNullOrWhiteSpace(termLimit.ToString()))
                throw new ArgumentNullException(nameof(termLimit));
            if (string.IsNullOrWhiteSpace(excess.ToString()))
                throw new ArgumentNullException(nameof(excess));
            if (string.IsNullOrWhiteSpace(premium.ToString()))
                throw new ArgumentNullException(nameof(premium));
            if (string.IsNullOrWhiteSpace(fSL.ToString()))
                throw new ArgumentNullException(nameof(fSL));
            if (string.IsNullOrWhiteSpace(brokerageRate.ToString()))
                throw new ArgumentNullException(nameof(brokerageRate));
            if (string.IsNullOrWhiteSpace(brokerage.ToString()))
                throw new ArgumentNullException(nameof(brokerage));
            if (string.IsNullOrWhiteSpace(vehicleCategory.ToString()))
                throw new ArgumentNullException(nameof(vehicleCategory));
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            ClientAgreementMVTerm clientAgreementMVTerm = new ClientAgreementMVTerm(createdBy, registration, year, make, model, termLimit, excess, premium, fSL, brokerageRate, brokerage, vehicleCategory, fleetNumber, clientAgreementTerm, vehicle, burnerpremium);
            await _clientAgreementMVTermRepository.AddAsync(clientAgreementMVTerm);
        }

        public async Task<List<ClientAgreementMVTerm>> GetAllAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm)
        {
            return await _clientAgreementMVTermRepository.FindAll().Where(cagmvt => cagmvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagmvt.DateDeleted == null && cagmvt.TermCategory == "active").ToListAsync();
        }

        public async Task UpdateAgreementMVTerm(ClientAgreementMVTerm clientAgreementMVTerm)
        {
            await _clientAgreementMVTermRepository.AddAsync(clientAgreementMVTerm);
        }

		public async Task DeleteAgreementMVTerm (User deletedBy, ClientAgreementMVTerm clientAgreementMVTerm)
		{
			clientAgreementMVTerm.Delete (deletedBy);
            await _clientAgreementMVTermRepository.UpdateAsync(clientAgreementMVTerm);
			await _clientAgreementMVTermRepository.RemoveAsync(clientAgreementMVTerm);
		}

        public async Task<List<ClientAgreementMVTerm>> GetAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm, Vehicle vehicle)
        {
            return await _clientAgreementMVTermRepository.FindAll().Where(cagmvt => cagmvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagmvt.Vehicle == vehicle).ToListAsync();            
        }
    }
}
