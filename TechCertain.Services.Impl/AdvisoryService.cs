using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace TechCertain.Services.Impl
{
    public class AdvisoryService : IAdvisoryService
    {
        IMapperSession<Advisory> _advisoryRepository;

        public AdvisoryService(IMapperSession<Advisory> advisoryRepository)
        {
            _advisoryRepository = advisoryRepository;
        }

        public async Task CreateAdvisory(Advisory advisory)
        {
            await _advisoryRepository.AddAsync(advisory);
        }

        public async Task<Advisory> GetAdvisoryByMilestone(Milestone milestone, Activity activity)
        {
            return await _advisoryRepository.FindAll().FirstOrDefaultAsync(a => a.Milestone == milestone && a.Activity == activity);            
        }

        public async Task UpdateAdvisory(Advisory advisory)
        {
            await _advisoryRepository.UpdateAsync(advisory);
        }
    }
}

