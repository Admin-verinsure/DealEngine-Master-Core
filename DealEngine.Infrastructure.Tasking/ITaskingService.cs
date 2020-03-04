using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.Tasking
{
	public interface ITaskingService
	{
        Task<UserTask> CreateTaskFor(User createdBy, Organisation createdFor, DateTime dueDate);
        Task<List<UserTask>> GetAllTasksFor(User user);
        Task<List<UserTask>> GetAllTasksFor (Organisation organisation);
        Task<UserTask> GetTask(Guid Id);
        Task CreateTaskFor(UserTask task);
        Task UpdateUserTask(UserTask userTask);
        Task<List<UserTask>> GetUserTasksByMilestone(Milestone milestone);
    }
}

