﻿using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IActivityService
    { 
        Task<Activity> GetActivityId(Guid activityId);
    }
}