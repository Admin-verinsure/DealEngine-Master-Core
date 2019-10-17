using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
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

        public void AddRule(User createdBy, string name, string description, Product product, string value)
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
            _ruleRepository.AddAsync(rule);
            _productRepository.UpdateAsync(product);
        }


        public IQueryable<Rule> GetAllRuleFor(Product product)
        {
            var rule = _ruleRepository.FindAll().Where(cagt => cagt.Product == product);
            return rule;
        }

        public Rule GetRuleByID(Guid Id)
        {
            Rule rule = _ruleRepository.GetByIdAsync(Id).Result;
            return rule;
        }
    }
}
