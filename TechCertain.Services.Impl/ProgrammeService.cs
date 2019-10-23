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
	public class ProgrammeService : IProgrammeService
	{
		IMapperSession<Programme> _programmeRepository;
		IMapperSession<ClientProgramme> _clientProgrammeRepository;
        IReferenceService _referenceService;

        public ProgrammeService (IMapperSession<Programme> programmeRepository, IMapperSession<ClientProgramme> clientProgrammeRepository, IReferenceService referenceService)
        {
			_programmeRepository = programmeRepository;
			_clientProgrammeRepository = clientProgrammeRepository;
            _referenceService = referenceService;
        }

		public async Task<ClientProgramme> CreateClientProgrammeFor(Guid programmeId, User creatingUser, Organisation owner)
		{
			return await CreateClientProgrammeFor(GetProgramme(programmeId).Result, creatingUser, owner);
		}

		public async Task<ClientProgramme> CreateClientProgrammeFor(Programme programme, User creatingUser, Organisation owner)
		{
			ClientProgramme clientProgramme = new ClientProgramme(creatingUser, owner, programme);
			await Update(clientProgramme);
			return clientProgramme;
		}

		public async Task<ClientProgramme> GetClientProgramme(Guid id)
		{
			return await _clientProgrammeRepository.GetByIdAsync(id);
		}

		public async Task<List<ClientProgramme>> GetClientProgrammesByOwner(Guid ownerOrganisationId)
		{
			return await _clientProgrammeRepository.FindAll().Where(cp => cp.Owner.Id == ownerOrganisationId).ToListAsync();
		}

		public async Task<IList<ClientProgramme>> GetClientProgrammesForProgramme(Guid programmeId)
		{
			Programme programme = GetProgramme(programmeId).Result;
			if (programme == null)
				return null;
			return programme.ClientProgrammes;
		}

		public async Task<Programme> GetProgramme(Guid id)
		{
			return await _programmeRepository.GetByIdAsync(id);
		}

		public async Task<Programme> GetProgrammesByOwner (Guid ownerOrganisationId)
		{
            return await _programmeRepository.FindAll().FirstOrDefaultAsync(cp => cp.Owner.Id == ownerOrganisationId);
        }

        public async Task<Programme> GetCoastGuardProgramme()
        {
            return await _programmeRepository.FindAll().FirstOrDefaultAsync(p => p.Name == "Demo Coastguard Programme");
        }

        public async Task Update (params ClientProgramme [] clientProgrammes)
		{
            foreach (ClientProgramme clientProgramme in clientProgrammes)
            {
                await _clientProgrammeRepository.AddAsync(clientProgramme).ConfigureAwait(true);
            }

		}

		public async Task<ClientProgramme> CloneForUpdate (ClientProgramme clientProgramme, User cloningUser)
		{
			ClientProgramme newClientProgramme = CreateClientProgrammeFor(clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner).Result;
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForUpdate (cloningUser);
			newClientProgramme.InformationSheet.Programme = newClientProgramme;
            newClientProgramme.BrokerContactUser = clientProgramme.BrokerContactUser;
            var reference = _referenceService.GetLatestReferenceId().Result;
            newClientProgramme.InformationSheet.ReferenceId = reference;
            newClientProgramme.InformationSheet.IsChange = true;
            await _referenceService.CreateClientInformationReference(newClientProgramme.InformationSheet);

            return newClientProgramme;
		}

		public async Task<ClientProgramme> CloneForRewenal (ClientProgramme clientProgramme, User cloningUser)
		{
			ClientProgramme newClientProgramme = CreateClientProgrammeFor(clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner).Result;
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForRenewal (cloningUser);
			newClientProgramme.InformationSheet.Programme = newClientProgramme;

			return newClientProgramme;
		}
	}
}

