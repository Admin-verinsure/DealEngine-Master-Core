using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class RuleService : IRuleService
    {
        IUnitOfWorkFactory _unitOfWork;
        IRepository<Rule> _ruleRepository;

        public RuleService(IUnitOfWorkFactory unitOfWork, IRepository<Rule> ruleRepository)
        {
            _unitOfWork = unitOfWork;
            _ruleRepository = ruleRepository;
        }

        public bool AddRule(User createdBy, string name, string description, Product product, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
				Rule rule = new Rule(createdBy, name, description, product, value);
                product.Rules.Add(rule);
                work.Commit();
            }

            return true;
        }


        public IQueryable<Rule> GetAllRuleFor(Product product)
        {
            var rule = _ruleRepository.FindAll().Where(cagt => cagt.Product == product);
            return rule;
        }

        public Rule GetRuleByID(Guid Id)
        {
            Rule rule = _ruleRepository.GetById(Id);
            return rule;
        }
    }
}
