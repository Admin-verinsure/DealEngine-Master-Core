using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IClientInformationService
    {
		[Obsolete]
        Task<ClientInformationSheet> IssueInformationFor (User createdBy, Organisation createdFor, InformationTemplate informationTemplate);        
        Task<ClientInformationSheet> IssueInformationFor(User createdBy, Organisation createdFor, ClientProgramme clientProgramme, string reference);
        Task<ClientInformationSheet> GetInformation (Guid informationSheetId);
        Task<List<ClientInformationSheet>> GetAllInformationFor (User owner);
        Task<List<ClientInformationSheet>> GetAllInformationFor (Organisation owner);
        Task<List<ClientInformationSheet>> GetAllInformationFor(String referenceId);
        Task UpdateInformation (ClientInformationSheet sheet);
		Task SaveAnswersFor(ClientInformationSheet sheet, IFormCollection collection);
        Task<List<ClientInformationSheet>> FindByBoatName(string searchValue);
        Task<SubClientInformationSheet> IssueSubInformationFor(SubClientProgramme subClientProgramme);
    }
}

