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
		IUnitOfWorkFactory _unitOfWork;
		ILogger _logging;
		IRepository<ClientInformationAnswer> _clientInfomationsheetAnswer;
		ILdapService _ldapService;

		public ClientInformationAnswerService(IUnitOfWorkFactory unitOfWork, ILogger logging, IRepository<ClientInformationAnswer> clientInfomationsheetAnswer, ILdapService ldapService)
        {
			_unitOfWork = unitOfWork;
			_logging = logging;
            _clientInfomationsheetAnswer = clientInfomationsheetAnswer;
			_ldapService = ldapService;
		}

		public ClientInformationAnswer CreateNewClaimHistory(ClientInformationAnswer clientInformationAnswer)
		{
            _clientInfomationsheetAnswer.Add(clientInformationAnswer);
             return clientInformationAnswer;
		}

		public ClientInformationAnswer CreateNewClaimHistory( string ClaimName,string value, ClientInformationSheet InformationSheetID)
		{
			ClientInformationAnswer answer = new ClientInformationAnswer(null, ClaimName,  value, InformationSheetID);
            // TODO - finish this later since I need to figure out what calls the controller function that calls this service function
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

