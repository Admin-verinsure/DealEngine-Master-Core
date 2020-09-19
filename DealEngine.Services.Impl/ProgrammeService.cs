using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using FluentNHibernate.Conventions;
using NHibernate.Util;

namespace DealEngine.Services.Impl
{
    public class ProgrammeService : IProgrammeService
    {
        IMapperSession<Programme> _programmeRepository;
        IMapperSession<ClientProgramme> _clientProgrammeRepository;
        IClientInformationService _clientInformationService;
        IReferenceService _referenceService;
        ICloneService _cloneService;

        public ProgrammeService(IMapperSession<Programme> programmeRepository,
            IClientInformationService clientInformationService,
            IMapperSession<ClientProgramme> clientProgrammeRepository,
            IReferenceService referenceService,
            ICloneService cloneService
            )
        {
            _cloneService = cloneService;
            _clientInformationService = clientInformationService;
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
            clientProgramme.BrokerContactUser = programme.BrokerContactUser;
            await Update(clientProgramme);
            return clientProgramme;
        }

        public async Task<ClientProgramme> GetClientProgramme(Guid id)
        {
            return await _clientProgrammeRepository.GetByIdAsync(id);
        }

        public async Task<List<ClientProgramme>> GetClientProgrammesByOwner(Guid ownerOrganisationId)
        {
            var list = await _clientProgrammeRepository.FindAll().Where(cp => cp.Owner.Id == ownerOrganisationId && cp.InformationSheet != null).ToListAsync();
            return list;
        }

        public async Task<List<ClientProgramme>> GetClientProgrammesForProgramme(Guid programmeId)
        {
            Programme programme = await GetProgramme(programmeId);
            var clientList = new List<ClientProgramme>();
            if (programme == null)
                return null;
            foreach (var client in programme.ClientProgrammes)
            {
                var isBaseClass = await IsBaseClass(client);
                if (isBaseClass)
                {
                    if (client.DateDeleted == null)
                    {
                        clientList.Add(client);
                    }
                }
            }

            return clientList;
        }

        public async Task<List<ClientProgramme>> GetSubClientProgrammesForProgramme(Guid programmeId)
        {
            Programme programme = await GetProgramme(programmeId);
            var clientList = new List<ClientProgramme>();
            if (programme == null)
                return null;
            foreach (var client in programme.ClientProgrammes)
            {
                var isBaseClass = await IsBaseClass(client);
                if (!isBaseClass)
                {
                    if (client.DateDeleted == null)
                    {
                        clientList.Add(client);
                    }
                }
            }

            return clientList;
        }


        public async Task<bool> IsBaseClass(ClientProgramme client)
        {
            var objectType = client.GetType();
            if (!objectType.IsSubclassOf(typeof(ClientProgramme)))
            {
                return true;
            }
            return false;
        }

        public async Task<Programme> GetProgramme(Guid id)
        {
            return await _programmeRepository.GetByIdAsync(id);
        }

        public async Task<List<Programme>> GetProgrammesByOwner(Guid ownerOrganisationId)
        {
            return await _programmeRepository.FindAll().Where(p => p.Owner.Id == ownerOrganisationId || p.IsPublic == true).ToListAsync();
        }

        public async Task<Programme> GetCoastGuardProgramme()
        {
            return await _programmeRepository.FindAll().FirstOrDefaultAsync(p => p.Name == "First Mate Cover");
        }

        public async Task Update(params ClientProgramme[] clientProgrammes)
        {
            foreach (ClientProgramme clientProgramme in clientProgrammes)
            {
                await _clientProgrammeRepository.AddAsync(clientProgramme);
            }

        }
        
        public async Task<ClientProgramme> CloneForRewenal (ClientProgramme clientProgramme, User cloningUser)
		{
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(_cloneService.GetCloneProfile());
            });

            var cloneMapper = mapperConfiguration.CreateMapper();
            ClientProgramme newClientProgramme = await CreateClientProgrammeFor(clientProgramme.BaseProgramme, cloningUser, clientProgramme.Owner);
			newClientProgramme.InformationSheet = clientProgramme.InformationSheet.CloneForRenewal (cloningUser, cloneMapper);
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

        public async Task AttachProgrammeToSharedRole(Programme programme, SharedDataRoleTemplate sharedRole)
        {
            programme.SharedDataRoleTemplates.Add(sharedRole);
            await _programmeRepository.UpdateAsync(programme);
        }

        public async Task AddClaimNotificationByMembership(ClaimNotification claimNotification)
        {
            var clientProgramme = await _clientProgrammeRepository.FindAll().FirstOrDefaultAsync(c => c.ClientProgrammeMembershipNumber == claimNotification.ClaimMembershipNumber);
            if (clientProgramme != null)
            {
                clientProgramme.InformationSheet.ClaimNotifications.Add(claimNotification);
                await _clientProgrammeRepository.UpdateAsync(clientProgramme);
            }
        }

        public async Task AddBusinessContractByMembership(BusinessContract businessContract)
        {
            var clientProgramme = await _clientProgrammeRepository.FindAll().FirstOrDefaultAsync(c => c.ClientProgrammeMembershipNumber == businessContract.MembershipNumber);
            if (clientProgramme != null)
            {
                clientProgramme.InformationSheet.BusinessContracts.Add(businessContract);
                await _clientProgrammeRepository.UpdateAsync(clientProgramme);
            }
        }

        public async Task AddPreRenewOrRefDataByMembership(PreRenewOrRefData preRenewOrRefData)
        {
            var clientProgramme = await _clientProgrammeRepository.FindAll().FirstOrDefaultAsync(c => c.ClientProgrammeMembershipNumber == preRenewOrRefData.RefField);
            if (clientProgramme != null)
            {
                clientProgramme.InformationSheet.PreRenewOrRefDatas.Add(preRenewOrRefData);
                await _clientProgrammeRepository.UpdateAsync(clientProgramme);
            }
        }

        public async Task<ClientProgramme> GetClientProgrammebyId(Guid clientProgrammeID)
        {
            return await _clientProgrammeRepository.GetByIdAsync(clientProgrammeID);
        }

        public async Task<SubClientProgramme> CreateSubClientProgrammeFor(Guid programmeId)
        {
            var programme = await GetClientProgrammebyId(programmeId);
            return await CreateSubClientProgrammeFor(programme);
        }

        public async Task<SubClientProgramme> CreateSubClientProgrammeFor(ClientProgramme programme)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(_cloneService.GetCloneProfile());
            });

            var cloneMapper = mapperConfiguration.CreateMapper();
            SubClientProgramme subClientProgramme = cloneMapper.Map<SubClientProgramme>(programme);
            return subClientProgramme;
        }

        public async Task<bool> HasProgrammebyMembership(string membershipNumber)
        {
            var clientProgramme = await _clientProgrammeRepository.FindAll().FirstOrDefaultAsync(c => c.ClientProgrammeMembershipNumber == membershipNumber);
            if (clientProgramme == null)
            {
                return false;
            }
            return true;
        }

        public async Task<SubClientProgramme> GetSubClientProgrammebyId(Guid subClientProgrammeId)
        {
            return (SubClientProgramme)await _clientProgrammeRepository.GetByIdAsync(subClientProgrammeId);
        }

        public async Task<bool> SubsystemCompleted(ClientProgramme clientProgramme)
        {
            foreach (var subClient in clientProgramme.SubClientProgrammes)
            {
                if (subClient.InformationSheet.Status != "Submitted")
                {
                    return false;
                }
            }

            return true;
        }

        public async Task Update(Programme programmes)
        {
            await _programmeRepository.UpdateAsync(programmes);
        }

        public async Task AttachProgrammeToDataRole(Programme programme, SharedDataRoleTemplate template)
        {
            if (!programme.SharedDataRoleTemplates.Contains(template))
            {
                programme.SharedDataRoleTemplates.Add(template);
                await _programmeRepository.UpdateAsync(programme);
            }
        }

        public async Task<ClientInformationSheet> CreateUIS(Guid programmeId, User user, Organisation organisation)
        {
            var ClientProgramme = await CreateClientProgrammeFor(programmeId, user, organisation);
            var Reference = await _referenceService.GetLatestReferenceId();
            var Sheet = await _clientInformationService.IssueInformationFor(user, organisation, ClientProgramme, Reference);
            return Sheet;
        }

        public async Task<bool> AddOrganisationByMembership(Organisation organisation, string membership)
        {
            var clientProgramme = await _clientProgrammeRepository.FindAll().FirstOrDefaultAsync(c => c.ClientProgrammeMembershipNumber == membership);
            if (clientProgramme != null)
            {
                clientProgramme.InformationSheet.Organisation.Add(organisation);
                await _clientProgrammeRepository.UpdateAsync(clientProgramme);
                return true;
            }
            return false;
        }

        public async Task<SubClientProgramme> GetSubClientProgrammeFor(Organisation Owner)
        {
            return (SubClientProgramme)await _clientProgrammeRepository.FindAll().FirstOrDefaultAsync(c => c.Owner == Owner);
        }

        private async Task<List<Programme>> GetProgrammes(IFormCollection collection)
        {
            var programmes = new List<Programme>();
            foreach (var Key in collection.Keys)
            {
                Guid.TryParse(collection[Key], out Guid Id);
                if (Id != null)
                {
                    var Programme = await GetProgrammeById(Id);
                    if (Programme != null)
                    {
                        programmes.Add(Programme);
                    }
                }

            }
            return programmes;
        }

        public async Task<List<ClientInformationSheet>> SearchProgrammes(IFormCollection collection)
        {
            var Value = collection["Value"].ToString();
            var Term = collection["Term"].ToString();
            var programmes = await GetProgrammes(collection);

            if (Term == "Advisor")
            {
                return await AdvisorSearch(programmes, Value);
            }
            if (Term == "Boat")
            {
                return await BoatSearch(programmes, Value);
            }
            if (Term == "Name")
            {
                return await ClientNameSearch(programmes, Value);
            }
            if (Term == "Reference")
            {
                return await ReferenceSearch(programmes, Value);
            }
            return null;
        }

        private async Task<List<ClientInformationSheet>> ReferenceSearch(List<Programme> programmes, string value)
        {
            var Sheets = new List<ClientInformationSheet>();
            foreach (var Programme in programmes)
            {
                foreach (var ClientProgrammes in Programme.ClientProgrammes.Where(c => c.InformationSheet.ReferenceId == value && c.DateDeleted == null || c.Agreements.Any(a => a.ReferenceId == value) && c.DateDeleted == null))
                {
                    Sheets.Add(ClientProgrammes.InformationSheet);
                }
            }
            return Sheets;
        }

        private async Task<List<ClientInformationSheet>> ClientNameSearch(List<Programme> programmes, string value)
        {
            var Sheets = new List<ClientInformationSheet>();
            foreach (var Programme in programmes)
            {
                foreach (var ClientProgrammes in Programme.ClientProgrammes.Where(c => c.Owner.Name == value && c.DateDeleted == null))
                {
                    Sheets.Add(ClientProgrammes.InformationSheet);
                }
            }
            return Sheets;
        }

        private async Task<List<ClientInformationSheet>> BoatSearch(List<Programme> programmes, string value)
        {
            var Sheets = new List<ClientInformationSheet>();
            foreach (var Programme in programmes)
            {
                foreach (var ClientProgrammes in Programme.ClientProgrammes.Where(c => c.InformationSheet.Boats.Any(b => b.BoatName == value) && c.DateDeleted == null))
                {
                    Sheets.Add(ClientProgrammes.InformationSheet);
                }
            }
            return Sheets;
        }

        private async Task<List<ClientInformationSheet>> AdvisorSearch(List<Programme> programmes, string value)
        {
            var Sheets = new List<ClientInformationSheet>();
            foreach (var Programme in programmes)
            {
                foreach (var ClientProgrammes in Programme.ClientProgrammes.Where(c => c.InformationSheet.Organisation.Any(s => s.InsuranceAttributes.Any(i => i.Name == "Advisor")) && c.DateDeleted == null))
                {
                    var organisation = ClientProgrammes.InformationSheet.Organisation.FirstOrDefault(o => o.Name == value);
                    if (organisation != null)
                    {
                        Sheets.Add(ClientProgrammes.InformationSheet);
                    }
                }
            }
            return Sheets;
        }

        public async Task<ClientProgramme> CloneForUpdate(User createdBy, IFormCollection formCollection)
        {            
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(_cloneService.GetCloneProfile());
            });

            var cloneMapper = mapperConfiguration.CreateMapper();

            ChangeReason changeReason = new ChangeReason(createdBy, formCollection);
            ClientProgramme PreClone = await GetClientProgramme(Guid.Parse(formCollection["DealId"]));
            ClientInformationSheet clientInformationSheet = new ClientInformationSheet(createdBy, PreClone.Owner, null);
            clientInformationSheet = cloneMapper.Map<ClientInformationSheet>(PreClone.InformationSheet);
            clientInformationSheet.ReferenceId = await _referenceService.GetLatestReferenceId();
            clientInformationSheet.IsChange = true;
            clientInformationSheet.Status = "Not Started";
            clientInformationSheet.PreviousInformationSheet = PreClone.InformationSheet;
            PreClone.Agreements.Clear();
            PreClone.DateCreated = DateTime.Now;
            PreClone.ChangeReason = changeReason;
            PreClone.InformationSheet = clientInformationSheet;
            await Update(PreClone);

            return PreClone;

        }

        public Task DeveloperTool()
        {
            throw new NotImplementedException();
        }

        public async Task<Programme> PostProgramme(User user, User broker, Programme Source, Programme Destination)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(_cloneService.GetCloneProfile());
            });
            var cloneMapper = mapperConfiguration.CreateMapper();
            Destination = cloneMapper.Map(Source, Destination);
            Destination.LastModified(user);
            Destination.BrokerContactUser = broker;
            await Update(Destination);
            return Destination;
        }
    }
}

