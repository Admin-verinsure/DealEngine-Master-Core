using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IProposalBuilderService
    {
        ProposalTemplate CreateProposalTemplate(Owner owner, string proposalTemplateName, bool isPrivate, Organisation organisation);
    }
}