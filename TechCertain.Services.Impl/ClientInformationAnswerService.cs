using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class ClientInformationAnswerService : IClientInformationAnswerService
    {
		IUnitOfWork _unitOfWork;		
		IMapperSession<ClientInformationAnswer> _clientInfomationsheetAnswer;
		ILdapService _ldapService;

		public ClientInformationAnswerService(IUnitOfWork unitOfWork, IMapperSession<ClientInformationAnswer> clientInfomationsheetAnswer, ILdapService ldapService)
        {
			_unitOfWork = unitOfWork;			
            _clientInfomationsheetAnswer = clientInfomationsheetAnswer;
			_ldapService = ldapService;
		}

		public ClientInformationAnswer CreateNewClaimHistory(ClientInformationAnswer clientInformationAnswer)
		{
            _clientInfomationsheetAnswer.Add(clientInformationAnswer);
             return clientInformationAnswer;
		}

        public ClientInformationAnswer CreateNewClaimHistory(string ClaimName, string value, string details, ClientInformationSheet InformationSheetID)
        {
            ClientInformationAnswer answer = new ClientInformationAnswer(null, ClaimName, value, details, InformationSheetID);
            // TODO - finish this later since I need to figure out what calls the controller function that calls this service function
            _clientInfomationsheetAnswer.Add(answer);

            return answer;
        }

        public ClientInformationAnswer GetClaimHistoryByName(string ClaimName, Guid InformationSheetID)
        {
            return _clientInfomationsheetAnswer.FindAll().FirstOrDefault(o => o.ItemName == ClaimName && o.ClientInformationSheet.Id== InformationSheetID);
        }

        public IQueryable<ClientInformationAnswer> GetAllClaimHistory()
        {
            // we don't want to query ldap. That way lies timeouts. Or Dragons.
            return _clientInfomationsheetAnswer.FindAll();
        }


    }
}

