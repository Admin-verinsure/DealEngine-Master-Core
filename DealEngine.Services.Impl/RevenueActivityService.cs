using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;

namespace DealEngine.Services.Impl
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

