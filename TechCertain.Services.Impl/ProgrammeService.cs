using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class ProgrammeService : IProgrammeService
	{
		IUnitOfWorkFactory _unitOfWorkFactory;
		IRepository<Programme> _programmeRepository;
		IRepository<ClientProgramme> _clientProgrammeRepository;
        IReferenceService _referenceService;

        public ProgrammeService (IUnitOfWorkFactory unitOfWorkFactory, IRepository<Programme> programmeRepository, IRepository<ClientProgramme> clientProgrammeRepository, IReferenceService referenceService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_programmeRepository = programmeRepository;
			_clientProgrammeRepository = clientProgrammeRepository;
            _referenceService = referenceService;
        }

		public ClientProgramme CreateClientProgrammeFor (Guid programmeId, User creatingUser, Organisation owner)
		{
			return CreateClientProgrammeFor (GetProgramme (programmeId), creatingUser, owner);
		}

		public ClientProgramme CreateClientProgrammeFor (Programme programme, User creatingUser, Organisation owner)
		{
			ClientProgramme clientProgramme = new ClientProgramme (creatingUser, owner, programme);
			Update (clientProgramme);
			return clientProgramme;
		}

		public ClientProgramme GetClientProgramme (Guid id)
		{
			return _clientProgrammeRepository.GetById (id);
		}

		public IEnumerable<ClientProgramme> GetClientProgrammesByOwner (Guid ownerOrganisationId)
		{
			return _clientProgrammeRepository.FindAll ().Where (cp => cp.Owner.Id == ownerOrganisationId);
		}

		public IEnumerable<ClientProgramme> GetClientProgrammesForProgramme (Guid programmeId)
		{
			Programme programme = GetProgramme (programmeId);
			if (programme == null)
				return null;
			return programme.ClientProgrammes;
		}

		public Programme GetProgramme (Guid id)
		{
			return _programmeRepository.GetById (id);
		}

		public IEnumerable<Programme> GetProgrammesByOwner (Guid ownerOrganisationId)
		{
			return GetAllProgrammes ().Where (cp => cp.Owner.Id == ownerOrganisationId);
		}

		public IEnumerable<Programme> GetAllProgrammes ()
		{
			return _programmeRepository.FindAll ();
		}

		public void Update (params ClientProgramme [] clientProgrammes)
		{
			using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork ()) {
				foreach (ClientProgramme clientProgramme in clientProgrammes)
					_clientProgrammeRepository.Add (clientProgramme);
				uow.Commit ();
			}
		}

		public ClientProgramme CloneForUpdate (ClientProgramme clientProgramme, User cloningUser)
		{
			ClientProgramme newClientProgramme = CreateClientProgrammeFor (clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner);
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForUpdate (cloningUser);
			newClientProgramme.InformationSheet.Programme = newClientProgramme;
            newClientProgramme.BrokerContactUser = clientProgramme.BrokerContactUser;
            var reference = _referenceService.GetLatestReferenceId();
            newClientProgramme.InformationSheet.ReferenceId = reference;
            newClientProgramme.InformationSheet.IsChange = true;
            _referenceService.CreateClientInformationReference(newClientProgramme.InformationSheet);

            return newClientProgramme;
		}

		public ClientProgramme CloneForRewenal (ClientProgramme clientProgramme, User cloningUser)
		{
			ClientProgramme newClientProgramme = CreateClientProgrammeFor (clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner);
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForRenewal (cloningUser);
			newClientProgramme.InformationSheet.Programme = newClientProgramme;

			return newClientProgramme;
		}
	}
}

