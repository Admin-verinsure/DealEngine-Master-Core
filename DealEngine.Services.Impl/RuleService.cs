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
    public class RuleService : IRuleService
    {
        IMapperSession<Rule> _ruleRepository;
        IMapperSession<Product> _productRepository;

        public RuleService(IMapperSession<Rule> ruleRepository, IMapperSession<Product> productRepository)
        {
            _ruleRepository = ruleRepository;
            _productRepository = productRepository;
        }

        public async Task AddRule(User createdBy, string name, string description, Product product, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

			Rule rule = new Rule(createdBy, name, description, product, value);
            product.Rules.Add(rule);
            await _ruleRepository.AddAsync(rule);
            await _productRepository.UpdateAsync(product);
        }


        public async Task<List<Rule>> GetAllRuleFor(Product product)
        {
            return await _ruleRepository.FindAll().Where(cagt => cagt.Product == product).ToListAsync();
        }

        public async Task<IEnumerable<Rule>> GetAllRules()
        {
            return await _ruleRepository.FindAll().ToListAsync();
        }

        public async Task<Rule> GetRuleByID(Guid Id)
        {
            return await _ruleRepository.GetByIdAsync(Id);
        }
    }
}
