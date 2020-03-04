using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IProposalBuilderService
    {
        Task<ProposalTemplate> CreateProposalTemplate(Owner owner, string proposalTemplateName, bool isPrivate, Organisation organisation);
    }
}