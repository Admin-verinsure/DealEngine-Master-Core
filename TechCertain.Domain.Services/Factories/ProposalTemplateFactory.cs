using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Domain.Services.Factories
{
    public class ProposalTemplateFactory : IEntityFactory
    {
        readonly IMapperSession<ProposalTemplate> _proposalTemplateRepository;

        public ProposalTemplateFactory(IMapperSession<ProposalTemplate> proposalTemplateRepository)
        {
            _proposalTemplateRepository = proposalTemplateRepository;
        }

        public ProposalTemplate CreateProposalTemplate(User createdBy, Owner owner, string name, bool isPrivate, Organisation organisation)
        {
            if (_proposalTemplateRepository.FindAll().Any(c => c.Name == name))
            {
                throw new ArgumentException("There is already a proposal template with that name!");
            }

            return new ProposalTemplate(createdBy, owner, name, isPrivate);
        }
    }
}
