using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IRuleService
    {
        Task AddRule(User createdBy, string name, string description, Product product, string value);
        Task<List<Rule>> GetAllRuleFor(Product product);
        Task<Rule> GetRuleByID(Guid Id);
        Task<IEnumerable<Rule>> GetAllRules();
    }
}
