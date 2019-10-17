using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Linq;
using TechCertain.Services.Interfaces;
using System;

namespace TechCertain.Services.Impl
{
    public class ProposalBuilderService : IProposalBuilderService
    {
        readonly IMapperSession<ProposalTemplate> _proposalTemplateRepository;

        public ProposalBuilderService(IMapperSession<ProposalTemplate> proposalTemplateRepository)
        {            
            _proposalTemplateRepository = proposalTemplateRepository;
        }

        public ProposalTemplate CreateProposalTemplate(Owner owner, string proposalTemplateName, bool isPrivate, Organisation organisation)
        {
            ProposalTemplate proposalTemplate = null;

            if (_proposalTemplateRepository.FindAll().Any(c => c.Name == proposalTemplateName))
            {
                throw new ArgumentException("There is already a proposal template with that name!");
            }
                proposalTemplate = new ProposalTemplate(owner, owner, proposalTemplateName, isPrivate);
                _proposalTemplateRepository.AddAsync(proposalTemplate);

            return proposalTemplate;
        }
    }
}
