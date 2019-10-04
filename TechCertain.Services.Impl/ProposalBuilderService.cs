using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Domain.Services.Factories;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ProposalBuilderService : IProposalBuilderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProposalTemplateFactory _proposalTemplateFactory;

        public ProposalBuilderService(IUnitOfWork unitOfWork, ProposalTemplateFactory proposalTemplateFactory)
        {
            _unitOfWork = unitOfWork;
            _proposalTemplateFactory = proposalTemplateFactory;
        }

        public ProposalTemplate CreateProposalTemplate(Owner owner, string proposalTemplateName, bool isPrivate, Organisation organisation)
        {
            ProposalTemplate proposalTemplate = null;

            using (var uow = _unitOfWork.BeginUnitOfWork()) {               

                //proposalTemplate = _proposalTemplateFactory.CreateProposalTemplate(owner, proposalTemplateName, false);

                //uow.Add<ProposalTemplate>(proposalTemplate);

                //uow.Complete();
            }

            return proposalTemplate;
        }        
    }
}
