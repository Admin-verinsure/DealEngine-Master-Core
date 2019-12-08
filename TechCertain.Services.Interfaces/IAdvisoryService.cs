using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IAdvisoryService
    {
        Task CreateAdvisory(Advisory advisory);
        Task UpdateAdvisory(Advisory advisory);
        Task<List<Advisory>> GetAdvisorysByMilestone(Milestone milestone);
    }
}
