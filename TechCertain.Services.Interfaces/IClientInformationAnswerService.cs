using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientInformationAnswerService
    {
        Task<ClientInformationAnswer> CreateNewClaimHistory(ClientInformationAnswer clientInformationAnswer);

        Task<ClientInformationAnswer> CreateNewClaimHistory(string ClaimName, string value, string details, ClientInformationSheet InformationSheetID);

        Task<ClientInformationAnswer> CreateNewSheetAns(string ClaimName, string value, ClientInformationSheet InformationSheetID);

        Task<ClientInformationAnswer> GetClaimHistoryByName(string ClaimName, Guid InformationSheetID);

        Task<ClientInformationAnswer> GetSheetAnsByName(string ClaimName, Guid InformationSheetID);
        Task<List<ClientInformationAnswer>> GetAllSheetAns();

        Task<List<ClientInformationAnswer>> GetAllClaimHistory();
    }
}

