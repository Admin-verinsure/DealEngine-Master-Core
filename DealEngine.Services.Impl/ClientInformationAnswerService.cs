using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
	public class ClientInformationAnswerService : IClientInformationAnswerService
    {	
		IMapperSession<ClientInformationAnswer> _clientInfomationsheetAnswer;

		public ClientInformationAnswerService(IMapperSession<ClientInformationAnswer> clientInfomationsheetAnswer)
        {		
            _clientInfomationsheetAnswer = clientInfomationsheetAnswer;
		}

		public async Task<ClientInformationAnswer> CreateNewClaimHistory(ClientInformationAnswer clientInformationAnswer)
		{
            await _clientInfomationsheetAnswer.AddAsync(clientInformationAnswer);
            return clientInformationAnswer;
		}
        
        public async Task<ClientInformationAnswer> CreateNewSheetAns(string ClaimName, string value, ClientInformationSheet InformationSheetID)
        {
            ClientInformationAnswer answer = new ClientInformationAnswer(null, ClaimName, value, InformationSheetID);
            // TODO - finish this later since I need to figure out what calls the controller function that calls this service function
            await _clientInfomationsheetAnswer.AddAsync(answer);

            return answer;
        }

        public async Task<ClientInformationAnswer> CreateNewSheetPMINZAns(string ClaimName, string value,string claimdetails, ClientInformationSheet InformationSheetID)
        {
            ClientInformationAnswer answer = new ClientInformationAnswer(null, ClaimName, value, claimdetails ,InformationSheetID);
            // TODO - finish this later since I need to figure out what calls the controller function that calls this service function
            await _clientInfomationsheetAnswer.AddAsync(answer);

            return answer;
        }
        public async Task<ClientInformationAnswer> CreateNewClaimHistory(string ClaimName, string value, string details, ClientInformationSheet InformationSheetID)
        {
            ClientInformationAnswer answer = new ClientInformationAnswer(null, ClaimName, value, details, InformationSheetID);
            // TODO - finish this later since I need to figure out what calls the controller function that calls this service function
            await _clientInfomationsheetAnswer.AddAsync(answer);

            return answer;
        }

        public async Task<ClientInformationAnswer> GetSheetAnsByName(string ClaimName, Guid InformationSheetID)
        {
            return await _clientInfomationsheetAnswer.FindAll().FirstOrDefaultAsync(o => o.ItemName == ClaimName && o.ClientInformationSheet.Id == InformationSheetID);
        }

        public async Task<ClientInformationAnswer> GetClaimHistoryByName(string ClaimName, Guid InformationSheetID)
        {
            return await _clientInfomationsheetAnswer.FindAll().FirstOrDefaultAsync(o => o.ItemName == ClaimName && o.ClientInformationSheet.Id== InformationSheetID);
        }
        
        public async Task<List<ClientInformationAnswer>> GetAllSheetAns()
        {
            // we don't want to query ldap. That way lies timeouts. Or Dragons.
            return await _clientInfomationsheetAnswer.FindAll().ToListAsync();
        }

        public async Task<List<ClientInformationAnswer>> GetAllClaimHistory()
        {
            // we don't want to query ldap. That way lies timeouts. Or Dragons.
            return await _clientInfomationsheetAnswer.FindAll().ToListAsync();
        }


    }
}

