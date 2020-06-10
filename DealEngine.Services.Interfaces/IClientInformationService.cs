﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
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
		Task SaveAnswersFor(ClientInformationSheet sheet, IFormCollection collection, User user);
        Task<List<ClientInformationSheet>> FindByBoatName(string searchValue);
        Task<SubClientInformationSheet> IssueSubInformationFor(ClientInformationSheet clientInformationSheet);
        Task<bool> IsBaseClass(ClientInformationSheet sheet);
        Task UnlockSheet(ClientInformationSheet sheet, User user);
        Task<SubClientInformationSheet> GetSubInformationSheetFor(Organisation principal);
        Task<List<ClientInformationSheet>> FindByAdvisoryName(string searchValue);
        Task<ClientInformationSheet> GetInformationSheetforOrg(Organisation organisation);
        Task RemoveOrganisationFromSheets(Organisation organisation);
    }
}

