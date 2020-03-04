using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IActivityService
    { 
        Task<Activity> GetActivityId(Guid activityId);
        Task UpdateActivity(Activity activity);
        Task<Activity> GetActivityByName(string name);
    }
}
