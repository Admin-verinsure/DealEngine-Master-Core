﻿using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IClientInformationService
    {
		[Obsolete]
		ClientInformationSheet IssueInformationFor (User createdBy, Organisation createdFor, InformationTemplate informationTemplate);

		ClientInformationSheet IssueInformationFor (User createdBy, Organisation createdFor, ClientProgramme clientProgramme, string reference);

		ClientInformationSheet GetInformation (Guid informationSheetId);

		IQueryable<ClientInformationSheet> GetAllInformationFor (User owner);

		IQueryable<ClientInformationSheet> GetAllInformationFor (Organisation owner);

        IQueryable<ClientInformationSheet> GetAllInformationFor(String referenceId);


        void UpdateInformation (ClientInformationSheet sheet);

		void SaveAnswersFor(ClientInformationSheet sheet, IFormCollection collection);
	}
}

