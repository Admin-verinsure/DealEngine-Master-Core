using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IProgrammeService
	{
        Task<Programme> GetProgramme (Guid id);
        Task<Programme> GetCoastGuardProgramme();
        Task<List<Programme>> GetProgrammesByOwner (Guid ownerOrganisationId);
		Task<ClientProgramme> GetClientProgramme (Guid id);
        Task<List<ClientProgramme>> GetClientProgrammesByOwner (Guid ownerOrganisationId);
		Task<List<ClientProgramme>> GetClientProgrammesForProgramme (Guid programmeId);
        Task<ClientProgramme> CreateClientProgrammeFor (Guid programmeId, User creatingUser, Organisation owner);
        Task<ClientProgramme> CreateClientProgrammeFor (Programme programme, User creatingUser, Organisation owner);
		Task Update (params ClientProgramme[] clientProgrammes);
        Task Update(Programme programmes);
        Task<ClientProgramme> CloneForUpdate (ClientProgramme clientProgramme, User cloningUser, ChangeReason changeReason);
        Task<ClientProgramme> CloneForRewenal (ClientProgramme clientProgramme, User cloningUser);
        Task AttachProgrammeToActivities(Programme programme, BusinessActivityTemplate businessActivityTemplate);
        Task<List<Programme>> GetAllProgrammes();
        Task<Programme> GetProgrammeById(Guid ProgrammeId);
        Task AttachProgrammeToTerritory(Programme programme, TerritoryTemplate territoryTemplate);
        Task AttachProgrammeToSharedRole(Programme programme, SharedDataRoleTemplate sharedRole);
        Task AddClaimNotificationByMembership(ClaimNotification claimNotification);
        Task AddBusinessContractByMembership(BusinessContract businessContract);
        Task AddPreRenewOrRefDataByMembership(PreRenewOrRefData preRenewOrRefData);
        Task<ClientProgramme> GetClientProgrammebyId(Guid clientProgrammeID);
        Task<List<ClientProgramme>> FindByOwnerName(string insuredName);
        Task<SubClientProgramme> CreateSubClientProgrammeFor(Guid programmeId);
        Task<bool> HasProgrammebyMembership(string membershipNumber);
        Task<SubClientProgramme> GetSubClientProgrammebyId(Guid subClientProgrammeId);
        Task<bool> IsBaseClass(ClientProgramme clientProgramme);
        Task<bool> SubsystemCompleted(ClientProgramme clientProgramme);
        Task<List<ClientProgramme>> GetSubClientProgrammesForProgramme(Guid programmeId);
        Task AttachProgrammeToDataRole(Programme programme, SharedDataRoleTemplate template);
        Task<ClientInformationSheet> CreateUIS(Guid programmeId, User user, Organisation organisation);
    }
}

