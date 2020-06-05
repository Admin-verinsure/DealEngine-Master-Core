﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.Tasking
{
	public interface ITaskingService
	{
        Task<List<UserTask>> GetAllActiveTasksFor(Organisation organisation);
        Task UpdateUserTask(UserTask userTask);
        Task<List<UserTask>> GetUserTasksByMilestone(Milestone milestone);
        Task CreateTask(UserTask task);
    }
}
