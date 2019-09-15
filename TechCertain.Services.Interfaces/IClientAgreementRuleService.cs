using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementRuleService
    {
        bool AddClientAgreementRule(User createdBy, Rule rule, string name, string description, Product product, string value, int orderNumber, string ruleCategory, string ruleRoleType, bool isPublic, ClientAgreement clientAgreement);

		bool AddClientAgreementRule(User createdBy, Rule rule, ClientAgreement clientAgreement);

		IQueryable<ClientAgreementRule> GetAllClientAgreementRuleFor(ClientAgreement clientAgreement);

        ClientAgreementRule GetClientAgreementRuleBy(Guid clientAgreementRuleId);
    }
}
