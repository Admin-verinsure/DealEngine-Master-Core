using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementRuleService : IClientAgreementRuleService
    {
        IMapperSession<ClientAgreementRule> _clientAgreementRuleRepository;
        IMapperSession<ClientAgreement> _clientAgreementRepository;

        public ClientAgreementRuleService(IMapperSession<ClientAgreementRule> clientAgreementRuleRepository, IMapperSession<ClientAgreement> clientAgreementRepository)
        {
            _clientAgreementRuleRepository = clientAgreementRuleRepository;
            _clientAgreementRepository = clientAgreementRepository;
        }

		public void AddClientAgreementRule (User createdBy, Rule rule, string name, string description, Product product, string value, int orderNumber, string ruleCategory, string ruleRoleType, bool isPublic, ClientAgreement clientAgreement)
		{
			if (string.IsNullOrWhiteSpace (name))
				throw new ArgumentNullException (nameof (name));
			if (string.IsNullOrWhiteSpace (description))
				throw new ArgumentNullException (nameof (description));
			if (product == null)
				throw new ArgumentNullException (nameof (product));
			if (string.IsNullOrWhiteSpace (value))
				throw new ArgumentNullException (nameof (value));
			if (string.IsNullOrWhiteSpace (orderNumber.ToString ()))
				throw new ArgumentNullException (nameof (orderNumber));
			if (clientAgreement == null)
				throw new ArgumentNullException (nameof (clientAgreement));

            ClientAgreementRule clientAgreementRule = new ClientAgreementRule(createdBy, rule, rule.Name, rule.Description, rule.Product, rule.Value, rule.OrderNumber, rule.RuleCategory, rule.RuleRoleType, rule.IsPublic, clientAgreement);
            clientAgreement.ClientAgreementRules.Add(clientAgreementRule);
            _clientAgreementRepository.UpdateAsync(clientAgreement);
        }

        public void AddClientAgreementRule(User createdBy, Rule rule, ClientAgreement clientAgreement)
        {
			AddClientAgreementRule(createdBy, rule, rule.Name, rule.Description, rule.Product, rule.Value, rule.OrderNumber, rule.RuleCategory, rule.RuleRoleType, rule.IsPublic, clientAgreement);
        }


        public IQueryable<ClientAgreementRule> GetAllClientAgreementRuleFor(ClientAgreement clientAgreement)
        {
            var clientAgreementRule = _clientAgreementRuleRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement);
            return clientAgreementRule;
        }

        public ClientAgreementRule GetClientAgreementRuleBy(Guid clientAgreementRuleId)
        {
            return _clientAgreementRuleRepository.GetByIdAsync(clientAgreementRuleId).Result;
        }
    }
}
