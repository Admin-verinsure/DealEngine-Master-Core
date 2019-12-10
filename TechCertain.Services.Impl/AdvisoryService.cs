using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<List<Advisory>> GetAdvisorysByMilestone(Milestone milestone)
        {
            return await _advisoryRepository.FindAll().Where(a=>a.Milestone == milestone && a.DateDeleted == null).ToListAsync();
        }
        public async Task UpdateAdvisory(Advisory advisory)
        {
            await _advisoryRepository.UpdateAsync(advisory);
        }
    }
}

