using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;

namespace TechCertain.Services.Impl
{
    public class RevenueActivityService : IRevenueActivityService
    {
        IMapperSession<RevenueByActivity> _revenueByActivityRepository;

        public RevenueActivityService(IMapperSession<RevenueByActivity> revenueByActivityRepository)
        {
            _revenueByActivityRepository = revenueByActivityRepository;
        }

        public async Task AddRevenueByActivity(RevenueByActivity revenueData)
        {
            await _revenueByActivityRepository.AddAsync(revenueData);
        }
    }
}

