using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientInformationAnswerService
    {
        ClientInformationAnswer CreateNewClaimHistory(ClientInformationAnswer clientInformationAnswer);

        ClientInformationAnswer CreateNewClaimHistory( string ClaimName, string value, ClientInformationSheet InformationSheetID);

        ClientInformationAnswer GetClaimHistoryByName(string ClaimName, Guid InformationSheetID);

        IQueryable<ClientInformationAnswer> GetAllClaimHistory();
    }
}

