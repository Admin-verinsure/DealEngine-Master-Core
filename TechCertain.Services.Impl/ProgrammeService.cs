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
            var programme = await GetProgramme(programmeId);
            return await CreateClientProgrammeFor(programme, creatingUser, owner);
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

        public async Task<Programme> GetClientProgrammebyName(string programmeName)
        {
            return await _programmeRepository.FindAll().FirstOrDefaultAsync(p => p.Name == "NZACS Programme");
        }


        public async Task<List<ClientProgramme>> GetClientProgrammesByOwner (Guid ownerOrganisationId)
        {
            return await _clientProgrammeRepository.FindAll().Where(cp => cp.Owner.Id == ownerOrganisationId).ToListAsync();
		}

		public async Task<IList<ClientProgramme>> GetClientProgrammesForProgramme(Guid programmeId)
		{
			Programme programme = await GetProgramme(programmeId);
            if (programme == null)
				return null;
			return programme.ClientProgrammes;
		}

		public async Task<Programme> GetProgramme(Guid id)
		{
			return await _programmeRepository.GetByIdAsync(id);
		}

		public async Task<List<Programme>> GetProgrammesByOwner (Guid ownerOrganisationId)
		{
            return await _programmeRepository.FindAll().Where(p => p.Owner.Id == ownerOrganisationId || p.IsPublic == true).ToListAsync();
        }

        public async Task<Programme> GetCoastGuardProgramme()
        {
            return await _programmeRepository.FindAll().FirstOrDefaultAsync(p => p.Name == "First Mate Cover");
        }

        public async Task Update (params ClientProgramme [] clientProgrammes)
		{
            foreach (ClientProgramme clientProgramme in clientProgrammes)
            {
                await _clientProgrammeRepository.AddAsync(clientProgramme);
            }

		}

		public async Task<ClientProgramme> CloneForUpdate (ClientProgramme clientProgramme, User cloningUser,ChangeReason changeReason)
		{
			ClientProgramme newClientProgramme = await CreateClientProgrammeFor(clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner);
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForUpdate (cloningUser);
            newClientProgramme.changeReason = changeReason;
			newClientProgramme.InformationSheet.Programme = newClientProgramme;
            newClientProgramme.BrokerContactUser = clientProgramme.BrokerContactUser;
            newClientProgramme.EGlobalClientNumber = clientProgramme.EGlobalClientNumber;
            newClientProgramme.EGlobalBranchCode = clientProgramme.EGlobalBranchCode;
            newClientProgramme.ClientProgrammeMembershipNumber = clientProgramme.ClientProgrammeMembershipNumber;
            var reference = await _referenceService.GetLatestReferenceId();
            newClientProgramme.InformationSheet.ReferenceId = reference;
            newClientProgramme.InformationSheet.IsChange = true;
            await _referenceService.CreateClientInformationReference(newClientProgramme.InformationSheet);

            return newClientProgramme;
		}

		public async Task<ClientProgramme> CloneForRewenal (ClientProgramme clientProgramme, User cloningUser)
		{
			ClientProgramme newClientProgramme = await CreateClientProgrammeFor(clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner);
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForRenewal (cloningUser);
			newClientProgramme.InformationSheet.Programme = newClientProgramme;

			return newClientProgramme;
		}

        public async Task AttachProgrammeToActivities(Programme programme, BusinessActivityTemplate businessActivityTemplate)
        {
            programme.BusinessActivityTemplates.Add(businessActivityTemplate);
            await _programmeRepository.UpdateAsync(programme);
        }

        public async Task AttachProgrammeToTerritory(Programme programme, TerritoryTemplate territoryTemplate)
        {
            programme.TerritoryTemplates.Add(territoryTemplate);
            await _programmeRepository.UpdateAsync(programme);
        }

        public async Task<List<Programme>> GetAllProgrammes()
        {
            return await _programmeRepository.FindAll().ToListAsync();
        }

        public async Task<Programme> GetProgrammeById(Guid ProgrammeId)
        {
            return await _programmeRepository.GetByIdAsync(ProgrammeId);
        }

        public async Task AttachProgrammeToharedRole(Programme programme, SharedDataRoleTemplate sharedRole)
        {
            programme.SharedDataRoleTemplates.Add(sharedRole);
            await _programmeRepository.UpdateAsync(programme);
        }
    }
}

