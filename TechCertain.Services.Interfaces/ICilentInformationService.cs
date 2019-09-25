using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface ICilentInformationService
    {
		[Obsolete]
		ClientInformationSheet IssueInformationFor (User createdBy, Organisation createdFor, InformationTemplate informationTemplate);

		ClientInformationSheet IssueInformationFor (User createdBy, Organisation createdFor, ClientProgramme clientProgramme, string reference);

		ClientInformationSheet GetInformation (Guid informationSheetId);

		IQueryable<ClientInformationSheet> GetAllInformationFor (User owner);

		IQueryable<ClientInformationSheet> GetAllInformationFor (Organisation owner);

		void UpdateInformation (ClientInformationSheet sheet);

		void SaveAnswersFor(ClientInformationSheet sheet, IFormCollection collection);
	}
}

