using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IProposalBuilderService
    {
        Task<ProposalTemplate> CreateProposalTemplate(Owner owner, string proposalTemplateName, bool isPrivate, Organisation organisation);
    }
}