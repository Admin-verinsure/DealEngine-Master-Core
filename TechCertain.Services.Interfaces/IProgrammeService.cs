using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IProgrammeService
	{
		Programme GetProgramme (Guid id);

		IEnumerable<Programme> GetProgrammesByOwner (Guid ownerOrganisationId);

		IEnumerable<Programme> GetAllProgrammes ();

		ClientProgramme GetClientProgramme (Guid id);


        IEnumerable<ClientProgramme> GetClientProgrammesByOwner (Guid ownerOrganisationId);

		IEnumerable<ClientProgramme> GetClientProgrammesForProgramme (Guid programmeId);

		ClientProgramme CreateClientProgrammeFor (Guid programmeId, User creatingUser, Organisation owner);

		ClientProgramme CreateClientProgrammeFor (Programme programme, User creatingUser, Organisation owner);

		void Update (params ClientProgramme[] clientProgrammes);

		ClientProgramme CloneForUpdate (ClientProgramme clientProgramme, User cloningUser);

		ClientProgramme CloneForRewenal (ClientProgramme clientProgramme, User cloningUser);
	}
}

