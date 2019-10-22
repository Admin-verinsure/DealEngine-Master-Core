using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IRuleService
    {
        Task AddRule(User createdBy, string name, string description, Product product, string value);
        Task<List<Rule>> GetAllRuleFor(Product product);
        Task<Rule> GetRuleByID(Guid Id);
    }
}
