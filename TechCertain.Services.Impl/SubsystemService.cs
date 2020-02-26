using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using System;

namespace TechCertain.Services.Impl
{
    public class SubsystemService : ISubsystemService
    {
        IOrganisationService _organisationService;
        IUserService _userService;
        IProgrammeService _programmeService;        
        IClientInformationService _clientInformationService;

        public SubsystemService(IOrganisationService organisationService,
            IUserService userService,
            IProgrammeService programmeService,            
            IClientInformationService clientInformationService)
        {
            _programmeService = programmeService;
            _organisationService = organisationService;
            _userService = userService;            
            _clientInformationService = clientInformationService;
        }

        public async Task CreateSubObjects(Guid clientProgrammeId, ClientInformationSheet sheet)
        {
            var principalOrganisations = await _organisationService.GetOrganisationPrincipals(sheet);
            foreach (var org in principalOrganisations)
            {
                var user = await _userService.GetUserByOrganisation(org);
                var subClientProgramme = await _programmeService.CreateSubClientProgrammeFor(clientProgrammeId, org);
                var clientProgramme = await _programmeService.GetClientProgrammebyId(clientProgrammeId);
                clientProgramme.SubClientProgrammes.Add(subClientProgramme);
                await _programmeService.Update(clientProgramme);                
                
                var subSheet = await _clientInformationService.IssueSubInformationFor(subClientProgramme);                
                sheet.SubClientInformationSheets.Add(subSheet);
                await _clientInformationService.UpdateInformation(sheet);
            }            
        }
    }
}

