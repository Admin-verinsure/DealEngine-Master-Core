using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IAdvisoryService
    {
        Task CreateAdvisory(Advisory advisory);
        Task<Advisory> GetAdvisoryByMilestone(Milestone milestone, Activity activity);
        Task UpdateAdvisory(Advisory advisory);
    }
}
