using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using System.Linq;
using TechCertain.Services.Interfaces;
using System;

namespace TechCertain.Services.Impl
{
    public class ProposalBuilderService : IProposalBuilderService
    {
        private readonly IUnitOfWork _unitOfWork;
        readonly IMapperSession<ProposalTemplate> _proposalTemplateRepository;

        public ProposalBuilderService(IUnitOfWork unitOfWork, IMapperSession<ProposalTemplate> proposalTemplateRepository)
        {
            _unitOfWork = unitOfWork;
            _proposalTemplateRepository = proposalTemplateRepository;
        }

        public ProposalTemplate CreateProposalTemplate(Owner owner, string proposalTemplateName, bool isPrivate, Organisation organisation)
        {
            ProposalTemplate proposalTemplate = null;

            if (_proposalTemplateRepository.FindAll().Any(c => c.Name == proposalTemplateName))
            {
                throw new ArgumentException("There is already a proposal template with that name!");
            }

            using (var uow = _unitOfWork.BeginUnitOfWork()) {               

                proposalTemplate = new ProposalTemplate(owner, owner, proposalTemplateName, isPrivate);
                _proposalTemplateRepository.Add(proposalTemplate);
                uow.Commit();
            }

            return proposalTemplate;
        }
    }
}
