﻿using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace DealEngine.Services.Impl
{
    public class ActivityService : IActivityService
    {
        IMapperSession<Activity> _activityRepository;

        public ActivityService(IMapperSession<Activity> activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task<Activity> GetActivityByName(string name)
        {
            return await _activityRepository.FindAll().FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<Activity> GetActivityId(Guid activityId)
        {
            return await _activityRepository.GetByIdAsync(activityId);
        }

        public async Task UpdateActivity(Activity activity)
        {
            await _activityRepository.UpdateAsync(activity);
        }
    }
}
