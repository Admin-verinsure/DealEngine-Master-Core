using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IRuleService
    {
        void AddRule(User createdBy, string name, string description, Product product, string value);
        IQueryable<Rule> GetAllRuleFor(Product product); 
        Rule GetRuleByID(Guid Id);
    }
}
