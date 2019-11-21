using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IProgrammeService
	{
        Task<Programme> GetProgramme (Guid id);

        Task<Programme> GetCoastGuardProgramme();

        Task<Programme> GetProgrammesByOwner (Guid ownerOrganisationId);

		Task<ClientProgramme> GetClientProgramme (Guid id);
        Task<Programme> GetClientProgrammebyName(string programmeName);

        Task<List<ClientProgramme>> GetClientProgrammesByOwner (Guid ownerOrganisationId);


		Task<IList<ClientProgramme>> GetClientProgrammesForProgramme (Guid programmeId);

        Task<ClientProgramme> CreateClientProgrammeFor (Guid programmeId, User creatingUser, Organisation owner);

        Task<ClientProgramme> CreateClientProgrammeFor (Programme programme, User creatingUser, Organisation owner);

		Task Update (params ClientProgramme[] clientProgrammes);

        Task<ClientProgramme> CloneForUpdate (ClientProgramme clientProgramme, User cloningUser, ChangeReason changeReason);

        Task<ClientProgramme> CloneForRewenal (ClientProgramme clientProgramme, User cloningUser);
	}
}

