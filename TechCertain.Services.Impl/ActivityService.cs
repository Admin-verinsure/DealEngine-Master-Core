using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace TechCertain.Services.Impl
{
    public class ActivityService : IActivityService
    {
        IMapperSession<Activity> _activityRepository;

        public ActivityService(IMapperSession<Activity> activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task<Activity> GetActivity(string Name)
        {
            return await _activityRepository.FindAll().FirstOrDefaultAsync(a => a.Name == Name);
        }

    }
}

